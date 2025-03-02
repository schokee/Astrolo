using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;

namespace Astrolo.YiJing.Metadata;

internal static class EmbeddedFile
{
    public static Stream Open(string filename)
    {
        return typeof(EmbeddedFile).Assembly.GetManifestResourceStream(typeof(EmbeddedFile), filename) ??
               throw new FileNotFoundException("File not found: " + filename);
    }

    public static IEnumerable<T> DeserializeCsv<T>(string name)
    {
        using var streamReader = new StreamReader(Open(name));
        using var reader = new CsvReader(streamReader, new CsvConfiguration(CultureInfo.InvariantCulture));

        foreach (var record in reader.GetRecords<T>())
        {
            yield return record;
        }
    }
}
