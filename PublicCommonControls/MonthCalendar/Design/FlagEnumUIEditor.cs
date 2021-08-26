using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace PublicCommonControls.WCalendar.Design
{
    internal class FlagEnumUIEditor : UITypeEditor
    {
        private readonly FlagCheckedListBox flagEnumCheckBox;
        public FlagEnumUIEditor()
        {
            this.flagEnumCheckBox = new FlagCheckedListBox { BorderStyle = BorderStyle.None };
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if(context != null && context.Instance != null && provider != null)
            {
                IWindowsFormsEditorService editorSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if(editorSvc != null)
                {
                    Enum e = (Enum)Convert.ChangeType(value, context.PropertyDescriptor.PropertyType);
                    this.flagEnumCheckBox.EnumValue = e;
                    editorSvc.DropDownControl(this.flagEnumCheckBox);
                    return this.flagEnumCheckBox.EnumValue;
                }
            }
            return null;
        }
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
    }
}
