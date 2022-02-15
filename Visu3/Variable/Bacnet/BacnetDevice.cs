using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.BACnet;

namespace Visu3
{
    public class BacnetDevice
    {
        public ushort Network { get; set; }

        public string IP { get; set; }

        public uint Instance { get; set; }

        public BacnetAddress Device { get; }

        public BacnetDevice(ushort network, string ip, uint instance)
        {
            this.Network = network;
            this.IP = ip;
            this.Instance = instance;
            this.Device = GetBacnetAddress();
        }

        public BacnetAddress GetBacnetAddress()
        {
            return new BacnetAddress(BacnetAddressTypes.IP, IP, Network);
        }

    }
}
