using Controllers.AlbaServer;
using System;
using System.Collections.Generic;
using System.Management.Automation;
using TerritoryTools.Common.AddressParser.Smart;
//using TerritoryTools.Entities;

namespace PowerShell
{
    [Cmdlet(VerbsCommon.Get,"Duplicates")]
    [OutputType(typeof(FavoriteStuff))]
    public class GetDuplicates : PSCmdlet
    {
        public const string StreetTypes = "ALLEY:ALY,ANNEX:ANX,ARCADE:ARC,AVENUE:AVE,BAYOO:BYU,BEACH:BCH,BEND:BND,BLUFF:BLF,BLUFFS:BLFS,BOTTOM:BTM,BOULEVARD:BLVD,BRANCH:BR,BRIDGE:BRG,BROOK:BRK,BROOKS:BRKS,BURG:BG,BURGS:BGS,BYPASS:BYP,CAMP:CP,CANYON:CYN,CAPE:CPE,CAUSEWAY:CSWY,CENTER:CTR,CENTERS:CTRS,CIRCLE:CIR,CIRCLES:CIRS,CLIFF:CLF,CLIFFS:CLFS,CLUB:CLB,COMMON:CMN,CORNER:COR,CORNERS:CORS,COURSE:CRSE,COURT:CT,COURTS:CTS,COVE:CV,COVES:CVS,CREEK:CRK,CRESCENT:CRES,CREST:CRST,CROSSING:XING,CROSSROAD:XRD,CURVE:CURV,DALE:DL,DAM:DM,DIVIDE:DV,DRIVE:DR,DRIVES:DRS,ESTATE:EST,ESTATES:ESTS,EXPRESSWAY:EXPY,EXTENSION:EXT,EXTENSIONS:EXTS,FALL:FALL,FALLS:FLS,FERRY:FRY,FIELD:FLD,FIELDS:FLDS,FLAT:FLT,FLATS:FLTS,FORD:FRD,FORDS:FRDS,FOREST:FRST,FORGE:FRG,FORGES:FRGS,FORK:FRK,FORKS:FRKS,FORT:FT,FREEWAY:FWY,GARDEN:GDN,GARDENS:GDNS,GATEWAY:GTWY,GLEN:GLN,GLENS:GLNS,GREEN:GRN,GREENS:GRNS,GROVE:GRV,GROVES:GRVS,HARBOR:HBR,HARBORS:HBRS,HAVEN:HVN,HEIGHTS:HTS,HIGHWAY:HWY,HILL:HL,HILLS:HLS,HOLLOW:HOLW,INLET:INLT,INTERSTATE:I,ISLAND:IS,ISLANDS:ISS,ISLE:ISLE,JUNCTION:JCT,JUNCTIONS:JCTS,KEY:KY,KEYS:KYS,KNOLL:KNL,KNOLLS:KNLS,LAKE:LK,LAKES:LKS,LAND:LAND,LANDING:LNDG,LANE:LN,LIGHT:LGT,LIGHTS:LGTS,LOAF:LF,LOCK:LCK,LOCKS:LCKS,LODGE:LDG,LOOP:LOOP,MALL:MALL,MANOR:MNR,MANORS:MNRS,MEADOW:MDW,MEADOWS:MDWS,MEWS:MEWS,MILL:ML,MILLS:MLS,MISSION:MSN,MOORHEAD:MHD,MOTORWAY:MTWY,MOUNT:MT,MOUNTAIN:MTN,MOUNTAINS:MTNS,NECK:NCK,ORCHARD:ORCH,OVAL:OVAL,OVERPASS:OPAS,PARK:PARK,PARKS:PARK,PARKWAY:PKWY,PARKWAYS:PKWY,PASS:PASS,PASSAGE:PSGE,PATH:PATH,PIKE:PIKE,PINE:PNE,PINES:PNES,PLACE:PL,PLAIN:PLN,PLAINS:PLNS,PLAZA:PLZ,POINT:PT,POINTS:PTS,PORT:PRT,PORTS:PRTS,PRAIRIE:PR,RADIAL:RADL,RAMP:RAMP,RANCH:RNCH,RAPID:RPD,RAPIDS:RPDS,REST:RST,RIDGE:RDG,RIDGES:RDGS,RIVER:RIV,ROAD:RD,ROADS:RDS,ROUTE:RTE,ROW:ROW,RUE:RUE,RUN:RUN,SHOAL:SHL,SHOALS:SHLS,SHORE:SHR,SHORES:SHRS,SKYWAY:SKWY,SPEEDWAY:SPEEDWAY,SPRING:SPG,SPRINGS:SPGS,SPUR:SPUR,SPURS:SPUR,SQUARE:SQ,SQUARES:SQS,STATION:STA,STREAM:STRM,STREET:ST,STREETS:STS,SUMMIT:SMT,TERRACE:TER,THROUGHWAY:TRWY,TRACE:TRCE,TRACK:TRAK,TRAIL:TRL,TUNNEL:TUNL,TURNPIKE:TPKE,UNDERPASS:UPAS,UNION:UN,UNIONS:UNS,VALLEY:VLY,VALLEYS:VLYS,VIADUCT:VIA,VIEW:VW,VIEWS:VWS,VILLAGE:VLG,VILLAGES:VLGS,VILLE:VL,VISTA:VIS,WALK:WALK,WALKS:WALK,WALL:WALL,WAY:WAY,WAYS:WAYS,WELL:WL,WELLS:WLS";

