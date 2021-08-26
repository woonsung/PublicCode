using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.Design;


namespace PublicCommonControls.WCalendar.Design
{
    internal class MonthCalendarCalendarUIEditor : UITypeEditor
    {
        private readonly ListBox calendarListBox;
        private IWindowsFormsEditorService editorSvc;
        public MonthCalendarCalendarUIEditor()
        {
            this.calendarListBox = new ListBox
            {
                BorderStyle = BorderStyle.None,
                SelectionMode = SelectionMode.One
            };
            this.calendarListBox.Click += CalendarListBox_Click;
        }
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            this.editorSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if(this.editorSvc != null)
            {
                MonthCalendar cal = context.Instance.GetType() == typeof(MonthCalendar)
                    ? (MonthCalendar)context.Instance : ((DatePicker)context.Instance).Picker;
                if(cal != null)
                {
                    CalendarItem currentitem = null;
                    this.calendarListBox.Items.Clear();
                    bool addPersian = true, addHebrew = true;
                    foreach (Calendar c in cal.Culture.OptionalCalendars)
                    {
                        CalendarItem it = new CalendarItem(c);
                        if (c.GetType() == typeof(PersianCalendar))
                            addPersian = false;
                        if (c.GetType() == typeof(HebrewCalendar))
                            addHebrew = false;
                        this.calendarListBox.Items.Add(it);
                        if (c == cal.CultureCalendar)
                            currentitem = it;
                    }
                    if (currentitem != null)
                        this.calendarListBox.SelectedItem = currentitem;
                    List<CalendarItem> items = new List<CalendarItem>();
                    if (addPersian)
                        items.Add(new CalendarItem(new PersianCalendar(), false));
                    if (addHebrew)
                        items.Add(new CalendarItem(new HebrewCalendar(), false));
                    items.Add(new CalendarItem(new JulianCalendar(), false));
                    items.Add(new CalendarItem(new ChineseLunisolarCalendar(), false));
                    items.Add(new CalendarItem(new JapaneseLunisolarCalendar(), false));
                    items.Add(new CalendarItem(new KoreanLunisolarCalendar(), false));
                    items.Add(new CalendarItem(new TaiwanLunisolarCalendar(), false));
                    foreach(var item in items)
                    {
                        this.calendarListBox.Items.Add(item);
                        if (item.Item.GetType() == cal.CultureCalendar.GetType())
                            this.calendarListBox.SelectedItem = item;
                    }
                    this.editorSvc.DropDownControl(this.calendarListBox);
                    if (this.calendarListBox.SelectedItem != null)
                        return ((CalendarItem)this.calendarListBox.SelectedItem).Item;
                    return cal.Culture.DateTimeFormat.Calendar;
                }
            }

            return null;
        }

        private void CalendarListBox_Click(object sender, EventArgs e)
        {
            if (this.editorSvc != null)
                this.editorSvc.CloseDropDown();
        }

        private class CalendarItem
        {
            public CalendarItem(Calendar cal, bool isCultureCalendar = true)
            {
                this.IsCultureCalendar = isCultureCalendar;
                this.Item = cal;
            }
            public Calendar Item { get; private set; }
            public bool IsCultureCalendar { get; set; }
            public override string ToString()
            {
                if (this.Item != null)
                {
                    string addString = string.Empty;
                    if (this.Item.GetType() == typeof(GregorianCalendar))
                        addString = " " + ((GregorianCalendar)this.Item).CalendarType;
                    if (!this.IsCultureCalendar)
                        addString += " not optional";
                    return this.Item.ToString().Replace("System.Globalization.", string.Empty).Replace("Calendar", string.Empty) + addString;
                }
                return string.Empty;
            }
        }
    }
}
