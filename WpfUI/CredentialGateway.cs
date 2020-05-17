using AlbaClient.WpfUI.Properties;
using TerritoryTools.Entities;

namespace AlbaClient.WpfUI
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
