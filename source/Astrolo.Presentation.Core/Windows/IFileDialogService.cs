namespace Astrolo.Presentation.Core.Windows
{
    public interface IFileDialogService
    {
        Task<string[]> SelectFilesToOpen(FileDialogSettings settings);

        Task<string> SelectFileToOpen(FileDialogSettings settings);

        Task<string> SelectFileToSave(FileDialogSettings settings);
    }

    public struct FileDialogSettings
    {
        public string Title { get; set; }

        public string Filter { get; set; }

        public string InitialDirectory { get; set; }

        public string InitialName { get; set; }

        public string DefaultExtension { get; set; }
    }
}
