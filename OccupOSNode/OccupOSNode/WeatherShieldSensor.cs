using System;
using Newtonsoft.Json;

namespace OccupOSNode {

    public class WeatherShieldSensor : Sensor {

      private  WeatherShieldController controller;
      private float temperature, humidity, pressure;
      private byte[] data;

      public WeatherShieldSensor(String id, String roomId, int floorNo, String sensorName = "", String departmentName = "") : base( id,  roomId,  floorNo,  sensorName = "",  departmentName = "") {
          controller = new WeatherShieldController();
          data = new byte[4];
    
      }

      public void poll() {

          if (controller.sendCommand(WeatherShieldController.CMD_GETTEMP_C_RAW, WeatherShieldController.PAR_GET_LAST_SAMPLE, ref data))
             temperature = controller.decodeShortValue(data);
          else
             temperature = 0.0f;
          if (controller.sendCommand(WeatherShieldController.CMD_GETHUM_RAW, WeatherShieldController.PAR_GET_LAST_SAMPLE, ref data))
             humidity = controller.decodeShortValue(data);
          else
             humidity = 0.0f;
          if (controller.sendCommand(WeatherShieldController.CMD_GETPRESS_RAW, WeatherShieldController.PAR_GET_LAST_SAMPLE,ref data))
             pressure = controller.decodeShortValue(data);
          else
             pressure = 0.0f;

          this.model.readingData = temperature.ToString() +"|"+ humidity.ToString() + "|" + pressure.ToString();
                
          this.sensorData = JsonConvert.SerializeObject(this.model);

        }
    }
}

