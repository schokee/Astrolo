using System.Collections;
using System.Collections.Specialized;
using Caliburn.Micro;

namespace Astrolo.Presentation.Core.Components
{
    public class PartCollection<T> : PropertyChangedBase, IList, IList<T>, INotifyCollectionChanged where T : class, IPart
    {
        private readonly List<T> _collection = new();
        private readonly object? _container;

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public PartCollection(object? container)
        {
            _container = container;
        }

        #region IList<T> Members

        public int IndexOf(T item)
        {
            return _collection.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _collection.Insert(index, item);
            item.Container = _container;

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            NotifyOfPropertyChange(nameof(Count));
        }

        public void RemoveAt(int index)
        {
            var oldItem = _collection[index];
            if (oldItem != null)
            {
                oldItem.Container = null;
            }

            _collection.RemoveAt(index);

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem, index));
            NotifyOfPropertyChange(nameof(Count));
        }

        public void Move(int oldIndex, int newIndex)
        {
            if (oldIndex == newIndex) return;

            var item = this[oldIndex];

            _collection.RemoveAt(oldIndex);
            _collection.Insert(newIndex, item);

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex));
        }

        public T this[int index]
        {
            get => _collection[index];
            set
            {
                var oldItem = _collection[index];
                if (ReferenceEquals(value, oldItem)) return;

                if (oldItem != null)
                {
                    oldItem.Container = null;
                }

                _collection[index] = value;

                if (value is not null)
                {
                    value.Container = _container;
                }
            }
        }

        #endregion

        #region ICollection<T> Members

        public void Add(T item)
        {
            Insert(Count, item);
        }

        public void AddRange(IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                Add(item);
            }
        }

        public void Clear()
        {
            if (_collection.Count == 0) return;

            foreach (var item in _collection)
            {
                if (item != null)
                    item.Container = null;
            }

            _collection.Clear();

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            NotifyOfPropertyChange(nameof(Count));
        }

        public bool Contains(T item)
        {
            return _collection.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _collection.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            var index = IndexOf(item);
            if (index < 0) return false;

            RemoveAt(index);
            return true;
        }

        #endregion

        #region ICollection Members

        public bool IsReadOnly => false;

        public int Count => _collection.Count;

        public object SyncRoot => ((IList)_collection).SyncRoot;

        public bool IsSynchronized => ((IList)_collection).IsSynchronized;

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection) _collection).CopyTo(array, index);
        }

        #endregion

        #region IList Members

        object? IList.this[int index]
        {
            get => this[index];
            set => this[index] = (T)value!;
        }

        bool IList.IsFixedSize => ((IList)_collection).IsFixedSize;

        bool IList.Contains(object? value)
        {
            return Contains((T)value!);
        }

        int IList.Add(object? value)
        {
            var indexOfAdd = Count;

            Add((T)value!);
            return indexOfAdd;
        }

        void IList.Remove(object? value)
        {
            Remove((T)value!);
        }

        public int IndexOf(object? value)
        {
            return IndexOf((T)value!);
        }

        void IList.Insert(int index, object? value)
        {
            Insert(index, (T)value!);
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        protected virtual void OnCollectionChanged()
        {
        }

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            CollectionChanged?.Invoke(this, args);
            NotifyOfPropertyChange("Item[]");

            OnCollectionChanged();
        }
    }
}
