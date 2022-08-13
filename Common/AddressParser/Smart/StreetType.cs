using System;
using System.Collections.Generic;

namespace TerritoryTools.Common.AddressParser.Smart
{
    public class StreetType
    {
        public const string Defaults = "ALLEY:ALY,ANNEX:ANX,ARCADE:ARC,AVENUE:AVE,BAYOO:BYU,BEACH:BCH,BEND:BND,BLUFF:BLF,BLUFFS:BLFS,BOTTOM:BTM,BOULEVARD:BLVD,BRANCH:BR,BRIDGE:BRG,BROOK:BRK,BROOKS:BRKS,BURG:BG,BURGS:BGS,BYPASS:BYP,CAMP:CP,CANYON:CYN,CAPE:CPE,CAUSEWAY:CSWY,CENTER:CTR,CENTERS:CTRS,CIRCLE:CIR,CIRCLES:CIRS,CLIFF:CLF,CLIFFS:CLFS,CLUB:CLB,COMMON:CMN,CORNER:COR,CORNERS:CORS,COURSE:CRSE,COURT:CT,COURTS:CTS,COVE:CV,COVES:CVS,CREEK:CRK,CRESCENT:CRES,CREST:CRST,CROSSING:XING,CROSSROAD:XRD,CURVE:CURV,DALE:DL,DAM:DM,DIVIDE:DV,DRIVE:DR,DRIVES:DRS,ESTATE:EST,ESTATES:ESTS,EXPRESSWAY:EXPY,EXTENSION:EXT,EXTENSIONS:EXTS,FALL:FALL,FALLS:FLS,FERRY:FRY,FIELD:FLD,FIELDS:FLDS,FLAT:FLT,FLATS:FLTS,FORD:FRD,FORDS:FRDS,FOREST:FRST,FORGE:FRG,FORGES:FRGS,FORK:FRK,FORKS:FRKS,FORT:FT,FREEWAY:FWY,GARDEN:GDN,GARDENS:GDNS,GATEWAY:GTWY,GLEN:GLN,GLENS:GLNS,GREEN:GRN,GREENS:GRNS,GROVE:GRV,GROVES:GRVS,HARBOR:HBR,HARBORS:HBRS,HAVEN:HVN,HEIGHTS:HTS,HIGHWAY:HWY,HILL:HL,HILLS:HLS,HOLLOW:HOLW,INLET:INLT,INTERSTATE:I,ISLAND:IS,ISLANDS:ISS,ISLE:ISLE,JUNCTION:JCT,JUNCTIONS:JCTS,KEY:KY,KEYS:KYS,KNOLL:KNL,KNOLLS:KNLS,LAKE:LK,LAKES:LKS,LAND:LAND,LANDING:LNDG,LANE:LN,LIGHT:LGT,LIGHTS:LGTS,LOAF:LF,LOCK:LCK,LOCKS:LCKS,LODGE:LDG,LOOP:LOOP,MALL:MALL,MANOR:MNR,MANORS:MNRS,MEADOW:MDW,MEADOWS:MDWS,MEWS:MEWS,MILL:ML,MILLS:MLS,MISSION:MSN,MOORHEAD:MHD,MOTORWAY:MTWY,MOUNT:MT,MOUNTAIN:MTN,MOUNTAINS:MTNS,NECK:NCK,ORCHARD:ORCH,OVAL:OVAL,OVERPASS:OPAS,PARK:PARK,PARKS:PARK,PARKWAY:PKWY,PARKWAYS:PKWY,PASS:PASS,PASSAGE:PSGE,PATH:PATH,PIKE:PIKE,PINE:PNE,PINES:PNES,PLACE:PL,PLAIN:PLN,PLAINS:PLNS,PLAZA:PLZ,POINT:PT,POINTS:PTS,PORT:PRT,PORTS:PRTS,PRAIRIE:PR,RADIAL:RADL,RAMP:RAMP,RANCH:RNCH,RAPID:RPD,RAPIDS:RPDS,REST:RST,RIDGE:RDG,RIDGES:RDGS,RIVER:RIV,ROAD:RD,ROADS:RDS,ROUTE:RTE,ROW:ROW,RUE:RUE,RUN:RUN,SHOAL:SHL,SHOALS:SHLS,SHORE:SHR,SHORES:SHRS,SKYWAY:SKWY,SPEEDWAY:SPEEDWAY,SPRING:SPG,SPRINGS:SPGS,SPUR:SPUR,SPURS:SPUR,SQUARE:SQ,SQUARES:SQS,STATION:STA,STREAM:STRM,STREET:ST,STREETS:STS,SUMMIT:SMT,TERRACE:TER,THROUGHWAY:TRWY,TRACE:TRCE,TRACK:TRAK,TRAIL:TRL,TUNNEL:TUNL,TURNPIKE:TPKE,UNDERPASS:UPAS,UNION:UN,UNIONS:UNS,VALLEY:VLY,VALLEYS:VLYS,VIADUCT:VIA,VIEW:VW,VIEWS:VWS,VILLAGE:VLG,VILLAGES:VLGS,VILLE:VL,VISTA:VIS,WALK:WALK,WALKS:WALK,WALL:WALL,WY:WAY,WAYS:WAYS,WELL:WL,WELLS:WLS";
        public const string PrefixDefaults = "HIGHWAY:HWY,SR";

        public static List<string> Split(string text)
        { 
            var streetTypes = new List<string>();
            foreach(string t in text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                foreach (string tt in t.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    streetTypes.Add(tt.ToUpper());
                }
            }

            return streetTypes;
        }

        public static Dictionary<string, string> Map(string text)
        {
            var streetTypes = new Dictionary<string, string>();
            foreach (string t in text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var entries = t.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if(entries.Length > 1)
                {
                    streetTypes[entries[0]] = entries[1];
                }
            }

            return streetTypes;
        }

        public static bool SamePrefix(string first, string second)
        {
            return Same(first, second, PrefixDefaults);
        }

        public static bool SameSuffix(string first, string second)
        { 
            return Same(first, second, Defaults);
        }

        public static bool Same(string first, string second, string defaults)
        {
            string a = $"{first}".Trim().ToUpper();
            string b = $"{second}".Trim().ToUpper();
            if (string.IsNullOrWhiteSpace(a) && string.IsNullOrWhiteSpace(b))
            {
                return true;
            }

            if (string.Equals(a, b, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var map = Map(defaults);
            if (map.TryGetValue(a, out string sta) && string.Equals(sta, b, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (map.TryGetValue(b, out string stb) && string.Equals(stb, a, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        public static string Normalize(string streetType)
        {
            string key = $"{streetType}".Trim().ToUpper();
            string normalized = key;
            var map = Map(Defaults);
            if (map.TryGetValue(key, out string shorter))
            {
                normalized = shorter;
            }

            if(normalized.Length < 2)
            {
                return normalized;
            }

            return normalized.Substring(0, 1).ToUpper() + normalized.Substring(1).ToLower();
        }
    }
}
