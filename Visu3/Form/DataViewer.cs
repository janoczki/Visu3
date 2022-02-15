using System;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using System.IO.BACnet;
namespace Visu3
{
    public partial class DataViewer : Form
    {
        public DataViewer()
        {
            InitializeComponent();
        }
        
        private void DataViewer_Load(object sender, EventArgs e)
        {
            global.Ini();
            Log.Append("Application start");
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DatapointFileReader.ReadFiles();
            var bacnetIpFile = DatapointFileReader.BacnetIpFile;
            var ModbusRtuFile = DatapointFileReader.ModbusRtuFile;
            var ModbusTcpFile = DatapointFileReader.ModbusTcpFile;

            if (bacnetIpFile != null)
            {
                BacnetCommunicator.Start(global.LocalEndPoint, global.WritePriority);
                foreach (var row in bacnetIpFile.Skip(1).ToArray())
                {
                    var parameters = row.Split(';');
                    var variable = VariableFactory.CreateBacnetVariable(parameters);
                    VariableList.Members.Add(variable);
                }
            }
            AddHeaders();
            AddContentToListview();
        }

        public void AddHeaders()
        {
            var headers = new string[] {"ID","Name","Description","Save","Network number", "Device IP", "Device instance", "Object ID", "Object Instance", "Value" };
            foreach (string header in headers)
            {
                listView1.Columns.Add(header, 1, HorizontalAlignment.Center);
            }
        }

        public void AddContentToListview()
        {
            foreach (Variable var in VariableList.Members)
            { 
                var GenPar = var.GeneralParameter;
                var BacPar = var.SpecificParameter as VariableBacnetIPParameter;
                var row = new string[]
                    {
                        GenPar.Id.ToString(),
                        GenPar.Name,
                        GenPar.Description,
                        GenPar.Save.ToString(),
                        BacPar.BacnetObject.DeviceNetwork.ToString(),
                        BacPar.BacnetObject.DeviceIP,
                        BacPar.BacnetObject.DeviceInstance.ToString(),
                        BacPar.BacnetObjectID.Type.ToString(),
                        BacPar.BacnetObjectID.instance.ToString(),
                        GenPar.Value
                    };
                listView1.Items.Add(new ListViewItem(row));
            }
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void QuitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log.Append("Quit application");
            Application.Exit();
        }
        
        private void StartProgress()
        {
            BacnetCommunicator.Start(global.LocalEndPoint, global.WritePriority);
            UItimer.Enabled = true;
            pollingTimer.Enabled = true;
        }

        private void pollingTimer_Tick(object sender, EventArgs e)
        {
            //pollingTimer.Interval = global.PollInterval;
            //var reader = new BackgroundWorker();
            //reader.DoWork += Workers.Reader.Work;
            //reader.RunWorkerCompleted += Workers.Reader.Completed;
            //reader.RunWorkerAsync();
        }

        //private void UITimer_Tick(object sender, EventArgs e)
        //{
        //    //foreach (var row in Datapoints.Table)
        //    //{
        //    //    var value = row[(int)DatapointDefinition.Columns.Value];
        //    //    var index = Datapoints.Table.IndexOf(row);
        //    //    listView1.RefreshValue(index, value);
        //    //    //listView1.Items[Datapoints.Table.IndexOf(row)].SubItems[(int)DatapointDefinition.Columns.Value].Text = row[(int)DatapointDefinition.Columns.Value];
        //    //}
        //}
        
        //private static bool OpenReaderWriter(ListViewItem selected)
        //{
        //    //var dp = new Datapoint(selected);

        //    //var bacnetObject = new BacnetObject(
        //    //    new Device(dp.NetworkNumber, dp.DeviceIP, dp.DeviceInstance).GetDevice(),
        //    //    new BacnetID(dp.BacnetObjectType, dp.BacnetObjectInstance).GetBacnetID());
        //    return true;
        //}

