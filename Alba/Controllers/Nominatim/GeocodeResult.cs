using System;
using System.Collections.Generic;
using System.Text;

namespace Controllers.Nominatim
{
    public class GeocodeResult
    {
        public GeocodedAddress[] Addresses { get; set; }
    }

    public class GeocodedAddress
    {
        public int place_id { get; set; }
        public string licence { get; set; }
        public string osm_type { get; set; }
        public int osm_id { get; set; }
        public string[] boundingbox { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }
        public string display_name { get; set; }
        public string _class { get; set; }
        public string type { get; set; }
        public float importance { get; set; }
    }
}
