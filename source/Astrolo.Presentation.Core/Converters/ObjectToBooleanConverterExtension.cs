using System.Windows.Data;
using System.Windows.Markup;

namespace Astrolo.Presentation.Core.Converters
{
    [MarkupExtensionReturnType(typeof(IValueConverter))]
    public class ObjectToBooleanConverterExtension : MarkupExtension
    {
        public bool Invert { get; set; }

        public bool HandleList { get; set; }

        public bool TryConvert { get; set; } = true;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new ObjectToBooleanConverter
            {
                Invert = Invert,
                HandleList = HandleList,
                TryConvert = TryConvert
            };
        }
    }
}
