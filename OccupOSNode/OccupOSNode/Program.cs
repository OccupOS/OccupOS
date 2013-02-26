using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OccupOSCloud;

namespace OccupOSNode {
    class Program { 
        static void Main(string[] args) {
            KinectRunner kinectrunner = new KinectRunner();
            Thread kthread = new Thread(new ThreadStart(kinectrunner.DelayedPoll));
            kthread.Start();
        }
    }

    public class KinectRunner {

        public void DelayedPoll() {
            OccupOSNode.Sensors.Kinect.NodeKinectSensor testsensor
                = new OccupOSNode.Sensors.Kinect.NodeKinectSensor("testsensor");

            while (true) {
                System.Threading.Thread.Sleep(5000);
                Console.WriteLine("Poll");
                SQLServerHelper test = new SQLServerHelper("tcp:dndo40zalb.database.windows.net,1433", "comp2014@dndo40zalb", "20041908kjH", "TestSQLDB");
                test.insertSensorData(1, 1, (testsensor.GetEntityCount().ToString()), DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now);
                //test.insertSensorData(1, 1, "5", DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now);
            }
        }
    }
}
