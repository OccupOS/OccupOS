using System;
using Microsoft.Kinect;
using OccupOS.CommonLibrary.Sensors;

/*===========================================================================================
 * NOTE: The KinectSensor class is not designed to work with the .NET Micro Framework!
 * When building for the Netduino you should not include this class or there will be errors.
 ============================================================================================*/

namespace OccupOSNode.Sensors.Kinect {
    internal class NodeKinectSensor : Sensor, ISoundSensor, IEntityPositionSensor, IEntityCountSensor
    {
        private KinectSensor ksensor;

        public NodeKinectSensor(String id) : base(id) {
            //unfinished, no stop conditions or polling
            initializeKinect(); //temp: single init attempt
        }

        public override string Poll() {
            throw new NotImplementedException();
        }

        public int GetEntityCount() {
            throw new NotImplementedException();
        }

        public Position[] GetEntityPositions() {
            throw new NotImplementedException();
        }

        public void initializeKinect() {
            if (KinectSensor.KinectSensors.Count > 0) {
                ksensor = KinectSensor.KinectSensors[0];

                if (ksensor != null && ksensor.Status == KinectStatus.Connected) {
                    ksensor.ColorStream.Enable();
                    ksensor.DepthStream.Enable();
                    var tsparams = new TransformSmoothParameters {
                        Smoothing = 0.2f,
                        Correction = 0.6f,
                        Prediction = 0.0f,
                        JitterRadius = 0.5f,
                        MaxDeviationRadius = 0.05f
                    }; //default params: 0.5f, 0.5f, 0.0f, 0.05f, 0.04f
                    ksensor.SkeletonStream.Enable(tsparams);
                    //ksensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(ksensor_AllFramesReady);
                    ksensor.Start();
                }
            }
        }

        /*Replace Event model with polling model:
        void ksensor_AllFramesReady(object sender, AllFramesReadyEventArgs e) {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame()) {
                if (depthFrame == null || skeletonFrame == null) {
                    return;
                }
                int[] playerdata = GetPlayerData(depthFrame, skeletonFrame);
            }
        }*/

        private int[] GetPlayerPosition(DepthImageFrame depthFrame, SkeletonFrame skeletonFrame) {
            Skeleton[] allskeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
            int[] pointdata = new int[skeletonFrame.SkeletonArrayLength * 3];
            skeletonFrame.CopySkeletonDataTo(allskeletons);
            int k = 0;

            foreach (Skeleton skeleton in allskeletons) {
                if (skeleton.TrackingState == SkeletonTrackingState.Tracked) {
                    Joint jointpoint = skeleton.Joints[JointType.Head];
                    if (jointpoint.TrackingState == JointTrackingState.Tracked) { //try other points?
                        var depthImagePoint = depthFrame.MapFromSkeletonPoint(jointpoint.Position);
                        pointdata[k] = depthImagePoint.Depth;
                        pointdata[k + 1] = depthImagePoint.X;
                        pointdata[k + 2] = depthImagePoint.Y;
                    } else {
                        pointdata[k] = 0;
                        pointdata[k + 1] = 0;
                        pointdata[k + 2] = 0;
                    }
                } else {
                    pointdata[k] = 0;
                    pointdata[k + 1] = 0;
                    pointdata[k + 2] = 0;
                }
                k += 3;
            }
            return pointdata;
        }

        private int GetSkeletonCount(SkeletonFrame skeletonFrame) {
            int count = 0;
            Skeleton[] allskeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
            skeletonFrame.CopySkeletonDataTo(allskeletons);
            foreach (Skeleton skeleton in allskeletons) {
                if (skeleton.TrackingState == SkeletonTrackingState.Tracked) count++;
            }
            return count;
        }

        public void StopSensor(KinectSensor sensor) {
            if (sensor != null) {
                sensor.Stop();
            }
        }
    }
}
