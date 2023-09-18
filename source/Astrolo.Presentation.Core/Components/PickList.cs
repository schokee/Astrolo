using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Caliburn.Micro;

namespace Astrolo.Presentation.Core.Components
{
    public sealed class PickList<T> : PropertyChangedBase, INotifyCollectionChanged, IReadOnlyList<T>, IDisposable where T : class, ISelectable
    {
        private readonly ObservableCollection<T> _collection;

        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        public event EventHandler? SelectionChanged;

        public PickList()
        {
            _collection = new ObservableCollection<T>();
            _collection.CollectionChanged += (_, e) => CollectionChanged?.Invoke(this, e);
        }

        public PickList(IEnumerable<T> source) : this()
        {
            AddRange(source);
        }

        public void Dispose()
        {
            foreach (var item in _collection)
            {
                item.SelectionChanged -= OnSelectionChanged;
            }
        }

        public T this[int index] => _collection[index];

        public bool IsEmpty => Count == 0;

        public bool HasItems => Count > 0;

        public int Count => _collection.Count;

        public int SelectionCount { get; private set; }

        public bool AnySelected => SelectionCount > 0;

        public bool? AllSelected
        {
            get => AnySelected ? SelectionCount == Count ? true : default(bool?) : false;
            // ReSharper disable once ValueParameterNotUsed
            set => SetSelection(SelectionCount < Count);
        }

        public string SelectionSummary
        {
            get
            {
                var count = Count;

                return IsEmpty
                    ? string.Empty
                    : AllSelected.HasValue ? $"{count} items" : $"{SelectionCount} of {count} selected";
            }
        }

        public IEnumerable<T> CurrentSelection
        {
            get { return _collection.Where(x => x.IsSelected); }
        }

        public IEnumerable<T> ClearedSelection
        {
            get { return _collection.Where(x => !x.IsSelected); }
        }

        public bool CanSelectAll => HasItems && AllSelected != true;

        public IObservable<object> ObserveSelectionChanges()
        {
            return Observable.FromEventPattern(h => SelectionChanged += h, h => SelectionChanged -= h);
        }

        public void SelectAll()
        {
            SetSelection(true);
        }

        public void ClearSelection()
        {
            SetSelection(false);
        }

        public void InvertSelection()
        {
            using (SuspendEvents())
            {
                foreach (var item in _collection)
                {
                    item.IsSelected = !item.IsSelected;
                }
            }
        }

        public void MoveAllTo(PickList<T> target)
        {
            MoveTo(target, _ => true);
        }

        public void MoveSelectionTo(PickList<T> target)
        {
            MoveTo(target, x => x.IsSelected);
        }

        public void MoveTo(PickList<T> target, Predicate<T> canBeMoved)
        {
            if (!HasItems) return;

            var anyMoved = false;

            for (var i = 0; i < Count;)
            {
                var item = _collection[i];

                if (!canBeMoved(item))
                {
                    ++i;
                    continue;
                }

                item.SelectionChanged -= OnSelectionChanged;
                _collection.RemoveAt(i);

                item.SelectionChanged += target.OnSelectionChanged;
                target._collection.Add(item);

                anyMoved = true;
            }

            if (!anyMoved) return;

            OnCollectionChanged();
            target.OnCollectionChanged();
        }

        public IDisposable SuspendEvents()
        {
            ++NotificationLockCount;

            return Disposable.Create(() =>
            {
                if (--NotificationLockCount == 0) UpdateStateOfSelection();
            });
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private int NotificationLockCount { get; set; }

        private void OnSelectionChanged(object? sender, EventArgs args)
        {
            if (NotificationLockCount == 0) UpdateStateOfSelection();
        }

        private void OnCollectionChanged()
        {
            UpdateStateOfSelection();
            NotifyOfPropertyChange(nameof(HasItems));
        }

        private void AddRange(IEnumerable<T> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            foreach (var item in items.Where(x => x != null))
            {
                _collection.Add(item);
                item.SelectionChanged += OnSelectionChanged;
            }

            OnCollectionChanged();
        }

        private void SetSelection(bool select)
        {
            if (IsEmpty || AllSelected == select) return;

            using (SuspendEvents())
            {
                foreach (var item in _collection)
                {
                    item.IsSelected = select;
                }
            }
        }

        private void UpdateStateOfSelection()
        {
            var currentCount = CurrentSelection.Count();
            if (currentCount == SelectionCount) return;

            SelectionCount = currentCount;

            NotifyOfPropertyChange(nameof(CanSelectAll));
            NotifyOfPropertyChange(nameof(AnySelected));
            NotifyOfPropertyChange(nameof(AllSelected));
            NotifyOfPropertyChange(nameof(SelectionSummary));
            NotifyOfPropertyChange(nameof(SelectionCount));

            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public static class PickList
    {
        public static PickList<Selectable<T>> ToPickList<T>(this IEnumerable<T>? source)
        {
            return source is not null
                ? new PickList<Selectable<T>>(source.Select(x => new Selectable<T>(x)))
                : new PickList<Selectable<T>>();
        }
    }
}
