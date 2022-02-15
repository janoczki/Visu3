using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.BACnet;
namespace Visu3
{
    public static class VariableFactory
    {

        public static Variable CreateBacnetVariable(string[] parameters)
        {
            uint covLifetime = 60;
            //4     cov
            //10    statustext
            var networkNumber = ushort.Parse(parameters[5]);
            var deviceIP = parameters[6];
            var deviceInstance = uint.Parse(parameters[7]);
            
            var device = new BacnetDevice(networkNumber, deviceIP, deviceInstance);
            var bacnetObjectID = parameters[8];
            var bacnetObjectInstance = uint.Parse(parameters[9]);
            var objectid = new BacnetObjectID(bacnetObjectID, bacnetObjectInstance);
            var statusTexts = parameters[10].Split('/');
            var cov = bool.Parse(parameters[4]);
            var bacnetObject = new BacnetObject(device, objectid, statusTexts, cov, covLifetime);
            bacnetObject.ValueChanged += BacnetObject_ValueChanged;
            var bacnetIPParameter = new VariableBacnetIPParameter(bacnetObject);



            var value = bacnetObject.Value;
            var id = int.Parse(parameters[0]);
            var name = parameters[1];
            var desc = parameters[2];
            var save = bool.Parse(parameters[3]);
            var generalParameter = new VariableGeneralParameter(id, name, desc, save, value);
            var variable = new Variable(generalParameter, bacnetIPParameter, "BACNET");
            variable.ValueChanged += Variable_ValueChanged;
            return variable;
        }

        private static void BacnetObject_ValueChanged(object source, BacnetObjectValueChangedEventArgs e)
        {
            var obj = source as BacnetObject;
            obj.Value = e.Value;
        }

        private static void Variable_ValueChanged(object source, BacnetObjectValueChangedEventArgs e)
        {

            var obj = source as Variable;
            obj.GeneralParameter.Value = e.Value;
        }
    }
}
