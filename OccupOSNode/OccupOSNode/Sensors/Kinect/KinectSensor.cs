using System;

namespace OccupOSNode.Sensors.Kinect {
    internal class KinectSensor : Sensor, ISoundSensor, IEntityPositionSensor, IEntityCountSensor
    {
        private KinectSensor ksensor;

        public KinectSensor(String id) : base(id)
        {

        }

        public override string Poll()
        {
            throw new NotImplementedException();
        }

        public int GetEntityCount()
        {
            throw new NotImplementedException();
        }

        public Position[] GetEntityPositions()
        {
            throw new NotImplementedException();
        }
    }
}
