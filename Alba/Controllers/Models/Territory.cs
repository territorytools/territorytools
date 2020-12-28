using System;
using System.Text;

namespace TerritoryTools.Alba.Controllers.Models
{
    public class Territory
    {
        public Territory(string id)
        {
            Id = id;
            Border = new Border();
        }

        /// <summary>
        /// Territory Database Id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Count of addresses in this territory.
        /// </summary>
        public string CountOfAddresses { get; set; }

        /// <summary>
        /// Territory Number.
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// A friendly name for the area that the territory is located geographically.
        /// </summary>
        public string Description { get; set; }
        public string CityCode { get; set; }
        public string CityArea { get; set; }
        public string ZipCodeSuffix { get; set; }
        public string Notes { get; set; }
        public Border Border { get; set; }

        public Color FillColor { get; set; }
        public string Status { get; internal set; }
        public string MobileLink { get; internal set; }
        public int? MonthsAgoCompleted { get; internal set; }
        public string SignedOutTo { get; internal set; }
        public DateTime? SignedOut { get; internal set; }
        public string LastCompletedBy { get; internal set; }
        public DateTime? LastCompleted { get; internal set; }
        public bool NeverCompleted { get; internal set; }
        public int YearsAgoCompleted { get; internal set; }

        public AlbaTerritoryBorder ToAlbaTerritoryBorder()
        {
            int.TryParse(Id, out int id);
            int.TryParse(CountOfAddresses, out int count);

            return new AlbaTerritoryBorder
            {
                Id = id,
                Border = Border,
                CountOfAddresses = count,
                Number = Number,
                Description = Description,
                Notes = Notes
            };
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append($"Territory: Id: {Id}, Number: {CountOfAddresses}, Code: {Number}");

            foreach (var v in Border.Vertices)
                builder.Append($"    {v.Latitude}, {v.Longitude}");

            return builder.ToString();
        }
    }
}