using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace AlbaClient.Controllers.AlbaServer
{
    public class AddressView
    {
        public string Id { get; set; }
        public string Group { get; set; }
        public string TerritoryDescriptionOrStatus { get; set; }
        public string Status { get; set; }
        public string Language { get; set; }
        public string Name { get; set; }
        public string Telephone { get; set; }
        public string Contacted { get; set; }
        public string GeocodeStatus { get; set; }

        /// <summary>
        /// Comma separated: Suite, Address, City, State
        /// </summary>
        public string FullAddress { get; set; }
        public string CreatedNote { get; set; }
        public string ModifiedNote { get; set; }
        public string TerritoryCompletedNote { get; set; }
        public string Notes { get; set; }
        public string PrivateNotes { get; set; }


    }

    public class AddressViewPageResultParser
    {
        const string signedOutErrorMessage = "Sorry, you have been signed out.";

        public static string Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value) 
                || value.StartsWith(signedOutErrorMessage))
            {
                throw new Exception(signedOutErrorMessage);
            }

            var nodes = JObject.Parse(value);
            var html = nodes.SelectToken("data.html") as JObject;

            var text = html.Property("addresses").Value.ToString();

            return text;
        }

        public static List<AddressView> GetAddresses(string html)
        {
            var users = new List<AddressView>();

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

                    var user = new AddressView
                    {
                        Id = userId
                    };

                    var colNodes = rowNode.SelectNodes("td");

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
                                    // TODO: Add later
                                    //user.Created = colNode.InnerText;
                                    break;
                                case 8:
                                    // user.LastSignIn = colNode.InnerText;
                                    // IP Address
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
