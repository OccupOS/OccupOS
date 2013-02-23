using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Net;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using System.Threading;

namespace OccupOSNode.Micro {
    public class Program {
        public static void Main() {
            // Create the manager / timers etc.
            OutputPort outPrt = new OutputPort(Pins.ONBOARD_LED, false);
            while (true)
            {
                outPrt.Write(true);
                Thread.Sleep(500);
                outPrt.Write(false);
                Thread.Sleep(500);
            }
        }
    }
}
