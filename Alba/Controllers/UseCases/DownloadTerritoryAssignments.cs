﻿using TerritoryTools.Alba.Controllers.AlbaServer;
using CsvHelper;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper.Configuration;

namespace TerritoryTools.Alba.Controllers.UseCases
{
    public class DownloadTerritoryAssignments
    {
        private AlbaConnection client;

        public DownloadTerritoryAssignments(AlbaConnection client)
        {
            this.client = client;
        }

        public void SaveAs(string fileName)
        {
            var resultString = client.DownloadString(
                RelativeUrlBuilder.GetTerritoryAssignments());

            string html = TerritoryAssignmentParser.Parse(resultString);

            SaveAs(html, fileName);
        }

        public void SaveAs(string html, string fileName)
        {
            var assignments = GetAssignments(html);
            
            SaveAs(assignments, fileName);
        }

        public static void SaveAs(List<AlbaAssignmentValues> assignments, string fileName)
        {
            using (var writer = new StreamWriter(fileName))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(assignments);
            }
        }

        public static List<AlbaAssignmentValues> LoadFromCsv(string path)
        {
            var list = new List<AlbaAssignmentValues>();
            if (string.IsNullOrWhiteSpace(path))
            {
                return list;
            }

            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "\t",
                BadDataFound = null,
                //BadDataFound = null,
                HeaderValidated = null,
                MissingFieldFound = null,
            };

