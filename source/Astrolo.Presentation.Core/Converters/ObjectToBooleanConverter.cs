using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Astrolo.Presentation.Core.Converters
{
    [ValueConversion(typeof(object), typeof(bool))]
    public class ObjectToBooleanConverter : IValueConverter
    {
        public bool Invert { get; set; }

        public bool HandleList { get; set; }

        public bool TryConvert { get; set; } = true;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = ConvertToBoolean(value, culture);
            return Invert ? !result : result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return targetType == typeof(bool) && value is bool b
                ? Invert ? !b : b
                : DependencyProperty.UnsetValue;
        }

        public bool ConvertToBoolean(object o, IFormatProvider culture)
        {
            if (o == null) return false;

            switch (o)
            {
                case bool flag:
                    return flag;

                case string s when !TryConvert:
                    return !string.IsNullOrWhiteSpace(s);

                case IConvertible convertible when TryConvert:
                    return convertible.ToBoolean(culture);
            }

            if (!HandleList) return true;

            switch (o)
            {
                case ICollection collection:
                    return collection.Count > 0;

                case IEnumerable enumerable:
                    return enumerable.Cast<object>().Any();

                default:
                    return false;
            }
        }
    }
}
