using CsvHelper.Configuration.Attributes;

namespace TerritoryTools.Alba.ListServices
{
    public class AddressCsv
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

        [Name("PHONE 1")]
        public string Phone1 { get; set; }

        [Name("PHONE 2")]
        public string Phone2 { get; set; }

        public string Notes { get; set; }
        public string Notes_private { get; set; }
        public string Account { get; set; }
    }
}
 