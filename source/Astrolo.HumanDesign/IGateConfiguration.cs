namespace Astrolo.HumanDesign
{
    public interface IGateConfiguration
    {
        IGateInfo Gate { get; }

        bool IsActive { get; }

        // Currently only used for rendering channels.
        GateActivation ActivationState { get; }
    }
}
