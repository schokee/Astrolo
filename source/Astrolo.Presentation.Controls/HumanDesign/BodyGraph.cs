using Astrolo.HumanDesign;
using Astrolo.HumanDesign.Charting;

namespace Astrolo.Presentation.Controls.HumanDesign
{
    public class BodyGraph : Control
    {
        static BodyGraph()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BodyGraph), new FrameworkPropertyMetadata(typeof(BodyGraph)));
        }

        public static readonly DependencyProperty ChartProperty = DependencyProperty.Register(nameof(Chart), typeof(Chart),
            typeof(BodyGraph), new FrameworkPropertyMetadata(default(Chart)));

        public Chart Chart
        {
            get => (Chart)GetValue(ChartProperty);
            set => SetValue(ChartProperty, value);
        }

        public static readonly DependencyProperty GateTemplateProperty = DependencyProperty.Register(
            nameof(GateTemplate), typeof(DataTemplate), typeof(BodyGraph), new PropertyMetadata(default(DataTemplate)));

        public DataTemplate GateTemplate
        {
            get => (DataTemplate)GetValue(GateTemplateProperty);
            set => SetValue(GateTemplateProperty, value);
        }

        public static readonly DependencyProperty ShowChannelsProperty = DependencyProperty.Register(
            nameof(ShowChannels), typeof(bool), typeof(BodyGraph), new PropertyMetadata(default(bool)));

        public bool ShowChannels
        {
            get => (bool)GetValue(ShowChannelsProperty);
            set => SetValue(ShowChannelsProperty, value);
        }

        public static readonly DependencyProperty ShowGatesProperty = DependencyProperty.Register(
            nameof(ShowGates), typeof(bool), typeof(BodyGraph), new PropertyMetadata(default(bool)));

        public bool ShowGates
        {
            get => (bool)GetValue(ShowGatesProperty);
            set => SetValue(ShowGatesProperty, value);
        }

        public static readonly DependencyProperty DesignProperty = DependencyProperty.Register(
            nameof(Design), typeof(Brush), typeof(BodyGraph), new PropertyMetadata(default(Brush)));

        public Brush Design
        {
            get => (Brush)GetValue(DesignProperty);
            set => SetValue(DesignProperty, value);
        }

        public static readonly DependencyProperty PersonalityProperty = DependencyProperty.Register(
            nameof(Personality), typeof(Brush), typeof(BodyGraph), new PropertyMetadata(default(Brush)));

        public Brush Personality
        {
            get => (Brush)GetValue(PersonalityProperty);
            set => SetValue(PersonalityProperty, value);
        }

        #region Gate Property

        public static readonly DependencyProperty GateProperty = DependencyProperty.RegisterAttached(
            "Gate", typeof(int), typeof(BodyGraph), new PropertyMetadata(default(int)));

        public static void SetGate(DependencyObject element, int value)
        {
            element.SetValue(GateProperty, value);
        }

        public static int GetGate(DependencyObject element)
        {
            return (int)element.GetValue(GateProperty);
        }

        #endregion

        #region Center Property

        public static readonly DependencyProperty CenterProperty = DependencyProperty.RegisterAttached(
            "Center", typeof(Center), typeof(BodyGraph), new PropertyMetadata(default(Center)));

        public static void SetCenter(DependencyObject element, Center value)
        {
            element.SetValue(CenterProperty, value);
        }

        public static Center GetCenter(DependencyObject element)
        {
            return (Center)element.GetValue(CenterProperty);
        }

        #endregion

        #region ActivationState Property

        public static readonly DependencyProperty ActivationStateProperty = DependencyProperty.RegisterAttached(
            "ActivationState", typeof(CenterState), typeof(BodyGraph), new PropertyMetadata(CenterState.Open));

        public static void SetActivationState(DependencyObject element, bool value)
        {
            element.SetValue(ActivationStateProperty, value);
        }

        public static CenterState GetActivationState(DependencyObject element)
        {
            return (CenterState)element.GetValue(ActivationStateProperty);
        }

        #endregion

        public override void OnApplyTemplate()
        {
            foreach (var channel in GetChildrenOfType<BodyChannel>("PART_Channels"))
            {
                BindingOperations.SetBinding(channel, BodyChannel.DesignProperty, new Binding
                {
                    Path = new PropertyPath(nameof(Design)),
                    Source = this,
                    Mode = BindingMode.OneWay
                });

                BindingOperations.SetBinding(channel, ForegroundProperty, new Binding
                {
                    Path = new PropertyPath(nameof(Personality)),
                    Source = this,
                    Mode = BindingMode.OneWay
                });
            }

            foreach (var child in GetChildrenOfType<ContentPresenter>("PART_Gates"))
            {
                var gate = GetGate(child);
                if (gate <= 0)
                    continue;

                BindingOperations.SetBinding(child, ContentPresenter.ContentTemplateProperty, new Binding
                {
                    Path = new PropertyPath(nameof(GateTemplate)),
                    RelativeSource = RelativeSource.TemplatedParent,
                    Mode = BindingMode.OneWay
                });

                BindingOperations.SetBinding(child, ContentPresenter.ContentProperty, new Binding
                {
                    Path = new PropertyPath($"Chart[{gate}]"),
                    RelativeSource = RelativeSource.TemplatedParent,
                    Mode = BindingMode.OneWay
                });
            }

            foreach (var child in GetChildrenOfType<Shape>("PART_Centers"))
            {
                var center = GetCenter(child);

                BindingOperations.SetBinding(child, ActivationStateProperty, new Binding
                {
                    Path = new PropertyPath($"Chart.Centers[{center}]"),
                    RelativeSource = RelativeSource.TemplatedParent,
                    Mode = BindingMode.OneWay
                });
            }
        }

        private IEnumerable<T> GetChildrenOfType<T>(string panelName)
        {
            return GetTemplateChild(panelName) is Panel panel
                ? panel.Children.OfType<T>()
                : [];
        }
    }
}
