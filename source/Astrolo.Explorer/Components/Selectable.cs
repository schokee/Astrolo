using Caliburn.Micro;

namespace Astrolo.Explorer.Components;

public class Selectable : PropertyChangedBase, ISelectable
{
    public event EventHandler SelectionChanged;

    public bool IsSelected
    {
        get;
        set
        {
            if (Set(ref field, value))
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
