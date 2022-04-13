namespace TerritoryTools.Alba.Controllers.PhoneTerritorySheets
{
    public class AssignSheetRequest
    {
        public string PublisherEmail { get; set; }
        public string OwnerEmail { get; set; }
        public string DocumentId { get; set; }
        public string SecurityToken { get; set; }
    }
}
