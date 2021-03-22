using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Alba.Controllers.Kml;
using TerritoryTools.Alba.Controllers.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TerritoryTools.Alba.Controllers.UseCases
{
    public class DownloadKmlFile
    {
        private AlbaConnection client;

        public DownloadKmlFile(AlbaConnection client)
        {
            this.client = client;
        }

        public List<TerritoryDetail> AllTerritories { get; set; } 
            = new List<TerritoryDetail>();

        public List<TerritoryDetail> SaveAs(string fileName)
        {
            var resultString = client.DownloadString(RelativeUrlBuilder.GetAllTerritoriesWithBorders());
            AllTerritories = TerritoryResultParser.Parse(resultString);

            var assignmentsResultString = client.DownloadString(
               RelativeUrlBuilder.GetTerritoryAssignments());

            string assignmentsHtml = TerritoryAssignmentParser.Parse(assignmentsResultString);

            var assignments = new DownloadTerritoryAssignments(client).GetAssignments(assignmentsHtml);

            foreach (var t in AllTerritories)
            {
                var assignment = assignments.FirstOrDefault(a => string.Equals(a.Number, t.Number, StringComparison.OrdinalIgnoreCase));

                if (assignment != null)
                {
                    int monthsAgoCompleted = assignment.MonthsAgoCompleted ?? 0;
                    int yearsAgoCompleted = monthsAgoCompleted / 12;

                    t.Status = assignment.Status;
                    t.MobileLink = assignment.MobileLink;
                    t.SignedOutTo = assignment.SignedOutTo;
                    t.SignedOut = assignment.SignedOut;
                    t.LastCompletedBy = assignment.LastCompletedBy;
                    t.LastCompleted = assignment.LastCompleted;                    
                    t.MonthsAgoCompleted = assignment.MonthsAgoCompleted;
                    t.YearsAgoCompleted = assignment.LastCompleted == null 
                        ? 99 
                        : yearsAgoCompleted;
                    t.NeverCompleted = assignment.LastCompleted == null;
                }
            }

            var filteredTerritories = Filter(AllTerritories);

            foreach (var t in filteredTerritories)
            {
                t.FillColor = FillColorFor(t);
            }

            var kml = TerritoryDetailToKmlConverter.Convert(filteredTerritories);
            new KmlGateway().Save(fileName, kml);

            return filteredTerritories.ToList();
        }

        protected virtual IEnumerable<TerritoryDetail> Filter(IEnumerable<TerritoryDetail> territories)
        {
            return territories;
        }

        protected virtual Color FillColorFor(TerritoryDetail territory)
        {
            var color = new Color();

            int oldestTerritory = AllTerritories.Max(t => t.MonthsAgoCompleted ?? 0);

            double completedPercent = (double)(territory.MonthsAgoCompleted ?? 0) / oldestTerritory;

            int completedOpacity = (int)((1.0 - completedPercent) * 128);

            color = new Color { A = completedOpacity, Blue = 0, Red = 0, Green = 0 }; // Black

            if (territory.NeverCompleted)
            {
                color = new Color { A = 0, Blue = 0, Red = 0, Green = 0 }; // Clear
            }

            if (territory.SignedOut != null)
            {
                color = new Color { A = 128, Blue = 0, Red = 255, Green = 0 }; // Red
            }

            return color;
        }
    }
}
