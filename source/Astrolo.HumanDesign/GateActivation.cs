namespace Astrolo.HumanDesign
{
    // TODO: Design and Personality should be made more generic (e.g. Left/Right). Maybe should be 'channel state'??

    [Flags]
    public enum GateActivation
    {
        None,
        Design,
        Personality,
        Both = Design | Personality
    }
}
