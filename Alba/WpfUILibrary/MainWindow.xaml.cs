﻿using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace AlbaClient.WpfUILibrary
{
    public partial class MainWindow : Window, IClientView
    {
        private ClientController controller;

        public MainWindow(ICredentialGateway credentialGateway)
        {
            InitializeComponent();
            controller = new ClientController(this);
            accountBox.Text = credentialGateway.AccountName;
            userBox.Text = credentialGateway.UserName;
            CredentialGateway = credentialGateway;
            azureMapsKey.Password = credentialGateway.GetKeyValue("AzureMapsSubscriptionKey");
        }

        public ICredentialGateway CredentialGateway { get; set; }

        private void logonButton_Click(object sender, RoutedEventArgs e)
        {
            CredentialGateway.AccountName = accountBox.Text;
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
            controller.DownloadAllAddressesButtonClick();
            Cursor = Cursors.Arrow;
        }

        private void downloadTerritoryAssignments_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;

            string path = controller.DownloadTerritoriyAssignments();

            if (!string.IsNullOrWhiteSpace(path))
            {
                Process.Start(path);
            }

            Cursor = Cursors.Arrow;
        }

        private void downloadUsers_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;

            string path = controller.DownloadUsers();

            if (!string.IsNullOrWhiteSpace(path))
            {
                Process.Start(path);
            }

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

        public bool LoadTerritoriesButtonEnabled
        {
            set { downloadTerritoriesButton.IsEnabled = value; }
        }

        public bool UploadKmlFilesButtonEnabled
        {
            set { uploadKmlFileButton.IsEnabled = value; }
        }

        public bool UploadAddressesButtonEnabled
        {
            set { uploadAddressesButton.IsEnabled = value; }
        }


        public bool DownloadAllAddressesButtonEnabled
        {
            set { downloadAllAddressesButton.IsEnabled = value; }
        }

        public bool DownloadTerritoryAssignmentsButtonEnabled
        {
            set { downloadTerritoryAssignmentsButton.IsEnabled = value; }
        }

        public bool DownloadUsersButtonEnabled
        {
            set { downloadUsersButton.IsEnabled = value; }
        }

        public string UploadDelayMs
        {
            get { return uploadDelayMs.Text; }
        }

        private OpenFileDialog openKmlFileDialog = new OpenFileDialog();

        public string OpenKmlFileDialog(string fileExt)
        {
            openKmlFileDialog.AddExtension = true;
            openKmlFileDialog.DefaultExt = "csv";

            if (openKmlFileDialog.ShowDialog() ?? false)
                return $"{openKmlFileDialog.FileName}.{openKmlFileDialog.DefaultExt}";
            else
                return string.Empty;
        }

        private SaveFileDialog saveKmlFileDialog = new SaveFileDialog();

        public string GetKmlFileNameToSaveAs(string defaultFileName, string fileExt)
        {
            saveKmlFileDialog.AddExtension = true;
            saveKmlFileDialog.DefaultExt = fileExt;

            if (string.Equals(fileExt, "csv", StringComparison.OrdinalIgnoreCase))
            {
                saveKmlFileDialog.Filter = "Comma Separated Values|*.csv";
            }
            else if (string.Equals(fileExt, "txt", StringComparison.OrdinalIgnoreCase))
            {
                saveKmlFileDialog.Filter = "Tab Separated Values|*.txt";
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

            controller.ImportAddressButtonClick(
                OpenAddressCsvFileDialog("csv"));
            
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
    }
}
