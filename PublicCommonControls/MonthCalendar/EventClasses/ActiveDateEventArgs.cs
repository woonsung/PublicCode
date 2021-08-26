using System;

namespace PublicCommonControls.WCalendar
{
    public class ActiveDateChangedEventArgs : DateEventArgs
    {
        public ActiveDateChangedEventArgs(DateTime date, bool boldDate)
        :base (date)
        {
            this.IsBoldDate = boldDate;
        }
        public bool IsBoldDate { get; protected set; }
    }
}
