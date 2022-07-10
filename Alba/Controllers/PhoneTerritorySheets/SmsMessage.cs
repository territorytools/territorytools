namespace TerritoryTools.Alba.Controllers.PhoneTerritorySheets
{
    public class SmsMessage
    {
        public string Id { get; set; }
        public string Timestamp { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public string Message { get; set; }
        public string SecurityToken { get; set; }
        public string LogDocumentId { get; set; }
        public string LogSheetName { get; set; }
    }
}
