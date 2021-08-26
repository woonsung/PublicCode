using System;

namespace PublicCommonControls.WCalendar
{
    internal class MonthCalendarEraRange
    {
        public MonthCalendarEraRange(int era, DateTime minDate)
        {
            this.Era = era;
            this.MinDate = minDate;
        }
        public MonthCalendarEraRange(int era, DateTime minDate, DateTime maxDate)
            :this(era, minDate)
        {
            this.MaxDate = maxDate;
        }
        public int Era { get; set; }
        public DateTime MinDate { get; set; }
        public DateTime MaxDate { get; set; }
    }
}
