using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OccupOSNode {
    class Program { 
        static void Main(string[] args) {
            //Kinect testing code - temporary
            OccupOSNode.Sensors.Kinect.NodeKinectSensor testsensor
                = new OccupOSNode.Sensors.Kinect.NodeKinectSensor("testsensor");
            while (true) {
                //Console.WriteLine(testsensor.GetEntityCount());
                testsensor.GetEntityPositions();
            }
        }
    }
}
