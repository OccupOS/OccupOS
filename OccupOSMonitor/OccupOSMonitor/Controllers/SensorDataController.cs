using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OccupOSMonitor.Controllers {
    using System.Data.SqlClient;
    using System.Text;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using OccupOSMonitor.Models;

    public class SensorDataController : ApiController {
        // GET api/sensordata
        //public IEnumerable<string> Get() {
        //    return new string[] { "{value}", "value2" };
        //}

        public JObject Get() {
            var connectionStringb = new SqlConnectionStringBuilder {
                DataSource =
                    "tcp:dndo40zalb.database.windows.net,1433",
                Encrypt = true,
                Password = "20041908kjH",
                UserID = "comp2014@dndo40zalb",
                InitialCatalog = "TestSQLDB",
                TrustServerCertificate = false
            };

            //var sensors = new ArrayList();
            var sensors = new List<string>();
            int sensorId = 0;
            int c = 0;
            string measuredData = "";
            string measuredData2 = "";
            string measuredAt = "";
            using (SqlConnection connection = new SqlConnection(connectionStringb.ConnectionString)) {
                //Selects latest 6 senordatas from different sensors
                string queryString = string.Format("SELECT TOP 2 * FROM (SELECT SensorMetadataId, MeasuredData, MeasuredAt," +
                        "ROW_NUMBER() OVER (PARTITION BY SensorMetadataId ORDER BY MeasuredAt DESC) AS RowNumber FROM   SensorData) AS a" +
                        " WHERE a.RowNumber = 1");
                SqlCommand command = new SqlCommand(queryString, connection);
                StringBuilder errorMessages = new StringBuilder();
                try {
                    System.Diagnostics.Debug.WriteLine("Test2");
                    command.Connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read()) {
                        if (c == 0) {
                            sensorId = reader.GetInt32(0); // Weight int
                            measuredData = reader.GetString(1); // Name string
                            measuredAt = reader.GetDateTime(2).ToString("yyyy-MM-dd HH:mm:ss"); // Breed string
                            c++;
                        } else {
                            measuredData2 = reader.GetString(1);
                            break;
                        }

                    }
                } catch (SqlException ex) {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    return new JObject("error");
                }
            }

            var sensorData = new JObject(
                new JProperty(
                    "sensordata",
                    new JArray(
                        new JObject(
                            new JProperty("id", sensorId),
                            new JProperty("measuredData", measuredData),
                            new JProperty("measuredAt", measuredAt),
                             new JProperty("measuredData2", measuredData2)))));

            return sensorData;
        }

        // GET api/sensordata/5
        public JObject Get(int id) {

            var connectionStringb = new SqlConnectionStringBuilder {
                DataSource =
                    "tcp:dndo40zalb.database.windows.net,1433",
                Encrypt = true,
                Password = "20041908kjH",
                UserID = "comp2014@dndo40zalb",
                InitialCatalog = "TestSQLDB",
                TrustServerCertificate = false
            };

            //var sensors = new ArrayList();
            var sensors = new List<string>();
            int sensorId = 0;
            string measuredData = "";
            string measuredAt = "";
            using (SqlConnection connection = new SqlConnection(connectionStringb.ConnectionString)) {
                string queryString = string.Format("SELECT Id, MeasuredData, measuredAt FROM SensorData ORDER BY MeasuredAt DESC");
                SqlCommand command = new SqlCommand(queryString, connection);
                StringBuilder errorMessages = new StringBuilder();
                try {
                    System.Diagnostics.Debug.WriteLine("Test2");
                    command.Connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read()) {
                        sensorId = reader.GetInt32(0);    // Weight int
                        measuredData = reader.GetString(1);  // Name string
                        measuredAt = reader.GetDateTime(2).ToString();// Breed string
                        break;
                    }
                } catch (SqlException ex) {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    return new JObject("error");
                }
            }

            var sensorData = new JObject(
                new JProperty(
                    "sensordata",
                    new JObject(
                //new JProperty("id", sensorId),
                        new JProperty("measuredData", measuredData),
                        new JProperty("measuredAt", measuredAt))));

            return sensorData;
        }

        // POST api/sensordata
        public void Post([FromBody]string value) {
        }

        // PUT api/sensordata/5
        public void Put(int id, [FromBody]string value) {
        }

        // DELETE api/sensordata/5
        public void Delete(int id) {
        }
    }
}
