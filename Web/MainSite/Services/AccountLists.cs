using System;
using System.Collections.Generic;

namespace WebUI.Services
{
    public interface IAccountLists
    {
        Dictionary<string, Area> Areas { get; set; }
    }

    public class AccountLists : IAccountLists
    {
        public AccountLists(string areas)
        {
            if (string.IsNullOrWhiteSpace(areas))
            {
                return;
            }

            foreach (string area in areas.Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                var areaSplit = area.Split(':', StringSplitOptions.None);
                string key = areaSplit.Length > 0 ? areaSplit[0] : string.Empty;
                Areas[key] = new Area
                {
                    Code = key,
                    Name = areaSplit.Length > 1 ? areaSplit[1] : string.Empty,
                    Group = areaSplit.Length > 2 ? areaSplit[2] : string.Empty,
                };
            }
        }
        
        public Dictionary<string, Area> Areas { get; set; } = new Dictionary<string, Area>();
    }

    public class Area
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
    }
}
