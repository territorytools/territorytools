namespace TerritoryTools.Alba.Controllers.AlbaServer
{
    public class LogonResult
    {
        public string[] log { get; set; }
        public object[] error { get; set; }
        public bool authenticated { get; set; }
        public string[] sql { get; set; }
        public User user { get; set; }
        public bool session { get; set; }
    }

    public class User
    {
        public int? id { get; set; }
        public string account_type { get; set; }
        public string account_name { get; set; }
        public string account_full_name { get; set; }
        public string primary_language_id { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string province { get; set; }
        public string country { get; set; }
        public string postcode { get; set; }
    }
}
