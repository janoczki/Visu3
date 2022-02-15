using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.BACnet;
namespace Visu3
{
    public class BacnetObjectValueChangedEventArgs : EventArgs
    {
        public string Value { get; set; }
    }
}
