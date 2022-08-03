using TerritoryTools.Alba.Controllers;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace TerritoryTools.Alba.SyncTool.Library
{
    public partial class MainWindow : Window, IClientView
    {
        private ClientController controller;

        public MainWindow(ICredentialGateway credentialGateway)
        {
            InitializeComponent();
            controller = new ClientController(this);
            albaHostBox.Text = credentialGateway.AlbaHost;
            accountBox.Text = credentialGateway.AccountName;
            accountIdBox.Text = $"{credentialGateway.AccountId}";
            userBox.Text = credentialGateway.UserName;
            CredentialGateway = credentialGateway;
            azureMapsKey.Password = credentialGateway.GetKeyValue("AzureMapsSubscriptionKey");
        }

        public ICredentialGateway CredentialGateway { get; set; }

        private void logonButton_Click(object sender, RoutedEventArgs e)
        {
            CredentialGateway.AlbaHost = albaHostBox.Text;
            CredentialGateway.AccountName = accountBox.Text;
            if (int.TryParse(accountIdBox.Text, out int accountId))
            {
                CredentialGateway.AccountId = accountId;
            }
            else
            {
                ShowMessageBox($"Account ID must be an integer.  You entered {accountIdBox.Text}");
            }

            CredentialGateway.UserName = userBox.Text;
            CredentialGateway.Save();
            controller.LogonButtonClick();
        }

        private void downloadTerritoriesBox_Click(object sender, RoutedEventArgs e)
        {
            controller.LoadTerritoriesButtonClick();
        }

        private void uploadKmlFileButton_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            controller.UploadKmlButtonClick();
            Cursor = Cursors.Arrow;
        }

        private void downloadAllAddressesButton_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            controller.DownloadAllAddressesButtonClick(this.accountIdBox.Text);
            Cursor = Cursors.Arrow;
        }

        private void downloadTerritoryAssignments_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;

            string path = controller.DownloadTerritoriyAssignments();

            if (!string.IsNullOrWhiteSpace(path))
            {
                ProcessStart(path);
            }

            Cursor = Cursors.Arrow;
        }

        private void downloadUsers_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;

            string path = controller.DownloadUsers();

            if (!string.IsNullOrWhiteSpace(path))
            {
                ProcessStart(path);
            }

            Cursor = Cursors.Arrow;
        }

        private void downloadUserInfo_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;

            string path = controller.DownloadUserInfo();

            if (!string.IsNullOrWhiteSpace(path))
            {
                ProcessStart(path);
            }

            Cursor = Cursors.Arrow;
        }

        private void downloadLanguages_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;

            controller.DownloadLanguages();

            Cursor = Cursors.Arrow;
        }

        private void loadLanguages_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;

            controller.LoadLanguages();

            Cursor = Cursors.Arrow;
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            statusBox.Text = string.Empty;
        }

        public void AppendResultText(string text)
        {
            statusBox.Text += text;
        }

        public void ClearResultText()
        {
            statusBox.Text = string.Empty;
        }

        public string AlbaHostText
        {
            get { return albaHostBox.Text; }
        }

        public string AccountBoxText
        {
            get { return accountBox.Text; }
        }

        public string UserBoxText
        {
            get { return userBox.Text; }
        }

        public string PasswordBoxText
        {
            get { return passwordBox.Password; }
        }

        public bool UploadButtonsEnabled
        {
            set 
            { 
                uploadAddressesButton.IsEnabled = value;
                uploadKmlFileButton.IsEnabled = value;
                uploadUsersInfoButton.IsEnabled = value;
            }
        }
  
        public bool DownloadButtonsEnabled
        {
            set 
            {
                loadLanguagesButton.IsEnabled = value;
                downloadTerritoriesButton.IsEnabled = value;
                downloadLanguagesButton.IsEnabled = value;
                downloadAllAddressesButton.IsEnabled = value; 
                downloadTerritoryAssignmentsButton.IsEnabled = value; 
                downloadUsersInfoButton.IsEnabled = value;
                downloadUsersButton.IsEnabled = value;
            }
        }

        public string UploadDelayMs
        {
            get { return uploadDelayMs.Text; }
        }

        private OpenFileDialog openKmlFileDialog = new OpenFileDialog();

        public string OpenFileDialog(string fileExt, string title = "Open File")
        {
            openKmlFileDialog.AddExtension = true;
            openKmlFileDialog.DefaultExt = fileExt;
            openKmlFileDialog.Title = title;

            if (openKmlFileDialog.ShowDialog() ?? false)
                return openKmlFileDialog.FileName;
            else
                return string.Empty;
        }

        private SaveFileDialog saveKmlFileDialog = new SaveFileDialog();

        public string GetFileNameToSaveAs(string defaultFileName, string fileExt)
        {
            saveKmlFileDialog.AddExtension = true;
            saveKmlFileDialog.DefaultExt = fileExt;

            if (string.Equals(fileExt, "csv", StringComparison.OrdinalIgnoreCase))
            {
                saveKmlFileDialog.Filter = "Comma Separated Values|*.csv";
            }
            else if (string.Equals(fileExt, "tsv", StringComparison.OrdinalIgnoreCase))
            {
                saveKmlFileDialog.Filter = "Tab Separated Values|*.tsv";
            }
            else if (string.Equals(fileExt, "txt", StringComparison.OrdinalIgnoreCase))
            {
                saveKmlFileDialog.Filter = "Text File (Tab Separated Values)|*.txt";
            }
            else if (string.Equals(fileExt, "html", StringComparison.OrdinalIgnoreCase))
            {
                saveKmlFileDialog.Filter = "HTML File|*.html";
            }
            else
            {
                saveKmlFileDialog.Filter = "Google Map File|*.kml";
            }

            saveKmlFileDialog.FileName = defaultFileName;

            if (saveKmlFileDialog.ShowDialog() ?? false)
                return $"{saveKmlFileDialog.FileName}";
            else
                return string.Empty;
        }

        public void ShowMessageBox(string message)
        {
            MessageBox.Show(message);
        }

        private void credentialsBoxes_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                controller.credentialsBoxesEnterKeyPressed();
        }

        public void ShowMainWindow()
        {
            this.Show();
        }

        private void importAddressesClick(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;

            controller.ImportAddressButtonClick();
            
            Cursor = Cursors.Arrow;
        }

        public string OpenAddressCsvFileDialog(string fileExt)
        {
            openKmlFileDialog.AddExtension = true;
            openKmlFileDialog.DefaultExt = fileExt;

            if (openKmlFileDialog.ShowDialog() ?? false)
                return openKmlFileDialog.FileName;
            else
                return string.Empty;

        }

        private void geocodeButtonClicked(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            
            controller.GeocodeAddressesClick(
                path: OpenAddressCsvFileDialog("csv"),
                key: azureMapsKey.Password);
            
            Cursor = Cursors.Arrow;
        }
        
        private void azureMapsKeyChanged(object sender, RoutedEventArgs args)
        {
            CredentialGateway.SetKeyValue("AzureMapsSubscriptionKey", azureMapsKey.Password);
        }

        private void accountBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void ProcessStart(string path)
        {
            try
            {
                Process.Start(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;

            controller.UploadUserInfo();

            Cursor = Cursors.Arrow;
        }
    }
}
