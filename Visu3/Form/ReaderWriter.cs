using System;
using System.Windows.Forms;

namespace Visu3
{

    public partial class ReaderWriter : Form
    {
        public ListViewItem Selected;
        private Variable _variable;
        //public ReaderWriter(Datapoint dp)
        //{
        //    InitializeComponent();
        //}

        public ReaderWriter(Variable variable)
        {
            InitializeComponent();
            this._variable = variable;
            TransferData();
        }

        //public void TransferData(ListViewItem selected)
        //{
        //    //this.Selected = selected;
        //    //nameLabel.Text = selected.SubItems[(int)DatapointDefinition.Columns.DatapointName].Text;
        //    //descLabel.Text = selected.SubItems[(int)DatapointDefinition.Columns.DatapointDescription].Text;
        //    //typeLabel.Text = selected.SubItems[(int)DatapointDefinition.Columns.DatapointDatatype].Text;
        //    //recLabel.Text = selected.SubItems[(int)DatapointDefinition.Columns.DatapointSave].Text;
        //    //objCovLabel.Text = selected.SubItems[(int)DatapointDefinition.Columns.DatapointCov].Text;
        //    //devIPLabel.Text = selected.SubItems[(int)DatapointDefinition.Columns.DeviceIp].Text;
        //    //devInstLabel.Text = selected.SubItems[(int)DatapointDefinition.Columns.DeviceInstance].Text;
        //    //objTypeLabel.Text = selected.SubItems[(int)DatapointDefinition.Columns.ObjectType].Text;
        //    //objInstLabel.Text = selected.SubItems[(int)DatapointDefinition.Columns.ObjectInstance].Text;
        //    //readedValueLabel.Text = selected.SubItems[(int)DatapointDefinition.Columns.Value].Text;
        //}

        public void TransferData()
        {
            var gen = _variable.GeneralParameter;
            var spec = _variable.SpecificParameter as VariableBacnetIPParameter;
            
            nameLabel.Text = gen.Name;
            descLabel.Text = gen.Description;
            //typeLabel.Text = selected.SubItems[(int)DatapointDefinition.Columns.DatapointDatatype].Text;
            recLabel.Text = gen.Save.ToString();
            objCovLabel.Text = spec.BacnetObject.COV.ToString();
            devIPLabel.Text = spec.BacnetObject.DeviceIP;
            devInstLabel.Text = spec.BacnetObject.DeviceInstance.ToString();
            objTypeLabel.Text = spec.BacnetObjectID.type.ToString();
            objInstLabel.Text = spec.BacnetObjectID.instance.ToString();
            readedValueLabel.Text = gen.Value;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var gen = _variable.GeneralParameter;
            readedValueLabel.Text = gen.Value;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void writeButton_Click(object sender, EventArgs e)
        {
            //var bacnetDevice = Bac.GetBacnetDevice(devIPLabel.Text, 1);
            //var bacnetObject = Bac.GetBacnetObject(objTypeLabel.Text, Convert.ToUInt16(objInstLabel.Text));
            //var value = valueToWriteTextbox.Text;
            //var format = typeLabel.Text;
            //var obj = new BacnetObjects.NormalObject(bacnetDevice, bacnetObject);
            //obj.Write(value, format, false);
            //Datapoints.Record(bacnetDevice, bacnetObject, value);
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            //var bacnetDevice = Bac.GetBacnetDevice(devIPLabel.Text, 1);
            //var bacnetObject = Bac.GetBacnetObject(objTypeLabel.Text, Convert.ToUInt16(objInstLabel.Text));
            //var format = typeLabel.Text;
            //var obj = new BacnetObjects.NormalObject(bacnetDevice, bacnetObject);
            //obj.Write("0", format, true);
            //Datapoints.Record(bacnetDevice, bacnetObject, obj.Read());
        }
    }
}
