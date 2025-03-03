using Astrolo.HumanDesign;

namespace Astrolo.Presentation.Controls.HumanDesign
{
    public class BodyChannel : Control
    {
        static BodyChannel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BodyChannel), new FrameworkPropertyMetadata(typeof(BodyChannel)));
        }

        public static readonly DependencyProperty DesignProperty = DependencyProperty.Register(
            nameof(Design), typeof(Brush), typeof(BodyChannel), new PropertyMetadata(default(Brush)));

        public Brush Design
        {
            get => (Brush)GetValue(DesignProperty);
            set => SetValue(DesignProperty, value);
        }

        public static readonly DependencyProperty GateProperty = DependencyProperty.Register(
            nameof(Gate), typeof(int), typeof(BodyChannel), new PropertyMetadata(default(int)));

        public int Gate
        {
            get => (int)GetValue(GateProperty);
            set => SetValue(GateProperty, value);
        }

        public static readonly DependencyProperty ActivationStateProperty = DependencyProperty.Register(
            nameof(ActivationState), typeof(GateActivation), typeof(BodyChannel), new PropertyMetadata(default(GateActivation), OnActivationStateChanged));

        public GateActivation ActivationState
        {
            get => (GateActivation)GetValue(ActivationStateProperty);
            set => SetValue(ActivationStateProperty, value);
        }

        private static void OnActivationStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BodyChannel)d).IsSplit = (GateActivation)e.NewValue == GateActivation.Both;
        }

        private static readonly DependencyPropertyKey IsSplitPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(IsSplit), typeof(bool), typeof(BodyChannel), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsSplitProperty = IsSplitPropertyKey.DependencyProperty;

        public bool IsSplit
        {
            get => (bool)GetValue(IsSplitProperty);
            private set => SetValue(IsSplitPropertyKey, value);
        }

        public static readonly DependencyProperty ChannelGeometryProperty = DependencyProperty.Register(
            nameof(ChannelGeometry), typeof(PathGeometry), typeof(BodyChannel), new PropertyMetadata(default(PathGeometry)));

        public PathGeometry ChannelGeometry
        {
            get => (PathGeometry)GetValue(ChannelGeometryProperty);
            set => SetValue(ChannelGeometryProperty, value);
        }

        public static readonly DependencyProperty SplitGeometryProperty = DependencyProperty.Register(
            nameof(SplitGeometry), typeof(PathGeometry), typeof(BodyChannel), new PropertyMetadata(default(PathGeometry)));

        public PathGeometry SplitGeometry
        {
            get => (PathGeometry)GetValue(SplitGeometryProperty);
            set => SetValue(SplitGeometryProperty, value);
        }
    }
}
