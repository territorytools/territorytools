using System.Text;

namespace TerritoryTools.Alba.Controllers.Models
{
    public class AlbaTerritoryBorder
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public Border Border { get; set; } = new Border();
        public int CountOfAddresses { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append($"Territory: Id: {Id}, Number: {CountOfAddresses}, Code: {Number}");

            foreach (var v in Border.Vertices)
                builder.Append("    " + v.Latitude + ", " + v.Longitude);

            return builder.ToString();
        }
    }
}