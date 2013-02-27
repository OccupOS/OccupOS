using System;
using System.Text;
using System.Collections.Generic;


namespace OccupOSMonitor.Models {

    public class SensorData : Entity {
        public virtual SensorMetadata SensorMetadata { get; set; }
        public virtual HwControllerMetadata HwControllerMetadata { get; set; }
        public virtual int SensorMetadataId { get; set; }
        public virtual int IntermediateHwMedadataId { get; set; }
        public virtual string MeasuredData { get; set; }
        public virtual System.DateTime MeasuredAt { get; set; }
        public virtual System.Nullable<System.DateTime> SendAt { get; set; }
        public virtual System.Nullable<System.DateTime> PolledAt { get; set; }
        public virtual System.DateTime UpdatedAt { get; set; }
        public virtual System.DateTime CreatedAt { get; set; }
    }
}
