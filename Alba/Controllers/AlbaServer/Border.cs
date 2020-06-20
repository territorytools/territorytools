namespace TerritoryTools.Alba.Controllers.AlbaServer
{
    /// <summary>
    /// One of the objects for deserializing the reponse to asking for all territories from Alba.
    /// Response to: GET /alba/ts?mod=territories&cmd=search&kinds%5B%5D=0&kinds%5B%5D=1&kinds%5B%5D=2&q=&sort=number&order=asc HTTP/1.1
    /// </summary>
    public class Border
    {
        /// <summary>
        /// Coordinates, longitude and latitude.
        /// </summary>
        public float[][] pl { get; set; }

        /// <summary>
        /// Territory Number, Description, and Notes.  Separated only by spaces.
        /// </summary>
        public string tt { get; set; }

        /// <summary>
        /// A count of how many addresses are within a territory's borders.
        /// </summary>
        public string num { get; set; }
    }
}
