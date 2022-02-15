using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.BACnet;
using System.ComponentModel;
using System.Timers;
namespace Visu3
{
    public class BacnetObject
    {
        public Timer ResubscriptionTimer = new Timer();

        public BacnetAddress BacnetDeviceID { get; set; }

        public BacnetObjectId BacnetObjectID { get; set; }

        public ushort DeviceNetwork { get; set; }

        public string DeviceIP { get; set; }

        public uint DeviceInstance { get; set; }

        public string[] StatusTexts { get; set; }

        public uint CovLifetime { get; set; }

        public bool COV { get; set; }

        public string Value { get; set; }

        public delegate void ValueChangedEventHandler(object source, BacnetObjectValueChangedEventArgs e);

        public event ValueChangedEventHandler ValueChanged;

        public BacnetObject(BacnetDevice bacnetDeviceID, BacnetObjectID bacnetObjectID, string[] statusTexts, bool cov, uint covlifetime)
        {
            this.BacnetDeviceID = bacnetDeviceID.Device;
            this.BacnetObjectID = bacnetObjectID.Object;
            this.DeviceNetwork = bacnetDeviceID.Network;
            this.DeviceIP = bacnetDeviceID.IP;
            this.DeviceInstance = bacnetDeviceID.Instance;
            this.StatusTexts = statusTexts;
            this.COV = cov;
            this.CovLifetime = covlifetime;
            if (COV)
            {
                Subscribe();
                ResubscriptionTimer.Interval = CovLifetime * 1000;
                ResubscriptionTimer.AutoReset = true;
                ResubscriptionTimer.Elapsed += OnResubscribe;
                ResubscriptionTimer.Start();
            } 
        }

        public BacnetObject()
        {

        }

        public string Read()
        {
            //UNDONE READ BACNET OBJECT
            IList<BacnetValue> response;
            BacnetCommunicator.Client.ReadPropertyRequest(BacnetDeviceID, BacnetObjectID, BacnetPropertyIds.PROP_PRESENT_VALUE, out response);
            //var asd = BacnetCommunicator.Client.ReadPropertyAsync(BacnetDeviceID,BacnetObjectID, BacnetPropertyIds.PROP_PRESENT_VALUE);
            //OnValueChanged();
            return response[0].ToString();
        }

        public void Write()
        {
            //UNDONE WRITE BACNET OBJECT
        }

        public void Subscribe()
        {
            BackgroundWorker subscriber = new BackgroundWorker();
            subscriber.DoWork += subscriber_DoWork;
            subscriber.RunWorkerAsync();
        }

        private void OnResubscribe(object source, System.Timers.ElapsedEventArgs e)
        {
            BacnetCommunicator.covResubscriptionCounter++;
            Subscribe();
        }

        private void subscriber_DoWork(object sender, DoWorkEventArgs e)
        {
            BacnetCommunicator.Client.SubscribeCOVRequest(BacnetDeviceID, BacnetObjectID, 0, false, false, CovLifetime);
        }

        public virtual void OnValueChanged(string value)
        {
            ValueChanged?.Invoke(this, new BacnetObjectValueChangedEventArgs() { Value = value });
        }
    }
}
