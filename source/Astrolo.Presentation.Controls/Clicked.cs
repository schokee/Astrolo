using System.Windows.Controls.Primitives;

namespace Astrolo.Presentation.Controls
{
    public static class Clicked
    {
        public static readonly DependencyProperty PointProperty = DependencyProperty.RegisterAttached(
            "Point", typeof(object), typeof(Clicked), new PropertyMetadata(default(object)));

        public static void SetPoint(ButtonBase element, object value)
        {
            element.SetValue(PointProperty, value);
        }

        public static object GetPoint(ButtonBase element)
        {
            return element.GetValue(PointProperty);
        }
    }
}
