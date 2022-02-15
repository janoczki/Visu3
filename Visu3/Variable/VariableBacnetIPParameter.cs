using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.BACnet;
namespace Visu3
{
    public class VariableBacnetIPParameter : VariableParameter
    {
        public BacnetObject BacnetObject { get; set; }

        public BacnetAddress BacnetDevice { get; }

        public BacnetObjectId BacnetObjectID { get; }

        public string IP { get; }

        public uint Instance { get; }

        public ushort NetworkNumber { get; }

        public VariableBacnetIPParameter(BacnetDevice bacnetDevice, BacnetObjectID bacnetObject)
        {
            this.BacnetDevice = bacnetDevice.Device;
            this.BacnetObjectID = bacnetObject.Object;
            //this.NetworkNumber = bacnetDevice.Network;
            //this.IP = bacnetDevice.IP;
            //this.Instance = bacnetDevice.Instance;
        }

        public VariableBacnetIPParameter(BacnetObject bacnetObject)
        {
            this.BacnetObject = bacnetObject;
            this.BacnetDevice = bacnetObject.BacnetDeviceID;
            this.BacnetObjectID = bacnetObject.BacnetObjectID;
        }
    }
}
