using System;
using TerritoryTools.Vault;

namespace TerritoryShell
{
    class Program
    {
        const string VaultName = "TerritoryWebVault";

        static void Main(string[] args)
        {
            string clientId = args[0];
            string clientSecret = args[1];
            string name = args[2];

            if (args.Length == 3)
            {
                Console.WriteLine($"Getting a vault secret for {name}....");
                var client = new AzureKeyVaultClient(
                    clientId,
                    clientSecret,
                    VaultName);

                string secret = client.GetSecret(name);

                Console.WriteLine($"Secret: {secret}");
            }
            else if (args.Length == 4)
            {
                Console.WriteLine($"Writing a vault secret for {name}....");
                var client = new AzureKeyVaultClient(
                    clientId,
                    clientSecret,
                    VaultName);

                string value = args[3];
                client.WriteSecret(name, value);
            }

            Console.WriteLine("Press any key to quit...");
            Console.ReadKey();
        }
    }
}
