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

        bool UploadButtonsEnabled { set; }
        bool DownloadButtonsEnabled { set; }

        string OpenFileDialog(string fileExt, string title = "");

        string GetFileNameToSaveAs(string defaultFileName, string fileExt);

        void ShowMessageBox(string message);

        void ShowMainWindow();
    }
}
