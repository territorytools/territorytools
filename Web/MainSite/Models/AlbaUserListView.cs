using System;
using System.Collections.Generic;

namespace WebUI.Models
{
    public class AlbaUserListView
    {
        public List<AlbaUserView> Users { get; set; }
            = new List<AlbaUserView>();

        public bool IsChecked { get; set; }
    }
}
