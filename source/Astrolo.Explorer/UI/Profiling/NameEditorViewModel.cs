using Astrolo.Explorer.Windows;

namespace Astrolo.Explorer.UI.Profiling;

public sealed class NameEditorViewModel : Dialog, IDataErrorInfo
{
    private string _newName;
    private string _error;

    private Func<string, bool> IsNameUnique { get; }

    public delegate NameEditorViewModel Factory(Func<string, bool> isNameUnique, string newName, bool rename = false);

    public NameEditorViewModel(Func<string, bool> isNameUnique, string newName, bool rename)
    {
        DisplayName = rename ? "Rename Profile" : "New Profile";

        IsRename = rename;
        IsNameUnique = isNameUnique;

        NewName = newName?.Trim() ?? string.Empty;
        OldName = NewName;
    }

    public bool IsRename { get; }

    public string OldName { get; }

    public string NewName
    {
        get => _newName;
        set
        {
            if (Set(ref _newName, value))
            {
                NotifyOfPropertyChange(nameof(CanAccept));
                NotifyOfPropertyChange(nameof(Error));
            }
        }
    }

    public override bool CanAccept => base.CanAccept && Error is null;

    #region IDataErrorInfo

    public string this[string columnName]
    {
        get
        {
            var newName = NewName?.Trim();

            return Error = columnName switch
            {
                nameof(NewName) when string.IsNullOrWhiteSpace(newName) => "Name cannot be blank",
                nameof(NewName) when newName.Equals(OldName) => null,
                nameof(NewName) when !IsNameUnique(newName) => "Name must be unique",
                _ => null
            };
        }
    }

    public string Error
    {
        get => _error;
        private set
        {
            if (Set(ref _error, value))
            {
                NotifyOfPropertyChange(nameof(CanAccept));
            }
        }
    }

    #endregion
}
