namespace Astrolo.Explorer.UI.Profiling;

public partial class ComparerView
{
    public static readonly DependencyProperty SubjectProperty = DependencyProperty.RegisterAttached(
        "Subject", typeof(Brush), typeof(ComparerView), new PropertyMetadata(default(Brush)));

    public static void SetSubject(DependencyObject element, Brush value)
    {
        element.SetValue(SubjectProperty, value);
    }

    public static Brush GetSubject(DependencyObject element)
    {
        return (Brush)element.GetValue(SubjectProperty);
    }

    public ComparerView()
    {
        InitializeComponent();
    }
}
