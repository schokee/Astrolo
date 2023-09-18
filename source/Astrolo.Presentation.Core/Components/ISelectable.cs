namespace Astrolo.Presentation.Core.Components
{
    public interface ISelectable
    {
        event EventHandler SelectionChanged;

        bool IsSelected { get; set; }
    }
}
