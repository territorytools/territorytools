using TerritoryTools.Alba.Controllers.AlbaServer;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Controllers.UseCases
{
    public class AlbaHtmlUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public DateTime Created { get; set; }
        public DateTime? LastSignIn { get; set; }
        public string LastIPAddress { get; set; }
        public DateTime Updated { get; set; }
    }

    public class DownloadUserManagementData
    {
        private AlbaConnection client;

        public DownloadUserManagementData(AlbaConnection client)
        {
            this.client = client;
        }

        public void SaveAs(string fileName)
        {
            var html = client.DownloadString(
                RelativeUrlBuilder.GetUserManagementPage());

            List<AlbaHtmlUser> users = GetUsers(html);
        }

        public static List<AlbaHtmlUser> GetUsers(string html)
        {
            var users = new List<AlbaHtmlUser>();

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var rowNodes = doc.DocumentNode.SelectNodes("//tr");

            if (rowNodes != null)
            {
                foreach (HtmlNode rowNode in rowNodes)
                {
                    string userIdString = rowNode.GetAttributeValue("id", string.Empty);

                    if (!string.IsNullOrWhiteSpace(userIdString))
                    {
                        userIdString = userIdString.Replace("us", string.Empty);
                    }

                    int.TryParse(userIdString, out int userId);

                    var user = new AlbaHtmlUser
                    {
                        Id = userId
                    };

                    var colNodes = rowNode.SelectNodes("td");

                    // Using the HTML InnerText property the date is smashed 
                    // against the IP Address like this:
                    // November 21, 2015192.168.1.1
                    // But the year is always four digits, so we can parse
                    // it with the following Regular Expression
                    var dateAndIPAddressPattern = new Regex(@"(\w+ \d+, \d{4})(\d+\.\d+\.\d+\.\d+)");

                    if (colNodes != null)
                    {
                        for (int col = 0; col < colNodes.Count; col++)
                        {
                            HtmlNode colNode = colNodes[col];
                            switch (col)
                            {
                                case 0:
                                    // User ID is already parsed from attribute
                                    break;
                                case 1:
                                    user.UserName = colNode.SelectSingleNode("strong").InnerText;
                                    break;
                                case 2:
                                    user.Name = colNode.InnerText;
                                    break;
                                case 3:
                                    user.Email = colNode.InnerText;
                                    break;
                                case 4:
                                    user.Telephone = colNode.InnerText;
                                    break;
                                case 5:
                                    // Password cannot be downloaded (and should not)
                                    break;
                                case 6:
                                    user.Role = colNode.SelectSingleNode("span").InnerText;
                                    break;
                                case 7:
                                    if (!string.IsNullOrWhiteSpace(colNode.InnerText))
                                    {
                                        user.Created = colNode.InnerText == null
                                        ? DateTime.MinValue
                                        : DateTime.Parse(colNode.InnerText);
                                    }

                                    break;
                                case 8:
                                    if (!string.IsNullOrWhiteSpace(colNode.InnerText))
                                    {
                                        var match = dateAndIPAddressPattern.Match(colNode.InnerText);
                                        if (match.Success && match.Groups.Count >= 2)
                                        {
                                            user.LastSignIn = DateTime.Parse(match.Groups[1].Value);
                                            user.LastIPAddress = match.Groups[2].Value;
                                        }
                                    }

                                    break;
                            }
                        }

                        users.Add(user);
                    }
                }
            }

            return users;
        }
    }
}
