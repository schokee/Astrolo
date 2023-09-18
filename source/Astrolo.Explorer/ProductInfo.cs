using System.Reflection;

namespace Astrolo.Explorer
{
    static class ProductInfo
    {
        static ProductInfo()
        {
            var assembly = Assembly.GetEntryAssembly();

            ProductName = assembly!.GetCustomAttribute<AssemblyProductAttribute>().Product;
            Version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
        }

        public static string ProductName { get; }

        public static string Version { get; }
    }
}
