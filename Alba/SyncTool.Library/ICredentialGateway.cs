namespace TerritoryTools.Alba.SyncTool.Library
{
    public interface ICredentialGateway
    {
        string AccountName { get; set; }
        string UserName { get; set; }
        string AlbaHost { get; set; }

        string GetKeyValue(string v);
        void Save();
        void SetKeyValue(string v, string password);
    }
}