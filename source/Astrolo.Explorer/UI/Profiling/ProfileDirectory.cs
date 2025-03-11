using System.Collections.ObjectModel;
using System.IO;
using System.Reactive.Linq;
using Astrolo.GeneKeys;
using Caliburn.Micro;
using MoreLinq.Extensions;
using Newtonsoft.Json;

namespace Astrolo.Explorer.UI.Profiling;

public sealed class ProfileDirectory : ObservableCollection<INamedProfile>, IProfileDirectory, IDisposable
{
    private static JsonSerializer Serializer { get; } = new();

    private readonly IDisposable _shutdown;
    private readonly GeneKeyTable _geneKeys;

    private event System.Action SaveRequested;

    public ProfileDirectory(GeneKeyTable geneKeys)
    {
        var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Astrolo");
        Directory.CreateDirectory(folder);

        FullPath = Path.Combine(folder, "astrolo-charts.json");

        _geneKeys = geneKeys;
        _shutdown = Observable
            .FromEvent(h => SaveRequested += h, h => SaveRequested -= h)
            .Throttle(TimeSpan.FromMilliseconds(500))
            .Subscribe(_ => Save());

        if (Load())
        {
            return;
        }

        Add(new ProfileDto
        {
            Name = "John Lennon",
            Points =
                "53.4;54.4;57.1;51.1;6.6;31.4;12.6;31.3;24.4;24.6;8.1;6.1;31.1;" +
                "57.2;51.2;48.2;21.2;41.2;44.1;59.4;46.5;2.1;24.6;8.2;6.4;31.3"
        });
    }

    public void Dispose()
    {
        _shutdown.Dispose();
    }

    public string FullPath { get; }

    public bool IsNameUnique(string name)
    {
        return string.IsNullOrWhiteSpace(name)
            ? throw new ArgumentException("Name cannot be blank", nameof(name))
            : this.All(x => !x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public INamedProfile AddNew(string name)
    {
        var newChart = IsNameUnique(name)
            ? CreateChart(name)
            : throw new ArgumentException("Name must be unique", nameof(name));

        Add(newChart);
        RequestSave();

        return newChart;
    }

    public override string ToString()
    {
        return FullPath;
    }

    private void RequestSave()
    {
        SaveRequested?.Invoke();
    }

    private INamedProfile CreateChart(string name)
    {
        var newChart = IsNameUnique(name)
            ? new NamedProfile(this, name, new ProfileEditor(n => _geneKeys[n]))
            : throw new ArgumentException("Name must be unique", nameof(name));

        return newChart;
    }

    private void Add(ProfileDto profileDto)
    {
        Add(profileDto.ToNamedProfile(this));
    }

    private bool Load()
    {
        this
            .OfType<IDisposable>()
            .ForEach(x => x.Dispose());

        Clear();

        if (File.Exists(FullPath))
        {
            try
            {
                using var stream = File.OpenRead(FullPath);

                LoadFrom(stream);
                return true;
            }
            catch
            {
                // Ignore
            }
        }

        return false;
    }

    private void Save()
    {
        try
        {
            using var stream = File.Create(FullPath);
            SaveTo(stream);
        }
        catch
        {
            // Ignore
        }
    }

    private void LoadFrom(Stream stream)
    {
        using var reader = new StreamReader(stream);
        using var jsonReader = new JsonTextReader(reader);

        var profiles = Serializer
            .Deserialize<List<ProfileDto>>(jsonReader)
            .DistinctBy(x => x.Name);

        foreach (var profileDto in profiles)
        {
            Add(profileDto);
        }
    }

    private void SaveTo(Stream stream)
    {
        var document = this
            .OrderBy(x => x.Name)
            .Select(ProfileDto.CreateFrom)
            .ToArray();

        using var writer = new StreamWriter(stream);
        using var jsonWriter = new JsonTextWriter(writer);

        jsonWriter.Formatting = Formatting.Indented;
        Serializer.Serialize(jsonWriter, document);
    }

    private sealed class NamedProfile : PropertyChangedBase, INamedProfile, IDisposable
    {
        private readonly ProfileDirectory _directory;
        private string _name;

        public NamedProfile(ProfileDirectory directory, string name, ProfileEditor editor)
        {
            _directory = directory;
            _name = name;

            Editor = editor;
            Editor.ProfileChanged += _directory.RequestSave;
        }

        public void Dispose()
        {
            Editor.ProfileChanged -= _directory.RequestSave;
        }

        public string Name
        {
            get => _name;
            set
            {
                var newName = value?.Trim() ?? throw new ArgumentNullException(nameof(value), "Name cannot be null");

                if (!Name.Equals(newName) && Set(ref _name, value))
                {
                    _directory.RequestSave();
                }
            }
        }

        public ProfileEditor Editor { get; }

        public void Delete()
        {
            _directory.Remove(this);
            _directory.RequestSave();
        }

        public override string ToString()
        {
            return Name;
        }
    }

    private class ProfileDto
    {
        public string Name { get; set; }

        public string Points { get; set; }

        public INamedProfile ToNamedProfile(ProfileDirectory source)
        {
            var result = source.CreateChart(Name);

            result.Editor.InitializeFrom(Points);
            return result;
        }


        public static ProfileDto CreateFrom(INamedProfile namedProfile)
        {
            return new ProfileDto
            {
                Name = namedProfile.Name,
                Points = string.Join(ProfileEditor.Delimiter, namedProfile.Editor.Points.Select(x => x.Value?.ToString() ?? string.Empty))
            };
        }
    }
}
