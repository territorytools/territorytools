using System;
using System.Collections.Generic;
using System.Linq;

namespace TerritoryTools.Alba.Controllers.AlbaBackupToS13
{
    public class S13Entry
    {
        public string Number { get; set; }
        public string Publisher { get; set; }
        public DateTime? CheckedOut { get; set; }
        public DateTime? CheckedIn { get; set; }

        public static List<S13Entry> LoadCsv(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            IEnumerable<S13Entry> csv = global::Controllers.UseCases.LoadCsv.LoadFrom<S13Entry>(path, ",");

            return csv.ToList();
        }

        public static void SaveToCsv(IEnumerable<S13Entry> entries, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            var rows = new List<S13EntryCsvRow>();

            foreach(var entry in entries)
            {
                rows.Add(new S13EntryCsvRow(entry));
            }

            global::Controllers.UseCases.LoadCsv.SaveTo(rows, path);
        }

        public override string ToString()
        {
            return $"{Number} {Publisher} Out:{CheckedOut} In:{CheckedIn}";
        }
    }
}
