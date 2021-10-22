using System.Collections.Generic;

namespace TerritoryTools.Alba.Controllers.AlbaBackupToS13
{
    public class NumberComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if(int.TryParse(x, out int xInt) && int.TryParse(y, out int yInt))
            {
                return xInt.CompareTo(yInt);
            }

            return x.CompareTo(y);
        }
    }
}
