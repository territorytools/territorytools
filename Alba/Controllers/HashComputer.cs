using System.Security.Cryptography;
using System.Text;

namespace AlbaClient
{
    public class HashComputer
    {
        public static string Hash(string input)
        {
            using (var sha1 = new SHA1Managed())
            {
                byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));

                return Hexify(hash);
            }
        }

        private static string Hexify(byte[] value)
        {
            var builder = new StringBuilder(value.Length * 2);

            foreach (byte b in value)
                builder.Append(b.ToString("X2"));

            return builder.ToString().ToLower();
        }
    }
}
