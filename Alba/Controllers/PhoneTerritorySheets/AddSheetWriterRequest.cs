namespace TerritoryTools.Alba.Controllers.PhoneTerritorySheets
{
    public class AddSheetWriterRequest
    {
        public string UserEmail { get; set; }
        public string DocumentId { get; set; }
        public string SecurityToken { get; set; }
    }
}
