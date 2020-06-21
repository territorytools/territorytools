﻿namespace TerritoryTools.Web.Data.Models
{
    public class ShortUrlCreationRequest
    {
        public int Id { get; set; }

        public string OriginalUrl { get; set; }

        public string HostName { get; set; }
        
        public string Path { get; set; }

        public string Subject { get; set; }

        public string LetterLink { get; set; }

        public string Note { get; set; }

        public string UserName { get; set; }
    }
}
