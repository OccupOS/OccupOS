namespace OccupOSNode.Sensors.Kinect {

    using System;
    using Microsoft.Kinect;
    using OccupOS.CommonLibrary.Sensors;
    using System.Collections;
    using IndianaJones.NETMF.Json;

/*===========================================================================================
 * NOTE: The KinectSensor class is not designed to work with the .NET Micro Framework!
 * When building for the Netduino you should not include this class.
 ============================================================================================*/

    internal class NodeKinectSensor : Sensor, ISoundSensor, IEntityPositionSensor, IEntityCountSensor
    {
        private struct SynchedFrames {
            public SkeletonFrame s_frame;
            public DepthImageFrame d_frame;

        }
        private KinectSensor ksensor;
        private static int QUEUE_MAX_LENGTH = 6;
        private static int MAX_TIME_DIFFERENCE = 200;

        public NodeKinectSensor(String id) : base(id) {
            //unfinished, no stop conditions or polling
            findKinect(); //temp: single init attempt
        }

        public override string GetDataAsJSON() {
            var sensorData = new SensorData {
                EntityCount = GetEntityCount(),
                EntityPositions = GetEntityPositions()
            };

            var jsonSerializer = new Serializer();
            return jsonSerializer.Serialize(sensorData);
        }

        public int GetEntityCount() {
            int count = 0;
            if (ksensor != null && ksensor.Status == KinectStatus.Connected) {
                using (SkeletonFrame skeletonFrame = ksensor.SkeletonStream.OpenNextFrame(1000)) {
                    if (skeletonFrame != null) {
                        count = CountSkeletons(skeletonFrame);
                    }
                }
            } else throw new Exception("Kinect sensor not found");
            return count;
        }

        public Position[] GetEntityPositions() {
            if (ksensor != null && ksensor.Status == KinectStatus.Connected) {
                SynchedFrames frames = PollSynchronizedFrames();
                if (frames.s_frame != null && frames.d_frame != null) {
                    int[] playerData = CalculatePlayerPositions(frames.s_frame, frames.d_frame);
                    Position[] entityPositions = new Position[frames.s_frame.SkeletonArrayLength];
                    for (int k = 0; k < entityPositions.Length; k++) {
                        entityPositions[k].X = playerData[k*3];
                        entityPositions[k].Y = playerData[(k*3)+1];
                        entityPositions[k].Depth = playerData[(k*3)+2];
                    }
                    return entityPositions;
                } else return null;
            } else throw new Exception("Kinect sensor not found");
        }

        public void findKinect() {
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

        private SynchedFrames PollSynchronizedFrames() {
            SynchedFrames frames = new SynchedFrames();
            Queue skeletonQueue = new Queue();
            Queue depthQueue = new Queue();
            long lowest = MAX_TIME_DIFFERENCE;
            for (int k = 0; k < QUEUE_MAX_LENGTH; k++) {
                skeletonQueue.Enqueue(ksensor.SkeletonStream.OpenNextFrame(100));
                depthQueue.Enqueue(ksensor.DepthStream.OpenNextFrame(100));
            }
            SkeletonFrame skeletonFrame = (SkeletonFrame)skeletonQueue.Dequeue();
                frames.s_frame = skeletonFrame;
                DepthImageFrame depthFrame = (DepthImageFrame)depthQueue.Dequeue();
                frames.d_frame = depthFrame;
            for (int l = 0; l < QUEUE_MAX_LENGTH - 1; l++) {
                if (skeletonFrame != null && depthFrame != null) {
                    long diff = Math.Abs(skeletonFrame.Timestamp - depthFrame.Timestamp);
                    if (diff == 0) break;
                    if (diff < lowest) {
                        lowest = diff;
                        frames.s_frame = skeletonFrame;
                        frames.d_frame = depthFrame;
                        if (skeletonFrame.Timestamp < depthFrame.Timestamp)
                            skeletonFrame = (SkeletonFrame)skeletonQueue.Dequeue();
                        else
                            depthFrame = (DepthImageFrame)depthQueue.Dequeue();
                    } else break;
                } else {
                    if (skeletonFrame == null) skeletonFrame = (SkeletonFrame)skeletonQueue.Dequeue();
                    if (depthFrame == null) depthFrame = (DepthImageFrame)depthQueue.Dequeue();
                }
            }
            return frames;
        }

        private int[] CalculatePlayerPositions(SkeletonFrame skeletonFrame, DepthImageFrame depthFrame) {
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

        private int CountSkeletons(SkeletonFrame skeletonFrame) {
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
