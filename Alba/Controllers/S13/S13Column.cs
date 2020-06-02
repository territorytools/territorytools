using System.Collections.Generic;

namespace Controllers.S13
{
    public class S13Column
    {
        public string Territory { get; set; }
        public List<S13Entry> Entries { get; set; } = new List<S13Entry>();
    }
}
