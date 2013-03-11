using System;
using System.Text;
using System.Collections.Generic;


namespace OccupOSMonitorNew.Models
{

    public class SensorDataTs {
        public  int SensorMetadataId { get; set; }
        public  int IntermediateHwMedadataId { get; set; }
        public  string MeasuredData { get; set; }
        public  System.DateTime MeasuredAt { get; set; }
    }
}
