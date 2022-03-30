namespace TerritoryTools.Alba.Controllers.PhoneTerritorySheets
{
    public class SheetExtractionRequest
    {
        public string TerritoryNumber { get; set; }
        public string Publisher { get; set; }
        public string FromDocumentId { get; set; }
        public string FromSheetName { get; set; }
        public string SecurityToken { get; set; }
    }
}
