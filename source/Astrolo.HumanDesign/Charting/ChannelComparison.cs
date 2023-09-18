namespace Astrolo.HumanDesign.Charting
{
    public sealed class ChannelComparison
    {
        private ChannelComparison(IChannel channel, ChannelMatch assessment, params ActiveGate[] gateConfigurations)
        {
            Channel = channel;
            Assessment = assessment;
            Gates = gateConfigurations;
            Activation = gateConfigurations.Aggregate(GateActivation.None, (result, x) => result | x.ActivationState);
        }

        public IChannel Channel { get; }

        public ChannelMatch Assessment { get; }

        public GateActivation Activation { get; }

        public bool HasConflict => Assessment == ChannelMatch.Compromise || Assessment == ChannelMatch.Dominant;

        public IReadOnlyCollection<ActiveGate> Gates { get; }

        public static ChannelComparison? Evaluate(IChannel channel, PersonalChart lhs, PersonalChart rhs)
        {
            var lStart = lhs[channel.StartGate];
            var lEnd = lhs[channel.EndGate];

            var rStart = rhs[channel.StartGate];
            var rEnd = rhs[channel.EndGate];

            var lNotDefined = !lStart.IsActive && !lEnd.IsActive;
            var rNotDefined = !rStart.IsActive && !rEnd.IsActive;

            if (lNotDefined && rNotDefined)
                return null;

            var lIsComplete = lStart.IsActive && lEnd.IsActive;
            var rIsComplete = rStart.IsActive && rEnd.IsActive;

            if (lIsComplete && rIsComplete)
            {
                return new ChannelComparison(channel, ChannelMatch.Friendship, new ActiveGate(lStart, true), new ActiveGate(rEnd, true));
            }

            if (lIsComplete)
            {
                return rNotDefined
                    ? new ChannelComparison(channel, ChannelMatch.Dominant, new ActiveGate(lStart, true), new ActiveGate(lEnd, true))
                    : new ChannelComparison(channel, ChannelMatch.Compromise, new ActiveGate(rStart, false), new ActiveGate(rEnd, false));
            }

            if (rIsComplete)
            {
                return lNotDefined
                    ? new ChannelComparison(channel, ChannelMatch.Dominant, new ActiveGate(rStart, false), new ActiveGate(rEnd, false))
                    : new ChannelComparison(channel, ChannelMatch.Compromise, new ActiveGate(lStart, true), new ActiveGate(lEnd, true));
            }

            if (lStart.IsActive && rEnd.IsActive || lEnd.IsActive && rStart.IsActive)
            {
                return new ChannelComparison(channel, ChannelMatch.Attraction, new ActiveGate(lStart, true), new ActiveGate(rEnd, false));
            }

            if (lStart.IsActive && rStart.IsActive)
            {
                return new ChannelComparison(channel, ChannelMatch.Dominant, new ActiveGate(lStart, true), new ActiveGate(rStart, false));
            }

            if (lEnd.IsActive && rEnd.IsActive)
            {
                return new ChannelComparison(channel, ChannelMatch.Dominant, new ActiveGate(lEnd, true), new ActiveGate(rEnd, false));
            }

            return null;
        }

        public class ActiveGate : IGateConfiguration
        {
            public ActiveGate(IGateConfiguration gate, bool leftSide) : this(gate.Gate, leftSide ? GateActivation.Design : GateActivation.Personality)
            {
            }

            private ActiveGate(IGateInfo gate, GateActivation state)
            {
                Gate = gate;
                ActivationState = state;
            }

            public IGateInfo Gate { get; }
            public bool IsActive => true;
            public GateActivation ActivationState { get; }

            public ActiveGate CombineWith(ActiveGate other)
            {
                return ReferenceEquals(Gate, other.Gate)
                    ? new ActiveGate(Gate, ActivationState | other.ActivationState)
                    : throw new ArgumentException(nameof(other));
            }
        }
    }
}
