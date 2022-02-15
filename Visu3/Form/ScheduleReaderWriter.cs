using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Globalization;
using System.ComponentModel;
namespace Visu3
{
    
    public partial class ScheduleReaderWriter : Form
    {
        private Variable _variable;

        private ListViewItem Selected { get; set; }

        private List<List<string>> Schedule { get; set; }

        private List<string> Actions { get; set; }

        private string Type { get; set; }

        private VariableBacnetIPParameter spec { get; set; }

        private ScheduleObject scheduleObject { get; set; }

        public ScheduleReaderWriter(Variable variable)
        {
            InitializeComponent();
            this._variable = variable;
            this.spec = this._variable.SpecificParameter as VariableBacnetIPParameter;
            scheduleObject = new ScheduleObject(spec.BacnetObject);
            TransferData();
            AddColumns();
        }

        public void TransferData()
        {
            objCovLabel.Text = spec.BacnetObject.COV.ToString();
            devIPLabel.Text = spec.BacnetObject.DeviceIP;
            devInstLabel.Text = spec.BacnetObject.DeviceInstance.ToString();
            objTypeLabel.Text = spec.BacnetObjectID.type.ToString();
            objInstLabel.Text = spec.BacnetObjectID.instance.ToString();
        }

        private DataGridViewTextBoxColumn TimeColumn()
        {
            return new DataGridViewTextBoxColumn() {
                Name = "Time",
                HeaderText = "Time",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells };
        }

        private DataGridViewColumn ActionColumn(string type)
        {
            if (scheduleObject.Type == "enumerated")
                return new DataGridViewComboBoxColumn() {
                Name = "Action",
                HeaderText = "Action",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                DataSource = scheduleObject.StatusTexts};
            return new DataGridViewTextBoxColumn() {
                Name = "Action",
                HeaderText = "Action",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader };
        }

        private void AddColumns()
        {
            var allColumns = new DataGridViewColumn[] {
                TimeColumn(),
                ActionColumn(Type),
                GetDayColumns()[0],
                GetDayColumns()[1],
                GetDayColumns()[2],
                GetDayColumns()[3],
                GetDayColumns()[4],
                GetDayColumns()[5],
                GetDayColumns()[6]};
            dataGridView2.Columns.AddRange(allColumns);
        }
        
        private DataGridViewCheckBoxColumn[] GetDayColumns()
        {
            var lst = new List<DataGridViewCheckBoxColumn>();
            for (int i = 1; i < 8; i++)
            {
                lst.Add(new DataGridViewCheckBoxColumn()
                {
                    Name = CultureInfo.InvariantCulture.DateTimeFormat.ShortestDayNames[i % 7],
                    HeaderText = CultureInfo.InvariantCulture.DateTimeFormat.ShortestDayNames[i % 7],
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader
                });
            }
            return lst.ToArray();
        }

        private void dataGridView2_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show(e.Exception.Message);
        }

        private void FillTimeComboBoxes()
        {
            var list_0_23 = Enumerable.Range(0, 24).Select(n => new string('0', 2 - n.ToString().Length) + n.ToString()).ToList(); list_0_23.AddRange(new List<string> { "**" });
            var list_0_59 = Enumerable.Range(0, 60).Select(n => new string('0', 2 - n.ToString().Length) + n.ToString()).ToList(); list_0_59.AddRange(new List<string> { "**" });
            var list_0_99 = Enumerable.Range(0, 100).Select(n => new string('0', 2 - n.ToString().Length) + n.ToString()).ToList(); list_0_99.AddRange(new List<string> { "**" });
            comboBox1.DataSource = list_0_23;
            comboBox2.DataSource = comboBox3.DataSource = list_0_59;
            comboBox4.DataSource = list_0_99;
        }

        private void UncheckDayCheckboxes()
        {
            foreach (var control in groupBox3.Controls)
            {
                var checkbox = (CheckBox)control;
                checkbox.Checked = false;
            }
        }

