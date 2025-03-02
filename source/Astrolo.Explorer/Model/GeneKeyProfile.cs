using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using Astrolo.GeneKeys;
using Astrolo.HumanDesign.Charting;
using Astrolo.YiJing;
using CsvHelper;
using Serilog;

namespace Astrolo.Explorer.Model;

using Figure = Astrology.Marker;

public sealed class GeneKeyProfile
{
    public GeneKeyTable GeneKeys { get; }

    public GeneKeyProfile(string name, PersonalChart chart, GeneKeyTable geneKeys, string birthplace = null, DateTime? timeOfBirth = null)
    {
        GeneKeys = geneKeys;
        Name = name;
        Birthplace = birthplace;
        TimeOfBirth = timeOfBirth;

        Chart = chart;
        Points = chart.SelectGeneKeys((sphere, chartValue) => new GeneKeyPoint(this, sphere, chartValue)).ToList();

        LineCounts = Enumerable.Range(1, 6)
            .Select(x => Points.Count(p => p.Line == x))
            .ToArray();
    }

    public string Name { get; }

    public bool IsSignatureValid { get; private set; }

    public string Birthplace { get; }

    public DateTime? TimeOfBirth { get; }

    public PersonalChart Chart { get; }

    public IReadOnlyList<GeneKeyPoint> Points { get; }

    public IEnumerable<IGeneKey> Keys => Points.Select(x => x.GeneKey);

    public IEnumerable<GeneKeyPoint> ActivationSequence => Points.Take(4);
    public IEnumerable<GeneKeyPoint> VenusSequence => Points.Skip(3).Take(6);

    public GeneKeyPoint this[Sphere sphere] => Points[(int)sphere];

    public GeneKeyPoint LifesWork => Points[0];
    public GeneKeyPoint Evolution => Points[1];
    public GeneKeyPoint Radiance => Points[2];
    public GeneKeyPoint Purpose => Points[3];
    public GeneKeyPoint Attraction => Points[4];
    public GeneKeyPoint IQ => Points[5];
    public GeneKeyPoint EQ => Points[6];
    public GeneKeyPoint SQ => Points[7];
    public GeneKeyPoint Vocation => Points[8];
    public GeneKeyPoint Culture => Points[9];
    public GeneKeyPoint Pearl => Points[10];

    public int[] LineCounts { get; }

    public bool MatchesAttractionSequenceOf(GeneKeyProfile other)
    {
        return MatchesStartingSequenceOf(other, 4);
    }

    public bool MatchesStartingSequenceOf(GeneKeyProfile other, int count)
    {
        return ReferenceEquals(this, other) || Keys.Take(count).SequenceEqual(other.Keys.Take(count));
    }

    public bool MatchesEndingSequenceOf(GeneKeyProfile other, int count)
    {
        return ReferenceEquals(this, other) || Keys.Skip(Points.Count - count).SequenceEqual(other.Keys.Skip(Points.Count - count));
    }

    public void ClearSelection()
    {
        foreach (var profilePoint in Points)
        {
            profilePoint.IsSelected = false;
        }
    }

    public XElement ToXElement()
    {
        return new("Profile",
            new XAttribute("Name", Name),
            new XAttribute("Sequence", string.Join(", ", Points)));
    }

    public override string ToString()
    {
        return Name;
    }

