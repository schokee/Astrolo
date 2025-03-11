namespace Astrolo.Explorer.UI.Profiling;

public interface IProfileDirectory : IReadOnlyCollection<INamedProfile>
{
    bool IsNameUnique(string name);

    INamedProfile AddNew(string name);
}
