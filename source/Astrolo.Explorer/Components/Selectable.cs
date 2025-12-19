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
            if (Set(ref _isSelected, value))
            {
                OnSelectionChanged();
            }
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
