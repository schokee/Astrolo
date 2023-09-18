using Astrolo.HumanDesign;
using Astrolo.Presentation.Controls.Halo;
using Astrolo.Presentation.Controls.YiJing;

namespace Astrolo.Presentation.Controls.HumanDesign
{
    [TemplatePart(Name = "PART_Gridlines", Type = typeof(ConcentricPanel))]
    [TemplatePart(Name = "PART_Labels", Type = typeof(OrbitPanel))]
    [TemplatePart(Name = "PART_Figures", Type = typeof(OrbitPanel))]
    public class RaveChart : Control
    {
        private static readonly Lazy<IReadOnlyList<IGateInfo>> AllGates;

        static RaveChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RaveChart), new FrameworkPropertyMetadata(typeof(RaveChart)));
            AllGates = new Lazy<IReadOnlyList<IGateInfo>>(() => GateDictionary.Create().InTransitOrder.ToList());
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("PART_Gridlines") is ConcentricPanel backPanel)
            {
                foreach (var gate in AllGates.Value)
                {
                    var location = gate.MandalaSlice;

                    if (gate.Number == 2)
                    {
                        var s1 = new HaloArc
                        {
                            Stroke = Brushes.Gold,
                            Angle = location.StartAngle,
                            Spread = location.SweepAngle,
                            StrokeThickness = 24,
                        };

                        backPanel.Children.Add(s1);
                        ConcentricPanel.SetBand(s1, 2);

                        var s2 = new HaloSlice
                        {
                            Fill = Brushes.Gold,
                            Angle = location.StartAngle,
                            Spread = location.SweepAngle
                        };

                        backPanel.Children.Add(s2);
                        ConcentricPanel.SetBand(s2, 4);

                    }

                    var ray = new HaloRay
                    {
                        Angle = location.StartAngle,
                        StrokeThickness = 1,
                        Stroke = Brushes.Gainsboro
                    };

                    ConcentricPanel.SetBand(ray, 1);

                    backPanel.Children.Add(ray);
                }
            }

            if (GetTemplateChild("PART_Labels") is OrbitPanel labelPanel)
            {
                var font = new FontFamily("Verdana");

                foreach (var gate in AllGates.Value)
                {
                    var location = gate.MandalaSlice;

                    var figure = new TextBlock
                    {
                        FontFamily = font,
                        Foreground = Brushes.DimGray,
                        Text = gate.Number.ToString(),
                        Margin = new Thickness(5),
                        //FontWeight = FontWeights.SemiBold,
                        VerticalAlignment = VerticalAlignment.Center
                    };


                    OrbitPanel.SetAngle(figure, location.StartAngle + location.SweepAngle / 2);
                    OrbitPanel.SetTranslateOnly(figure, true);

                    labelPanel.Children.Add(figure);


                }
            }

            if (GetTemplateChild("PART_Figures") is OrbitPanel forePanel)
            {
                foreach (var gate in AllGates.Value)
                {
                    var location = gate.MandalaSlice;

                    var figure = new Border
                    {
                        Background = Brushes.Transparent,
                        ToolTip = gate.Number.ToString(),
                        Child = new HexagramShape
                        {
                            StrokeThickness = 1.3,
                            Spacer = 0.2f,
                            Fill = Brushes.DimGray,
                            Width = 17,
                            Height = 20,
                            Hexagram = gate.Hexagram
                        }
                    };

                    OrbitPanel.SetAngle(figure, location.StartAngle + location.SweepAngle / 2);
                    OrbitPanel.SetTranslateOnly(figure, true);

                    forePanel.Children.Add(figure);
                }
            }
        }
    }
}
