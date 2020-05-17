using Controllers.AlbaServer;
using CsvHelper;
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

            using (var reader = new StreamReader(path))
            using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.Delimiter = "\t";
                csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header.ToLower();
                csv.Configuration.BadDataFound = null;
                csv.Configuration.HeaderValidated = null;
                csv.Configuration.MissingFieldFound = null;
                return csv.GetRecords<AlbaAddressExport>().ToList();
            }
        }

        public static void SaveTo(
            IEnumerable<AlbaAddressExport> addresses, 
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
