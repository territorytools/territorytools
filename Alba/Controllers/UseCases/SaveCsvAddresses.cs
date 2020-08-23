using Controllers.AlbaServer;
using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Controllers.UseCases
{
    public class SaveCsvAddresses
    {
        public static void SaveTo(
            IEnumerable<AlbaAddressImport> addresses,
            string path)
        {
            using (var writer = new StreamWriter(path))
            using (CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.Configuration.Delimiter = ",";
                csv.WriteRecords(addresses);
            }
        }
    }
}
