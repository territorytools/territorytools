using Controllers.AlbaServer;
using CsvHelper;
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

            using (var reader = new StreamReader(path))
            using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.Delimiter = "\t";
                csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header.ToLower();
                return csv.GetRecords<AlbaAddressImport>().ToList();
            }
        }

        public static void SaveTo(
            IEnumerable<AlbaAddressImport> addresses, 
            string path)
        {
            using (var writer = new StreamWriter(path))
            using (CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.Configuration.Delimiter = "\t";
                csv.WriteRecords(addresses);
            }
        }
    }
}
