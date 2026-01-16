using Astrolo.YiJing;
using Caliburn.Micro;

namespace Astrolo.Explorer.UI.Cleromancy;

public sealed class Change : PropertyChangedBase
{
    public event EventHandler StateChanged;

    public Change(Movement movement)
    {
        StartState = movement.StartState.IsYang;
        EndState = movement.EndState.IsYang;
    }

    public bool StartState
    {
        get;
        set
        {
            if (Set(ref field, value))
            {
                NotifyOfPropertyChange(nameof(IsMoving));
                StateChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public bool EndState
    {
        get;
        set
        {
            if (Set(ref field, value))
            {
                NotifyOfPropertyChange(nameof(IsMoving));
                StateChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public bool IsMoving => StartState != EndState;
}
