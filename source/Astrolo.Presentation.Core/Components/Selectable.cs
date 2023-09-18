using Caliburn.Micro;

namespace Astrolo.Presentation.Core.Components
{
    public class Selectable : PropertyChangedBase, ISelectable
    {
        private bool _isSelected;

        public event EventHandler? SelectionChanged;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (value == _isSelected) return;
                _isSelected = value;
                NotifyOfPropertyChange();
                OnSelectionChanged();
            }
        }

        protected virtual void OnSelectionChanged()
        {
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public class Selectable<TValue> : Selectable
    {
        public Selectable(TValue value)
        {
            Value = value;
        }

        public TValue Value { get; }
    }
}
