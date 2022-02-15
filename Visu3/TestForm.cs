using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Visu3
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        private void TestForm_Load(object sender, EventArgs e)
        {
            int i = 1;
            foreach (var control in Controls)
            {
                if (control is TextBox)
                {
                    var c = control as TextBox;
                    c.Tag = i;
                    i++;
                }
            }
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            foreach (var control in Controls)
            {
                if (control is TextBox)
                {
                    var c = control as TextBox;
                    var tag = int.Parse(c.Tag.ToString());
                    if (c.Text != c.Tag.ToString() + " : " + VariableList.Members[tag].GeneralParameter.Value)
                    {
                        c.BackColor = Color.Yellow;
                        c.Text = c.Tag.ToString() + " : " + VariableList.Members[tag].GeneralParameter.Value;
                    }
                    else
                    {
                        c.BackColor = Color.LightGray;
                    }
                    
                }
            }

        }
    }
}
