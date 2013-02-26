using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace OccupOSCloud
{
   public class SensorDataTest : TableEntity
    {
        public SensorDataTest(int Id, int SensorMetadataId)
        {
            this.RowKey = Id.ToString();
            this.PartitionKey = SensorMetadataId.ToString();
        }
        public SensorDataTest() { }

        public int IntermediateHwMetadataId
        {
            get;
            set;
        }

        public string MeasuredData
        {
            get;
            set;
        }

        public DateTime MeasuredAt
        {
            get;
            set;
        }

        public DateTime SendAt
        {
            get;
            set;
        }

        public DateTime PolledAt
        {
            get;
            set;
        }

        public DateTime UpdatedAt
        {
            get;
            set;
        }

        public DateTime CreatedAt
        {
            get;
            set;
        }
    }
}
