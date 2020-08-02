﻿namespace Controllers.AlbaServer
{
    public class AlbaAddressSave
    {
        public int? Address_ID { get; set; } 
        public int? Territory_ID { get; set; }
        public int LanguageId { get; set; }
        public int StatusId { get; set; } 
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
    }
}
