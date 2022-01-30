﻿using CsvHelper;
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

            using (var reader = new StreamReader(path))
            using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.Delimiter = delimiter;
                csv.Configuration.PrepareHeaderForMatch =
                    (string header, int index) => header.ToLower();

                csv.Configuration.BadDataFound = null;
                csv.Configuration.HeaderValidated = null;
                csv.Configuration.MissingFieldFound = null;
                csv.Configuration.IgnoreBlankLines = true;
                csv.Configuration.ShouldSkipRecord = row => row[0].StartsWith("#TYPE");
                return csv.GetRecords<T>().ToList();
            }
        }

        public static void SaveTo(
            IEnumerable<T> addresses, 
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
