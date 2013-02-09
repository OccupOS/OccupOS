using System;
using System.Collections;
using System.IO;
using OccupOSNode.Sensors;

namespace OccupOSNode {
    class StorageDeviceMissingException : Exception {
        public StorageDeviceMissingException(string message)
            : base(message) { }
    }

    class NodeController {
        ArrayList sensors;
        ArrayList sensorReadings;

        public NodeController() {
            var sensors = new ArrayList();

            var rootDirectory = new DirectoryInfo(@"\SD\");
            if (rootDirectory.Exists) 
            {
                LoadConfiguration();
            }
            else 
            {
                throw new StorageDeviceMissingException("Couldn't find a connected SD card.");
            }
        }

        private void LoadConfiguration() { throw new NotImplementedException(); }

        public void PollSensors()
        {
            foreach (object s in sensors)
            {
                if (s is Sensor) 
                {
                    sensorReadings.Add(((Sensor)s).Poll());
                }
            }
        }
    }
}
