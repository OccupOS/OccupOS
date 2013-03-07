using ServiceStack.Common.Web;
using ServiceStack.OrmLite;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using OccupOSMonitorNew.Models;
using System.Data.SqlClient;

namespace OccupOSMonitorNew
{
    [Route("/SensorData")]
    [Route("/SensorData/{Id}")]
    public class SensorDataReq
    {
        public int Id { get; set; }
    }
    
    public class SensorDataResp
    {
        public int Id {get;set;}
        public int SensorMetadataId { get; set; }
        public int IntermediateHwMedadataId { get; set; }
        public string MeasuredData { get; set; }
        public System.DateTime MeasuredAt { get; set; }
    }

    public class SensorDataService : Service
    {
        List<SensorDataResp> responses =  new List<SensorDataResp>() { new SensorDataResp { Id = 1, IntermediateHwMedadataId = 1, MeasuredAt = DateTime.Now, MeasuredData = "test1", SensorMetadataId = 1 }, new SensorDataResp { Id = 4, IntermediateHwMedadataId = 2, MeasuredAt = DateTime.Now, MeasuredData = "test2", SensorMetadataId = 2 } };
        List<SensorData> resp;
        public object Get(SensorDataReq request)
        {
            OrmLiteConfig.DialectProvider = SqlServerDialect.Provider;
            var connectionStringb = new SqlConnectionStringBuilder
            {
                DataSource =
                    "tcp:dndo40zalb.database.windows.net,1433",
                Encrypt = true,
                Password = "20041908kjH",
                UserID = "comp2014@dndo40zalb",
                InitialCatalog = "TestSQLDB",
                TrustServerCertificate = false
            };
         //All db access now uses the above dialect provider
            var dbFactory = new OrmLiteConnectionFactory(
    "Data Source=tcp:dndo40zalb.database.windows.net,1433;Initial Catalog=TestSQLDB;User ID=comp2014@dndo40zalb;Password=20041908kjH;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;",  //Connection String
    SqlServerDialect.Provider);
            using (IDbConnection db = dbFactory.OpenDbConnection())
          {
              if (request.Id > 0)
              {
                  resp = db.Select<SensorData>();
                  var response = resp.Where<SensorData>(x => x.SensorMetadataId == request.Id);
                  return new HttpResult(response,ContentType.Json);
              }
              else
              {
                  resp = db.Select<SensorData>();
                  return new HttpResult(resp, ContentType.Json);
              }
          }
        }
    }

}