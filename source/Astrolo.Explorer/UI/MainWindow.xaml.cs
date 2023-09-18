using Astrolo.Explorer.Properties;
using Astrolo.Presentation.Core;

namespace Astrolo.Explorer.UI
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            Closing += delegate
            {
                Settings.Default.MainWindowPlacement = this.GetPlacement();
                Settings.Default.Save();
            };
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.SetPlacement(Settings.Default.MainWindowPlacement);
        }
    }
}
