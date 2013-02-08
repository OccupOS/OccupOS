using System;
using Microsoft.SPOT;
using System.Collections;

namespace OccupOSNode
{
    class PackageManager
    {

        ArrayList sensors;
        ArrayList sensorReadings;

        public PackageManager(int numberOfSensors)
        {
            sensors = new ArrayList();
        }

        public void pollSensors()
        {
            for (int i = 0; i < sensors.Count; i++)
            {
                if (sensors[i] is Sensor)
                {
                    sensorReadings.Add(((Sensor)sensors[i]).poll());
                }
            }
        }
    }
}
