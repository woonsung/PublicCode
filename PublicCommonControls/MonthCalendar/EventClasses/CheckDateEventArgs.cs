using System;

namespace PublicCommonControls.WCalendar
{
    public class CheckDateEventArgs : EventArgs
    {
        public CheckDateEventArgs(DateTime date, bool valid)
        {
            this.Date = date;
            this.IsValid = valid;
        }
        public DateTime Date { get; set; }
        public bool IsValid { get; set; }
    }
}
