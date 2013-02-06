using System;
using Microsoft.SPOT;

namespace OccupOSNode
{
    class PackageManager
    {
        Sensor[] sensors;
        String[] sensorReadings;
        int numberOfSensors;
        public PackageManager(int numberOfSensors)
        {
            this.numberOfSensors = numberOfSensors;
             sensors = new Sensor[numberOfSensors];
             sensorReadings = new String[numberOfSensors];
        }

        public void pollSensors()
        {
            for (int i = 0; i < numberOfSensors;i++ )
            {
                sensors[i].poll();
                sensorReadings[i] = sensors[i].getPackage();
            }
        }
    }
}
