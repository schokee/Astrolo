using System.Globalization;

namespace Astrolo.Presentation.Core
{
    public enum TextCasingConversion
    {
        None,
        Lower,
        Upper,
        Title
    }

    public static partial class Extensions
    {
        public static string? ConvertCase(this string? source, TextCasingConversion casing, CultureInfo culture)
        {
            if (source is null) return null;

            switch (casing)
            {
                case TextCasingConversion.Lower:
                    return source.ToLower(culture);
                case TextCasingConversion.Upper:
                    return source.ToUpper(culture);
                case TextCasingConversion.Title:
                    return culture.TextInfo.ToTitleCase(source);
            }

            return source;
        }
    }
}
