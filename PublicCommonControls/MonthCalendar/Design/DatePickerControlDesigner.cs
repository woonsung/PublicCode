using System.Collections;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;


namespace PublicCommonControls.WCalendar.Design
{
    internal class DatePickerControlDesigner : ControlDesigner
    {
        public override SelectionRules SelectionRules
        {
            get { return SelectionRules.RightSizeable | SelectionRules.LeftSizeable | SelectionRules.Moveable | SelectionRules.Visible; }
        }
        public override DesignerActionListCollection ActionLists
        {
            get { return new DesignerActionListCollection { new MonthCalendarControlDesigner.MonthCalendarControlDesignerActionList(this.Component) }; }
        }
        protected override void PreFilterProperties(IDictionary properties)
        {
            base.PreFilterProperties(properties);
            properties.Remove("BackgroundImage");
            properties.Remove("Text");
            properties.Remove("ImeMode");
            properties.Remove("Padding");
            properties.Remove("BackgroundImageLayout");
        }

    }
}