        private static bool OpenReaderWriter(Variable variable)
        {
            var rw = Application.OpenForms["ReaderWriter"] as ReaderWriter ??
                new ReaderWriter(variable);
            rw.Show();
            return true;
        }

        //private static bool OpenReaderWriter(Datapoint dp)
        //{
        //    //var rw = Application.OpenForms["ReaderWriter"] as ReaderWriter;
        //    //if (rw != null) rw.Close();
        //    //rw = new ReaderWriter(dp);
        //    //rw.Show();
        //    return true;
        //}

        //private static bool OpenScheduleReaderWriter(ListViewItem selected)
        //{
        //    //var dp = new Datapoint(selected);
        //    //var actionType = dp.BacnetObjectDataFormat;
        //    //var allCommand = dp.StateTexts;
        //    //var bacnetDevice = new Device(dp.NetworkNumber, dp.DeviceIP, dp.DeviceInstance).GetDevice();
        //    //var bacnetID = new BacnetID(dp.BacnetObjectType, dp.BacnetObjectInstance).GetBacnetID();
        //    //var bacnetDatapoint = new ScheduleObject(bacnetDevice, bacnetID);
        //    //var schedule = bacnetDatapoint.GetSchedule();
        //    //var scheduleCommands = bacnetDatapoint.GetScheduleCommands(schedule);
        //    //var srw = Application.OpenForms["ScheduleReaderWriter"] as ScheduleReaderWriter ??
        //    //    new ScheduleReaderWriter(selected, schedule, scheduleCommands, allCommand, actionType, actionType);
        //    //srw.Show();
        //    return true;
        //}

        private static bool OpenScheduleReaderWriter(Variable variable)
        {
            var srw = Application.OpenForms["ScheduleReaderWriter"] as ScheduleReaderWriter ??
                new ScheduleReaderWriter(variable);
            srw.Show();
            return true;
            //var dp = new Datapoint(selected);
            //var actionType = dp.BacnetObjectDataFormat;
            //var allCommand = dp.StateTexts;
            //var bacnetDevice = new Device(dp.NetworkNumber, dp.DeviceIP, dp.DeviceInstance).GetDevice();
            //var bacnetID = new BacnetID(dp.BacnetObjectType, dp.BacnetObjectInstance).GetBacnetID();
            //var bacnetDatapoint = new ScheduleObject(bacnetDevice, bacnetID);
            //var schedule = bacnetDatapoint.GetSchedule();
            //var scheduleCommands = bacnetDatapoint.GetScheduleCommands(schedule);
            //var srw = Application.OpenForms["ScheduleReaderWriter"] as ScheduleReaderWriter ??
            //    new ScheduleReaderWriter(selected, schedule, scheduleCommands, allCommand, actionType, actionType);
            //srw.Show();
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var selectedIndex = listView1.SelectedIndices[0];
            var variable = VariableList.Members[selectedIndex];
            var specificParameter = variable.SpecificParameter as VariableBacnetIPParameter;

            var operation = specificParameter.BacnetObject.BacnetObjectID.type == BacnetObjectTypes.OBJECT_SCHEDULE ? 
                OpenScheduleReaderWriter(variable) : 
                OpenReaderWriter(variable);
        }

        private void DataViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            Log.Append("Application stop");
            while (Log.inProgress) { };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var probaform = new TestForm();
            probaform.Show();
            //var Model = new MyModel();
            //textBox1.DataBindings.Add("Text", Model, nameof(MyModel.Name), false, DataSourceUpdateMode.OnPropertyChanged);

            //string temp = VariableList.Members[15].GeneralParameter.Value;
            //textBox1.Text = temp;
        }

        private void UItimer_Tick(object sender, EventArgs e)
        {

        }
    }

    public class MyModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _name;
        public string Name
        {
            get { return _name;}
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                };
            }
            }

    private void OnPropertyChanged(string name)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        }

        
    }
}
