using AlbaClient.WpfUILibrary;
using System.Windows;

namespace AlbaClient.WpfUI
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
