using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.BACnet;
namespace Visu3
{
    class BacnetCommunicator
    {
        public static int covResubscriptionCounter = 0;
        public static BacnetClient Client;

        public static void Start(string localEndpoint, uint writePriority)
        {
            Client = new BacnetClient(new BacnetIpUdpProtocolTransport(0xBAC0, false, false, 1472, localEndpoint)) { WritePriority = writePriority };
            Client.OnCOVNotification += handler_OnCOVNotification;
            try
            {
                Client.Start();
            }
            catch (Exception ex)
            {

                MessageService.SendErrolMessage(ex.Message.ToString(), ex.ToString());
            }
            
        }

        public static void handler_OnCOVNotification(BacnetClient sender, BacnetAddress bacnetDevice, byte invoke_id, uint subscriberProcessIdentifier, BacnetObjectId initiatingDeviceIdentifier, BacnetObjectId Object, uint timeRemaining, bool needConfirm, ICollection<BacnetPropertyValue> values, BacnetMaxSegments max_segments)
        {
            if ((BacnetPropertyIds)values.ToList()[0].property.propertyIdentifier == BacnetPropertyIds.PROP_PRESENT_VALUE)
            {
                var count = VariableList.Members.Count;
                for (int i = 0; i < count; i++)
                {
                    var variable = VariableList.Members[i];
                    if (variable.Type != "BACNET") continue;
                    var spec = variable.SpecificParameter as VariableBacnetIPParameter;
                    var bnd = spec.BacnetObject.BacnetDeviceID;
                    var bno = spec.BacnetObject.BacnetObjectID;

                    if (bnd.Equals(bacnetDevice) && bno.Equals(Object))
                    {
                        var value = values.ToList()[0].value[0].ToString();
                        spec.BacnetObject.OnValueChanged(value);
                        variable.OnValueChanged(value);
                        break;
                    }
                }
                //foreach (Variable variable in VariableList.Members)
                //{
                //    if (variable.Type != "BACNET") continue;
                //    var spec = variable.SpecificParameter as VariableBacnetIPParameter;
                //    var bnd = spec.BacnetObject.BacnetDeviceID;
                //    var bno = spec.BacnetObject.BacnetObjectID;

                //    if (bnd.Equals(bacnetDevice) && bno.Equals(Object))
                //    {
                //        spec.BacnetObject.Value = values.ToList()[0].value[0].ToString();
                //        spec.BacnetObject.OnValueChanged();
                //        break;
                //    }
                //}
            }
            if (needConfirm) sender.SimpleAckResponse(bacnetDevice, BacnetConfirmedServices.SERVICE_CONFIRMED_COV_NOTIFICATION, invoke_id);
        }
    }
}
