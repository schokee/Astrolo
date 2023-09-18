using System.Windows.Input;

namespace Astrolo.Presentation.Core.Input
{
    public class DelegateCommand<T> : ICommand
    {
        private Action<T?> ExecuteAction { get; }
        private Func<T?, bool> CanExecuteAction { get; }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public DelegateCommand(Action<T?> execute) : this(execute, _ => true)
        {
        }

        public DelegateCommand(Action<T?> execute, Func<T?, bool> canExecute)
        {
            ExecuteAction = execute;
            CanExecuteAction = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return CanExecuteAction((T?)parameter);
        }

        public void Execute(object? parameter)
        {
            ExecuteAction((T?)parameter);
        }
    }

    public class DelegateCommand : DelegateCommand<object>
    {
        public DelegateCommand(Action execute) : base(_ => execute(), _ => true)
        {
        }

        public DelegateCommand(Action<object?> execute, Func<object?, bool> canExecute) : base(execute, canExecute)
        {
        }

        public DelegateCommand(Action<object?> execute) : base(execute)
        {
        }
    }
}
