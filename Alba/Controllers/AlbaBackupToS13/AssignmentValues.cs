using System;
using System.Collections.Generic;
using System.Linq;
using Controllers.UseCases;

namespace TerritoryTools.Alba.Controllers.AlbaBackupToS13
{
    public class AssignmentValues
    {
        public string Number { get; set; }
        public string Description { get; set; }
        public string Kind { get; set; }
        public string Status { get; set; }
        public DateTime? LastCompleted { get; set; }
        public string LastCompletedBy { get; set; }
        public DateTime? SignedOut { get; set; }   
        public string SignedOutTo { get; set; }

        public static List<AssignmentValues> LoadFromCsv(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));
            try
            {
                IEnumerable<AssignmentValues> csv = LoadCsv
                    .LoadFrom<AssignmentValues>(path);

                return csv.ToList();
            }
            catch(Exception e)
            {
                throw new Exception($"Path:{path} Error:{e.Message}", e);
            }
        }
    }
}
