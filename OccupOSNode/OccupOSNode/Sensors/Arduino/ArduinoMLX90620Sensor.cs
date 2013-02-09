using System;

namespace OccupOSNode.Sensors.Arduino
{
    class ArduinoMLX90620Sensor : Sensor, IEntityCountSensor
    {
        public ArduinoMLX90620Sensor(string id) : base(id)
        {
        }

        public override string Poll() 
        {
            throw new NotImplementedException();
        }

        public int GetEntityCount()
        {
            throw new NotImplementedException();
        }
    }
}
