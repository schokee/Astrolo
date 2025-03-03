using Astrolo.Explorer.Components;

namespace Astrolo.Explorer.Model;

public class Filter<T> : Selectable
{
    private readonly Func<T, bool> _predicate;
    private string _label;

    public Filter(string label, Func<T, bool> predicate = null)
    {
        Label = label;
        _predicate = predicate;
    }

    public string Label
    {
        get => _label;
        set
        {
            if (value == _label) return;
            _label = value;
            NotifyOfPropertyChange();
        }
    }

    public virtual bool Includes(T candidate)
    {
        return _predicate?.Invoke(candidate) != false;
    }

    public override string ToString()
    {
        return Label;
    }
}
