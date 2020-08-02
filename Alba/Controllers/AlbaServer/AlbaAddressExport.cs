using CsvHelper;
using CsvHelper.TypeConversion;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Controllers.AlbaServer
{
    public class AlbaAddressExport
    {
        public int? Address_ID { get; set; } 
        public int? Territory_ID { get; set; }
        public string Language { get; set; }
        public string Status { get; set; } 
        public string Name { get; set; } 
        public string Suite { get; set; } 
        public string Address { get; set; } 
        public string City { get; set; } 
        public string Province { get; set; } 
        public string Postal_code { get; set; } 
        public string Country { get; set; }
        public double? Latitude { get; set; } 
        public double? Longitude { get; set; }
        public string Telephone { get; set; }
        public string Owner { get; set; }
        public string Notes { get; set; } 
        public string Notes_private { get; set; }
        public string Account { get; set; }
        public string Created { get; set; }
        public string Modified { get; set; }
        public string Contacted { get; set; }
        public string Geocoded { get; set; }
        public string Territory_number { get; set; }
        public string Territory_description { get; set; }

        public static IEnumerable<AlbaAddressExport> LoadFrom(string path)
        {
            var list = new List<AlbaAddressExport>();
            if (string.IsNullOrWhiteSpace(path))
            {
                return list;
            }

            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.Delimiter = "\t";
                csv.Configuration.BadDataFound = null;
                return csv.GetRecords<AlbaAddressExport>().ToList();
            }
        }
    }
}
