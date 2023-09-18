using System.ComponentModel;
using System.Reflection;
using System.Windows.Markup;

namespace Astrolo.Presentation.Core.Converters
{
    [MarkupExtensionReturnType(typeof(object[]))]
    public class EnumValuesExtension : MarkupExtension
    {
        public EnumValuesExtension()
        {
        }

        public EnumValuesExtension(Type enumType)
        {
            EnumType = enumType;
        }

        [ConstructorArgument("enumType")]
        public Type? EnumType { get; set; }

        public bool Sorted { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var values = Enum.GetValues(EnumType ?? throw new ArgumentException("The enumeration's type is not set.", nameof(EnumType)));

            return Sorted ? values.Cast<object>().OrderBy(x => GetEnumDescription(x.ToString()!)).ToArray() : values;
        }

        private string GetEnumDescription(string name)
        {
            return EnumType?.GetField(name)?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? name;
        }
    }
}
