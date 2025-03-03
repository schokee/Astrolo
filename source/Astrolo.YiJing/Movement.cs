namespace Astrolo.YiJing;

public readonly struct Movement(Line startState, Line endState)
{
    public Line StartState { get; } = startState;

    public Line EndState { get; } = endState;

    public bool IsChange => EndState != StartState;
}
