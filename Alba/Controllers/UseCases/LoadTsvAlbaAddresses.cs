using Controllers.AlbaServer;
using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Controllers.UseCases
{
    public class LoadTsvAlbaAddresses
    {
        public static IEnumerable<AlbaAddressExport> LoadFrom(string path)
        {
            var list = new List<AlbaAddressExport>();
            if (string.IsNullOrWhiteSpace(path))
            {
                return list;
            }

            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "\t",
                BadDataFound = null,
                HeaderValidated = null,
                MissingFieldFound = null,
                PrepareHeaderForMatch = args => args.Header.ToLower()
            };

            using (var reader = new StreamReader(path))
            using (CsvReader csv = new CsvReader(reader, configuration))
            {
                return csv.GetRecords<AlbaAddressExport>().ToList();
            }
        }

        public static void SaveTo(
            IEnumerable<AlbaAddressExport> addresses, 
            string path)
        {
            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "\t",
            };

            using (var writer = new StreamWriter(path))
            using (CsvWriter csv = new CsvWriter(writer, configuration))
            {
                csv.WriteRecords(addresses);
            }
        }
    }
}
