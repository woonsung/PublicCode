using System;

namespace PublicCommonControls.WCalendar
{
    public static class ExtensionMethods
    {
        public static bool Contains(this System.Windows.Forms.SelectionRange range, DateTime date)
        {
            date = date.Date;
            if (range.Start == DateTime.MinValue)
                return date == range.End;
            if (range.End == DateTime.MaxValue)
                return date == range.Start;
            return date >= range.Start && date <= range.End;
        }
    }
}
