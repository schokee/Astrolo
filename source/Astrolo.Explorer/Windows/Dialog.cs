using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace Astrolo.Explorer.Windows
{
    public abstract class Dialog : Screen, IDialog
    {
        public bool HasBeenConfirmed { get; private set; }

        public virtual bool CanAccept => !HasBeenConfirmed;

        public virtual async Task Accept()
        {
            await TryCloseAsync(true);
        }

        public async Task Cancel()
        {
            await TryCloseAsync(false);
        }

        public override async Task TryCloseAsync(bool? dialogResult = null)
        {
            if (HasBeenConfirmed)
            {
                throw new InvalidOperationException("Dialog can only be confirmed once per invocation");
            }

            HasBeenConfirmed = dialogResult == true;
            NotifyOfPropertyChange(nameof(CanAccept));

            await base.TryCloseAsync(dialogResult);
        }

        public override async Task<bool> CanCloseAsync(CancellationToken cancellationToken = default)
        {
            return HasBeenConfirmed || await ConfirmCancelAsync();
        }

        protected override async Task OnActivatedAsync(CancellationToken cancellationToken)
        {
            await base.OnActivatedAsync(cancellationToken);
            HasBeenConfirmed = false;
        }

        protected virtual Task<bool> ConfirmCancelAsync()
        {
            return Task.FromResult(true);
        }
    }
}
