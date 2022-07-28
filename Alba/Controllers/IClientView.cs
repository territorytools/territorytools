namespace TerritoryTools.Alba.Controllers
{
    public interface IClientView
    {
        void AppendResultText(string text);

        void ClearResultText();

        string AlbaHostText { get;  }

        string AccountBoxText { get; }
        
        string UserBoxText { get; }

        string PasswordBoxText { get; }

        string UploadDelayMs { get; }

        bool LoadTerritoriesButtonEnabled { set; }

        bool UploadKmlFilesButtonEnabled { set; }

        bool DownloadAllAddressesButtonEnabled { set; }

        bool DownloadTerritoryAssignmentsButtonEnabled { set; }

        bool DownloadUsersButtonEnabled { set; }
        
        bool UploadAddressesButtonEnabled { set; }
        bool DownloadUserInfoButtonEnabled { set; }

        string OpenFileDialog(string fileExt, string title = "");

        string GetFileNameToSaveAs(string defaultFileName, string fileExt);

        void ShowMessageBox(string message);

        void ShowMainWindow();
    }
}
