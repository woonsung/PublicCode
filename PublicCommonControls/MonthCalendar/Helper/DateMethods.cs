using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PublicCommonControls.WCalendar
{
    internal static class DateMethods
    {
        public static int GetDaysInMonth(MonthCalendarDate date)
        {
            return GetDaysInMonth(date.Calendar, date.Year, date.Month);
        }
        public static int GetDaysInMonth(Calendar cal, int year, int month)
        {
            return (cal ?? CultureInfo.CurrentUICulture.Calendar).GetDaysInMonth(year, month);
        }
        public static int GetWeekOfYear(CultureInfo info, Calendar cal, DateTime date)
        {
            CultureInfo ci = info ?? CultureInfo.CurrentUICulture;
            Calendar c = cal ?? ci.Calendar;
            return c.GetWeekOfYear(date, ci.DateTimeFormat.CalendarWeekRule, ci.DateTimeFormat.FirstDayOfWeek);
        }
        public static string[,] GetDayNames(ICustomFormatProvider provider)
        {
            List<string> abbDayNames = new List<string>(provider.AbbreviatedDayNames);
            List<string> shortestDayName = new List<string>(provider.ShortestDayNames);
            List<string> dayNames = new List<string>(provider.DayNames);
            string firstDayName = provider.GetAbbreviatedDayName(provider.FirstDayOfWeek);
            int firstNameIndex = abbDayNames.IndexOf(firstDayName);
            string[,] names = new string[3, dayNames.Count];
            int j = 0;
            for(int i  = firstNameIndex; i < abbDayNames.Count; i++, j++)
            {
                names[0, j] = dayNames[i];
                names[1, j] = abbDayNames[i];
                names[2, j] = shortestDayName[i];
            }
            for(int i = 0; i < firstNameIndex; i++, j++)
            {
                names[0, j] = dayNames[i];
                names[1, j] = abbDayNames[i];
                names[2, j] = shortestDayName[i];
            }
            return names;
        }
        public static string[] GetDayNames(ICustomFormatProvider provider, int index)
        {
            if (index < 0 || index > 2)
                index = 0;
            string[,] dayNames = GetDayNames(provider);
            string[] names = new string[dayNames.GetLength(1)];
            for(int i = 0; i < names.Length; i++)
            {
                names[i] = dayNames[index, i];
            }
            return names;
        }
        public static List<DayOfWeek> GetWorkDays(CalendarDayOfWeek nonWorkDays)
        {
            List<DayOfWeek> workDays = new List<DayOfWeek>();
            for (int i = 0; i < 7; i++)
                workDays.Add((DayOfWeek)i);
            GetSysDaysOfWeek(nonWorkDays).ForEach(day =>
            {
                if (workDays.Contains(day))
                    workDays.Remove(day);
            });
            return workDays;
        }
        public static bool IsRTLCulture(CultureInfo info)
        {
            return CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft;
        }
        public static string GetNumberString(int number, string[] nativeDigits, bool prefixZero)
        {
            if (nativeDigits == null || nativeDigits.Length != 10)
                return prefixZero ? number.ToString("##00") : number.ToString(CultureInfo.CurrentUICulture);
            return GetNativeNumberString(number, nativeDigits, prefixZero);
        }
        public static string GetNativeNumberString(int number, string[] nativeDigits, bool prefixZero)
        {
            if (nativeDigits == null || nativeDigits.Length != 10)
                return string.Empty;
            if (number > -1 && number < 10)
                return prefixZero ? nativeDigits[0] + nativeDigits[number] : nativeDigits[number];
            var list = new List<int>();
            while(number != 0)
            {
                list.Insert(0, number % 10);
                number = number / 10;
            }
            return list.ConvertAll(i => nativeDigits[i]).Aggregate((s1, s2) => s1 + s2);
        }
        private static List<DayOfWeek> GetSysDaysOfWeek(CalendarDayOfWeek days)
        {
            List<DayOfWeek> list = new List<DayOfWeek>();
            if((days & CalendarDayOfWeek.Monday) == CalendarDayOfWeek.Monday)
                list.Add(DayOfWeek.Monday);
            if ((days & CalendarDayOfWeek.Tuesday) == CalendarDayOfWeek.Tuesday)
                list.Add(DayOfWeek.Tuesday);
            if ((days & CalendarDayOfWeek.Wednesday) == CalendarDayOfWeek.Wednesday)
                list.Add(DayOfWeek.Wednesday);
            if ((days & CalendarDayOfWeek.Thursday) == CalendarDayOfWeek.Thursday)
                list.Add(DayOfWeek.Thursday);
            if ((days & CalendarDayOfWeek.Friday) == CalendarDayOfWeek.Friday)
                list.Add(DayOfWeek.Friday);
            if ((days & CalendarDayOfWeek.Saturday) == CalendarDayOfWeek.Saturday)
                list.Add(DayOfWeek.Saturday);
            if ((days & CalendarDayOfWeek.Sunday) == CalendarDayOfWeek.Sunday)
                list.Add(DayOfWeek.Sunday);
            return list;

        }
    }
}
