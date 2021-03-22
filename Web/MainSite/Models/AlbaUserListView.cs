using System;
using System.Collections.Generic;

namespace TerritoryTools.Web.MainSite.Models
{
    public class AlbaUserListView
    {
        public List<AlbaUserView> Users { get; set; }
            = new List<AlbaUserView>();

        public bool IsChecked { get; set; }
    }
}
