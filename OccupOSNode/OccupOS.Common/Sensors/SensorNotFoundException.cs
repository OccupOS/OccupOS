using System;
using Microsoft.SPOT;

namespace OccupOS.CommonLibrary.Sensors {

    //[Serializable]
    public class SensorNotFoundException : System.Exception {
        public SensorNotFoundException() {}
        public SensorNotFoundException(String msg) : base(msg) {}
        public SensorNotFoundException(string message, System.Exception inner) : base(message, inner) {}

        /* No System.Runtime.Serialization for micro framework?
        protected SensorNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) {} */
    }
}
