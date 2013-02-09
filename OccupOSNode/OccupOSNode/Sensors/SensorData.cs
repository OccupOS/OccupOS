namespace OccupOSNode.Sensors
{
    public struct Position
    {
        public int X;
        public int Y;
        public float Depth;
    }

    class SensorData
    {
        public float Humidity;
        public float Pressure;
        public float Temperature;

        public float AnalogLight;
    }
}