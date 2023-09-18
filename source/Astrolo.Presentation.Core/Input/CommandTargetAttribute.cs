using System.Reflection;
using System.Windows.Input;

namespace Astrolo.Presentation.Core.Input
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandTargetAttribute : Attribute
    {
        private readonly PropertyInfo? _source;

        public CommandTargetAttribute(Type type, string property)
        {
            _source = type.GetProperty(property, BindingFlags.Static | BindingFlags.Public);

            if (_source is null || !typeof(ICommand).IsAssignableFrom(_source.PropertyType))
            {
                throw new ArgumentException($"Property {property} on {type} is not of type {typeof(ICommand)}");
            }
        }

        public ICommand? Source => (ICommand?)_source?.GetValue(null, null);
    }
}
