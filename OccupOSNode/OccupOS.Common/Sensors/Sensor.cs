using System;

namespace OccupOS.CommonLibrary.Sensors
{
    public abstract class Sensor 
    {
        public string ID { get; private set; }

        protected Sensor(String id) 
        {
            ID = id;
        }

        public abstract SensorData GetData();
    }
}