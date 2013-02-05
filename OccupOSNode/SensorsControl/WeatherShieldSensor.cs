using System;
using Newtonsoft.Json;
namespace FirstExample
{
    public class WeatherShieldSensor : Sensor
    {
      private  WeatherShieldController controller;
      private  String data;
        public WeatherShieldSensor(String id, String roomId, int floorNo, String sensorName = "", String departmentName = "") : base( id,  roomId,  floorNo,  sensorName = "",  departmentName = "")
        {
            controller = new WeatherShieldController();

            
        }

        public void poll()
        {

            data = JsonConvert.SerializeObject(this);

        }
    }
}

