using System;

namespace TerritoryShell
{
    class Program
    {
        static void Main(string[] args)
        {
            string clientId = args[0];
            string clientSecret = args[1];
            string name = args[2];

            if (args.Length == 3)
            {
                Console.WriteLine($"Getting a vault secret for {name}....");
                string secret = AzureKeyVaultClient.GetSecret(clientId, clientSecret, name);

                Console.WriteLine($"Secret: {secret}");
            }
            else if (args.Length == 4)
            {
                Console.WriteLine($"Writing a vault secret for {name}....");
                
                string value = args[3];
                AzureKeyVaultClient.WriteSecret(clientId, clientSecret, name, value);
            }

            Console.WriteLine("Press any key to quit...");
            Console.ReadKey();
        }
    }
}
