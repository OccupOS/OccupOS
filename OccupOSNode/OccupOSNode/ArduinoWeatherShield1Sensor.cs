using IndianaJones.NETMF.Json;
using System;

namespace OccupOSNode {

    class ArduinoWeatherShield1Sensor : Sensor {

        private  ArduinoWeatherShield1Controller controller;
        private float temperature, humidity, pressure;
        private byte[] data;

        public ArduinoWeatherShield1Sensor(String id) : base(id) {
            controller = new ArduinoWeatherShield1Controller();
            data = new byte[4];
        }

        public override String poll() {

            if (controller.sendCommand(ArduinoWeatherShield1Controller.CMD_GETTEMP_C_RAW, ArduinoWeatherShield1Controller.PAR_GET_LAST_SAMPLE, ref data))
                temperature = controller.decodeShortValue(data);
            else
                temperature = 0.0f;
            if (controller.sendCommand(ArduinoWeatherShield1Controller.CMD_GETHUM_RAW, ArduinoWeatherShield1Controller.PAR_GET_LAST_SAMPLE, ref data))
                humidity = controller.decodeShortValue(data);
            else
                humidity = 0.0f;
            if (controller.sendCommand(ArduinoWeatherShield1Controller.CMD_GETPRESS_RAW, ArduinoWeatherShield1Controller.PAR_GET_LAST_SAMPLE,ref data))
                pressure = controller.decodeShortValue(data);
            else
                pressure = 0.0f;

            this.model.readingData = temperature.ToString() +"|"+ humidity.ToString() + "|" + pressure.ToString();

            Serializer jsonSerializer = new Serializer();
            return jsonSerializer.Serialize(this.model);
        }
    }
}

