namespace Astrolo.Explorer.UI.Profiling
{
    public interface INamedProfile
    {
        public string Name { get; set; }

        public ProfileEditor Editor { get; }

        public void Delete();
    }
}
