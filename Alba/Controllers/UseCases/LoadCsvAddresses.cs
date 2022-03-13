using Controllers.AlbaServer;
using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Controllers.UseCases
{
    public class LoadCsvAddresses
    {
        public static IEnumerable<AlbaAddressImport> LoadFrom(string path)
        {
            var list = new List<AlbaAddressImport>();
            if (string.IsNullOrWhiteSpace(path))
            {
                return list;
            }

            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "\t",
                PrepareHeaderForMatch = args => args.Header.ToLower()
            };

            using (var reader = new StreamReader(path))
            using (CsvReader csv = new CsvReader(reader, configuration))
            {
                return csv.GetRecords<AlbaAddressImport>().ToList();
            }
        }

        public static void SaveTo(
            IEnumerable<AlbaAddressImport> addresses, 
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
