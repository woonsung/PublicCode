using System;
using System.ComponentModel;
using System.Globalization;

namespace PublicCommonControls.WCalendar.Design
{
    internal class MonthCalendarColorTableTypeConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            return base.CanConvertTo(context, destinationType);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
                return "(color table)";

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
