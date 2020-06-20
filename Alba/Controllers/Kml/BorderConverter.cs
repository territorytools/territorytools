using Alba.Controllers.Models;
using System;
using System.Linq;

namespace Alba.Controllers.Kml
{
    public class BorderConverter
    {
        public static Border From(string coordinates)
        {
            return new BorderConverter().BorderFrom(coordinates);
        }

        public static string From(Border border)
        {
            return new BorderConverter().CoordinatesFrom(border);
        }

        private Border newBorder;

        public Border BorderFrom(string coordinates)
        {
            newBorder = new Border();

            if (!string.IsNullOrWhiteSpace(coordinates))
                foreach (var latlon in coordinates.Split(' '))
                    AddVertexToBorderFrom(latlon);

            return newBorder;
        }

        private void AddVertexToBorderFrom(string latlon)
        {
            if (!string.IsNullOrWhiteSpace(latlon))
            {
                var coordinateSet = latlon.Split(',');
                TryAddVertex(latlon, coordinateSet);
            }
        }

        private void TryAddVertex(string latlon, string[] coordinateSet)
        {
            try
            {
                newBorder.Vertices.Add(VertexFrom(coordinateSet));
            }
            catch (Exception e)
            {
                var message = string.Format(
                    "Cannot parse and add vertex.  String parsing:{0}",
                    latlon);

                throw new Exception(message, e);
            }
        }

        private static Vertex VertexFrom(string[] coordinateSet)
        {
            return new Vertex(double.Parse(coordinateSet[1]), double.Parse(coordinateSet[0]));
        }

        private string CoordinatesFrom(Border border)
        {
            return string.Join(" ", border.Vertices.Select(v => v.Longitude + "," + v.Latitude));
        }
    }
}
