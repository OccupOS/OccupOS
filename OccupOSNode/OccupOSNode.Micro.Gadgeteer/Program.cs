using System;
using Microsoft.SPOT;
using Microsoft.WindowsAzure.MobileServices;
using OccupOSNode.Micro.Gadgeteer;
using GT = Gadgeteer;
using Gadgeteer.Modules.GHIElectronics;
using System.Net.Sockets;

namespace GadgeteerDemo
{
    public partial class Program
    {
        private readonly GT.Timer timer = new GT.Timer(2000);

        //TODO: add your mobile service URI and app key below from the Windows Azure Portal https://manage.windowsazure.com 
        public static MobileServiceClient MobileService = new MobileServiceClient(
            new Uri("http://occuposcloud.azure-mobile.net/"), 
            "dtvuHNJYYBSSiIHjsCclvdqYhsakny68"
        );

        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            wifi_RS21.DebugPrintEnabled = true;

            wifi_RS21.Interface.Open();

            wifi_RS21.Interface.NetworkInterface.EnableDhcp();
            wifi_RS21.Interface.NetworkInterface.EnableDynamicDns();
            wifi_RS21.Interface.NetworkInterface.EnableStaticIP("192.168.1.117", "255.255.255.0", "192.168.1.254");

            Debug.Print("Scanning for WiFi networks");
            GHI.Premium.Net.WiFiNetworkInfo[] wiFiNetworkInfo = wifi_RS21.Interface.Scan();
            if (wiFiNetworkInfo != null)
            {
                Debug.Print("Found WiFi network");
                Debug.Print("0: " + wiFiNetworkInfo[0].SSID);
                Debug.Print("Joining " + wiFiNetworkInfo[0].SSID);
                wifi_RS21.Interface.Join(wiFiNetworkInfo[0], "69B3625573");
                Debug.Print("Connected?: " + wifi_RS21.Interface.IsLinkConnected.ToString());
            }
            else
            {
                Debug.Print("Didn't find any WiFi networks");
            }

            timer.Tick += timer_Tick;
            timer.Start();

            Debug.Print("Finished setup");
        }

        void timer_Tick(GT.Timer timer)
        {
            var lightPercentage = (int) lightSensor.ReadLightSensorPercentage();
            //create a new sensor reading and set the values
            var reading = new SensorReading() {
                SensorID = "morrison-home",
                Light = lightPercentage,
                DateAdded = DateTime.UtcNow
            };

            var json = MobileService.GetTable("SensorData").Insert(reading);
        }
    }
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      