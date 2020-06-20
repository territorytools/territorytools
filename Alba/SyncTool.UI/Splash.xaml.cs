using Alba.SyncTool.Library;
using System.Windows;

namespace Alba.SyncTool.UI
{
    /// <summary>
    /// Interaction logic for Splash.xaml
    /// </summary>
    public partial class Splash : Window
    {
        public Splash()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Hide();

            new MainWindow(new CredentialGateway()).Show();

            Close();
        }
    }
}
