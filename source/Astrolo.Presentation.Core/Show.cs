using System.Reactive.Linq;
using Astrolo.Presentation.Core.Windows;
using Caliburn.Micro;
using Serilog;

namespace Astrolo.Presentation.Core
{
    public static class Show
    {
        public static IUserPrompt Prompt => IoC.Get<IUserPrompt>();

        public static void Alert(Exception exception, string logMessage, string? caption = null)
        {
            Log.Error(exception, logMessage);
            Prompt.Alert(exception.Message, caption);
        }

        public static async Task<T> WindowAsync<T>(Func<T, bool>? wasCancelled = null) where T : IDeactivate
        {
            var model = IoC.Get<T>();

            var closed = model
                .ObserveDeactivation()
                .FirstAsync(x => x.WasClosed);

            await IoC.Get<IWindowManager>().ShowWindowAsync(model);
            await closed;

            if (wasCancelled?.Invoke(model) != true)
                return model;

            throw new OperationCanceledException();
        }


        public static async Task<TDialog> DialogAsync<TDialog>(Action<TDialog>? initializeDialog = null) where TDialog : IDialog
        {
            var dialog = IoC.Get<TDialog>();

            initializeDialog?.Invoke(dialog);

            return await dialog.ShowModalAsync();
        }

        public static async Task<TDialog> ShowModalAsync<TDialog>(this TDialog dialog) where TDialog : IDialog
        {
            var result = await IoC.Get<IWindowManager>().ShowDialogAsync(dialog);
            if (result != true)
                throw new OperationCanceledException("Dialog cancelled");

            return dialog;
        }
    }

}
