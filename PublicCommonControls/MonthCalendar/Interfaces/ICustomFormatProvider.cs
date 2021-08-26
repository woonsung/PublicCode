using System;
using System.Globalization;

namespace PublicCommonControls.WCalendar
{
    public interface ICustomFormatProvider
    {
        DateTimeFormatInfo DateTimeFormat { get; set; }
        Calendar Calendar { get; set; }
        string[] DayNames { get; set; }
        string[] AbbreviatedDayNames { get; set; }
        string[] ShortestDayNames { get; set; }
        DayOfWeek FirstDayOfWeek { get; set; }
        string[] MonthNames { get; set; }
        string[] AbbreviatedMonthNames { get; set; }
        string DateSeparator { get; set; }
        string ShortDatePattern { get; set; }
        string LongDatePattern { get; set; }
        string MonthDayPattern { get; set; }
        string YearMonthPattern { get; set; }
        bool IsRTLLanguage { get; set; }
        string GetDayName(DayOfWeek dayofweek);
        string GetAbbreviatedDayName(DayOfWeek dayofweek);
        string GetShortestDayName(DayOfWeek dayofweek);
        string GetMonthName(int year, int month);
        string GetAbbreviatedMonthName(int year, int month);
        int GetMonthsInYear(int year);
        string GetEraName(int era);


    }
}
