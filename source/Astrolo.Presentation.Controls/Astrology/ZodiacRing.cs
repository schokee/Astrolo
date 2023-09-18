using Astrolo.Astrology;
using Astrolo.Geometry;
using Astrolo.Presentation.Controls.Halo;

namespace Astrolo.Presentation.Controls.Astrology
{
    [TemplatePart(Name = "PART_Ring", Type = typeof(Panel))]
    [TemplatePart(Name = "PART_Icons", Type = typeof(OrbitPanel))]
    public sealed class ZodiacRing : Control
    {
        static ZodiacRing()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ZodiacRing), new FrameworkPropertyMetadata(typeof(ZodiacRing)));
        }

        public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register(
            nameof(Thickness), typeof(double), typeof(ZodiacRing), new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public double Thickness
        {
            get => (double)GetValue(ThicknessProperty);
            set => SetValue(ThicknessProperty, value);
        }

        public static readonly DependencyProperty IconSizeProperty = DependencyProperty.Register(
            nameof(IconSize), typeof(double), typeof(ZodiacRing), new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public double IconSize
        {
            get => (double)GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }

        public static readonly DependencyProperty EarthProperty = DependencyProperty.Register(
            nameof(Earth), typeof(Brush), typeof(ZodiacRing), new PropertyMetadata(default(Brush)));

        public Brush Earth
        {
            get => (Brush)GetValue(EarthProperty);
            set => SetValue(EarthProperty, value);
        }

        public static readonly DependencyProperty FireProperty = DependencyProperty.Register(
            nameof(Fire), typeof(Brush), typeof(ZodiacRing), new PropertyMetadata(default(Brush)));

        public Brush Fire
        {
            get => (Brush)GetValue(FireProperty);
            set => SetValue(FireProperty, value);
        }

        public static readonly DependencyProperty WaterProperty = DependencyProperty.Register(
            nameof(Water), typeof(Brush), typeof(ZodiacRing), new PropertyMetadata(default(Brush)));

        public Brush Water
        {
            get => (Brush)GetValue(WaterProperty);
            set => SetValue(WaterProperty, value);
        }

        public static readonly DependencyProperty AirProperty = DependencyProperty.Register(
            nameof(Air), typeof(Brush), typeof(ZodiacRing), new PropertyMetadata(default(Brush)));

        public Brush Air
        {
            get => (Brush)GetValue(AirProperty);
            set => SetValue(AirProperty, value);
        }

        public override void OnApplyTemplate()
        {
            var midAngle = Angle.FromDegrees(ChartMetrics.DegreesPerSign / 2d);

            if (GetTemplateChild("PART_Ring") is Panel background)
            {
                foreach (var info in Zodiac.AllSigns)
                {
                    var arc = new HaloArc
                    {
                        Angle = ChartMetrics.StartAngle(info.Sign),
                        Spread = ChartMetrics.DegreesPerSign,
                        ToolTip = info.ToString()
                    };

                    BindingOperations.SetBinding(arc, Shape.StrokeProperty, new Binding
                    {
                        Source = this,
                        Path = new PropertyPath(info.Element.ToString())
                    });

                    BindingOperations.SetBinding(arc, Shape.StrokeThicknessProperty, new Binding
                    {
                        Source = this,
                        Path = new PropertyPath(nameof(Thickness))
                    });

                    background.Children.Add(arc);
                }
            }

            if (GetTemplateChild("PART_Icons") is OrbitPanel panel)
            {
                foreach (var info in Zodiac.AllSigns)
                {
                    var icon = new ZodiacIcon
                    {
                        IsHitTestVisible = false,
                        Sign = info.Sign,
                        Stretch = Stretch.Uniform
                    };

                    BindingOperations.SetBinding(icon, Shape.FillProperty, new Binding
                    {
                        Source = this,
                        Path = new PropertyPath(nameof(Foreground))
                    });

                    BindingOperations.SetBinding(icon, WidthProperty, new Binding
                    {
                        Source = this,
                        Path = new PropertyPath(nameof(IconSize))
                    });

                    BindingOperations.SetBinding(icon, HeightProperty, new Binding
                    {
                        Source = this,
                        Path = new PropertyPath(nameof(IconSize))
                    });

                    OrbitPanel.SetAngle(icon, ChartMetrics.StartAngle(info.Sign) + midAngle);

                    panel.Children.Add(icon);
                }
            }
        }
    }
}
