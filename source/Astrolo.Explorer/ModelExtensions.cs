using System.Diagnostics;
using System.Text.RegularExpressions;
using Astrolo.Core;
using Astrolo.GeneKeys;

namespace Astrolo.Explorer;

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

    public static string GenerateUniqueName<T>(this IEnumerable<T> source, string prefix, Func<T, string> getDisplayName)
    {
        var pattern = new Regex($@"{prefix} \((\d+)\)");

        var maxValue = source.Select(GetValue).DefaultIfEmpty().Max();

        return maxValue != null ? $"{prefix} ({maxValue + 1})" : prefix;

        int? GetValue(T x)
        {
            var name = getDisplayName(x);

            if (name == prefix)
            {
                return 0;
            }

            var match = pattern.Match(name);
            return match.Success ? int.Parse(match.Groups[1].Value) : null;
        }
    }
}
