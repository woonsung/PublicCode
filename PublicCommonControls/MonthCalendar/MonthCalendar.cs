using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace PublicCommonControls.WCalendar
{
    [Designer(typeof(Design.MonthCalendarControlDesigner))]
    [DefaultEvent("DateSelected")]
    [DefaultProperty("ViewStart")]
    [ToolboxBitmap(typeof(System.Windows.Forms.MonthCalendar))]
    public class MonthCalendar : Control
    {
        private const int SETREDRAW = 11;
        private readonly BoldedDatesCollection boldDatesCollection;
        private readonly BoldedDateCategoryCollection boldDateCategroyCollection;
        private bool rightToLeftLayout;
        private bool showFooter;
        private bool selectionStarted;
        private bool showingMenu;
        private bool showWeekHeader;
        private bool inUpdate;
        private bool useShortestDayNames;
        private bool useNativeDitits;
        private int headerHeight;
        private int dayWidth;
        private int dayHeight;
        private int footerHeight;
        private int weekNumberWidth;
        private int dayNameHeight;
        private int monthWidth;
        private int monthHeight;
        private int scrollChange;
        private int maxSelectionCount;
        private MonthCalendarRenderer renderer;
        private Font headerFont;
        private Font footerFont;
        private Font dayHeaderFont;
        private Size calendarDimensions;
        private Point mouseLocation;
        private DateTime viewStart;
        private DateTime realStart;
        private DateTime lastVisibleDate;
        private DateTime yearSelected;
        private DateTime monthSelected;
        private MonthCalendarHitType currentHitType;
        private List<DateTime> boldedDates;
        private Rectangle footerRect;
        private Rectangle leftArrowRect;
        private Rectangle rightArrowRect;
        private Rectangle currentMoveBounds;
        private ContentAlignment dayTextAlign;
        private DateTime selectionStart;
        private DateTime selectionEnd;
        private DateTime minDate;
        private DateTime maxDate;
        private MonthCalendarMonth[] months;
        private MonthCalendarMouseMoveFlags mouseMoveFlags;
        private SelectionRange selectionStartRange;
        private SelectionRange backupRange;
        private List<SelectionRange> selectionRanges;
        private MonthCalendarSelectionMode daySelectionMode;
        private CalendarDayOfWeek nonWorkDays;
        private CultureInfo culture;
        private Calendar cultureCalendar;
        private ICustomFormatProvider formatProvider;
        private MonthCalendarEraRange[] eraRanges;
        private ContextMenuStrip monthMenu;
        private ToolStripMenuItem tsmiJan;
        private ToolStripMenuItem tsmiFeb;
        private ToolStripMenuItem tsmiMar;
        private ToolStripMenuItem tsmiApr;
        private ToolStripMenuItem tsmiMay;
        private ToolStripMenuItem tsmiJun;
        private ToolStripMenuItem tsmiJul;
        private ToolStripMenuItem tsmiAug;
        private ToolStripMenuItem tsmiSep;
        private ToolStripMenuItem tsmiOct;
        private ToolStripMenuItem tsmiNov;
        private ToolStripMenuItem tsmiDez;
        private ContextMenuStrip yearMenu;
        private ToolStripMenuItem tsmiYear1;
        private ToolStripMenuItem tsmiYear2;
        private ToolStripMenuItem tsmiYear3;
        private ToolStripMenuItem tsmiYear4;
        private ToolStripMenuItem tsmiYear5;
        private ToolStripMenuItem tsmiYear6;
        private ToolStripMenuItem tsmiYear7;
        private ToolStripMenuItem tsmiYear8;
        private ToolStripMenuItem tsmiYear9;
        private ToolStripMenuItem tsmiA1;
        private ToolStripMenuItem tsmiA2;
        private IContainer components;
        private bool extendSelection;
        public MonthCalendar()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.Selectable | ControlStyles.AllPaintingInWmPaint
                | ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor | ControlStyles.SupportsTransparentBackColor, true);
            this.InitializeComponent();
            this.extendSelection = true;
            this.showFooter = true;
            this.showWeekHeader = true;
            this.mouseLocation = Point.Empty;
            this.yearSelected = DateTime.MinValue;
            this.monthSelected = DateTime.MinValue;
            this.selectionStart = DateTime.Today;
            this.selectionEnd = DateTime.Today;
            this.currentHitType = MonthCalendarHitType.None;
            this.boldedDates = new List<DateTime>();
            this.boldDatesCollection = new BoldedDatesCollection();
            this.boldDateCategroyCollection = new BoldedDateCategoryCollection(this);
            this.currentMoveBounds = Rectangle.Empty;
            this.mouseMoveFlags = new MonthCalendarMouseMoveFlags();
            this.selectionRanges = new List<SelectionRange>();
            this.daySelectionMode = MonthCalendarSelectionMode.Manual;
            this.nonWorkDays = CalendarDayOfWeek.Saturday | CalendarDayOfWeek.Sunday;
            this.culture = CultureInfo.CurrentCulture;
            this.cultureCalendar = CultureInfo.CurrentUICulture.DateTimeFormat.Calendar;
            this.eraRanges = GetEraRanges(this.cultureCalendar);
            this.minDate = this.cultureCalendar.MinSupportedDateTime.Date < new DateTime(1900, 1, 1) ? new DateTime(1900, 1, 1) : this.cultureCalendar.MinSupportedDateTime.Date;
            this.maxDate = this.cultureCalendar.MaxSupportedDateTime.Date > new DateTime(9998, 12, 31) ? new DateTime(9998, 12, 31) : this.cultureCalendar.MaxSupportedDateTime.Date;
            this.formatProvider = new MonthCalendarFormatProvider(this.culture, null, this.culture.TextInfo.IsRightToLeft) { MonthCalendar = this };
            this.renderer = new MonthCalendarRenderer(this);
            this.calendarDimensions = new Size(1, 1);
            this.headerFont = new Font("Segoe UI", 9f, FontStyle.Regular);
            this.footerFont = new Font("Arial", 9f, FontStyle.Bold);
            this.dayHeaderFont = new Font("Segoe UI", 8f, FontStyle.Regular);
            this.dayTextAlign = ContentAlignment.MiddleCenter;
            this.UpdateYearMenu(DateTime.Today.Year);
            this.SetStartDate(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1));
            this.CalculateSize(true);
        }
        [Category("Action")]
        [Description("Is raised, when the user selected a date or date range.")]
        public event EventHandler<DateRangeEventArgs> DateSelected;
        [Category("Action")]
        [Description("Is raised, when the user changed the month or year.")]
        public event EventHandler<DateRangeEventArgs> DateChanged;
        [Category("Action")]
        [Description("Is raised, when the mouse is over a date.")]
        public event EventHandler<ActiveDateChangedEventArgs> ActiveDateChanged;
        [Category("Action")]
        [Description("Is rased when the selection extension ended.")]
        public event EventHandler SelectionExtendEnd;
        [Category("Action")]
        [Description("Is raised when a date in selection mode 'Day' was clicked.")]
        public event EventHandler<DateEventArgs> DateClicked;

        internal event EventHandler<DateEventArgs> InternalDateSelected;

        [Category("Appearance")]
        [Description("Sets the first displayed month and year.")]
        public DateTime ViewStart
        {
            get
            {
                return this.viewStart;
            }
            set
            {
                if (value == this.viewStart || value < this.minDate || value > this.maxDate)
                    return;
                this.SetStartDate(value);
                this.UpdateMonths();
                this.Refresh();

            }
        }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DateTime ViewEnd
        {
            get
            {
                MonthCalendarDate dt = new MonthCalendarDate(this.cultureCalendar, this.viewStart).GetEndDateOfWeek(this.formatProvider)
                    .FirstOfMonth.AddMonth(this.months != null ? this.months.Length - 1 : 1).FirstOfMonth;
                int daysInMonth = dt.DaysInMonth;
                dt = dt.AddDays(daysInMonth - 1);
                return dt.Date;
            }
        }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DateTime RealStartDate
        {
            get { return this.realStart; }
        }
        [Category("Behavior")]
        [Description("The viewable minimum month and year.")]
        public DateTime MinDate
        {
            get
            {
                return this.minDate;
            }
            set
            {
                if (value < this.CultureCalendar.MinSupportedDateTime || value > this.CultureCalendar.MaxSupportedDateTime || value >= this.maxDate)
                    return;
                value = this.GetMinDate(value);
                this.minDate = value.Date;
                this.UpdateMonths();
                int dim1 = Math.Max(1, this.calendarDimensions.Width * this.calendarDimensions.Height);
                int dim2 = this.months != null ? this.months.Length : 1;
                if (dim1 != dim2)
                    this.SetStartDate(new MonthCalendarDate(this.CultureCalendar, this.viewStart).AddMonth(dim2 - dim1).Date);
                this.Invalidate();
            }
        }
        [Category("Behavior")]
        [Description("The viewable maximum month and year.")]
        public DateTime MaxDate
        {
            get
            {
                return this.maxDate;
            }
            set
            {
                if (value < this.CultureCalendar.MinSupportedDateTime || value > this.CultureCalendar.MaxSupportedDateTime || value <= this.minDate)
                    return;
                value = this.GetMaxDate(value);
                this.maxDate = value.Date;
                this.UpdateMonths();
                int dim1 = Math.Max(1, this.calendarDimensions.Width * this.calendarDimensions.Height);
                int dim2 = this.months != null ? this.months.Length : 1;
                if (dim1 != dim2)
                    this.SetStartDate(new MonthCalendarDate(this.CultureCalendar, this.viewStart).AddMonth(dim2 - dim1).Date);
                this.Invalidate();
            }
        }
        [Category("Appearance")]
        [Description("The number of rows and columns of months in the calendar.")]
        [DefaultValue(typeof(Size), "1,1")]
        public Size CalendarDimensions
        {
            get
            {
                return this.calendarDimensions;
            }
            set
            {
                if (value == this.calendarDimensions || value.IsEmpty)
                    return;
                value.Width = Math.Max(1, Math.Min(value.Width, 7));
                value.Height = Math.Max(1, Math.Min(value.Height, 7));
                this.calendarDimensions = value;
                this.inUpdate = true;
                this.Width = value.Width * monthWidth;
                this.Height = (value.Height * this.monthHeight) + (this.showFooter ? this.footerHeight : 0);
                this.inUpdate = false;
                this.scrollChange = Math.Max(0, Math.Min(this.scrollChange, this.calendarDimensions.Width * this.calendarDimensions.Height));
                this.CalculateSize(false);
            }
        }
        [Category("Appearance")]
        [Description("The font for the header.")]
        public Font HeaderFont
        {
            get
            {
                return this.headerFont;
            }
            set
            {
                if (value == this.headerFont || value == null)
                    return;
                this.BeginUpdate();
                if (this.headerFont != null)
                    this.headerFont.Dispose();
                this.headerFont = value;
                this.CalculateSize(false);
                this.EndUpdate();
            }
        }
        [Category("Appearance")]
        [Description("The font for the footer.")]
        public Font FooterFont
        {
            get
            {
                return this.footerFont;
            }
            set
            {
                if (value == this.footerFont || value == null)
                    return;
                this.BeginUpdate();
                if (this.footerFont != null)
                    this.footerFont.Dispose();
                this.footerFont = value;
                this.CalculateSize(false);
                EndUpdate();
            }
        }
        [Category("Appearance")]
        [Description("The font for the day header.")]
        public Font DayHeaderFont
        {
            get
            {
                return this.dayHeaderFont;
            }
            set
            {
                if (value == this.headerFont || value == null)
                    return;
                this.BeginUpdate();
                if (this.dayHeaderFont != null)
                    this.dayHeaderFont.Dispose();
                this.dayHeaderFont = value;
                this.CalculateSize(false);
                this.EndUpdate();
            }
        }
        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                this.BeginUpdate();
                base.Font = value;
                this.CalculateSize(false);
                this.EndUpdate();
            }
        }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Size Size
        {
            get { return base.Size; }
            set { base.Size = value; }
        }
        [DefaultValue(typeof(ContentAlignment), "MiddleCenter")]
        [Description("Determines the text alignment for the days.")]
        public ContentAlignment DayTextAlignment
        {
            get
            {
                return this.dayTextAlign;
            }
            set
            {
                if (value == this.dayTextAlign)
                    return;
                this.dayTextAlign = value;
                this.Invalidate();
            }
        }
        [DefaultValue(false)]
        [Category("Appearance")]
        [Description("Indicates whether to use the RTL layout.")]
        public bool RightToLeftLayout
        {
            get
            {
                return this.rightToLeftLayout;
            }
            set
            {
                if (value == this.rightToLeftLayout)
                    return;
                this.rightToLeftLayout = value;
                this.formatProvider.IsRTLLanguage = this.UseRTL;
                Size calDim = this.calendarDimensions;
                this.UpdateMonths();
                this.CalendarDimensions = calDim;
                this.Refresh();
            }
        }
        [DefaultValue(true)]
        [Category("Appearance")]
        [Description("Indicates whether to show the footer.")]
        public bool ShowFooter
        {
            get
            {
                return this.showFooter;
            }
            set
            {
                if (value == this.showFooter)
                    return;
                this.showFooter = value;
                this.Height += value ? this.footerHeight : -this.footerHeight;
                this.Invalidate();
            }
        }
        [DefaultValue(true)]
        [Category("Appearance")]
        [Description("Indicates whether to show the week header.")]
        public bool ShowWeekHeader
        {
            get
            {
                return this.showWeekHeader;
            }
            set
            {
                if (this.showWeekHeader == value)
                    return;
                this.showWeekHeader = value;
                this.CalculateSize(false);
            }
        }
        [DefaultValue(false)]
        [Category("Appearance")]
        [Description("Indecates whether to use the shortest or the abbreviated day names in th day header.")]
        public bool UseShortestDayNames
        {
            get
            {
                return this.useShortestDayNames;
            }
            set
            {
                this.useShortestDayNames = value;
                this.CalculateSize(false);
            }
        }
        [DefaultValue(false)]
        [Category("Appearance")]
        [Description("Indicates whether to use the native digits as specified by the current Culture property.")]
        public bool UseNativeDigits
        {
            get { return this.useNativeDitits; }
            set
            {
                if (value == this.useNativeDitits)
                    return;
                this.useNativeDitits = value;
                this.Refresh();
            }
        }
        [Description("The bolded dates in th month calendar.")]
        public List<DateTime> BoldedDates
        {
            get
            {
                return this.boldedDates;
            }
            set
            {
                this.boldedDates = value ?? new List<DateTime>();
            }
        }
        [Description("The bolded dates in the calendar.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public BoldedDatesCollection BoldedDatesCollection
        {
            get
            {
                return this.boldDatesCollection;
            }
        }
        [Description("The bold date categories in th calendar.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public BoldedDateCategoryCollection BoldedDateCategoryCollection
        {
            get
            {
                return this.BoldedDateCategoryCollection;
            }
        }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DateTime SelectionStart
        {
            get
            {
                return this.selectionStart;
            }
            set
            {
                value = value.Date;
                if (value < this.CultureCalendar.MinSupportedDateTime || value > this.CultureCalendar.MaxSupportedDateTime)
                    return;
                if (value < this.minDate)
                    value = this.minDate;
                else if (value > this.maxDate)
                    value = this.maxDate;
                switch (this.daySelectionMode)
                {
                    case MonthCalendarSelectionMode.Day:
                        {
                            this.selectionStart = value;
                            this.selectionEnd = value;
                            break;
                        }
                    case MonthCalendarSelectionMode.WorkWeek:
                    case MonthCalendarSelectionMode.FullWeek:
                        {
                            MonthCalendarDate dt = new MonthCalendarDate(this.CultureCalendar, value).GetFirstDayInWeek(this.formatProvider);
                            this.selectionStart = dt.Date;
                            this.selectionEnd = dt.GetEndDateOfWeek(this.formatProvider).Date;
                            break;
                        }
                    case MonthCalendarSelectionMode.Month:
                        {
                            MonthCalendarDate dt = new MonthCalendarDate(this.CultureCalendar, value).FirstOfMonth;
                            this.selectionStart = dt.Date;
                            this.selectionEnd = dt.AddMonth(1).AddDays(-1).Date;
                            break;
                        }
                    case MonthCalendarSelectionMode.Manual:
                        {
                            value = this.GetSelectionDate(this.selectionEnd, value);
                            if (value == DateTime.MinValue)
                            {
                                this.selectionEnd = value;
                                this.selectionStart = value;
                            }
                            else
                            {
                                this.selectionStart = value;
                                if (this.selectionEnd == DateTime.MinValue)
                                    this.selectionEnd = value;
                            }
                            break;
                        }
                }
                this.Refresh();
            }
        }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DateTime SelectionEnd
        {
            get
            {
                return this.selectionEnd;
            }
            set
            {
                value = value.Date;
                if (value < this.CultureCalendar.MinSupportedDateTime || value > this.CultureCalendar.MaxSupportedDateTime || this.daySelectionMode != MonthCalendarSelectionMode.Manual)
                    return;
                if (value < this.minDate)
                    value = this.minDate;
                else if (value > this.maxDate)
                    value = this.maxDate;
                if (value == DateTime.MinValue || this.selectionStart == DateTime.MinValue)
                {
                    this.selectionStart = value;
                    this.selectionEnd = value;
                    this.Refresh();
                    return;
                }
                this.selectionEnd = value;
                this.Refresh();
            }
        }
        [Category("Behavior")]
        [Description("The selection range.")]
        public SelectionRange SelectionRange
        {
            get
            {
                return new SelectionRange(this.SelectionStart, this.selectionEnd);
            }
            set
            {
                if (value == null)
                {
                    this.ResetSelectionRange();
                    return;
                }
                switch (this.daySelectionMode)
                {
                    case MonthCalendarSelectionMode.Day:
                    case MonthCalendarSelectionMode.WorkWeek:
                    case MonthCalendarSelectionMode.FullWeek:
                    case MonthCalendarSelectionMode.Month:
                        this.selectionStart = this.selectionStart == value.Start ? value.End : value.Start;
                        break;
                    case MonthCalendarSelectionMode.Manual:
                        this.selectionStart = value.Start;
                        this.selectionEnd = value.End;
                        break;

                }
            }
        }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<SelectionRange> SelectionRanges
        {
            get
            {
                return this.selectionRanges;
            }
        }
        [DefaultValue(0)]
        [Category("Behavior")]
        [Description("The number of months the calendar is going for- or backwards if clicked on an arrow."
             + " A value of 0 indicates the last visible month is the first month (fowards) and vice versa.")]
        public int ScrollChange
        {
            get
            {
                return this.scrollChange;
            }
            set
            {
                if (value == this.scrollChange)
                    return;
                this.scrollChange = value;
            }
        }
        [DefaultValue(0)]
        [Category("Behavior")]
        [Description("The maximum selectable days. A value of 0 means no limit.")]
        public int MaxSelectionCount
        {
            get
            {
                return this.maxSelectionCount;
            }
            set
            {
                if (value == this.maxSelectionCount)
                    return;
                this.maxSelectionCount = Math.Max(0, value);
            }
        }
        [DefaultValue(MonthCalendarSelectionMode.Manual)]
        [Category("Behavior")]
        [Description("Sets the day selection mode.")]
        public MonthCalendarSelectionMode SelectionMode
        {
            get
            {
                return this.daySelectionMode;
            }
            set
            {
                if (value == this.daySelectionMode || !Enum.IsDefined(typeof(MonthCalendarSelectionMode), value))
                    return;
                this.daySelectionMode = value;
                this.SetSelectionRange(value);
                this.Refresh();
            }
        }
        [DefaultValue(CalendarDayOfWeek.Saturday | CalendarDayOfWeek.Sunday)]
        [Category("Behavior")]
        [Description("Sets the non working days.")]
        [Editor(typeof(Design.FlagEnumUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public CalendarDayOfWeek NonWorkDays
        {
            get
            {
                return this.nonWorkDays;
            }
            set
            {
                if (value == this.nonWorkDays)
                    return;
                this.nonWorkDays = value;
                if (this.daySelectionMode == MonthCalendarSelectionMode.WorkWeek)
                    this.Refresh();
            }
        }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MonthCalendarRenderer Renderer
        {
            get
            {
                return this.renderer;
            }
            set
            {
                if (value == null)
                    return;
                this.renderer = value;
                this.Refresh();
            }
        }
        [Category("Behavior")]
        [Description("The culture used by the MonthCalendar.")]
        [TypeConverter(typeof(Design.CultureInfoCustomTypeConverter))]
        public CultureInfo Culture
        {
            get
            {
                return this.culture;
            }
            set
            {
                if (value == null || value.IsNeutralCulture)
                    return;
                this.culture = value;
                this.formatProvider.DateTimeFormat = value.DateTimeFormat;
                this.CultureCalendar = null;
                if (DateMethods.IsRTLCulture(value))
                {
                    this.RightToLeft = RightToLeft.Yes;
                    this.RightToLeftLayout = true;
                }
                else
                {
                    this.RightToLeft = RightToLeft.Inherit;
                    this.RightToLeftLayout = false;
                }
                this.formatProvider.IsRTLLanguage = this.UseRTL;
            }
        }
        [Category("Behavior")]
        [Description("The calendar used by the MonthCalendar.")]
        [Editor(typeof(Design.MonthCalendarCalendarUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(Design.MonthCalendarCalendarTypeConverter))]
        public Calendar CultureCalendar
        {
            get
            {
                return this.cultureCalendar;
            }
            set
            {
                if (value == null)
                    value = this.culture.Calendar;
                this.cultureCalendar = value;
                this.formatProvider.Calendar = value;
                if (value.GetType() == typeof(PersianCalendar) && !value.IsReadOnly)
                    value.TwoDigitYearMax = 1410;
                foreach (Calendar c in this.culture.OptionalCalendars)
                {
                    if (value.GetType() == c.GetType())
                    {
                        if (value.GetType() == typeof(GregorianCalendar))
                        {
                            GregorianCalendar g1 = (GregorianCalendar)value;
                            GregorianCalendar g2 = (GregorianCalendar)c;
                            if (g1.CalendarType != g2.CalendarType)
                                continue;
                        }
                        this.culture.DateTimeFormat.Calendar = c;
                        this.formatProvider.DateTimeFormat = this.culture.DateTimeFormat;
                        this.cultureCalendar = c;
                        value = c;
                        break;
                    }
                }
                this.eraRanges = GetEraRanges(value);
                this.ReAssignSelectionMode();
                this.minDate = this.GetMinDate(value.MinSupportedDateTime);
                this.maxDate = this.GetMaxDate(value.MaxSupportedDateTime);
                this.SetStartDate(DateTime.Today);
                this.CalculateSize(false);
            }
        }
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The used colors for the month calendar.")]
        public MonthCalendarColorTable ColorTable
        {
            get
            {
                return this.renderer.ColorTable;
            }
            set
            {
                if (value == null)
                    return;
                this.renderer.ColorTable = value;
            }
        }
        [TypeConverter(typeof(Design.MonthCalendarNamesProviderTypeConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Behavior")]
        [Description("Culture dependent settings for month/day names and date formatting.")]
        public ICustomFormatProvider FormatProvider
        {
            get { return this.formatProvider; }
            set { this.formatProvider = value ?? new MonthCalendarFormatProvider(this.culture, null, this.culture.TextInfo.IsRightToLeft); }
        }
        internal Size MonthSize
        {
            get { return new Size(this.monthWidth, this.monthHeight); }
        }
        internal Size DaySize
        {
            get { return new Size(this.dayWidth, this.dayHeight); }
        }
        internal Size FooterSize
        {
            get { return new Size(this.Width, this.footerHeight); }
        }
        internal Size HeaderSize
        {
            get { return new Size(this.monthWidth, this.headerHeight); }
        }
        internal Size DayNamesSize
        {
            get { return new Size(this.dayWidth * 7, this.dayNameHeight); }
        }
        internal Size WeekNumberSize
        {
            get { return new Size(this.weekNumberWidth, this.dayHeight * 7); }
        }
        internal DateTime MouseOverDay
        {
            get { return this.mouseMoveFlags.Day; }
        }
        internal bool UseRTL
        {
            get { return this.RightToLeft == RightToLeft.Yes && this.rightToLeftLayout; }
        }
        internal ButtonState LeftButtonState
        {
            get
            {
                return this.mouseMoveFlags.LeftArrow ? ButtonState.Pushed : ButtonState.Normal;
            }
        }
        internal ButtonState RightButtonState
        {
            get
            {
                return this.mouseMoveFlags.RightArrow ? ButtonState.Pushed : ButtonState.Normal;
            }
        }
        internal MonthCalendarHitType CurrentHitType
        {
            get { return this.currentHitType; }
        }
        internal ContextMenuStrip MonthMenu
        {
            get { return this.monthMenu; }
        }
        internal ContextMenuStrip YearMenu
        {
            get { return this.yearMenu; }
        }
        internal MonthCalendarEraRange[] EraRanges
        {
            get { return this.eraRanges; }
        }

        public void BeginUpdate()
        {
            SendMessage(this.Handle, SETREDRAW, false, 0);
        }
        public void EndUpdate()
        {
            SendMessage(this.Handle, SETREDRAW, true, 0);
            this.Refresh();
        }
        public MonthCalendarHitTest HitTest(Point location)
        {
            if (!this.ClientRectangle.Contains(location))
                return MonthCalendarHitTest.Empty;
            if (this.rightArrowRect.Contains(location))
                return new MonthCalendarHitTest(this.GetNewScrollDate(false), MonthCalendarHitType.Arrow, this.rightArrowRect);
            if (this.leftArrowRect.Contains(location))
                return new MonthCalendarHitTest(this.GetNewScrollDate(true), MonthCalendarHitType.Arrow, this.leftArrowRect);
            if (this.showFooter && this.footerRect.Contains(location))
                return new MonthCalendarHitTest(DateTime.Today, MonthCalendarHitType.Footer, this.footerRect);
            foreach(MonthCalendarMonth month in this.months)
            {
                MonthCalendarHitTest hit = month.HitTest(location);
                if (!hit.IsEmpty)
                    return hit;
            }
            return MonthCalendarHitTest.Empty;
        }

        internal List<DateTime> GetBoldedDates()
        {
            List<DateTime> dates = new List<DateTime>();
            this.boldedDates.ForEach(date =>
            {
                if (!dates.Contains(date))
                    dates.Add(date);
            });
            return dates;
        }
        internal bool IsSelcted(DateTime date)
        {
            bool selected = this.SelectionRange.Contains(date);
            this.selectionRanges.ForEach(range =>
            {
                selected |= range.Contains(date);
            });
            if(this.daySelectionMode == MonthCalendarSelectionMode.WorkWeek)
            {
                List<DayOfWeek> workDays = DateMethods.GetWorkDays(this.nonWorkDays);
                return workDays.Contains(date.DayOfWeek) && selected;
            }
            return selected;
        }
        internal void EnsureSelectedDateIsVisible()
        {
            if(this.RealStartDate > this.selectionStart || this.selectionStart > this.lastVisibleDate)
            {
                this.SetStartDate(new DateTime(this.selectionStart.Year, this.selectionStart.Month, 1));
                this.UpdateMonths();
            }
        }
        internal void SetLeftArrowRect(Rectangle rect)
        {
            if (this.UseRTL)
                this.rightArrowRect = rect;
            else
                this.leftArrowRect = rect;
        }
        internal void SetRightArrowRect(Rectangle rect)
        {
            if (this.UseRTL)
                this.leftArrowRect = rect;
            else
                this.rightArrowRect = rect;
        }
        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                if (this.headerFont != null)
                    this.headerFont.Dispose();
                if (this.footerFont != null)
                    this.footerFont.Dispose();
                if (this.dayHeaderFont != null)
                    this.dayHeaderFont.Dispose();
            }
            base.Dispose(disposing);
        }
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            this.CalculateSize(false);
        }
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if(this.daySelectionMode !=  MonthCalendarSelectionMode.Day)
                return base.ProcessDialogKey(keyData);
            MonthCalendarDate dt = new MonthCalendarDate(this.cultureCalendar, this.selectionStart);
            bool retValue = false;
            if (keyData == Keys.Left)
            {
                this.selectionStart = dt.AddDays(-1).Date;
                retValue = true;
            }
            else if (keyData == Keys.Right)
            {
                this.selectionStart = dt.AddDays(1).Date;
                retValue = true;
            }
            else if (keyData == Keys.Up)
            {
                this.selectionStart = dt.AddDays(-7).Date;
                retValue = true;
            }
            else if (keyData == Keys.Down)
            {
                this.selectionStart = dt.AddDays(7).Date;
                retValue = true;
            }
            if(retValue)
            {
                if (this.selectionStart < this.minDate)
                    this.selectionStart = this.minDate;
                else if (this.selectionStart > this.maxDate)
                    this.selectionStart = this.maxDate;
                this.SetSelectionRange(this.daySelectionMode);
                this.EnsureSelectedDateIsVisible();
                this.RaiseInternalDateSelected();
                this.Invalidate();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
                this.extendSelection = true;
            base.OnKeyDown(e);
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if(e.KeyCode == Keys.ControlKey)
            {
                this.extendSelection = false;
                this.RaiseSelectExtendEnd();
            }
            base.OnKeyUp(e);
        }
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            base.SetBoundsCore(x, y, width, height, specified);
            if (this.Created || (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime))
                this.CalculateSize(true);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.calendarDimensions.IsEmpty || this.inUpdate)
                return;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.renderer.DrawControlBackground(e.Graphics);
            foreach (MonthCalendarMonth month in this.months)
            {
                if (month == null || !e.ClipRectangle.IntersectsWith(month.Bounds))
                    continue;
                MonthCalendarHeaderState state = this.GetMonthHeaderState(month.Date);
                this.renderer.DrawTitleBackground(e.Graphics, month, state);
                this.renderer.DrawMonthBodyBackground(e.Graphics, month);
                this.renderer.DrawDayHeaderBackground(e.Graphics, month);
                this.renderer.DrawMonthHeader(e.Graphics, month, state);
                this.renderer.DrawDayHeader(e.Graphics, month.DayNamesBounds);
                if (this.showWeekHeader)
                {
                    this.renderer.DrawWeekHeaderBackground(e.Graphics, month);
                    foreach (MonthCalendarWeek week in month.Weeks)
                    {
                        if (!week.Visible)
                            continue;
                        this.renderer.DrawWeekHeaderItem(e.Graphics, week);
                    }
                }
                foreach (MonthCalendarDay day in month.Days)
                {
                    if (!day.Visible)
                        continue;
                    this.renderer.DrawDay(e.Graphics, day);
                }
                this.renderer.DrawWeekHeaderSeparator(e.Graphics, month.WeekBounds);
            }
            if(this.showFooter)
            {
                this.renderer.DrawFooterBackground(e.Graphics, this.footerRect, this.mouseMoveFlags.Footer);
                this.renderer.DrawFooter(e.Graphics, this.footerRect, this.mouseMoveFlags.Footer);
            }
            using (Pen p = new Pen(MonthCalendarAbstractRenderer.GetGrayColor(this.Enabled, this.renderer.ColorTable.Border)))
            {
                Rectangle r = this.ClientRectangle;
                r.Width--;
                r.Height--;
                e.Graphics.DrawRectangle(p, r);
            }
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            this.Focus();
            this.Capture = true;
            this.selectionStartRange = null;
            if(e.Button == MouseButtons.Left)
            {
                MonthCalendarHitTest hit = this.HitTest(e.Location);
                this.currentMoveBounds = hit.Bounds;
                this.currentHitType = hit.Type;
                switch(hit.Type)
                {
                    case MonthCalendarHitType.Day:
                        {
                            SelectionRange oldRange = this.SelectionRange;
                            if (!this.extendSelection || this.daySelectionMode != MonthCalendarSelectionMode.Manual)
                                this.selectionRanges.Clear();
                            switch (this.daySelectionMode)
                            {
                                case MonthCalendarSelectionMode.Day:
                                    {
                                        this.OnDateClicked(new DateEventArgs(hit.Date));
                                        if (this.selectionStart != hit.Date)
                                        {
                                            this.SelectionStart = hit.Date;
                                            this.RaiseDateSelected();
                                        }
                                        break;
                                    }
                                case MonthCalendarSelectionMode.WorkWeek:
                                    {
                                        DateTime firstDay = new MonthCalendarDate(this.CultureCalendar, hit.Date).GetFirstDayInWeek(this.formatProvider).Date;
                                        List<DayOfWeek> workDays = DateMethods.GetWorkDays(this.nonWorkDays);
                                        this.SelectionEnd = DateTime.MinValue;
                                        this.selectionStart = DateTime.MinValue;
                                        SelectionRange currentRange = null;
                                        for (int i = 0; i < 7; i++)
                                        {
                                            DateTime toAdd = firstDay.AddDays(i);
                                            if (workDays.Contains(toAdd.DayOfWeek))
                                            {
                                                if (currentRange == null)
                                                    currentRange = new SelectionRange(DateTime.MinValue, DateTime.MinValue);
                                                if (currentRange.Start == DateTime.MinValue)
                                                    currentRange.Start = toAdd;
                                                else
                                                    currentRange.End = toAdd;
                                            }
                                            else if (currentRange != null)
                                            {
                                                this.selectionRanges.Add(currentRange);
                                                currentRange = null;
                                            }
                                        }
                                        if (this.selectionRanges.Count >= 1)
                                        {
                                            this.SelectionRange = this.selectionRanges[0];
                                            this.selectionRanges.RemoveAt(0);
                                            if (this.SelectionRange != oldRange)
                                                this.RaiseDateSelected();
                                        }
                                        else
                                            this.Refresh();
                                        break;
                                    }
                                case MonthCalendarSelectionMode.FullWeek:
                                    {
                                        MonthCalendarDate dt = new MonthCalendarDate(this.CultureCalendar, hit.Date).GetFirstDayInWeek(this.formatProvider);
                                        this.selectionStart = dt.Date;
                                        this.selectionEnd = dt.GetEndDateOfWeek(this.formatProvider).Date;
                                        if (this.SelectionRange != oldRange)
                                        {
                                            this.RaiseDateSelected();
                                            this.Refresh();
                                        }
                                        break;
                                    }
                                case MonthCalendarSelectionMode.Month:
                                    {
                                        MonthCalendarDate dt = new MonthCalendarDate(this.CultureCalendar, hit.Date).FirstOfMonth;
                                        this.selectionStart = dt.Date;
                                        this.SelectionEnd = dt.AddMonth(1).AddDays(-1).Date;
                                        if (this.SelectionRange != oldRange)
                                        {
                                            this.RaiseDateSelected();
                                            this.Refresh();
                                        }
                                        break;
                                    }
                                case MonthCalendarSelectionMode.Manual:
                                    {
                                        if (this.extendSelection)
                                        {
                                            var range = this.selectionRanges.Find(r => hit.Date >= r.Start && hit.Date <= r.End);
                                            if (range != null)
                                                this.SelectionRanges.Remove(range);
                                        }
                                        this.selectionStarted = true;
                                        this.backupRange = this.SelectionRange;
                                        this.selectionEnd = DateTime.MinValue;
                                        this.selectionStart = hit.Date;
                                        break;
                                    }
                            }
                            break;
                        }
                    case MonthCalendarHitType.Week:
                        {
                            this.selectionRanges.Clear();
                            if(this.MaxSelectionCount > 6 || this.maxSelectionCount == 0)
                            {
                                this.backupRange = this.SelectionRange;
                                this.selectionStarted = true;
                                this.selectionEnd = new MonthCalendarDate(this.CultureCalendar, hit.Date).GetEndDateOfWeek(this.formatProvider).Date;
                                this.selectionStart = hit.Date;
                                this.selectionStartRange = this.SelectionRange;
                            }
                            break;
                        }
                    case MonthCalendarHitType.MonthName:
                        {
                            this.monthSelected = hit.Date;
                            this.mouseMoveFlags.HeaderDate = hit.Date;
                            this.Invalidate(hit.InvalidateBounds);
                            this.Update();
                            this.UpdateMonthMenu(this.CultureCalendar.GetYear(hit.Date));
                            this.monthMenu.Tag = hit.Date;
                            this.showingMenu = true;
                            this.monthMenu.Show(this, hit.Bounds.Right, e.Location.Y);
                            break;
                        }
                    case MonthCalendarHitType.MonthYear:
                        {
                            this.yearSelected = hit.Date;
                            this.mouseMoveFlags.HeaderDate = hit.Date;
                            this.Invalidate(hit.InvalidateBounds);
                            this.Update();
                            this.UpdateYearMenu(this.CultureCalendar.GetYear(hit.Date));
                            this.yearMenu.Tag = hit.Date;
                            this.showingMenu = true;
                            this.yearMenu.Show(this, hit.Bounds.Right, e.Location.Y);
                            break;
                        }
                    case MonthCalendarHitType.Arrow:
                        {
                            if(this.SetStartDate(hit.Date))
                            {
                                this.UpdateMonths();
                                this.RaiseDateChanged();
                                this.mouseMoveFlags.HeaderDate = this.leftArrowRect.Contains(e.Location) ? this.months[0].Date : this.months[this.calendarDimensions.Width - 1].Date;
                                this.Refresh();
                            }
                            break;
                        }
                    case MonthCalendarHitType.Footer:
                        {
                            this.selectionRanges.Clear();
                            bool raiseDateChanged = false;
                            SelectionRange range = this.SelectionRange;
                            if(DateTime.Today < this.months[0].FirstVisibleDate || DateTime.Today > this.lastVisibleDate)
                            {
                                if (this.SetStartDate(DateTime.Today))
                                {
                                    this.UpdateMonths();
                                    raiseDateChanged = true;
                                }
                                else
                                    break;
                            }
                            this.selectionStart = DateTime.Today;
                            this.selectionEnd = DateTime.Today;
                            this.SetSelectionRange(this.daySelectionMode);
                            this.OnDateClicked(new DateEventArgs(DateTime.Today));
                            if (range != this.SelectionRange)
                                this.RaiseDateSelected();
                            if (raiseDateChanged)
                                this.RaiseDateChanged();
                            this.Refresh();
                            break;
                        }
                    case MonthCalendarHitType.Header:
                        {
                            this.Invalidate(hit.Bounds);
                            this.Update();
                            break;
                        }
                }
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Location == this.mouseLocation)
                return;
            this.mouseLocation = e.Location;
            this.mouseMoveFlags.BackupAndReset();
            MonthCalendarHitTest hit = this.HitTest(e.Location);
            if(e.Button == MouseButtons.Left)
            {
                if (this.selectionStarted)
                {
                    if (hit.Type == MonthCalendarHitType.Day && this.currentHitType == MonthCalendarHitType.Day && this.currentMoveBounds != hit.Bounds)
                    {
                        this.currentMoveBounds = hit.Bounds;
                        this.selectionEnd = hit.Date;
                    }
                    else if (hit.Type == MonthCalendarHitType.Week && this.currentHitType == MonthCalendarHitType.Week)
                    {
                        this.mouseMoveFlags.WeekHeader = true;
                        DateTime endDate = new MonthCalendarDate(this.CultureCalendar, hit.Date).AddDays(6).Date;
                        if (this.currentMoveBounds != hit.Bounds)
                        {
                            this.currentMoveBounds = hit.Bounds;
                            if (this.selectionStart == this.selectionStartRange.End)
                            {
                                if (endDate > this.selectionStart)
                                {
                                    this.selectionStart = this.selectionStartRange.Start;
                                    this.selectionEnd = endDate;
                                }
                                else
                                {
                                    this.selectionEnd = hit.Date;
                                }
                            }
                            else
                            {
                                if (endDate > this.selectionStart)
                                    this.selectionEnd = endDate;
                                else
                                {
                                    this.selectionStart = this.selectionStartRange.End;
                                    this.selectionEnd = hit.Date;
                                }
                            }
                        }
                    }
                }
                else
                {
                    switch (hit.Type)
                    {
                        case MonthCalendarHitType.MonthName:
                            {
                                this.mouseMoveFlags.MonthName = hit.Date;
                                this.mouseMoveFlags.HeaderDate = hit.Date;
                                this.Invalidate(hit.InvalidateBounds);
                                break;
                            }
                        case MonthCalendarHitType.MonthYear:
                            {
                                this.mouseMoveFlags.Year = hit.Date;
                                this.mouseMoveFlags.HeaderDate = hit.Date;
                                this.Invalidate(hit.InvalidateBounds);
                                break;
                            }
                        case MonthCalendarHitType.Header:
                            {
                                this.mouseMoveFlags.HeaderDate = hit.Date;
                                this.Invalidate(hit.InvalidateBounds);
                                break;
                            }
                        case MonthCalendarHitType.Arrow:
                            {
                                bool useRTL = this.UseRTL;
                                if (this.leftArrowRect.Contains(e.Location))
                                {
                                    this.mouseMoveFlags.LeftArrow = !UseRTL;
                                    this.mouseMoveFlags.RightArrow = useRTL;
                                    this.mouseMoveFlags.HeaderDate = this.months[0].Date;
                                }
                                else
                                {
                                    this.mouseMoveFlags.LeftArrow = useRTL;
                                    this.mouseMoveFlags.RightArrow = !useRTL;
                                    this.mouseMoveFlags.HeaderDate = this.months[this.calendarDimensions.Width - 1].Date;
                                }
                                this.Invalidate(hit.InvalidateBounds);
                                break;
                            }
                        case MonthCalendarHitType.Footer:
                            {
                                this.mouseMoveFlags.Footer = true;
                                this.Invalidate(hit.InvalidateBounds);
                                break;
                            }
                        default:
                            {
                                this.Invalidate();
                                break;
                            }
                    }
                }
            }
            else if (e.Button == MouseButtons.None)
            {
                switch (hit.Type)
                {
                    case MonthCalendarHitType.Day:
                        {
                            this.mouseMoveFlags.Day = hit.Date;
                            var bold = this.GetBoldedDates().Contains(hit.Date) || this.boldDatesCollection.Exists(d => d.Value.Date == hit.Date.Date);
                            this.OnActiveDateChanged(new ActiveDateChangedEventArgs(hit.Date, bold));
                            this.InvalidateMonth(hit.Date, true);
                            break;
                        }
                    case MonthCalendarHitType.Week:
                        {
                            this.mouseMoveFlags.WeekHeader = true;
                            break;
                        }
                    case MonthCalendarHitType.MonthName:
                        {
                            this.mouseMoveFlags.MonthName = hit.Date;
                            this.mouseMoveFlags.HeaderDate = hit.Date;
                            break;
                        }
                    case MonthCalendarHitType.MonthYear:
                        {
                            this.mouseMoveFlags.Year = hit.Date;
                            this.mouseMoveFlags.HeaderDate = hit.Date;
                            break;
                        }
                    case MonthCalendarHitType.Header:
                        {
                            this.mouseMoveFlags.HeaderDate = hit.Date;
                            break;
                        }
                    case MonthCalendarHitType.Arrow:
                        {
                            bool useRTL = this.UseRTL;
                            if (this.leftArrowRect.Contains(e.Location))
                            {
                                this.mouseMoveFlags.LeftArrow = !useRTL;
                                this.mouseMoveFlags.RightArrow = useRTL;
                                this.mouseMoveFlags.HeaderDate = this.months[0].Date;
                            }
                            else if (this.rightArrowRect.Contains(e.Location))
                            {
                                this.mouseMoveFlags.LeftArrow = useRTL;
                                this.mouseMoveFlags.RightArrow = !useRTL;
                                this.mouseMoveFlags.HeaderDate = this.months[this.calendarDimensions.Width - 1].Date;
                            }
                            break;
                        }
                    case MonthCalendarHitType.Footer:
                        {
                            this.mouseMoveFlags.Footer = true;
                            break;
                        }
                }
                if(this.mouseMoveFlags.LeftArrowChanged)
                {
                    this.Invalidate(this.UseRTL ? this.rightArrowRect : this.leftArrowRect);
                    this.Update();
                }
                if(this.mouseMoveFlags.RightArrowChanged)
                {
                    this.Invalidate(this.UseRTL ? this.leftArrowRect : this.rightArrowRect);
                    this.Update();
                }
                if (this.mouseMoveFlags.HeaderDateChanged)
                    this.Invalidate();
                else if (this.mouseMoveFlags.MonthNameChanged || this.mouseMoveFlags.YearChanged)
                {
                    SelectionRange range1 = new SelectionRange(this.mouseMoveFlags.MonthName, this.mouseMoveFlags.Backup.MonthName);
                    SelectionRange range2 = new SelectionRange(this.mouseMoveFlags.Year, this.mouseMoveFlags.Backup.Year);
                    this.Invalidate(this.months[this.GetIndex(range1.End)].TitleBounds);
                    if (range1.End != range2.End)
                    {
                        this.Invalidate(this.months[this.GetIndex(range2.End)].TitleBounds);
                    }
                }
                if(this.mouseMoveFlags.DayChanged)
                {
                    this.InvalidateMonth(this.mouseMoveFlags.Day, false);
                    this.InvalidateMonth(this.mouseMoveFlags.Backup.Day, false);
                }
                if(this.mouseMoveFlags.FooterChanged)
                {
                    this.Invalidate(this.footerRect);
                }
            }
            if(this.mouseMoveFlags.WeekHeaderChanged)
            {
                this.Cursor = this.mouseMoveFlags.WeekHeader ? Cursors.UpArrow : Cursors.Default;
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if(e.Button == MouseButtons.Left && this.selectionStarted)
            {
                this.SelectionRanges.Add(new SelectionRange(this.SelectionRange.Start, this.SelectionRange.End));
                this.selectionStarted = false;
                this.Refresh();
                if (this.backupRange.Start != this.SelectionRange.Start || this.backupRange.End != this.SelectionRange.End)
                    this.RaiseDateSelected();
                 
            }
            this.currentHitType = MonthCalendarHitType.None;
            this.Capture = false;
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.mouseMoveFlags.LeftArrow = false;
            this.mouseMoveFlags.RightArrow = false;
            this.mouseMoveFlags.MonthName = DateTime.MinValue;
            this.mouseMoveFlags.Year = DateTime.MinValue;
            this.mouseMoveFlags.Footer = false;
            this.mouseMoveFlags.Day = DateTime.MinValue;
            if (!this.showingMenu)
                this.mouseMoveFlags.HeaderDate = DateTime.MinValue;
            this.Invalidate();
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (!this.showingMenu)
                this.Scroll(e.Delta > 0);
        }
        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            if(this.Parent != null&& this.Created)
            {
                this.UpdateMonths();
                this.Invalidate();
            }
        }
        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);
            this.formatProvider.IsRTLLanguage = this.UseRTL;
            this.UpdateMonths();
            this.Invalidate();
        }
        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            this.Refresh();
        }
        protected virtual void OnDateSelected(DateRangeEventArgs e)
        {
            if (this.DateSelected != null)
                this.DateSelected(this, e);
        }
        protected virtual void OnDateClicked(DateEventArgs e)
        {
            if (this.DateClicked != null)
                this.DateClicked(this, e);
        }
        protected virtual void OnDateChanged(DateRangeEventArgs e)
        {
            if (this.DateChanged != null)
                this.DateChanged(this, e);
        }
        protected virtual void OnActiveDateChanged(ActiveDateChangedEventArgs e)
        {
            if (this.ActiveDateChanged != null)
                this.ActiveDateChanged(this, e);
        }

        private static MonthCalendarEraRange[] GetEraRanges(Calendar cal)
        {
            if(cal.Eras.Length == 1)
                return new[] { new MonthCalendarEraRange(cal.Eras[0], cal.MinSupportedDateTime.Date, cal.MaxSupportedDateTime.Date) };
            List<MonthCalendarEraRange> ranges = new List<MonthCalendarEraRange>();
            int currentEra = -1;
            DateTime date = cal.MinSupportedDateTime;
            while(date < cal.MaxSupportedDateTime.Date)
            {
                int era = cal.GetEra(date);
                if (era != currentEra)
                {
                    ranges.Add(new MonthCalendarEraRange(era, date));
                    if (currentEra != -1)
                        ranges[ranges.Count - 2].MaxDate = cal.AddDays(date, -1);
                    currentEra = era;
                }
                date = cal.AddDays(date, 1);
            }
            ranges[ranges.Count - 1].MaxDate = date;
            return ranges.ToArray();
        }
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr wnd, int msg, bool param, int Iparam);

        private MonthCalendarEraRange GetEraRange(int era)
        {
            foreach(MonthCalendarEraRange e in this.eraRanges)
            {
                if (e.Era == era)
                    return e;
            }
            return new MonthCalendarEraRange(this.CultureCalendar.GetEra(DateTime.Today),
                this.CultureCalendar.MinSupportedDateTime.Date, this.CultureCalendar.MaxSupportedDateTime.Date);
        }
        private MonthCalendarEraRange GetEraRange()
        {
            return this.GetEraRange(this.CultureCalendar.GetEra(DateTime.Today));
        }
        private void CalculateSize(bool changeDimension)
        {
            if (this.inUpdate)
                return;
            this.inUpdate = true;
            using (Graphics g = this.CreateGraphics())
            {
                SizeF daySize = g.MeasureString("30", this.Font);
                SizeF weekNumSize = g.MeasureString("59", this.Font);
                MonthCalendarDate date = new MonthCalendarDate(this.CultureCalendar, this.viewStart);
                SizeF monthNameSize = g.MeasureString(this.FormatProvider.GetMonthName(date.Year, date.Month), this.HeaderFont);
                SizeF yearStringSize = g.MeasureString(this.viewStart.ToString("yyyy"), this.headerFont);
                SizeF footerStringSize = g.MeasureString(this.viewStart.ToShortDateString(), this.FooterFont);
                this.headerHeight = Math.Max((int)Math.Max(monthNameSize.Height + 3, yearStringSize.Height) + 1, 15);
                this.dayWidth = Math.Max(12, (int)daySize.Width + 1) + 5;
                this.dayHeight = Math.Max(Math.Max(12, (int)weekNumSize.Height + 1), (int)daySize.Height + 1) + 2;
                this.footerHeight = Math.Max(12, (int)footerStringSize.Height + 1);
                this.weekNumberWidth = this.showWeekHeader ? Math.Max(12, (int)weekNumSize.Width + 1) + 2 : 0;
                this.dayNameHeight = 14;
                foreach(string str in DateMethods.GetDayNames(this.formatProvider, this.useShortestDayNames ? 2 : 1))
                {
                    SizeF dayNameSize = g.MeasureString(str, this.dayHeaderFont);
                    this.dayWidth = Math.Max(this.dayWidth, (int)dayNameSize.Width + 1);
                    this.dayNameHeight = Math.Max(this.dayNameHeight, (int)dayNameSize.Height + 1);
                }
                this.monthWidth = this.weekNumberWidth + (this.dayWidth * 7) + 1;
                this.monthHeight = this.headerHeight + this.dayNameHeight + (this.dayHeight * 6) + 1;
                if(changeDimension)
                {
                    int calWidthDim = Math.Max(1, this.Width / this.monthWidth);
                    int calHeightDim = Math.Max(1, this.Height / this.monthHeight);
                    this.CalendarDimensions = new Size(calWidthDim, calHeightDim);
                }
                this.Height = (this.monthHeight * this.calendarDimensions.Height) + (this.showFooter ? this.footerHeight : 0);
                this.Width = this.monthWidth * this.calendarDimensions.Width;
                this.footerRect = new Rectangle(1, this.Height - this.footerHeight - 1, this.Width - 2, this.footerHeight);
                this.UpdateMonths();
            }
            this.inUpdate = false;
            this.Refresh();
        }
        private bool SetStartDate(DateTime start)
        {
            if (start < DateTime.MinValue.Date || start > DateTime.MaxValue.Date)
                return false;
            DayOfWeek firstDayOfWeek = this.formatProvider.FirstDayOfWeek;
            MonthCalendarDate dt = new MonthCalendarDate(this.CultureCalendar, this.maxDate);
            if (start > this.maxDate)
                start = dt.AddMonth(1 - this.months.Length).FirstOfMonth.Date;
            if (start < this.minDate)
                start = this.minDate;
            dt = new MonthCalendarDate(this.CultureCalendar, start);
            int length = this.months != null ? this.months.Length - 1 : 0;
            while (dt.Date > this.minDate && dt.Day != 1)
                dt = dt.AddDays(-1);
            MonthCalendarDate endDate = dt.AddMonth(length);
            MonthCalendarDate endDateDay = endDate.AddDays(endDate.DaysInMonth - 1 - (endDate.Day - 1));
            if (endDate.Date >= this.maxDate || endDateDay.Date >= this.maxDate)
                dt = new MonthCalendarDate(this.CultureCalendar, this.maxDate).AddMonth(-length).FirstOfMonth;
            this.viewStart = dt.Date;
            while (dt.Date > this.CultureCalendar.MinSupportedDateTime.Date && dt.DayOfWeek != firstDayOfWeek)
                dt = dt.AddDays(-1);
            this.realStart = dt.Date;
            return true;
        }
        private int GetIndex(DateTime monthYear)
        {
            return (from month in this.months where month != null where month.Date == monthYear select month.Index).FirstOrDefault();
        }
        private MonthCalendarMonth GetMonth(DateTime day)
        {
            if (day == DateTime.MinValue)
                return null;
            return this.months.Where(month => month != null).FirstOrDefault(month => month.ContainsDate(day));
        }
        public void UpdateMonths()
        {
            int x = 0, y = 0, index = 0;
            int calWidthDim = this.calendarDimensions.Width;
            int calHeightDim = this.calendarDimensions.Height;
            List<MonthCalendarMonth> monthList = new List<MonthCalendarMonth>();
            MonthCalendarDate dt = new MonthCalendarDate(this.CultureCalendar, this.viewStart);
            if(dt.GetEndDateOfWeek(this.formatProvider).Month != dt.Month)
                dt = dt.GetEndDateOfWeek(this.formatProvider).FirstOfMonth;
            if (this.UseRTL)
            {
                x = this.monthWidth * (calWidthDim - 1);
                for (int i = 0; i < calHeightDim; i++)
                {
                    for (int j = calWidthDim; j >= 0; j--)
                    {
                        if (dt.Date >= this.MaxDate)
                            break;
                        monthList.Add(new MonthCalendarMonth(this, dt.Date) { Location = new Point(x, y), Index = index++ });
                        x -= this.monthWidth;
                        dt = dt.AddMonth(1);
                    }
                    x = this.monthWidth * (calWidthDim - 1);
                    y += this.monthHeight;
                }
            }
            else
            {
                for(int i = 0; i < calHeightDim; i++)
                {
                    for(int j = 0; j < calWidthDim; j++)
                    {
                        if (dt.Date >= this.maxDate)
                            break;
                        monthList.Add(new MonthCalendarMonth(this, dt.Date) { Location = new Point(x, y), Index = index++ });
                        x += this.monthWidth;
                        dt = dt.AddMonth(1);
                    }
                    x = 0;
                    y += this.monthHeight;
                }
            }
            this.lastVisibleDate = monthList[monthList.Count - 1].LastVisibleDate;
            this.months = monthList.ToArray();
        }
        private void UpdateMonthMenu(int year)
        {
            int i = 1;
            int monthsInYear = this.CultureCalendar.GetMonthsInYear(year);
            foreach(ToolStripMenuItem item in this.monthMenu.Items)
            {
                if(i <= monthsInYear)
                {
                    item.Tag = i;
                    item.Text = this.formatProvider.GetMonthName(year, i++);
                    item.Visible = true;
                }
                else
                {
                    item.Tag = null;
                    item.Text = string.Empty;
                    item.Visible = false;
                }
            }
        }
        private void UpdateYearMenu(int year)
        {
            year -= 4;
            int maxYear = this.CultureCalendar.GetYear(this.maxDate);
            int minYear = this.CultureCalendar.GetYear(this.minDate);
            if (year + 8 > maxYear)
                year = maxYear - 8;
            else if (year < minYear)
                year = minYear;
            year = Math.Max(1, year);
            foreach (ToolStripMenuItem item in this.yearMenu.Items)
            {
                item.Text = DateMethods.GetNumberString(year, this.UseNativeDigits ? this.Culture.NumberFormat.NativeDigits : null, false);
                item.Tag = year;
                item.Font = new Font("Tahoma", 8.25F, year == this.CultureCalendar.GetYear(DateTime.Today) ? FontStyle.Bold : FontStyle.Regular);
                item.ForeColor = year == this.CultureCalendar.GetYear(DateTime.Today) ? Color.FromArgb(251, 200, 79) : Color.Black;
                year++;
            }
        }
        private void MonthClick(object sender, EventArgs e)
        {
            MonthCalendarDate currentMonthYear = new MonthCalendarDate(this.CultureCalendar, (DateTime)this.monthMenu.Tag);
            int monthClicked = (int)((ToolStripMenuItem)sender).Tag;
            if(currentMonthYear.Month != monthClicked)
            {
                MonthCalendarDate dt = new MonthCalendarDate(this.CultureCalendar, new DateTime(currentMonthYear.Year, monthClicked, 1, this.CultureCalendar));
                DateTime newStartDate = dt.AddMonth(-this.GetIndex(currentMonthYear.Date)).Date;
                if(this.SetStartDate(newStartDate))
                {
                    this.UpdateMonths();
                    this.RaiseDateChanged();
                    this.Focus();
                    this.Refresh();
                }
            }
        }
        private void YearClick(object sender, EventArgs e)
        {
            DateTime currentMonthYear = (DateTime)this.yearMenu.Tag;
            int yearClicked = (int)((ToolStripMenuItem)sender).Tag;
            MonthCalendarDate dt = new MonthCalendarDate(this.CultureCalendar, currentMonthYear);
            if(dt.Year != yearClicked)
            {
                MonthCalendarDate newStartDate = new MonthCalendarDate(this.cultureCalendar, new DateTime(yearClicked, dt.Month, 1, this.CultureCalendar)).AddMonth(-this.GetIndex(currentMonthYear));
                if(this.SetStartDate(newStartDate.Date))
                {
                    this.UpdateMonths();
                    this.RaiseDateChanged();
                    this.Focus();
                    this.Refresh();
                }
            }
        }
        private void MonthMenuClosed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            this.monthSelected = DateTime.MinValue;
            this.showingMenu = false;
            this.Invalidate(this.months[this.GetIndex((DateTime)this.monthMenu.Tag)].TitleBounds);
        }
        private void YearMenuClosed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            this.yearSelected = DateTime.MinValue;
            this.showingMenu = false;
            this.Invalidate(this.months[this.GetIndex((DateTime)this.yearMenu.Tag)].TitleBounds);
        }
        private void RaiseDateSelected()
        {
            SelectionRange range = this.SelectionRange;
            DateTime selStart = range.Start;
            DateTime selEnd = range.End;
            if (selStart == DateTime.MinValue)
                selStart = selEnd;
            this.OnDateSelected(new DateRangeEventArgs(selStart, selEnd));
        }
        private void RaiseDateChanged()
        {
            this.OnDateChanged(new DateRangeEventArgs(this.realStart, this.lastVisibleDate));
        }
        private void RaiseSelectExtendEnd()
        {
            var handler = this.SelectionExtendEnd;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
        private void RaiseInternalDateSelected()
        {
            if (this.InternalDateSelected != null)
                this.InternalDateSelected(this, new DateEventArgs(this.selectionStart));
        }
        private void Scroll(bool up)
        {
            if(this.SetStartDate(this.GetNewScrollDate(up)))
            {
                this.UpdateMonths();
                this.RaiseDateChanged();
                this.Refresh();
            }
        }
        private DateTime GetNewScrollDate(bool up)
        {
            if ((this.lastVisibleDate == this.maxDate && !up) || (this.months[0].FirstVisibleDate == this.minDate && up))
                return ViewStart;
            MonthCalendarDate dt = new MonthCalendarDate(this.CultureCalendar, this.viewStart);
            int monthsToAdd = (this.scrollChange == 0 ? Math.Max((this.calendarDimensions.Width * this.calendarDimensions.Height) - 1, 1) : this.scrollChange) * (up ? -1 : 1);
            int length = this.months == null ? Math.Max(1, this.calendarDimensions.Width * this.calendarDimensions.Height) : this.months.Length;
            MonthCalendarDate newStartMonthDate = dt.AddMonth(monthsToAdd);
            MonthCalendarDate lastMonthDate = newStartMonthDate.AddMonth(length - 1);
            MonthCalendarDate lastMonthEndDate = lastMonthDate.AddDays(lastMonthDate.DaysInMonth - 1 - lastMonthDate.Day);
            if(newStartMonthDate.Date < this.minDate)
                newStartMonthDate = new MonthCalendarDate(this.CultureCalendar, this.minDate);
            else if(lastMonthEndDate.Date >= this.maxDate || lastMonthDate.Date >= this.maxDate)
            {
                MonthCalendarDate maxdt = new MonthCalendarDate(this.CultureCalendar, this.maxDate).FirstOfMonth;
                newStartMonthDate = maxdt.AddMonth(1 - length);
            }
            return newStartMonthDate.Date;
        }
        private MonthCalendarHeaderState GetMonthHeaderState(DateTime monthDate)
        {
            MonthCalendarHeaderState state = MonthCalendarHeaderState.Default;
            if (this.monthSelected == monthDate)
                state = MonthCalendarHeaderState.MonthNameSelected;
            else if (this.yearSelected == monthDate)
                state = MonthCalendarHeaderState.YearSelected;
            else if (this.mouseMoveFlags.MonthName == monthDate)
                state = MonthCalendarHeaderState.MonthNameActive;
            else if (this.mouseMoveFlags.Year == monthDate)
                state = MonthCalendarHeaderState.YearActive;
            else if (this.mouseMoveFlags.HeaderDate == monthDate)
                state = MonthCalendarHeaderState.Active;
            return state;
        }
        private void InvalidateMonth(DateTime date, bool refreshInvalid)
        {
            if(date == DateTime.MinValue)
            {
                if (refreshInvalid)
                    this.Refresh();
                return;
            }
            MonthCalendarMonth month = this.GetMonth(date);
            if (month != null)
            {
                this.Invalidate(month.MonthBounds);
                this.Update();
            }
            else if (date > this.lastVisibleDate)
            {
                this.Invalidate(this.months[this.months.Length - 1].Bounds);
                this.Update();
            }
            else if (refreshInvalid)
                this.Refresh();
        }
        private DateTime GetSelectionDate(DateTime baseDate, DateTime newSelectionDate)
        {
            if (this.maxSelectionCount == 0 || baseDate == DateTime.MinValue)
                return newSelectionDate;
            if(baseDate >= this.CultureCalendar.MinSupportedDateTime && newSelectionDate >= this.CultureCalendar.MinSupportedDateTime && 
                baseDate <= this.CultureCalendar.MaxSupportedDateTime && newSelectionDate <= this.CultureCalendar.MaxSupportedDateTime)
            {
                int days = (baseDate - newSelectionDate).Days;
                if(Math.Abs(days) >= this.maxSelectionCount)
                {
                    newSelectionDate = new MonthCalendarDate(this.CultureCalendar, baseDate).AddDays(days < 0 ? this.maxSelectionCount - 1 : 1 - this.maxSelectionCount).Date;
                }
                return newSelectionDate;
            }
            return DateTime.MinValue;
        }
        private DateTime GetMinDate(DateTime date)
        {
            DateTime dt = new DateTime(1900, 1, 1);
            DateTime minEra = this.GetEraRange().MinDate;
            if (this.CultureCalendar.GetType() == typeof(JapaneseLunisolarCalendar))
                minEra = new DateTime(1989, 4, 1);
            DateTime mindate = minEra < dt ? dt : minEra;
            return date < mindate ? mindate : date;
        }
        private DateTime GetMaxDate(DateTime date)
        {
            DateTime dt = new DateTime(9998, 12, 31);
            DateTime maxEra = this.GetEraRange().MaxDate;
            DateTime maxdate = maxEra > dt ? dt : maxEra;
            return date > maxdate ? maxdate : date;
        }
        private void ReAssignSelectionMode()
        {
            this.SelectionRange = null;
            MonthCalendarSelectionMode selMode = this.daySelectionMode;
            this.daySelectionMode = MonthCalendarSelectionMode.Manual;
            this.SelectionMode = selMode;
        }
        private void SetSelectionRange(MonthCalendarSelectionMode selMode)
        {
            switch (selMode)
            {
                case MonthCalendarSelectionMode.Day:
                    {
                        this.selectionEnd = this.selectionStart;
                        break;
                    }
                case MonthCalendarSelectionMode.WorkWeek:
                case MonthCalendarSelectionMode.FullWeek:
                    {
                        MonthCalendarDate dt = new MonthCalendarDate(this.CultureCalendar, this.selectionStart).GetFirstDayInWeek(this.formatProvider);
                        this.selectionStart = dt.Date;
                        this.selectionEnd = dt.AddDays(6).Date;
                        break;
                    }
                case MonthCalendarSelectionMode.Month:
                    {
                        MonthCalendarDate dt = new MonthCalendarDate(this.CultureCalendar, this.selectionStart).FirstOfMonth;
                        this.selectionStart = dt.Date;
                        this.selectionEnd = dt.AddMonth(1).AddDays(-1).Date;
                        break;
                    }
            }
        }
        internal bool ShouldSerializeCulture()
        {
            return this.culture.LCID != CultureInfo.CurrentUICulture.LCID;
        }
        internal void ResetCulture()
        {
            this.minDate = CultureInfo.CurrentUICulture.DateTimeFormat.Calendar.MinSupportedDateTime;
            if (this.minDate < new DateTime(1900, 1, 1))
                this.minDate = new DateTime(1900, 1, 1);
            this.maxDate = CultureInfo.CurrentUICulture.DateTimeFormat.Calendar.MaxSupportedDateTime.Date;
            if (this.maxDate > new DateTime(9998, 12, 31))
                this.maxDate = new DateTime(9998, 12, 31);
            this.Culture = CultureInfo.CurrentUICulture;
        }
        internal bool ShouldSerializeCultureCalendar()
        {
            if(this.CultureCalendar.GetType() == this.culture.Calendar.GetType() && this.CultureCalendar.GetType() == typeof(GregorianCalendar))
            {
                GregorianCalendar g1 = (GregorianCalendar)this.CultureCalendar;
                GregorianCalendar g2 = (GregorianCalendar)this.culture.Calendar;
                return this.CultureCalendar != this.culture.Calendar && g1.CalendarType != g2.CalendarType;
            }
            return this.CultureCalendar != this.culture.Calendar && this.CultureCalendar.GetType() != this.culture.Calendar.GetType();
        }
        internal void ResetCultureCalendar()
        {
            this.CultureCalendar = this.culture.Calendar;
        }
        internal bool ShoulSerializeMindate()
        {
            return this.minDate != this.GetMinDate(this.CultureCalendar.MinSupportedDateTime.Date);
        }
        internal void ResetMinDate()
        {
            this.minDate = this.GetMinDate(this.CultureCalendar.MinSupportedDateTime.Date);
        }
        internal bool ShouldSerializeMaxDate()
        {
            return this.MaxDate != this.GetMaxDate(this.CultureCalendar.MaxSupportedDateTime.Date);
        }
        internal void ResetMaxDate()
        {
            this.maxDate = this.GetMaxDate(this.CultureCalendar.MaxSupportedDateTime.Date);
        }
        internal bool ShouldSerializeColorTable()
        {
            MonthCalendarColorTable table = new MonthCalendarColorTable();
            MonthCalendarColorTable currentTable = this.ColorTable;
            return table.BackgroundGradientBegin != currentTable.BackgroundGradientBegin
            || table.BackgroundGradientEnd != currentTable.BackgroundGradientEnd
            || table.BackgroundGradientMode != currentTable.BackgroundGradientMode
            || table.Border != currentTable.Border
            || table.DayActiveGradientBegin != currentTable.DayActiveGradientBegin
            || table.DayActiveGradientEnd != currentTable.DayActiveGradientEnd
            || table.DayActiveGradientMode != currentTable.DayActiveGradientMode
            || table.DayActiveText != currentTable.DayActiveText
            || table.DayActiveTodayCircleBorder != currentTable.DayActiveTodayCircleBorder
            || table.DayHeaderGradientBegin != currentTable.DayHeaderGradientBegin
            || table.DayHeaderGradientEnd != currentTable.DayHeaderGradientEnd
            || table.DayHeaderGradientMode != currentTable.DayHeaderGradientMode
            || table.DayHeaderText != currentTable.DayHeaderText
            || table.DaySelectedGradientBegin != currentTable.DaySelectedGradientBegin
            || table.DaySelectedGradientEnd != currentTable.DaySelectedGradientEnd
            || table.DaySelectedGradientMode != currentTable.DaySelectedGradientMode
            || table.DaySelectedText != currentTable.DaySelectedText
            || table.DaySelectedTodayCircleBorder != currentTable.DaySelectedTodayCircleBorder
            || table.DayText != currentTable.DayText
            || table.DayTextBold != currentTable.DayTextBold
            || table.DayTodayCircleBorder != currentTable.DayTodayCircleBorder
            || table.DayTrailingText != currentTable.DayTrailingText
            || table.FooterActiveGradientBegin != currentTable.FooterActiveGradientBegin
            || table.FooterActiveGradientEnd != currentTable.FooterActiveGradientEnd
            || table.FooterActiveGradientMode != currentTable.FooterActiveGradientMode
            || table.FooterActiveText != currentTable.FooterActiveText
            || table.FooterGradientBegin != currentTable.FooterGradientBegin
            || table.FooterGradientEnd != currentTable.FooterGradientEnd
            || table.FooterGradientMode != currentTable.FooterGradientMode
            || table.FooterText != currentTable.FooterText
            || table.FooterTodayCircleBorder != currentTable.FooterTodayCircleBorder
            || table.HeaderActiveArrow != currentTable.HeaderActiveArrow
            || table.HeaderActiveGradientBegin != currentTable.HeaderActiveGradientBegin
            || table.HeaderActiveGradientEnd != currentTable.HeaderActiveGradientEnd
            || table.HeaderActiveGradientMode != currentTable.HeaderActiveGradientMode
            || table.HeaderActiveText != currentTable.HeaderActiveText
            || table.HeaderArrow != currentTable.HeaderArrow
            || table.HeaderGradientBegin != currentTable.HeaderGradientBegin
            || table.HeaderGradientEnd != currentTable.HeaderGradientEnd
            || table.HeaderGradientMode != currentTable.HeaderGradientMode
            || table.HeaderSelectedText != currentTable.HeaderSelectedText
            || table.HeaderText != currentTable.HeaderText
            || table.MonthBodyGradientBegin != currentTable.MonthBodyGradientBegin
            || table.MonthBodyGradientEnd != currentTable.MonthBodyGradientEnd
            || table.MonthBodyGradientMode != currentTable.MonthBodyGradientMode
            || table.MonthSeparator != currentTable.MonthSeparator
            || table.WeekHeaderGradientBegin != currentTable.WeekHeaderGradientBegin
            || table.WeekHeaderGradientEnd != currentTable.WeekHeaderGradientEnd
            || table.WeekHeaderGradientMode != currentTable.WeekHeaderGradientMode
            || table.WeekHeaderText != currentTable.WeekHeaderText;
        }
        internal void ResetColorTable()
        {
            this.ColorTable = new MonthCalendarColorTable();
            this.Invalidate();
        }
        private bool ShouldSerializeViewStart()
        {
            return this.viewStart != new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        }
        private void ResetViewStart()
        {
            this.viewStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        }
        private bool ShouldSerializeHeaderFont()
        {
            return this.headerFont.Name != "Segoe UI" || !this.headerFont.Size.Equals(9f) || this.headerFont.Style != FontStyle.Regular;
        }
        private void ResetHeaderFont()
        {
            this.HeaderFont = new Font("Segoe UI", 9f, FontStyle.Regular);
        }
        private bool ShouldSerializeFooterFont()
        {
            return this.footerFont.Name != "Arial" || !this.footerFont.Size.Equals(9f) || this.footerFont.Style != FontStyle.Bold;
        }
        private void ResetFooterFont()
        {
            this.footerFont = new Font("Arial", 9f, FontStyle.Bold);
        }
        private bool ShouldSerializeDayHeaderFont()
        {
            return this.dayHeaderFont.Name != "Segoe UI" || !this.dayHeaderFont.Size.Equals(8f) || this.dayHeaderFont.Style != FontStyle.Regular;
        }
        private void ResetDayHeaderFont()
        {
            this.dayHeaderFont = new Font("Segoe UI", 8f, FontStyle.Regular);
        }
        private bool ShouldSerializeFont()
        {
            return this.Font.Name != "Microsoft Sans Serif" || !this.Font.Size.Equals(8.25f) || this.Font.Style != FontStyle.Regular;
        }
        private new void ResetFont()
        {
            this.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular);
        }
        private bool ShouldSerializeBoldedDates()
        {
            return this.boldedDates.Count != 0;
        }
        private void ResetBoldedDates()
        {
            this.boldedDates.Clear();
            this.Refresh();
        }
        private bool ShouldSerializeSelectionRange()
        {
            return this.selectionStart != DateTime.Today || this.selectionStart != this.selectionEnd;
        }
        private void ResetSelectionRange()
        {
            this.selectionEnd = DateTime.Today;
            this.selectionStart = DateTime.Today;
            this.Refresh();
        }
        private void InitializeComponent()
        {
            this.components = new Container();
            this.monthMenu = new ContextMenuStrip(this.components);
            this.tsmiJan = new ToolStripMenuItem();
            this.tsmiFeb = new ToolStripMenuItem();
            this.tsmiMar = new ToolStripMenuItem();
            this.tsmiApr = new ToolStripMenuItem();
            this.tsmiMay = new ToolStripMenuItem();
            this.tsmiJun = new ToolStripMenuItem();
            this.tsmiJul = new ToolStripMenuItem();
            this.tsmiAug = new ToolStripMenuItem();
            this.tsmiSep = new ToolStripMenuItem();
            this.tsmiOct = new ToolStripMenuItem();
            this.tsmiNov = new ToolStripMenuItem();
            this.tsmiDez = new ToolStripMenuItem();
            this.tsmiA1 = new ToolStripMenuItem();
            this.tsmiA2 = new ToolStripMenuItem();
            this.yearMenu = new ContextMenuStrip();
            this.tsmiYear1 = new ToolStripMenuItem();
            this.tsmiYear2 = new ToolStripMenuItem();
            this.tsmiYear3 = new ToolStripMenuItem();
            this.tsmiYear4 = new ToolStripMenuItem();
            this.tsmiYear5 = new ToolStripMenuItem();
            this.tsmiYear6 = new ToolStripMenuItem();
            this.tsmiYear7 = new ToolStripMenuItem();
            this.tsmiYear8 = new ToolStripMenuItem();
            this.tsmiYear9 = new ToolStripMenuItem();
            this.monthMenu.SuspendLayout();
            this.yearMenu.SuspendLayout();
            this.SuspendLayout();

            this.monthMenu.Items.AddRange(new ToolStripItem[]
            {
                this.tsmiJan,
                this.tsmiFeb,
                this.tsmiMar,
                this.tsmiApr,
                this.tsmiMay,
                this.tsmiJul,
                this.tsmiJun,
                this.tsmiAug,
                this.tsmiSep,
                this.tsmiOct,
                this.tsmiNov,
                this.tsmiDez,
                this.tsmiA1,
                this.tsmiA2
            });
            this.monthMenu.Name = "monthMenu";
            this.monthMenu.ShowImageMargin = false;
            this.monthMenu.Size = new Size(54, 312);
            this.monthMenu.Closed += MonthMenuClosed;

            this.tsmiJan.Size = new Size(78, 22);
            this.tsmiJan.Tag = 1;
            this.tsmiJan.Click += MonthClick;

            this.tsmiFeb.Size = new Size(78, 22);
            this.tsmiFeb.Tag = 2;
            this.tsmiFeb.Click += MonthClick;

            this.tsmiMar.Size = new Size(78, 22);
            this.tsmiMar.Tag = 3;
            this.tsmiMar.Click += MonthClick;

            this.tsmiApr.Size = new Size(78, 22);
            this.tsmiApr.Tag = 4;
            this.tsmiApr.Click += MonthClick;

            this.tsmiMay.Size = new Size(78, 22);
            this.tsmiMay.Tag = 5;
            this.tsmiMay.Click += MonthClick;

            this.tsmiJun.Size = new Size(78, 22);
            this.tsmiJun.Tag = 6;
            this.tsmiJun.Click += MonthClick;

            this.tsmiJul.Size = new Size(78, 22);
            this.tsmiJul.Tag = 7;
            this.tsmiJul.Click += MonthClick;

            this.tsmiAug.Size = new Size(78, 22);
            this.tsmiAug.Tag = 8;
            this.tsmiAug.Click += MonthClick;

            this.tsmiSep.Size = new Size(78, 22);
            this.tsmiSep.Tag = 9;
            this.tsmiSep.Click += MonthClick;

            this.tsmiOct.Size = new Size(78, 22);
            this.tsmiOct.Tag = 10;
            this.tsmiOct.Click += MonthClick;

            this.tsmiNov.Size = new Size(78, 22);
            this.tsmiNov.Tag = 11;
            this.tsmiNov.Click += MonthClick;

            this.tsmiDez.Size = new Size(78, 22);
            this.tsmiDez.Tag = 12;
            this.tsmiDez.Click += MonthClick;

            this.tsmiA1.Size = new Size(78, 22);
            this.tsmiA1.Click += this.MonthClick;

            this.tsmiA2.Size = new Size(78, 22);
            this.tsmiA2.Click += this.MonthClick;

            this.yearMenu.Items.AddRange(new ToolStripItem[]
            {
                this.tsmiYear1,
                this.tsmiYear2,
                this.tsmiYear3,
                this.tsmiYear4,
                this.tsmiYear5,
                this.tsmiYear6,
                this.tsmiYear7,
                this.tsmiYear8,
                this.tsmiYear9,
            });
            this.yearMenu.Name = "yearMenu";
            this.yearMenu.ShowImageMargin = false;
            this.yearMenu.ShowItemToolTips = false;
            this.yearMenu.Size = new Size(54, 202);
            this.yearMenu.Closed += this.YearMenuClosed;

            this.tsmiYear1.Size = new Size(53, 22);
            this.tsmiYear1.Click += this.YearClick;

            this.tsmiYear2.Size = new Size(53, 22);
            this.tsmiYear2.Click += this.YearClick;

            this.tsmiYear3.Size = new Size(53, 22);
            this.tsmiYear3.Click += this.YearClick;

            this.tsmiYear4.Size = new Size(53, 22);
            this.tsmiYear4.Click += this.YearClick;

            this.tsmiYear5.Size = new Size(53, 22);
            this.tsmiYear5.Click += this.YearClick;

            this.tsmiYear6.Size = new Size(53, 22);
            this.tsmiYear6.Click += this.YearClick;

            this.tsmiYear7.Size = new Size(53, 22);
            this.tsmiYear7.Click += this.YearClick;

            this.tsmiYear8.Size = new Size(53, 22);
            this.tsmiYear8.Click += this.YearClick;

            this.tsmiYear9.Size = new Size(53, 22);
            this.tsmiYear9.Click += this.YearClick;

            this.monthMenu.ResumeLayout(false);
            this.yearMenu.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
