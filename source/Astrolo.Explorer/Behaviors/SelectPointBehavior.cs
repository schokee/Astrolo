using System.Windows.Input;
using Astrolo.Presentation.Controls;
using Microsoft.Xaml.Behaviors;

namespace Astrolo.Explorer.Behaviors;

public sealed class SelectPointBehavior : Behavior<FrameworkElement>
{
    public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
        nameof(Command), typeof(ICommand), typeof(SelectPointBehavior), new PropertyMetadata(default(ICommand)));

    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.CommandBindings.Add(new CommandBinding(Commands.SelectPointCommand,
            (_, args) => Command?.Execute(args.Parameter),
            (_, args) => args.CanExecute = Command?.CanExecute(args.Parameter) == true));
    }
}
