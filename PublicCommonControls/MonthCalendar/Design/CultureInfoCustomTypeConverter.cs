using System;
using System.Collections.Generic;
using System.Globalization;
using System.ComponentModel;


namespace PublicCommonControls.WCalendar.Design
{
    internal class CultureInfoCustomTypeConverter : CultureInfoConverter
    {
        private StandardValuesCollection values;
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            base.GetStandardValues(context);
            if(this.values == null)
            {
                List<CultureInfo> list = new List<CultureInfo>(CultureInfo.GetCultures(CultureTypes.AllCultures));
                list.RemoveAll(c => c.IsNeutralCulture);
                list.Sort((c1, c2) =>
                {
                    if (c1 == null)
                        return c2 == null ? 0 : -1;
                    if (c2 == null)
                        return 1;
                    return CultureInfo.CurrentCulture.CompareInfo.Compare(c1.DisplayName, c2.DisplayName, CompareOptions.StringSort);
                });
                this.values = new StandardValuesCollection(list);
            }
            return this.values;
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var retValue = base.ConvertTo(context, culture, value, destinationType);
            //if (destinationType == typeof(string) && value is CultureInfo)
            //{
            //   var ci = (CultureInfo)value;

            //   var name = ci.DisplayName;

            //   if (string.IsNullOrEmpty(name))
            //   {
            //      name = ci.Name;
            //   }

            //   return name;
            //}

            return retValue;
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return base.ConvertFrom(context, culture, value);
        }
    }
}
