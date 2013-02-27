using System;
using System.Text;
using System.Collections.Generic;


namespace OccupOSMonitor.Models {

    public class HwControllerMetadata : Entity {
        public HwControllerMetadata() {
            SensorDatas = new List<SensorData>();
            SensorMetadatas = new List<SensorMetadata>();
        }
        public virtual string ExternalId { get; set; }
        public virtual string DepartmentName { get; set; }
        public virtual string BuildingName { get; set; }
        public virtual System.DateTime UpdatedAt { get; set; }
        public virtual System.DateTime CreatedAt { get; set; }
        public virtual System.Nullable<int> UpdaterId { get; set; }
        public virtual System.Nullable<int> CreatorId { get; set; }
        public virtual System.Nullable<int> FloorNr { get; set; }
        public virtual string RoomId { get; set; }
        public virtual IList<SensorData> SensorDatas { get; set; }
        public virtual IList<SensorMetadata> SensorMetadatas { get; set; }
    }
}
