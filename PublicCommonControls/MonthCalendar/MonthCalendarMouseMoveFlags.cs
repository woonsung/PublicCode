using System;

namespace PublicCommonControls.WCalendar
{
    internal class MonthCalendarMouseMoveFlags
    {
        private MonthCalendarMouseMoveFlags backup;
        public MonthCalendarMouseMoveFlags()
        {
            this.Reset();
        }
        public bool LeftArrow { get; set; }
        public bool RightArrow { get; set; }
        public bool WeekHeader { get; set; }
        public bool Footer { get; set; }
        public DateTime MonthName { get; set; }
        public DateTime Year { get; set; }
        public DateTime HeaderDate { get; set; }
        public DateTime Day { get; set; }
        public MonthCalendarMouseMoveFlags Backup
        {
            get { return this.backup ?? (this.backup = new MonthCalendarMouseMoveFlags()); }
        }
        public bool LeftArrowChanged
        {
            get { return this.LeftArrow != this.Backup.LeftArrow; }
        }
        public bool RightArrowChanged
        {
            get { return this.RightArrow != this.Backup.RightArrow; }
        }
        public bool WeekHeaderChanged
        {
            get { return this.WeekHeader != this.Backup.WeekHeader; }
        }
        public bool MonthNameChanged
        {
            get { return this.MonthName != this.Backup.MonthName; }
        }
        public bool YearChanged
        {
            get { return this.Year != this.Backup.Year; }
        }
        public bool HeaderDateChanged
        {
            get { return this.HeaderDate != this.Backup.HeaderDate; }
        }
        public bool DayChanged
        {
            get { return this.Day != this.Backup.Day; }
        }
        public bool FooterChanged
        {
            get { return this.Footer != this.Backup.Footer; }
        }
        public void Reset()
        {
            this.LeftArrow = this.RightArrow = this.WeekHeader = this.Footer = false;
            this.MonthName = this.Year = this.HeaderDate = this.Day = DateTime.MinValue;
        }
        public void BackupAndReset()
        {
            this.Backup.LeftArrow = this.LeftArrow;
            this.Backup.RightArrow = this.RightArrow;
            this.Backup.WeekHeader = this.WeekHeader;
            this.Backup.MonthName = this.MonthName;
            this.Backup.Year = this.Year;
            this.Backup.HeaderDate = this.HeaderDate;
            this.Backup.Day = this.Day;
            this.Backup.Footer = this.Footer;
            this.Reset();
        }


    }
}
