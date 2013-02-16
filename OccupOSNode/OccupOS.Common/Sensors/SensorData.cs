namespace OccupOS.CommonLibrary.Sensors
{
    public struct Position
    {
        public int X;
        public int Y;
        public float Depth;
    }

    public class SensorData
    {
        public float Humidity;
        public float Pressure;
        public float Temperature;

        public float AnalogLight;
    }
}