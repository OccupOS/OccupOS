using System;
using System.Collections;
using System.IO;

namespace OccupOSNode {
    class StorageDeviceMissingException : Exception {
        public StorageDeviceMissingException(string message)
            : base(message) { }
    }

    class PackageManager {
        ArrayList sensors;
        ArrayList sensorReadings;

        public PackageManager() {
            ArrayList sensors = new ArrayList();

            var rootDirectory = new DirectoryInfo(@"\SD\");
            if (rootDirectory.Exists) {
                LoadConfiguration();
            }
            else {
                throw new StorageDeviceMissingException("Couldn't find a connected SD card.");
            }
        }

        private void LoadConfiguration() { throw new NotImplementedException(); }

        public void pollSensors() {
            for (int i = 0; i < sensors.Count; i++) {
                if (sensors[i] is Sensor) {
                    sensorReadings.Add(((Sensor)sensors[i]).poll());
                }
            }
        }
    }
}
