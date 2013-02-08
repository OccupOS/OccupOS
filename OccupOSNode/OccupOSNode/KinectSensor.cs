using System;
using Microsoft.SPOT;
using Microsoft.Kinect;

namespace OccupOSNode {
    class KinectSensor : Sensor {
        KinectSensor ksensor;

        public KinectSensor(String id) : base(id) {

        }
    }
}
