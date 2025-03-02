using Astrolo.Explorer.Components;
using Astrolo.GeneKeys;
using Astrolo.Geometry;
using Astrolo.HumanDesign;
using Astrolo.YiJing;

namespace Astrolo.Explorer.UI.Visualisation.Mandala;

public sealed class MandalaFigure : Selectable, IGateConfiguration
{
    private static readonly Angle Half = Seconds.ToAngle(MandalaGeometry.SecondsPerHexagram / 2);
    private bool _isEmphasized;

    public MandalaFigure(IGeneKey geneKey)
    {
        GeneKey = geneKey;
        IsSelected = true;
    }

    public IGeneKey GeneKey { get; }

    public HexagramFigure Hexagram => GeneKey.Hexagram;

    public double StartAngle => GeneKey.Gate.MandalaSlice.StartAngle - Half;

    public bool IsEmphasized
    {
        get => _isEmphasized;
        set => Set(ref _isEmphasized, value);
    }

    #region IGateConfiguration

    public IGateInfo Gate => GeneKey.Gate;
    public bool IsActive => IsSelected;
    public GateActivation ActivationState => IsSelected ? GateActivation.Personality : GateActivation.None;

    #endregion

    protected override void OnSelectionChanged()
    {
        base.OnSelectionChanged();
        NotifyOfPropertyChange(nameof(IsActive));
        NotifyOfPropertyChange(nameof(ActivationState));
    }
}
