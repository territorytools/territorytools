using Controllers.AlbaServer;
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

        public static AddressCsv From(AddressCsv from)
        {
            return new AddressCsv
            {
                Address_ID = from.Address_ID,
                Territory_ID = from.Territory_ID,
                Language = from.Language,
                Status = from.Status,
                Name = from.Name,
                Suite = from.Suite,
                Address = from.Address,
                City = from.City,
                Province = from.Province,
                Postal_code = from.Postal_code,
                Country = from.Country,
                Latitude = from.Latitude,
                Longitude = from.Longitude,
                Telephone = from.Telephone,
                //Phone1,
                //Phone2,
                Notes = from.Notes,
                Notes_private = from.Notes_private,
                Account = from.Account
            };
        }
    }
}
 