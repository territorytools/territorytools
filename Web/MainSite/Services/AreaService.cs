using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TerritoryTools.Web.MainSite.Services
{
    public class AreaService
    {
        public const string ParentAreaPrefix = "PARENT/";

        private readonly IConfiguration _configuration;

        public AreaService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<Models.Area> All()
        {
            var areas = new List<Models.Area>();
            string areaNamesString = _configuration.GetValue<string>("TT_AreaNames");
            if (!string.IsNullOrWhiteSpace(areaNamesString))
            {
                string[] areaRows = areaNamesString.Split(";", StringSplitOptions.RemoveEmptyEntries);
                foreach (string areaRow in areaRows)
                {
                    string[] areaRowParts = areaRow.Split(":", StringSplitOptions.RemoveEmptyEntries);
                    Models.Area area = new Models.Area
                    {
                        Code = areaRowParts.Length > 0 ? areaRowParts[0] : "",
                        Name = areaRowParts.Length > 1 ? areaRowParts[1] : "",
                        Parent = areaRowParts.Length > 2 ? areaRowParts[2] : "",
                    };

                    areas.Add(area);
                }
            }

            var parents = areas.GroupBy(a => a.Parent).Select(a => a.Key).ToList();
            foreach (var parent in parents)
            {
                areas.Add(new Models.Area { 
                    Code = ParentAreaPrefix + parent, 
                    Parent = parent,
                    IsParent = true
                });
            }

            areas = areas
                .OrderBy(a => a.Parent)
                .ThenBy(a => a.Name)
                .ToList();

            return areas;
        }
    }
}
