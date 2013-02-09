using System;

namespace OccupOSNode.Sensors
{
    abstract class Sensor 
    {
        public string ID { get; private set; }

        protected Sensor(String id) 
        {
            ID = id;
        }

        public abstract String Poll();	
    }
}