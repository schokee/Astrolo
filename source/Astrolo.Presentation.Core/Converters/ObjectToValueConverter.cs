using System.Globalization;
using System.Windows.Data;

namespace Astrolo.Presentation.Core.Converters
{
    public abstract class ObjectToValueConverter<T> : IValueConverter
    {
        public T? TrueValue { get; set; }

        public T? FalseValue { get; set; }

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertToBoolean(value) ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static bool ConvertToBoolean(object? value)
        {
            switch (value)
            {
                case bool b:
                    return b;

                case int i:
                    return i != 0;

                case string s:
                    return !string.IsNullOrEmpty(s);

                default:
                    return value != null;
            }
        }
    }
}
