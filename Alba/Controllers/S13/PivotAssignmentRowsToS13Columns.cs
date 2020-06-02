using Controllers.S13;
using CsvHelper;
using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Controllers.UseCases
{
    public class PivotAssignmentRowsToS13Columns
    {
        public static List<AssignmentRowRaw> LoadFrom(string path)
        {
            var list = new List<AssignmentRowRaw>();
            if (string.IsNullOrWhiteSpace(path))
            {
                return list;
            }

            using (var reader = new StreamReader(path))
            using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.Delimiter = ",";
                csv.Configuration.BadDataFound = null;
                csv.Configuration.HeaderValidated = null;
                csv.Configuration.MissingFieldFound = null;
                return csv.GetRecords<AssignmentRowRaw>().ToList();
            }
        }

        public static List<S13Column> PivotFrom(IList<AssignmentRowRaw> rows)
        {
            var columns = new List<S13Column>();
            var territories = rows.GroupBy(r => r.Territory).ToList();
            foreach (var territory in territories)
            {
                var column = new S13Column()
                {
                    Territory = territory.Key
                };

                foreach (var entry in territory.OrderBy(e => e.Entry))
                {
                    DateTime? checkedIn = null;
                    if (DateTime.TryParse(entry.CheckedIn, out DateTime checkedInDate))
                    {
                        checkedIn = checkedInDate;
                    }

                    DateTime? checkedOut = null;
                    if (DateTime.TryParse(entry.CheckedOut, out DateTime checkedOutDate))
                    {
                        checkedOut = checkedOutDate;
                    }

                    column.Entries.Add(
                        new S13Entry
                        {
                            Publisher = entry.Publisher,
                            CheckedIn = checkedIn,
                            CheckedOut = checkedOut
                        });
                }

                columns.Add(column);
            }

            return columns;
        }

        public static List<TerritoryLastCompleted> LastCompletedFrom(IList<AssignmentRowRaw> rows)
        {
            var parsedRows = new List<TerritoryLastCompleted>();
            foreach (var row in rows)
            {
                DateTime? checkedIn = null;
                if (DateTime.TryParse(row.CheckedIn, out DateTime checkedInDate))
                {
                    checkedIn = checkedInDate;
                }

                DateTime? checkedOut = null;
                if (DateTime.TryParse(row.CheckedOut, out DateTime checkedOutDate))
                {
                    checkedOut = checkedOutDate;
                }

                var parsed = new TerritoryLastCompleted
                {
                    Territory = row.Territory,
                    Publisher = row.Publisher,
                    CheckedOut = checkedOut,
                    CheckedIn = checkedIn,
                };

                parsedRows.Add(parsed);
            }

            var territories = parsedRows.GroupBy(r => r.Territory).ToList();
            var assignments = new List<TerritoryLastCompleted>();
            foreach (var territory in territories)
            {
                var lastCheckedIn = territory.OrderBy(e => e.CheckedIn).Last();

                var assignment = new TerritoryLastCompleted()
                {
                    Territory = lastCheckedIn.Territory,
                    TimesWorked = territory.Count(),
                    Publisher = lastCheckedIn.Publisher,
                    CheckedOut = lastCheckedIn.CheckedOut,
                    CheckedIn = lastCheckedIn.CheckedIn,
                };

                assignments.Add(assignment);
            }

            return assignments;
        }

        public static void SaveTo(
            IList<S13Column> columns, 
            string path)
        {
            using (var writer = new StreamWriter(path))
            using (CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.Configuration.Delimiter = ",";
                int columnCount = columns.Count;
                int rowCount = columns.Max(c => c.Entries.Count);

                for (int i = 0; i < columnCount; i++)
                {
                    csv.WriteField(columns[i].Territory);
                }

                csv.NextRecord();

                for (int r = 0; r < rowCount; r++)
                {
                    for (int i = 0; i < columnCount; i++)
                    {
                        string text = "";
                        if (columns[i].Entries.Count > r)
                        {
                            text = columns[i].Entries[r].Publisher;
                        }

                        csv.WriteField(text);
                    }

                    csv.NextRecord();

                    for (int i = 0; i < columnCount; i++)
                    {
                        string text = "";
                        if (columns[i].Entries.Count > r)
                        {
                            DateTime? date = columns[i].Entries[r].CheckedOut;
                            if (date != null)
                            {
                                var d = (DateTime)date;
                                text = $"{d.Month}/{d.Day}/{d.Year}";
                            }
                        }

                        csv.WriteField(text);
                    }

                    csv.NextRecord();

                    for (int i = 0; i < columnCount; i++)
                    {
                        string text = "";
                        if (columns[i].Entries.Count > r)
                        {
                            DateTime? date = columns[i].Entries[r].CheckedIn;
                            if(date != null)
                            {
                                var d = (DateTime)date;
                                text = $"{d.Month}/{d.Day}/{d.Year}";
                            }
                        }

                        csv.WriteField(text);
                    }

                    csv.NextRecord();
                }
            }
        }

        public static void SaveTo(
               IEnumerable<TerritoryLastCompleted> assignments,
               string path)
        {
            using (var writer = new StreamWriter(path))
            using (CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.Configuration.Delimiter = ",";
                var options = new TypeConverterOptions { Formats = new[] { "MM/dd/yyyy" } };
                csv.Configuration.TypeConverterOptionsCache.AddOptions<DateTime?>(options);
                csv.WriteRecords(assignments);
            }
        }
    }
}
