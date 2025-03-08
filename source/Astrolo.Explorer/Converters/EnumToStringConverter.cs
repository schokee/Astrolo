using System.Globalization;
using System.Reflection;
using System.Windows.Markup;

namespace Astrolo.Explorer.Converters;

[MarkupExtensionReturnType(typeof(IValueConverter))]
[ValueConversion(typeof(object), typeof(string))]
public class EnumToStringConverter : MarkupExtension, IValueConverter
{
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return new EnumToStringConverter();
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return ConvertToString(value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public static string ConvertToString(object value)
    {
        if (value == null)
            return null;

        var type = value.GetType();

        if (type.IsEnum)
        {
            value = type.GetField(value.ToString())?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? value;
        }

        return value.ToString();
    }
}
