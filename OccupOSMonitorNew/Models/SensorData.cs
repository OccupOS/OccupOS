using System;
using System.Text;
using System.Collections.Generic;


namespace OccupOSMonitorNew.Models
{

    public class SensorData {
        public int Id { get; set; }
      //  public  SensorMetadata SensorMetadata { get; set; }
     //   public  HwControllerMetadata HwControllerMetadata { get; set; }
        public  int SensorMetadataId { get; set; }
        public  int IntermediateHwMedadataId { get; set; }
        public  string MeasuredData { get; set; }
        public  System.DateTime MeasuredAt { get; set; }
        public  System.Nullable<System.DateTime> SendAt { get; set; }
        public  System.Nullable<System.DateTime> PolledAt { get; set; }
        public  System.DateTime UpdatedAt { get; set; }
        public  System.DateTime CreatedAt { get; set; }
    }
}
