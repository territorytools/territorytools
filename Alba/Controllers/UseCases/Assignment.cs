using System;

namespace Alba.Controllers.UseCases
{
    public class Assignment
    {
        public int Id { get; set; }
        public string IdString { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public string Kind { get; set; }
        public int Addresses { get; set; }
        public string Status { get; set; }
        public DateTime? LastCompleted { get; set; }
        public string LastCompletedBy { get; set; }
        public DateTime? SignedOut { get; set; }
        public string SignedOutString { get; set; }
        public string SignedOutTo { get; set; }
        public int? MonthsSignedOut { get; set; }
        public int? MonthsAgoCompleted { get; set; }

        public string TimeSpanCompletedString()
        {
            if(TimeSpanCompleted().TotalSeconds == 0)
            {
                return "Never";
            }
            else if(TimeSpanCompleted() <= TimeSpan.FromDays(30))
            {
                return $"{(int)TimeSpanCompleted().TotalDays} days";
            }
            else 
            {
                int months = (int)TimeSpanCompleted().TotalDays / 30;
                return $"{months} months";
            }
        }

        public TimeSpan TimeSpanCompleted()
        {
            if (LastCompleted == null)
            {
                return TimeSpan.FromSeconds(0);
            }

            return DateTime.Now.Subtract((DateTime)LastCompleted);
        }
        public string MobileLink { get; set; }
    }
}
