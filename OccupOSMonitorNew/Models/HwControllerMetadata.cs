using System;
using System.Text;
using System.Collections.Generic;


namespace OccupOSMonitorNew.Models
{

    public class HwControllerMetadata {
        public HwControllerMetadata() {
            SensorDatas = new List<SensorData>();
            SensorMetadatas = new List<SensorMetadata>();
        }
        public  string ExternalId { get; set; }
        public  string DepartmentName { get; set; }
        public  string BuildingName { get; set; }
        public  System.DateTime UpdatedAt { get; set; }
        public  System.DateTime CreatedAt { get; set; }
        public  System.Nullable<int> UpdaterId { get; set; }
        public  System.Nullable<int> CreatorId { get; set; }
        public  System.Nullable<int> FloorNr { get; set; }
        public  string RoomId { get; set; }
        public  IList<SensorData> SensorDatas { get; set; }
        public  IList<SensorMetadata> SensorMetadatas { get; set; }
    }
}
