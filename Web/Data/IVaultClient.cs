namespace TerritoryTools.Vault
{
    public interface IVaultClient
    {
        string GetSecret(string name);
        void WriteSecret(string name, string value);
    }
}