        private void EnableActionField()
        {
            numericUpDown1.Enabled = scheduleObject.Type != "enumerated";
            comboBox5.Enabled = scheduleObject.Type == "enumerated";
        }

        private void FillActionCombobox()
        {
            var commands = scheduleObject.StatusTexts.ToList();
            var commandsForGui = new List<string>();
            foreach (var command in commands)
            {
                if (command == "") continue;
                commandsForGui.Add(command);
            }
            if (comboBox5.Enabled) comboBox5.DataSource = commandsForGui;
        }

        private void InitElements()
        {
            FillTimeComboBoxes();
            UncheckDayCheckboxes();
            EnableActionField();
            FillActionCombobox();
            SetActionFieldValueFormat();
        }

        private void SetActionFieldValueFormat()
        {
            numericUpDown1.DecimalPlaces = scheduleObject.Type == "float" ? 3 : 0;
            numericUpDown1.Minimum = decimal.MinValue;
            numericUpDown1.Maximum = decimal.MaxValue;
        }

        private void ScheduleReaderWriter_Load(object sender, EventArgs e)
        {
            InitElements();
            var schedule = scheduleObject.GetSchedule();
            Actions = scheduleObject.GetScheduleCommands(schedule);
            foreach (var command in Actions)
            {
                if (command == "") continue;
                var whichDay = new List<bool>();
                foreach (var day in schedule) {whichDay.Add(day.Contains(command));}
                var time = command.Split(new string[] { " = " }, StringSplitOptions.None)[0];
                var val = command.Split(new string[] { " = " }, StringSplitOptions.None)[1];
                var row = CreateRow(time, scheduleObject.StatusTexts, val, whichDay);
                dataGridView2.Rows.Add(row);
            } 
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var val = comboBox5.Enabled == true ? Array.IndexOf(scheduleObject.StatusTexts,comboBox5.Text).ToString() : numericUpDown1.Value.ToString();
            var time = comboBox1.Text + ":" + comboBox2.Text + ":" + comboBox3.Text + ":" + comboBox4.Text;
            var whichDay = new List<bool> {checkBox1.Checked, checkBox2.Checked, checkBox3.Checked, checkBox4.Checked, checkBox5.Checked, checkBox6.Checked, checkBox7.Checked };
            var row = CreateRow(time, scheduleObject.StatusTexts, val, whichDay);
            
            if (AddRow(row))
            {
                dataGridView2.Sort(dataGridView2.Columns["Time"], ListSortDirection.Ascending);
                InitElements();
            }
        }

        private bool AddRow(dynamic[] row)
        {
            if (row == null)
            {
                MessageService.SendWarninglMessage("Row without selected day. The row will not be added.", "Invalid action");
                return false;
            } 
            foreach (DataGridViewRow existingRow in dataGridView2.Rows)
            {
                if (row[0] == existingRow.Cells[0].Value.ToString() && row[1] == existingRow.Cells[1].Value.ToString())
                {
                    MessageService.SendWarninglMessage("Row with given time and command already exists. The row will not be added.", "Existing action");
                    return false;
                }
            }
            dataGridView2.Rows.Add(row);
            return true;
        }

        private dynamic[] CreateRow(string time, string[] possibleCommands, string val, List<bool> whichDay)
        {
            if (whichDay.Any(x => x))
            {
                return new dynamic[] {
                    time, scheduleObject.StatusTexts.Length !=1 ? scheduleObject.StatusTexts[int.Parse(val)] : val.ToString(),
                    whichDay[0], whichDay[1], whichDay[2], whichDay[3], whichDay[4], whichDay[5], whichDay[6]};
            }
            return null;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedCells.Count>0)
            {
                dataGridView2.Rows.RemoveAt(dataGridView2.SelectedCells[0].RowIndex);
            }
        }

        private void btnSendAndClose_Click(object sender, EventArgs e)
        {
            byte[] sch = new byte[] { 62,14,15,14,15,14,15,14,15,14,15,14,15,14,15,63};
            scheduleObject.WriteSchedule(sch);
        }
    }
}
