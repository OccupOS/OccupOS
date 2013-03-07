using System;
using System.Text;
using System.Collections.Generic;


namespace OccupOSMonitorNew.Models
{

    public class SensorMetadata {
        public SensorMetadata() {
            SensorDatas = new List<SensorData>();
        }
        public  HwControllerMetadata HwControllerMetadata { get; set; }
        public  string ExternalId { get; set; }
        public  string SensorName { get; set; }
        public  string RoomId { get; set; }
        public  System.Nullable<int> FloorNr { get; set; }
        public  System.Nullable<decimal> GeoLongitude { get; set; }
        public  System.Nullable<decimal> GeoLatidude { get; set; }
        public  System.DateTime UpdatedAt { get; set; }
        public  System.DateTime CreatedAt { get; set; }
        public  System.Nullable<int> UpdaterId { get; set; }
        public  System.Nullable<int> CreatorId { get; set; }
        public  int HwControllerMetadataId { get; set; }
        public  IList<SensorData> SensorDatas { get; set; }
    }
}
