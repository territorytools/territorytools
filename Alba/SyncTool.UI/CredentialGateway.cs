using TerritoryTools.Alba.SyncTool.UI.Properties;
using TerritoryTools.Alba.SyncTool.Library;

namespace TerritoryTools.Alba.SyncTool.UI
{
    public class CredentialGateway : ICredentialGateway
    {
        public string AccountName
        {
            get
            {
                return Settings.Default.AccountName;
            }
            set
            {
                Settings.Default.AccountName = value;
            }
        }

        public int AccountId
        {
            get
            {
                return Settings.Default.AccountId;
            }
            set
            {
                Settings.Default.AccountId = value;
            }
        }

        public string UserName
        {
            get
            {
                return Settings.Default.UserName;
            }
            set
            {
                Settings.Default.UserName = value;
            }
        }

        public string AlbaHost 
        {
            get
            {
                return Settings.Default.AlbaHost;
            }
            set
            {
                Settings.Default.AlbaHost = value;
            }
        }

        public string GetKeyValue(string key)
        {
            return Settings.Default[key] as string;
        }

        public void SetKeyValue(string key, string value)
        {
            Settings.Default[key] = value;
        }


        public void Save()
        {
            Settings.Default.Save();
        }
    }
}
