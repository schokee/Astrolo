using System.Diagnostics;
using Astrolo.Core;
using Astrolo.GeneKeys;

namespace Astrolo.Explorer
{
    internal static class ModelExtensions
    {
        public static Process Browse(this Uri uri)
        {
            return Process.Start(new ProcessStartInfo(uri.ToString())
            {
                UseShellExecute = true,
                Verb = "open"
            });
        }

        public static string GetMnemonicOrDefault(this Enum value)
        {
            return value.GetAttributeOfType<MnemonicAttribute>()?.Mnemonic ?? value.ToString();
        }
    }
}
