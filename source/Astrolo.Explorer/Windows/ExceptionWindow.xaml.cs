namespace Astrolo.Explorer.Windows;

public partial class ExceptionWindow
{
    public ExceptionWindow()
    {
        InitializeComponent();
    }

    public static void Show(Exception exception, string info)
    {
        var window = new ExceptionWindow
        {
            Message = {Text = exception.Message},
            Details = {Text = info ?? exception.ToString()}
        };

        window.ShowDialog();
    }
}
