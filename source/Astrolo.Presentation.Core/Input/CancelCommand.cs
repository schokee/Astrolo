using System.Windows.Input;

namespace Astrolo.Presentation.Core.Input
{
    public sealed class CancelCommand : ICommand
    {
        private readonly CancellationTokenSource _source = new();

        public event EventHandler? CanExecuteChanged;

        public CancellationToken CancellationToken => _source.Token;

        public bool CanExecute(object? parameter)
        {
            return !_source.IsCancellationRequested;
        }

        public void Execute(object? parameter)
        {
            if (_source.IsCancellationRequested) return;

            _source.Cancel();
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

}
