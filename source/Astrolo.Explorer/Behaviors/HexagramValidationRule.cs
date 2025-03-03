using System.Globalization;
using Astrolo.YiJing;

namespace Astrolo.Explorer.Behaviors;

public sealed class HexagramValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if (value is string { Length: > 0 } s && uint.TryParse(s, out var v) && v is > 0 and <= HexagramInfo.TotalHexagrams)
        {
            return new(true, null);
        }

        return new(false, $"Please enter a value in the range 1 - {HexagramInfo.TotalHexagrams}");
    }
}
