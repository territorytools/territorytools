using TerritoryTools.Alba.ListServices;

namespace Controllers.AlbaServer
{
    public class AlbaAddressImport
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
        public string Notes { get; set; } 
        public string Notes_private { get; set; }

        public override string ToString()
        {
            return $"{Address_ID} {Name} {Address}, {Suite}, {City}, {Province} {Postal_code}".Trim();
        }

        public static AlbaAddressImport From(AlbaAddressExport export)
        {
            return new AlbaAddressImport
            {
                Address_ID= export.Address_ID,
                Territory_ID= export.Territory_ID,
                Language= export.Language,
                Status= export.Status,
                Name= export.Name,
                Suite= export.Suite,
                Address= export.Address,
                City= export.City,
                Province= export.Province,
                Postal_code= export.Postal_code,
                Country= export.Country,
                Latitude= export.Latitude,
                Longitude= export.Longitude,
                Telephone= export.Telephone,
                Notes= export.Notes,
                Notes_private= export.Notes_private
            };
        }

        public static AlbaAddressImport From(AddressCsv from)
        {
            return new AlbaAddressImport
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
                Notes = from.Notes,
                Notes_private = from.Notes_private
            };
        }
    }
}