    public static void Serialize(Stream stream, IEnumerable<GeneKeyProfile> profiles)
    {
        using var streamWriter = new StreamWriter(stream, Encoding.UTF8, 1024, true);
        using var writer = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);
        writer.WriteRecords(profiles.OrderBy(x => x.Name).Select(HumanDesignRecord.CreateFrom));
    }

    public static IEnumerable<GeneKeyProfile> DeserializeCsv(Stream stream, GeneKeyTable geneKeys)
    {
        using var streamReader = new StreamReader(stream);
        using var reader = new CsvReader(streamReader, CultureInfo.InvariantCulture);

        if (reader.Read() && reader.ReadHeader())
        {
            reader.ValidateHeader<HumanDesignRecord>();

            while (reader.Read())
            {
                GeneKeyProfile geneKeyProfile;

                var text = reader.Parser.RawRecord;
                var line = reader.Parser.Row;

                try
                {
                    geneKeyProfile = reader.GetRecord<HumanDesignRecord>().ToProfile(geneKeys);
                }
                catch (Exception exception)
                {
                    Log.Error(exception, "Failed to read line {0}: {1}", line, text);
                    continue;
                }

                yield return geneKeyProfile;
            }
        }
    }

    private string ComputeHash()
    {
        var plainText = string.Concat(Chart.Values.Select(x => x.LineOfHexagram)) +
                        (Birthplace ?? string.Empty) +
                        (TimeOfBirth?.ToString() ?? string.Empty);

        return BitConverter
            .ToString(System.Security.Cryptography.MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(plainText)))
            .Replace("-", string.Empty);
    }

    [AttributeUsage(AttributeTargets.Property)]
    private class FigureMappingAttribute : Attribute
    {
        public Figure Figure { get; }
        public bool IsDesign { get; }

        public FigureMappingAttribute(Figure figure, bool isDesign = true)
        {
            Figure = figure;
            IsDesign = isDesign;
        }
    }

    private class HumanDesignRecord
    {
        private static IReadOnlyDictionary<(Figure, bool), PropertyInfo> ValueProps { get; }

        static HumanDesignRecord()
        {
            ValueProps = typeof(HumanDesignRecord)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.PropertyType == typeof(string))
                .Select(x => (Property: x, KeyMapping: x.GetCustomAttribute<FigureMappingAttribute>()))
                .Where(x => x.KeyMapping != null)
                .ToDictionary(x => (x.KeyMapping.Figure, x.KeyMapping.IsDesign), x => x.Property);
        }

        /// ReSharper disable MemberCanBePrivate.Local
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        // ReSharper disable UnusedMember.Local
        // ReSharper disable UnusedMember.Global
        public string Name { get; set; }

        [FigureMapping(Figure.Sun, false)]
        public string PSun { get; set; }
        [FigureMapping(Figure.Earth, false)]
        public string PEarth { get; set; }
        [FigureMapping(Figure.NorthNode, false)]
        public string PNthNode { get; set; }
        [FigureMapping(Figure.SouthNode, false)]
        public string PSthNode { get; set; }
        [FigureMapping(Figure.Moon, false)]
        public string PMoon { get; set; }
        [FigureMapping(Figure.Mercury, false)]
        public string PMercury { get; set; }
        [FigureMapping(Figure.Venus, false)]
        public string PVenus { get; set; }
        [FigureMapping(Figure.Mars, false)]
        public string PMars { get; set; }
        [FigureMapping(Figure.Jupiter, false)]
        public string PJupiter { get; set; }
        [FigureMapping(Figure.Saturn, false)]
        public string PSaturn { get; set; }
        [FigureMapping(Figure.Uranus, false)]
        public string PUranus { get; set; }
        [FigureMapping(Figure.Neptune, false)]
        public string PNeptune { get; set; }
        [FigureMapping(Figure.Pluto, false)]
        public string PPluto { get; set; }

        [FigureMapping(Figure.Sun)]
        public string DSun { get; set; }
        [FigureMapping(Figure.Earth)]
        public string DEarth { get; set; }
        [FigureMapping(Figure.NorthNode)]
        public string DNthNode { get; set; }
        [FigureMapping(Figure.SouthNode)]
        public string DSthNode { get; set; }
        [FigureMapping(Figure.Moon)]
        public string DMoon { get; set; }
        [FigureMapping(Figure.Mercury)]
        public string DMercury { get; set; }
        [FigureMapping(Figure.Venus)]
        public string DVenus { get; set; }
        [FigureMapping(Figure.Mars)]
        public string DMars { get; set; }
        [FigureMapping(Figure.Jupiter)]
        public string DJupiter { get; set; }
        [FigureMapping(Figure.Saturn)]
        public string DSaturn { get; set; }
        [FigureMapping(Figure.Uranus)]
        public string DUranus { get; set; }
        [FigureMapping(Figure.Neptune)]
        public string DNeptune { get; set; }
        [FigureMapping(Figure.Pluto)]
        public string DPluto { get; set; }

        public string Birthplace { get; set; }
        public string TimeOfBirth { get; set; }
        public string Signature { get; set; }

        // ReSharper restore UnusedAutoPropertyAccessor.Local
        // ReSharper restore MemberCanBePrivate.Local
        // ReSharper restore UnusedMember.Local
        // ReSharper restore UnusedMember.Global

        public GeneKeyProfile ToProfile(GeneKeyTable geneKeys)
        {
            var humanDesignChart = PersonalChart.Create(geneKeys.Gates, (figure, isDesign) => this[figure, isDesign]);
            var result = new GeneKeyProfile(Name, humanDesignChart, geneKeys, Birthplace, TimeOfBirthOrNull);

            result.IsSignatureValid = string.Equals(Signature, result.ComputeHash());
            return result;
        }

        public static HumanDesignRecord CreateFrom(GeneKeyProfile source)
        {
            var result = new HumanDesignRecord
            {
                Name = source.Name,
                Birthplace = source.Birthplace,
                TimeOfBirthOrNull = source.TimeOfBirth
            };

            result.Initialize(source.Chart.Values);
            result.Signature = source.ComputeHash();

            return result;
        }

        private DateTime? TimeOfBirthOrNull
        {
            get => DateTime.TryParse(TimeOfBirth, out var timeOfBirth) ? timeOfBirth : default(DateTime?);
            set => TimeOfBirth = value?.ToString();
        }

        private LineOfHexagram this[Figure figure, bool isDesign]
        {
            get => LineOfHexagram.Parse((string)ValueProps[(figure, isDesign)].GetValue(this));
            set => ValueProps[(figure, isDesign)].SetValue(this, value.ToString());
        }

        private void Initialize(IEnumerable<ChartValue> values)
        {
            foreach (var item in values)
            {
                this[item.Marker, item.IsDesign] = item.LineOfHexagram;
            }
        }
    }
}
