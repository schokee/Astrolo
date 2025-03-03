namespace Astrolo.Explorer.Components;

public interface ISelectable
{
    event EventHandler SelectionChanged;

    bool IsSelected { get; set; }
}
