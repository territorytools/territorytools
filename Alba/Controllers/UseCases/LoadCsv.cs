using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Controllers.UseCases
{
    public class LoadCsv<T>
    {
        public static IEnumerable<T> LoadFrom(
            string path, 
            string delimiter = null)
        {
            var list = new List<T>();
            if (string.IsNullOrWhiteSpace(path))
            {
                return list;
            }

            if (delimiter == null)
            {
                switch(Path.GetExtension(path).ToLower())
                {
                    case ".csv":
                        delimiter = ",";
                        break;
                    default: // .txt .tsv
                        delimiter = "\t";
                        break;
                }
            }

            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = delimiter,
                PrepareHeaderForMatch = args => args.Header.ToLower(),
                BadDataFound = null,
                HeaderValidated = null,
                MissingFieldFound = null,
                IgnoreBlankLines = true,
                ShouldSkipRecord = args => args.Record[0].StartsWith("#TYPE"),
            };

            using (var reader = new StreamReader(path))
            using (CsvReader csv = new CsvReader(reader, configuration))
            {
                return csv.GetRecords<T>().ToList();
            }
        }

        public static void SaveTo(
            IEnumerable<T> addresses,
            string path)
        {
            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "\t"
            };
            using (var writer = new StreamWriter(path))
            using (CsvWriter csv = new CsvWriter(writer, configuration))
            {
                csv.WriteRecords(addresses);
            }
        }
    }
}
