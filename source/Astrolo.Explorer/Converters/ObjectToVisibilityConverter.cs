using System.Globalization;
using System.Windows.Markup;

namespace Astrolo.Explorer.Converters;

[MarkupExtensionReturnType(typeof(IValueConverter))]
[ValueConversion(typeof(object), typeof(Visibility))]
public class ObjectToVisibilityConverter : MarkupExtension, IValueConverter, IMultiValueConverter
{
    public bool HiddenOnly { get; set; }

    public bool Invert { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return new ObjectToVisibilityConverter
        {
            Invert = Invert,
            HiddenOnly = HiddenOnly
        };
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var show = value != null;

        switch (value)
        {
            case Visibility v:
                show = v == Visibility.Visible;
                break;
            case bool b:
                show = b;
                break;
            case int i:
                show = i != 0;
                break;
            case string s:
                show = !string.IsNullOrEmpty(s);
                break;
        }

        return ToVisibility(show);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var show = values?.All(x => x is true) == true;
        return ToVisibility(show);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    private Visibility ToVisibility(bool show)
    {
        return show ^ Invert ? Visibility.Visible : HiddenOnly ? Visibility.Hidden : Visibility.Collapsed;
    }
}
