using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace Astrolo.Presentation.Core.Input
{
    public static class Commanding
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.RegisterAttached("Source", typeof(CommandBindingCollection),
            typeof(Commanding), new PropertyMetadata(default(CommandBindingCollection), OnSourceChanged));

        public static void SetSource(UIElement element, CommandBindingCollection value)
        {
            element.SetValue(SourceProperty, value);
        }

        public static CommandBindingCollection GetSource(UIElement element)
        {
            return (CommandBindingCollection)element.GetValue(SourceProperty);
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is UIElement uiElement)) return;

            uiElement.CommandBindings.Clear();
            uiElement.CommandBindings.AddRange(e.NewValue as CommandBindingCollection ?? CreateBindings(e.NewValue));
        }

        public static CommandBindingCollection CreateBindings(object? target)
        {
            if (target is null)
            {
                return new CommandBindingCollection();
            }

            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;

            var collection = target as IEnumerable ?? new[] { target };

            return new CommandBindingCollection(collection
                .OfType<object>()
                .SelectMany(o => o.GetType()
                    .GetMethods(bindingFlags)
                    .Select(x => CreateCommandBinding(o, x, bindingFlags))
                    .Where(x => x != null))
                .ToArray());
        }

        private static CommandBinding? CreateCommandBinding(object target, MethodInfo method, BindingFlags bindingFlags)
        {
            var command = GetAssociatedCommand(method);

            if (command is null)
            {
                return null;
            }

            var executeHandler = new ExecutedRoutedEventHandler((_, e) =>
            {
                var hasArg = method.GetParameters().Length == 1;

                Caliburn.Micro.Action.Invoke(target, method.Name, parameters: hasArg ? new[] { e.Parameter } : null);
                //method.Invoke(target, bindingFlags, null, hasArg ? new[] { e.Parameter } : null, CultureInfo.CurrentCulture);
                e.Handled = true;
            });

            var canExecuteProperty = target.GetType().GetProperty("Can" + method.Name, bindingFlags | BindingFlags.GetProperty);

            if (canExecuteProperty?.PropertyType == typeof(bool))
            {
                return new CommandBinding(command, executeHandler, (_, e) =>
                {
                    e.CanExecute = (bool)canExecuteProperty.GetValue(target)!;
                    e.Handled = true;
                });
            }

            var canExecuteMethod = target.GetType().GetMethod("Can" + method.Name, bindingFlags);

            if (canExecuteMethod == null || canExecuteMethod.GetParameters().Length > 1 || canExecuteMethod.ReturnType != typeof(bool))
            {
                return new CommandBinding(command, executeHandler);
            }

            var canExecuteHandler = new CanExecuteRoutedEventHandler((_, e) =>
            {
                var args = canExecuteMethod.GetParameters().Length == 1 ? new[] { e.Parameter } : null;

                e.CanExecute = (bool)canExecuteMethod.Invoke(target, bindingFlags, null, args, CultureInfo.CurrentCulture)!;
                e.Handled = true;
            });

            return new CommandBinding(command, executeHandler, canExecuteHandler);
        }

        private static ICommand? GetAssociatedCommand(MethodInfo method)
        {
            if (method.GetParameters().Length > 1 || !(method.ReturnType == typeof(void) || typeof(Task).IsAssignableFrom(method.ReturnType))) return null;

            var targetAttribute = method.GetCustomAttributes(typeof(CommandTargetAttribute), false).Cast<CommandTargetAttribute>().FirstOrDefault();
            return targetAttribute?.Source;
        }
    }
}
