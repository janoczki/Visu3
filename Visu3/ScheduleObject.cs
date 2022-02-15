using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.BACnet;
using System.IO.BACnet.Serialize;
using System.IO.BACnet.Storage;

namespace Visu3
{
    class ScheduleObject : BacnetObject
    {
        public string Type { get; set; }

        public ScheduleObject(BacnetDevice bacnetDeviceID, BacnetObjectID bacnetObjectID, string[] statusTexts, bool cov, uint covlifetime) : base(bacnetDeviceID, bacnetObjectID, statusTexts, cov, covlifetime)
        {
            this.BacnetDeviceID = bacnetDeviceID.Device;
            this.BacnetObjectID = bacnetObjectID.Object;
            this.DeviceNetwork = bacnetDeviceID.Network;
            this.DeviceIP = bacnetDeviceID.IP;
            this.DeviceInstance = bacnetDeviceID.Instance;
            this.StatusTexts = statusTexts;
            this.Type = StatusTexts.Length > 1 ? "enumerated" : "float";
        }

        public ScheduleObject(BacnetObject bacnetObject)
        {
            this.BacnetDeviceID = bacnetObject.BacnetDeviceID;
            this.BacnetObjectID = bacnetObject.BacnetObjectID;
            this.DeviceNetwork = bacnetObject.DeviceNetwork;
            this.DeviceIP = bacnetObject.DeviceIP;
            this.DeviceInstance = bacnetObject.DeviceInstance;
            this.StatusTexts = bacnetObject.StatusTexts;
            this.Type = StatusTexts.Length > 1 ? "enumerated" : "float";
        }

        public byte[] lofasz()
        {
            var task = Task.Run(() => ReadSchedule());
            if (task.Wait(TimeSpan.FromSeconds(3)))
                return task.Result;
            else
                MessageService.SendErrolMessage("Reading schedule failed", "Communication failure");
            return null;
        }
        private byte[] ReadSchedule()
        {
            byte[] InOutBuffer = null;
            BacnetCommunicator.Client.RawEncodedDecodedPropertyConfirmedRequest(
                BacnetDeviceID,
                BacnetObjectID,
                BacnetPropertyIds.PROP_WEEKLY_SCHEDULE,
                BacnetConfirmedServices.SERVICE_CONFIRMED_READ_PROPERTY,
                ref InOutBuffer);
            return InOutBuffer;
        }

        public List<List<string>> GetSchedule()
        {
            var InOutBuffer = lofasz();
            var days = new List<List<string>>();
            byte tag_number;
            int offset = 1;
            uint len_value_type;

            if (InOutBuffer == null) return new List<List<string>>();

            for (int i = 1; i < 8; i++)
            {
                var isContent = false;
                var day = new List<string>();
                var time = "";

                
                offset += ASN1.decode_tag_number(InOutBuffer, offset, out tag_number);
                while (!ASN1.IS_CLOSING_TAG(InOutBuffer[offset]))
                {
                    isContent = true;
                    BacnetValue value;
                    String command;

                    // Times
                    offset += ASN1.decode_tag_number_and_value(InOutBuffer, offset, out tag_number, out len_value_type);
                    offset += parseDate(InOutBuffer, offset, out time);

                    // Value
                    offset += ASN1.decode_tag_number_and_value(InOutBuffer, offset, out tag_number, out len_value_type);
                    offset += ASN1.bacapp_decode_data(InOutBuffer, offset, InOutBuffer.Length, (BacnetApplicationTags)tag_number, len_value_type, out value);

                    command = value.Tag != BacnetApplicationTags.BACNET_APPLICATION_TAG_NULL ? time + " = " + Property.SerializeValue(value, value.Tag) : time + " = null";
                    day.Add(command);
                }
                if (!isContent) day.Add("");
                isContent = false;
                offset++;
                days.Add(day);
            }
            return days;
        }

        private static int parseDate(byte[] InOutBuffer, int offset, out string value)
        {
            var h = new string('0', 2 - InOutBuffer[offset].ToString().Replace("255", "**").Length) + InOutBuffer[offset].ToString().Replace("255", "**");
            var m = new string('0', 2 - InOutBuffer[offset + 1].ToString().Length) + InOutBuffer[offset + 1].ToString();
            var s = new string('0', 2 - InOutBuffer[offset + 2].ToString().Length) + InOutBuffer[offset + 2].ToString();
            var hu = new string('0', 2 - InOutBuffer[offset + 3].ToString().Length) + InOutBuffer[offset + 3].ToString();
            value = h + ":" + m + ":" + s + ":" + hu;
            return 4;
        }

        public List<string> GetScheduleCommands(List<List<string>> schedule)
        {
            var actions = new List<string>();
            foreach (var day in schedule)
            {
                foreach (var action in day)
                {
                    if (actions.Contains(action) || action == "") continue;
                    actions.Add(action);
                }
            }
            actions.Sort();
            return actions;
        }

        public void WriteSchedule(byte[] schedule)
        {
            BacnetCommunicator.Client.RawEncodedDecodedPropertyConfirmedRequest(
                BacnetDeviceID,
                BacnetObjectID,
                BacnetPropertyIds.PROP_WEEKLY_SCHEDULE,
                BacnetConfirmedServices.SERVICE_CONFIRMED_WRITE_PROPERTY,
                ref schedule);
        }
    }

}
