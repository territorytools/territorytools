using TerritoryTools.Alba.ListServices;

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

        public static AlbaAddressExport From(AddressCsv from)
        {
            return new AlbaAddressExport
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
                //Owner = from.Owner,
                Notes = from.Notes,
                Notes_private = from.Notes_private,
                Account = from.Account,
                //Created = from.Created,
                //Modified = from.Modified,
                //Contacted = from.Contacted,
                //Geocoded = from.Geocoded,
                //Territory_number = from.Territory_number,
                //Territory_description = from.Territory_description
            };
        }
    }
}
