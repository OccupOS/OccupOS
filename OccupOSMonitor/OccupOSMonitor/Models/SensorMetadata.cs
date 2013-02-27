using System;
using System.Text;
using System.Collections.Generic;


namespace OccupOSMonitor.Models {

    public class SensorMetadata : Entity {
        public SensorMetadata() {
            SensorDatas = new List<SensorData>();
        }
        public virtual HwControllerMetadata HwControllerMetadata { get; set; }
        public virtual string ExternalId { get; set; }
        public virtual string SensorName { get; set; }
        public virtual string RoomId { get; set; }
        public virtual System.Nullable<int> FloorNr { get; set; }
        public virtual System.Nullable<decimal> GeoLongitude { get; set; }
        public virtual System.Nullable<decimal> GeoLatidude { get; set; }
        public virtual System.DateTime UpdatedAt { get; set; }
        public virtual System.DateTime CreatedAt { get; set; }
        public virtual System.Nullable<int> UpdaterId { get; set; }
        public virtual System.Nullable<int> CreatorId { get; set; }
        public virtual int HwControllerMetadataId { get; set; }
        public virtual IList<SensorData> SensorDatas { get; set; }
    }
}
