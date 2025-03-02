using Caliburn.Micro;

namespace Astrolo.Explorer.Components;

public class Selectable : PropertyChangedBase, ISelectable
{
    private bool _isSelected;

    public event EventHandler SelectionChanged;

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

public class Selectable<TValue>(TValue value) : Selectable
{
    public TValue Value { get; } = value;
}
