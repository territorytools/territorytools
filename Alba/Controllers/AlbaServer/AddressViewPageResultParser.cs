﻿using System.Text.Json;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;

namespace TerritoryTools.Alba.Controllers.AlbaServer
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

            return JsonDocument.Parse(value)
                .RootElement
                .GetProperty("data")
                .GetProperty("html")
                .GetProperty("addresses")
                .GetString();
        }

        public static List<AddressView> GetAddresses(string html)
        {
            var users = new List<AddressView>();
            // TODO: This doesn't seem worth implementing now
            return users;
        }
    }
}
