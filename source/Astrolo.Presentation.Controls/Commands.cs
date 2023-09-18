using System.Windows.Input;

namespace Astrolo.Presentation.Controls
{
    public static class Commands
    {
        public static ICommand SelectPointCommand { get; } = new RoutedCommand(nameof(SelectPointCommand), typeof(Commands));
    }
}
