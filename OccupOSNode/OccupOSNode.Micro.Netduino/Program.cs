using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Net;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using System.Threading;
using OccupOSNode.Micro.NetworkControllers.Arduino;


namespace OccupOSNode.Micro {
    public class Program {
      private static OutputPort outPrt = new OutputPort(Pins.ONBOARD_LED, false);
        public static void Main() {
            blink();
           ArduinoEthernetController controller = new ArduinoEthernetController("192.168.1.127",1333);
           blink1();
           if (controller.connect() == null) blink();
           else
               blink1();
           while (true)
           {
               if(controller.sendData("test")>1) Program.blink3();
           }
        /*    // Create the manager / timers etc.
            while (true)
            {
               
            }*/

        }
           static void blink()
        {
            
            outPrt.Write(true);
            Thread.Sleep(2000);
            outPrt.Write(false);
            Thread.Sleep(1000);
            outPrt.Write(true);
            Thread.Sleep(2000);
            outPrt.Write(false);
            Thread.Sleep(1000);
            outPrt.Write(true);
            Thread.Sleep(2000);
            outPrt.Write(false);
            Thread.Sleep(1000);
        }
           static void blink1()
           {

               outPrt.Write(true);
               Thread.Sleep(2000);
               outPrt.Write(false);
               Thread.Sleep(1000);
               outPrt.Write(true);
               Thread.Sleep(2000);
               outPrt.Write(false);
               Thread.Sleep(1000);
           }
           static void blink3()
           {

               outPrt.Write(true);
               Thread.Sleep(500);
               outPrt.Write(false);
               Thread.Sleep(500);
               outPrt.Write(true);
               Thread.Sleep(500);
               outPrt.Write(false);
               Thread.Sleep(500);
               outPrt.Write(true);
               Thread.Sleep(500);
               outPrt.Write(false);
               Thread.Sleep(1000);
           }
    }
}
