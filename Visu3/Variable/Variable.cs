using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.BACnet;
namespace Visu3
{
    public class Variable
    {
        public VariableGeneralParameter GeneralParameter { get; set; }
        
        public VariableParameter SpecificParameter { get; set; }

        public string Type { get; set; }

        public delegate void ValueChangedEventHandler(object source, BacnetObjectValueChangedEventArgs e);

        public event ValueChangedEventHandler ValueChanged;

        public Variable(VariableGeneralParameter generalParameter, VariableBacnetIPParameter bacnetIpSpecificParameter, string type)
        {
            this.GeneralParameter = generalParameter;
            this.SpecificParameter = bacnetIpSpecificParameter as VariableBacnetIPParameter;
            this.Type = type;
        }

        public Variable(VariableGeneralParameter generalParameter, VariableModbusRTUParameter modbusRtuSpecificParameter, string type)
        {
            this.GeneralParameter = generalParameter;
            this.SpecificParameter = modbusRtuSpecificParameter as VariableModbusRTUParameter;
            this.Type = type;
        }

        public virtual void OnValueChanged(string value)
        {
            ValueChanged?.Invoke(this, new BacnetObjectValueChangedEventArgs() { Value = value });
            var a = VariableList.Members.IndexOf(this);
            var frm = System.Windows.Forms.Application.OpenForms["DataViewer"] as DataViewer;
            frm.listView1.RefreshValue(a, value);
        }
    }
}
