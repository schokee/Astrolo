using System.Globalization;

namespace Astrolo.Explorer.UI.Visualisation;

[ValueConversion(typeof(int), typeof(Brush))]
public sealed class IndexToBrushConverter : IValueConverter
{
    public Brush[] Palette { get; set; } =
    [
        Brushes.Gold,
        Brushes.DarkOrange,
        Brushes.MediumVioletRed,
        Brushes.RoyalBlue,
        Brushes.DodgerBlue,
        Brushes.YellowGreen,
        Brushes.Gray
    ];

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int index)
        {
            return Palette[index % Palette.Length];
        }

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
