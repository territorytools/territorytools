using Controllers.AlbaServer;
using CsvHelper;
using CsvHelper.Configuration;
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
            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
            };

            using (var writer = new StreamWriter(path))
            using (CsvWriter csv = new CsvWriter(writer, configuration))
            {
                csv.WriteRecords(addresses);
            }
        }
    }
}
