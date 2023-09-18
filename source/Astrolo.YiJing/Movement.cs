namespace Astrolo.YiJing
{
    public readonly struct Movement
    {
        public Movement(Line startState, Line endState)
        {
            StartState = startState;
            EndState = endState;
        }

        public Line StartState { get; }

        public Line EndState { get; }

        public bool IsChange => EndState != StartState;
    }
}
