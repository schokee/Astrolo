using System.Windows.Data;
using System.Windows.Markup;

namespace Astrolo.Presentation.Core.Converters
{
    [MarkupExtensionReturnType(typeof(IValueConverter))]
    public class ObjectToStringConverterExtension : MarkupExtension
    {
        public ObjectToStringConverterExtension()
        {
        }

        public ObjectToStringConverterExtension(TextCasingConversion casing)
        {
            Casing = casing;
        }

        [ConstructorArgument("casing")]
        public TextCasingConversion Casing { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new ObjectToStringConverter {Casing = Casing};
        }
    }
}
