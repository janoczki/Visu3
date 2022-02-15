using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Visu3
{
    public class DatapointFileReader
    {
        public static string[] BacnetIpFile { get; set; }

        public static string[] ModbusTcpFile { get; set; }

        public static string[] ModbusRtuFile { get; set; }

        private static string[] _readBacnetIpFile()
        {
            try
            {
                var file = File.ReadAllLines(@"C:\Visu\Projects\UJ\datapoints.bacnetip", Encoding.Default);
                return file;
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        private static string[] _readModbusTcpFile()
        {
            try
            {
                var file = File.ReadAllLines(@"C:\Visu\Projects\UJ\datapoints.modbustcp", Encoding.Default);
                return file;
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        private static string[] _readModbusRtuFile()
        {
            try
            {
                var file = File.ReadAllLines(@"C:\Visu\Projects\UJ\datapoints.modbusrtu", Encoding.Default);
                return file;
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        public static void ReadFiles()
        {
            BacnetIpFile = _readBacnetIpFile();
            ModbusTcpFile = _readModbusTcpFile();
            ModbusRtuFile = _readModbusRtuFile();
        }

    }
}