        Parser parser;
        List<ParsedAddress> parsedMasterList; // = new List<Address>();
        List<string> errors = new List<string>();

        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public AlbaAddressImport Address { get; set; }

        [Parameter]
        public List<AlbaAddressImport> MasterList { get; set; }

        [Parameter]
        public List<string> Cities { get; set; }

        [Parameter]
        public SwitchParameter IncludeSelf { get; set; }

        // This method gets called once for each cmdlet in the pipeline when the pipeline starts executing
        protected override void BeginProcessing()
        {
            //var validCities = new List<string> { "Bellevue", "Lynnwood", "Kirkland", "Bothell" };
            var streetTypes = StreetType.Split(StreetTypes);
            parser = new Parser(Cities, streetTypes);

            WriteVerbose($"Parser Loaded Cities: {Cities.Count} Street Types:{streetTypes.Count}");

            parsedMasterList = new List<ParsedAddress>();
            errors = new List<string>();
            foreach (var master in MasterList)
            {
                string text = $"{master.Address}, {master.Suite}, {master.City}, {master.Province} {master.Postal_code}";
                //WriteVerbose($"MasterIn: {text}");
                var parsed = parser.Parse(text);
                //WriteVerbose($"MasterOut: {parsed}");
                parsedMasterList.Add(new ParsedAddress() { Address = parsed, AlbaAddressImport = master });
            }

            WriteVerbose($"parsedMasterList: {parsedMasterList.Count}");
        }

        // This method will be called for each input received from the pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            try
            {
                // TODO: Use the AlbaAddressInput to pass the data, only parse for comparison

                string text = $"{Address.Address}, {Address.Suite}, {Address.City}, {Address.Province} {Address.Postal_code}";
                var parsed = parser.Parse(text);
                //WriteVerbose(parsed.ToString());
                if (!string.IsNullOrWhiteSpace(parsed.FailedAddress))
                {
                    errors.Add($"{parsed.ErrorMessage}: {parsed.FailedAddress} ");
                }

                var duplicates = new List<ParsedAddress>();
                foreach(var master in parsedMasterList)
                {
                    if (master.AlbaAddressImport.Address_ID != Address.Address_ID
                        && master.Address.SameAs(parsed))
                    {
                        //WriteVerbose($"    {parsed} == {master.Address}");
                        duplicates.Add(master);
                    }
                }

                //WriteVerbose($"Duplicates: {duplicates.Count}");

                if (duplicates.Count > 0)
                {
                    //foreach (var dup in duplicates)
                    //{
                    //    if(IncludeSelf)
                    //    {
                    //        WriteObject(Address);
                    //    }

                    //    WriteObject(dup.AlbaAddressImport);
                    //}
                    var a = new DuplicateAddress(Address.Address_ID, Address);
                    a.DuplicationStatus = "Original";
                    WriteObject(a);

                    foreach (var dup in duplicates)
                    {
                        var da = new DuplicateAddress(Address.Address_ID, dup.AlbaAddressImport);
                        da.DuplicationStatus = "Duplicate";
                        WriteObject(da);
                    }
                }
            }
            catch(Exception)
            {
                //Skip
            }
        }

        // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
            WriteVerbose("ERRORS:");
            foreach(string error in errors)
            {
                WriteVerbose(error);
            }
        }
    }

    public class ParsedAddress
    {
        public string Text { get; set; }
        public AlbaAddressImport AlbaAddressImport { get; set; }
        public Address Address { get; set; }
        public List<AlbaAddressImport> Duplicates { get; set; } = new List<AlbaAddressImport>();
    }

    public class DuplicateAddress : AlbaAddressImport
    {
        public DuplicateAddress(int? duplicateOfAddressId, AlbaAddressImport copy)
        {
            DuplicateOf = duplicateOfAddressId;
            Address_ID = copy.Address_ID;
            Territory_ID = copy.Territory_ID;
            Language = copy.Language;
            Status = copy.Status;
            Name = copy.Name;
            Suite = copy.Suite;
            Address = copy.Address;
            City = copy.City;
            Province = copy.Province;
            Postal_code = copy.Postal_code;
            Country = copy.Country;
            Latitude = copy.Latitude;
            Longitude = copy.Longitude;
            Telephone = copy.Telephone;
            Notes = copy.Notes;
            Notes_private = copy.Notes_private;
        }

        public int? DuplicateOf { get; set; }
        public string DuplicationStatus { get; set; }
    }
}
