using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Xml;
using System.Xml.Serialization;

namespace PublicCommonControls.WCalendar.Design
{
    internal class MonthCalendarControlDesigner : ControlDesigner
    {
        public override SelectionRules SelectionRules
        {
            get
            {
                return SelectionRules.BottomSizeable | SelectionRules.RightSizeable | SelectionRules.Moveable | SelectionRules.Visible;
            }
        }
        public override DesignerActionListCollection ActionLists
        {
            get
            {
                return new DesignerActionListCollection { new MonthCalendarControlDesignerActionList(this.Component) };

            }
        }
        protected override void PreFilterProperties(IDictionary properties)
        {
            base.PreFilterProperties(properties);
            properties.Remove("BackgroundImage");
            properties.Remove("ForeColor");
            properties.Remove("Text");
            properties.Remove("ImeMode");
            properties.Remove("Padding");
            properties.Remove("BackgroundImageLayout");
            properties.Remove("BackColor");
        }
        internal class MonthCalendarControlDesignerActionList : DesignerActionList
        {
            private const string FileFilter = "XML File (*.xml)|*.xml";
            private readonly MonthCalendar cal;
            private readonly IComponentChangeService iccs;
            private readonly DesignerActionUIService designerUISvc;
            public MonthCalendarControlDesignerActionList(IComponent component)
                :base(component)
            {
                Type compType = component.GetType();
                if (component == null || (compType != typeof(MonthCalendar) && compType != typeof(DatePicker)))
                    throw new InvalidOperationException("MonthCalendarDesigner : component is null or not of the correct one");
                if (compType == typeof(DatePicker))
                    this.cal = ((DatePicker)component).Picker;
                else
                    this.cal = (MonthCalendar)component;
                this.iccs = (IComponentChangeService)GetService(typeof(IComponentChangeService));
                this.designerUISvc = (DesignerActionUIService)GetService(typeof(DesignerActionUIService));
            }
            public override bool AutoShow
            {
                get => true;
                set => base.AutoShow = value;
            }
            public override DesignerActionItemCollection GetSortedActionItems()
            {
                DesignerActionItemCollection actionItems = new DesignerActionItemCollection
                {
                    new DesignerActionMethodItem(this, "LoadColorTable", "Load the color table", "",
                                                "Loads the color table from an XML-file", true),
                    new DesignerActionMethodItem(this, "SaveColorTable", "Save the color table", "",
                                                "Saves the color table to an XML-file" , true)
                };
                return actionItems;
            }
            internal void SaveColorTable()
            {
                this.designerUISvc.HideUI(this.Component);
                using (SaveFileDialog dlg = new SaveFileDialog { Filter = FileFilter })
                {
                    if(dlg.ShowDialog() == DialogResult.OK)
                    {
                        using (XmlWriter writer = new XmlTextWriter(dlg.FileName, Encoding.UTF8))
                        {
                            new XmlSerializer(typeof(MonthCalendarColorTable)).Serialize(writer, this.cal.ColorTable);
                            writer.Flush();
                            writer.Close();
                        }
                    }
                }

            }
            internal void LoadColorTable()
            {
                this.designerUISvc.HideUI(this.Component);
                using (OpenFileDialog dlg = new OpenFileDialog { Filter = FileFilter })
                {
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        using (var fs = new FileStream(dlg.FileName, FileMode.Open))
                        {
                            MonthCalendarColorTable colorTable = (MonthCalendarColorTable)new XmlSerializer(typeof(MonthCalendarColorTable)).Deserialize(fs);
                            MonthCalendarColorTable oldTable = this.cal.ColorTable;
                            this.cal.ColorTable = colorTable;
                            string propName = this.Component.GetType() == typeof(DatePicker) ? "PickerColorTable" : "ColorTable";
                            this.iccs.OnComponentChanged(this.Component, TypeDescriptor.GetProperties(this.Component)[propName], oldTable, colorTable);
                            this.cal.Invalidate();
                        }
                    }

                }
            }
        }
    }
}
