using Astrolo.YiJing;
using Caliburn.Micro;

namespace Astrolo.Explorer.UI.Cleromancy;

public sealed class Change : PropertyChangedBase
{
    private bool _startState;
    private bool _endState;

    public event EventHandler StateChanged;

    public Change(Movement movement)
    {
        StartState = movement.StartState.IsYang;
        EndState = movement.EndState.IsYang;
    }

    public bool StartState
    {
        get => _startState;
        set
        {
            if (value.Equals(_startState)) return;
            _startState = value;
            NotifyOfPropertyChange();
            NotifyOfPropertyChange(nameof(IsMoving));
            StateChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool EndState
    {
        get => _endState;
        set
        {
            if (value == _endState) return;
            _endState = value;
            NotifyOfPropertyChange();
            NotifyOfPropertyChange(nameof(IsMoving));
            StateChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool IsMoving => StartState != EndState;
}
