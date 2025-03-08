using Astrolo.Astrology;
using Astrolo.YiJing;
using Caliburn.Micro;

namespace Astrolo.Explorer.UI.Profiling;

public sealed class ProfilePoint(ProfileEditor editor, Marker marker, bool isDesign) : PropertyChangedBase, IDataErrorInfo
{
    public event Action<ProfilePoint> ValueChanged;

    public Marker Marker { get; } = marker;

    public bool IsDesign { get; } = isDesign;

    private string _valueAsText;
    public string ValueAsText
    {
        get => _valueAsText;
        set
        {
            if (Set(ref _valueAsText, value))
            {
                NotifyOfPropertyChange(nameof(Value));
                ValueChanged?.Invoke(this);
            }
        }
    }

    public LineOfHexagram? Value
    {
        get => LineOfHexagram.TryParse(ValueAsText, out var result) ? result : default(LineOfHexagram?);
        set => ValueAsText = value is null ? string.Empty : value.Value.ToString();
    }

    #region IDataErrorInfo

    string IDataErrorInfo.this[string columnName]
    {
        get
        {
            Error = columnName == nameof(ValueAsText) ? editor.Validate(this) : null;
            return Error;
        }
    }

    public string Error { get; private set; }

    #endregion
}
