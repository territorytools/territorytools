namespace TerritoryTools.Alba.Controllers.PhoneTerritorySheets
{
    public class SheetExtractionRequest
    {
        public string TerritoryNumber { get; set; }
        public string PublisherName { get; set; }
        public string PublisherEmail { get; set; }
        public string OwnerEmail { get; set; }
        public string FromDocumentId { get; set; }
        public string FromSheetName { get; set; }
        public string SecurityToken { get; set; }
    }
}
