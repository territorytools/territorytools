using System;

namespace TerritoryShell
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Getting a vault secret....");
            string secret = VaultClient.GetSecret();
            Console.WriteLine($"Secret: {secret}");
            Console.WriteLine("Press any key to quit...");
            Console.ReadKey();
        }
    }
}
