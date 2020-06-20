namespace TerritoryTools.Alba.Controllers.Models
{
    /// <summary>
    /// Coordinates
    /// </summary>
    public class Vertex
    {
        public Vertex(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public double Latitude;
        public double Longitude;
    }
}