using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace Astrolo.Presentation.Core.Converters
{
    [ValueConversion(typeof(object), typeof(string))]
    public class ObjectToStringConverter : IValueConverter
    {
        public TextCasingConversion Casing { get; set; }

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertToString(value)?.ConvertCase(Casing, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static string? ConvertToString(object? value)
        {
            if (value is null) return null;

            var type = value.GetType();

            if (type.IsEnum)
            {
                value = type.GetField(value.ToString()!)?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? value;
            }

            return value.ToString();
        }
    }
}
