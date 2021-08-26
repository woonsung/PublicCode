using System;

namespace PublicCommonControls.WCalendar
{
    public class DateEventArgs : EventArgs
    {
        public DateEventArgs(DateTime date)
        {
            this.Date = date;
        }
        public DateTime Date { get; protected set; }
    }

}
