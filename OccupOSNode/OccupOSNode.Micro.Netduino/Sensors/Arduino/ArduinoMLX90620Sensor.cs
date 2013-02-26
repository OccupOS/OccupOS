namespace OccupOSNode.Micro.Sensors.Arduino {

using System;
using OccupOS.CommonLibrary.Sensors;

    class ArduinoMLX90620Sensor : Sensor, IEntityCountSensor
    {
        public ArduinoMLX90620Sensor(string id) : base(id)
        {
        }

        public override SensorData GetData() 
        {
            throw new NotImplementedException();
        }

        public int GetEntityCount()
        {
            throw new NotImplementedException();
        }
    }
}
