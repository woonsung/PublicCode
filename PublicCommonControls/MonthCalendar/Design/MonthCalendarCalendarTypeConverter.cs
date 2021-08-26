using System;
using System.ComponentModel;
using System.Globalization;

namespace PublicCommonControls.WCalendar.Design
{
    internal class MonthCalendarCalendarTypeConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            return base.CanConvertTo(context, destinationType);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if(destinationType == typeof(string) && value != null && value is Calendar)
            {
                string addString = string.Empty;
                Calendar cal = (Calendar)value;
                if(cal.GetType() == typeof(GregorianCalendar))
                {
                    addString = " " + ((GregorianCalendar)cal).CalendarType;
                }
                return cal.ToString().Replace("System.Globalization.", "").Replace("Calendar", "") + addString;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
