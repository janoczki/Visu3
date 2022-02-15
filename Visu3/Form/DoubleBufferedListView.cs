using System.Windows.Forms;

namespace Visu3
{
    public class DoubleBufferedListView : ListView
    {
        public DoubleBufferedListView()
        {
            //Activate double buffering
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.EnableNotifyMessage, true);
        }

        protected override void OnNotifyMessage(Message m)
        {
            //Filter out the WM_ERASEBKGND message
            if (m.Msg != 0x14)
            {
                base.OnNotifyMessage(m);
            }
        }

        public void RefreshValue(int rowIndex, string value)
        {
            if (this.InvokeRequired) //Is this method being called from a different thread
                Invoke(new MethodInvoker(() => this.Items[rowIndex].SubItems[9].Text = value));
            //else //it's cool, this is the original thread, procceed
            //    this.Items[rowIndex].SubItems[9].Text = value;

            ////Items[rowIndex].SubItems["Value"].Text = value;
            ////Items[rowIndex].SubItems[(int)DatapointDefinition.Columns.Value].Text = content;
        }
    }
}
