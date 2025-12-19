using Astrolo.Explorer.Components;

namespace Astrolo.Explorer.Model;

public class Filter<T>(string label, Func<T, bool> predicate = null) : Selectable
{
    public string Label { get; } = label;

    public virtual bool Includes(T candidate)
    {
        return predicate?.Invoke(candidate) != false;
    }

    public override string ToString()
    {
        return Label;
    }
}
