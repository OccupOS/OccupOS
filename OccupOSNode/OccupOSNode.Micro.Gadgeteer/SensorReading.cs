using System;
using Microsoft.Azure.Zumo.MicroFramework.Core;
using Microsoft.SPOT;

namespace OccupOSNode.Micro.Gadgeteer {
    public class SensorReading : IMobileServiceEntity {
        public int ID { get; set; }
        public string SensorID { get; set; }
        public double Light { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
