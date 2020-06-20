using TerritoryTools.Alba.Controllers.AlbaServer;
using CsvHelper;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Controllers.UseCases
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class DownloadUsers
    {
        private AuthorizationClient client;

        public DownloadUsers(AuthorizationClient client)
        {
            this.client = client;
        }

        public void SaveAs(string fileName)
        {
            // Downloads html instead of json Territory Assignments
            var resultString = client.DownloadString(
                RelativeUrlBuilder.GetTerritoryAssignmentsPage());

            List<User> users = GetUsers(GetUsersHtml(resultString));

            using (var writer = new StreamWriter(fileName))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))

            {
                csv.WriteRecords(users);
            }
        }

        public static string GetUsersHtml(string javaScriptString)
        {
            string innerHtml = string.Empty;

            var doc = new HtmlDocument();
            doc.LoadHtml(javaScriptString);
            string starter = "var userSelect = \"";
            var rowNodes = doc.DocumentNode.SelectNodes("//html/body/script");
            if (rowNodes != null)
            {
                foreach (HtmlNode rowNode in rowNodes)
                {
                    if (rowNode.InnerHtml.StartsWith(starter))
                    {
                        innerHtml = rowNode.InnerHtml;
                    }
                }
            }

            if (!innerHtml.StartsWith(starter))
            {
                throw new Exception("The JavaScript string is not formatted how it was expected!");
            }

            string trimmed = innerHtml
                .Substring(starter.Length, innerHtml.Length - starter.Length - 3);

            // Replace remove backslash from escaped quotes and forward slashes 
            string html = trimmed.Replace("\\\"", "\"").Replace("\\/", "/");

            return html;
        }

        public static List<User> GetUsers(string html)
        {
            var users = new List<User>();

            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            // Users are stored in a select element (Combo box), each 'option' is a
            // selectable user, get each one
            var rowNodes = doc.DocumentNode.SelectNodes("//select/optgroup/option");

            if (rowNodes != null)
            {
                foreach (HtmlNode rowNode in rowNodes)
                {
                    string userIdString = rowNode.GetAttributeValue("value", string.Empty);
                    string email = rowNode.GetAttributeValue("rel", string.Empty);
                    string name = rowNode.InnerText;

                    int.TryParse(userIdString, out int userId);
                    email = string.IsNullOrWhiteSpace(email) ? string.Empty : email.ToLower();

                    users.Add(
                        new User
                        {
                            Id = userId,
                            Name = name,
                            Email = email,
                        });
                }
            }

            return users;
        }
    }
}
