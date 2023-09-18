using Microsoft.Win32;

namespace Astrolo.Presentation.Core.Windows
{
    public class FileDialogService : IFileDialogService
    {
        public Task<string[]> SelectFilesToOpen(FileDialogSettings settings)
        {
            var dialog = new OpenFileDialog
            {
                InitialDirectory = settings.InitialDirectory,
                DefaultExt = settings.DefaultExtension,
                Filter = settings.Filter,
                FileName = settings.InitialName,
                Title = settings.Title,
                Multiselect = true,
                CheckFileExists = true
            };

            return ShowDialog(dialog, x => x.FileNames);
        }

        public Task<string> SelectFileToOpen(FileDialogSettings settings)
        {
            var dialog = new OpenFileDialog
            {
                InitialDirectory = settings.InitialDirectory,
                DefaultExt = settings.DefaultExtension,
                Filter = settings.Filter,
                FileName = settings.InitialName,
                Title = settings.Title,
                Multiselect = false,
                CheckFileExists = true
            };

            return ShowDialog(dialog, x => x.FileName);
        }

        public Task<string> SelectFileToSave(FileDialogSettings settings)
        {
            var dialog = new SaveFileDialog
            {
                InitialDirectory = settings.InitialDirectory,
                DefaultExt = settings.DefaultExtension,
                Filter = settings.Filter,
                FileName = settings.InitialName,
                Title = settings.Title,
                OverwritePrompt = true
            };

            return ShowDialog(dialog, x => x.FileName);
        }

        private static Task<T> ShowDialog<TDialog, T>(TDialog dialog, Func<TDialog, T> selectResult) where TDialog : FileDialog
        {
            var result = new TaskCompletionSource<T>();

            if (dialog.ShowDialog() == true)
            {
                result.SetResult(selectResult(dialog));
            }
            else
            {
                result.SetCanceled();
            }

            return result.Task;
        }
    }
}
