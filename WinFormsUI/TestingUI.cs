using System;
using System.Windows.Forms;

namespace AlbaClient.WinFormsUI
{
    public partial class TestingUI : Form, IClientView
    {
        private ClientController controller;

        public TestingUI()
        {
            InitializeComponent();
            controller = new ClientController(this);
            saveFileDialog.DefaultExt = ".kml";
        }

        private void logonButton_Click(object sender, EventArgs e)
        {
            controller.LogonButtonClick();
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            testResults.Text = null;
        }

        private void loadTerritoriesButton_Click(object sender, EventArgs e)
        {
            controller.LoadTerritoriesButtonClick();
        }

        private void openKmlButton_Click(object sender, EventArgs e)
        {
            controller.UploadKmlButtonClick();
        }

        public void AppendResultText(string text)
        {
            testResults.Text += text;
        }

        public void ClearResultText()
        {
            testResults.Text = string.Empty;
        }

        public string AccountBoxText
        {
            get { return this.accountBox.Text; }
        }

        public string UserBoxText
        {
            get { return this.userBox.Text; }
        }

        public string PasswordBoxText
        {
            get { return this.passwordBox.Text; }
        }

        public bool LoadTerritoriesButtonEnabled
        {
            set { this.loadTerritoriesButton.Enabled = value; }
        }

        public bool UploadKmlFilesButtonEnabled
        {
            set { this.openKmlButton.Enabled = value; }
        }

        public string UploadDelayMs
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool DownloadAllAddressesButtonEnabled { set => throw new NotImplementedException(); }
        public bool DownloadTerritoryAssignmentsButtonEnabled { set => throw new NotImplementedException(); }
        public bool DownloadUsersButtonEnabled { set => throw new NotImplementedException(); }

        public string OpenKmlFileDialog(string fileExt)
        {
            if (openKmlFileDialog.ShowDialog() == DialogResult.OK)
                return openKmlFileDialog.FileName;
            else
                return string.Empty;
        }

        public string GetKmlFileNameToSaveAs(string defaultFileName, string fileExt)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                return saveFileDialog.FileName;
            else
                return string.Empty;
        }

        public void ShowMessageBox(string message)
        {
            MessageBox.Show(message);
        }

        public void ShowMainWindow()
        {
            throw new NotImplementedException();
        }
    }
}
