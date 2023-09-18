using System.Windows.Input;
using Astrolo.Presentation.Core.Input;

namespace Astrolo.Presentation.Core.Components
{
    public class CancellableProgressState : ProgressState
    {
        private readonly CancelCommand _cancelCommand = new();

        public CancellableProgressState(string description) : base(description)
        {
        }

        public ICommand CancelCommand => _cancelCommand;

        public CancellationToken CancellationToken => _cancelCommand.CancellationToken;
    }
}
