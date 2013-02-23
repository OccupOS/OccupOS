using Microsoft.SPOT;
using GT = Gadgeteer;
using Gadgeteer.Modules.GHIElectronics;

namespace GadgeteerDemo
{
    public partial class Program
    {
        private readonly GT.Timer timer = new GT.Timer(2000);

        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            Debug.Print("Scanning for WiFi networks");
            GHI.Premium.Net.WiFiNetworkInfo[] wiFiNetworkInfo = wifi_RS21.Interface.Scan();
            if (wiFiNetworkInfo != null)
            {
                Debug.Print("Found WiFi network");
                Debug.Print("0: " + wiFiNetworkInfo[0].SSID);
            }
            else
            {
                Debug.Print("Didn't find any WiFi networks");
            }

            timer.Tick += new GT.Timer.TickEventHandler(timer_Tick);
            timer.Start();

            Debug.Print("Finished setup");
        }

        void timer_Tick(GT.Timer timer)
        {
            int lightSensorPercentage = (int) lightSensor.ReadLightSensorPercentage();
            Debug.Print("Current (rounded) light sensor percentage: " + lightSensorPercentage.ToString());
        }
    }
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      