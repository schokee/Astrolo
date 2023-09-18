using Astrolo.GeneKeys;
using Astrolo.Geometry;
using Astrolo.HumanDesign;
using Astrolo.Presentation.Core.Components;
using Astrolo.YiJing;

namespace Astrolo.Explorer.UI.Visualisation.Mandala
{
    public sealed class MandalaFigure : Selectable, IGateConfiguration
    {
        private static readonly Angle Half = Seconds.ToAngle(MandalaGeometry.SecondsPerHexagram / 2);

        public MandalaFigure(IGeneKey geneKey)
        {
            GeneKey = geneKey;
            IsSelected = true;
        }

        public IGeneKey GeneKey { get; }

        public HexagramFigure Hexagram => GeneKey.Hexagram;

        public double StartAngle => GeneKey.Gate.MandalaSlice.StartAngle - Half;

        #region IGateConfiguration

        public IGateInfo Gate => GeneKey.Gate;
        public bool IsActive => IsSelected;
        public GateActivation ActivationState => GateActivation.None;

        #endregion
    }
}
