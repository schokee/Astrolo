using System.Windows.Data;
using System.Windows.Markup;

namespace Astrolo.Presentation.Core.Converters
{
    [MarkupExtensionReturnType(typeof(IValueConverter))]
    public class ObjectToVisibilityConverterExtension : MarkupExtension
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
    }
}