            using (var reader = new StreamReader(path))
            using (CsvReader csv = new CsvReader(reader, configuration))
            {
                return csv.GetRecords<AlbaAssignmentValues>().ToList();
            }
        }

        public List<AlbaAssignmentValues> GetAssignments()
        {
            var resultString = client.DownloadString(
                RelativeUrlBuilder.GetTerritoryAssignments());

            string html = TerritoryAssignmentParser.Parse(resultString);

            return GetAssignments(html);
        }

        public List<AlbaAssignmentValues> GetAssignments(string html)
        {
            //string html = File.ReadAllText("assignments.html");
            //var resultString = client.DownloadString(
            //    RelativeUrlBuilder.GetTerritoryAssignments());

            //string html = TerritoryAssignmentParser.Parse(resultString);

            var assignments = new List<AlbaAssignmentValues>();

            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var rowNodes = doc.DocumentNode.SelectNodes("//tr");

            if (rowNodes != null)
            {
                int row = 0;
                foreach (HtmlNode rowNode in rowNodes)
                {
                    var colNodes = rowNode.SelectNodes("td");
                    if (colNodes != null)
                    {
                        var assignment = new AlbaAssignmentValues();
                        for (int col = 0; col < colNodes.Count; col++)
                        {
                            HtmlNode colNode = colNodes[col];
                            switch (col)
                            {
                                case 0:
                                    ParseId(assignment, colNode);
                                    break;
                                case 1:
                                    ParseNumber(assignment, colNode);
                                    break;
                                case 2:
                                    if (int.TryParse(colNode.InnerText, out int result))
                                        assignment.Addresses = result;
                                    break;
                                case 3:
                                    var nodes = colNode.SelectNodes("div/ul/li/a");
                                    if (nodes == null)
                                    {
                                        break;
                                    }

                                    foreach (var node in nodes)
                                    {
                                        string className = node.GetAttributeValue("class", null);
                                        if (string.Equals(className.Trim(), "cmd-open"))
                                        {
                                            string rel = node.GetAttributeValue("rel", null);
                                            if (!string.IsNullOrWhiteSpace(rel))
                                            {
                                                if (rel.Contains("/print-mk?"))
                                                {
                                                    assignment.PrintLink = rel;
                                                }
                                                else
                                                {
                                                    assignment.MobileLink = rel;
                                                }
                                            }
                                        }
                                        else if (string.Equals(className.Trim(), "cmd-print"))
                                        {
                                            string rel = node.GetAttributeValue("rel", null);
                                            if (!string.IsNullOrWhiteSpace(rel))
                                            {
                                                if (rel.Contains("/print-mk?"))
                                                {
                                                    assignment.PrintLink = rel;
                                                }
                                            }
                                        }
                                    }
                                    break;
                                case 5:
                                    assignment.Status = colNode.InnerText;
                                    break;
                                case 6:
                                    if (assignment.Status == "Available")
                                    {
                                        string text = colNode.InnerText;
                                        if (!string.Equals(text, "Never completed", StringComparison.OrdinalIgnoreCase))
                                        {
                                            if (text.StartsWith("Last completed "))
                                            {
                                                text = text.Replace("Last completed ", string.Empty);
                                                string[] entries = text.Split(new string[] { " by " }, StringSplitOptions.RemoveEmptyEntries);
                                                if (entries.Length == 2)
                                                {
                                                    if (DateTime.TryParse(entries[0], out DateTime date))
                                                    {
                                                        assignment.LastCompleted = date;
                                                    }

                                                    assignment.LastCompletedBy = entries[1].Trim();
                                                }
                                            }
                                        }
                                    }
                                    else if (string.Equals(assignment.Status, "Signed-Out", StringComparison.OrdinalIgnoreCase))
                                    {
                                        string[] entries = colNode.InnerHtml.Split(
                                            new string[] { "</strong><br><small class=\"muted\">" },
                                            StringSplitOptions.RemoveEmptyEntries);

                                        if (entries.Length == 2)
                                        {
                                            assignment.SignedOutTo = entries[0].Replace("<strong class=\"person\">", "").Trim();

                                            assignment.SignedOutString = entries[1].Replace("</small>", "");
                                            if (DateTime.TryParse(assignment.SignedOutString, out DateTime date))
                                            {
                                                assignment.SignedOut = date;
                                            }
                                        }
                                    }
                                    break;
                                case 7:
                                    ParseMonthsSignedout(assignment, colNode);
                                    break;
                                case 8:
                                    ParseMonthsAgoCompleted(assignment, colNode);
                                    break;
                            }
                        }

                        assignments.Add(assignment);
                    }

                    row++;
                }
            }

            return assignments;
        }

        private static void ParseMonthsAgoCompleted(AlbaAssignmentValues assignment, HtmlNode colNode)
        {
            string innerText = string.Empty;
            try
            {
                innerText = colNode.InnerText;
                if (!string.IsNullOrWhiteSpace(innerText))
                {
                    if (int.TryParse(innerText, out int result))
                    {
                        assignment.MonthsAgoCompleted = result;
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception($"Error parsing months ago completed: {innerText} Line: {colNode.Line}Pos: {colNode.LinePosition}");
            }
        }

        private static void ParseMonthsSignedout(AlbaAssignmentValues assignment, HtmlNode colNode)
        {
            string innerText = string.Empty;
            try
            {
                innerText = colNode.InnerText;
                if (!string.IsNullOrWhiteSpace(innerText))
                {
                    if (int.TryParse(innerText, out int result))
                    {
                        assignment.MonthsSignedOut = result;
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception($"Error parsing months ago signed out: {innerText} Line: {colNode.Line} Pos: {colNode.LinePosition}");
            }
        }

        private static void ParseId(AlbaAssignmentValues assignment, HtmlNode colNode)
        {
            string innerText = string.Empty;
            try
            {
                innerText = colNode.InnerText;
                assignment.IdString = innerText;
                if (int.TryParse(innerText, out int result))
                {
                    assignment.Id = result;
                }
            }
            catch (Exception)
            {
                throw new Exception($"Error parsing ID: {innerText} Line: {colNode.Line} Pos:{colNode.LinePosition}");
            }
        }

        private static void ParseNumber(AlbaAssignmentValues assignment, HtmlNode colNode)
        {
            string text = string.Empty;
            try
            {
                text = colNode.InnerHtml;
                if (string.IsNullOrWhiteSpace(text))
                {
                    return;
                }

                if (text.Contains("&nbsp;") && text.Contains("<span class=\"label tk1_bg\">"))
                {
                    string[] entries = text.Split(
                        new string[] { "&nbsp;" },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (entries.Length == 2)
                    {
                        assignment.Number = entries[0]
                            .Replace("<b>", "")
                            .Replace("</b>", "")
                            .Trim();

                        string next = entries[1]
                            .Replace("<span class=\"label tk1_bg\">", "")
                            .Replace("</span>", "")
                            .Trim();

                        string[] nexts = next.Split(
                            new string[] { "<br>" }, 
                            StringSplitOptions.RemoveEmptyEntries);

                        if (nexts.Length > 0)
                        {
                            assignment.Kind = nexts[0];
                        }

                        if (nexts.Length == 2)
                        {
                            assignment.Description = nexts[1];
                        }
                    }
                }
                else
                {
                    string[] entries = text.Split(
                        new string[] { "<br>" },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (entries.Length == 2)
                    {
                        assignment.Number = entries[0].Replace("<b>", "").Replace("</b>", "").Trim();
                        assignment.Description = entries[1];
                        assignment.Kind = "Regular";
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception($"Error parsing territory number and description: {text} Line: {colNode.Line}");
            }
        }
    }
}
