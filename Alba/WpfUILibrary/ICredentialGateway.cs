namespace AlbaClient.WpfUILibrary
{
    public interface ICredentialGateway
    {
        string AccountName { get; set; }
        string UserName { get; set; }
        string GetKeyValue(string v);
        void Save();
        void SetKeyValue(string v, string password);
    }
}