using System.Collections.Generic;
using System.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate;
using NHibernate.Cfg;

namespace OccupOSMonitor.Controllers {
    using System.Collections;
    using System.Data.SqlClient;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Web;

    using NHibernate.Cfg.MappingSchema;
    using NHibernate.Mapping.ByCode;

    using OccupOSMonitor.Models;

    public class ValuesController : ApiController {

        ISessionFactory sessionFactory;

        /// <summary>
        /// Method to create session and manage entities
        /// </summary>
        /// <returns></returns>
        ISession OpenSession() {
            if (sessionFactory == null) {
                var cfg = new Configuration();
                var data = cfg.Configure(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hibernate.cfg.xml"));
                var mapper = new ModelMapper();
                mapper.AddMappings(Assembly.GetExecutingAssembly().GetExportedTypes());
                HbmMapping domainMapping = mapper.CompileMappingForAllExplicitlyAddedEntities();
                data.AddMapping(domainMapping);
                //cfg.AddDirectory(new System.IO.DirectoryInfo(HttpContext.Current.Server.MapPath(@"Models\NHibernate\Mappings")));
                this.sessionFactory = data.BuildSessionFactory();
            }

            return this.sessionFactory.OpenSession();
        }

        // GET api/values
        public IEnumerable<string> Get() {
            return new string[] { "value3", "value9" };
        }

        // GET api/values/5
        //[NhSessionManagement]
        public string Get(int id) {
            //Working for Azure!
            //System.Diagnostics.Debug.WriteLine("Test");
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
            using (SqlConnection connection = new SqlConnection(connectionStringb.ConnectionString)) {
                string queryString = string.Format("SELECT MeasuredData FROM SensorData ORDER BY MeasuredAt DESC");
                SqlCommand command = new SqlCommand(queryString, connection);
                StringBuilder errorMessages = new StringBuilder();
                try {
                    System.Diagnostics.Debug.WriteLine("Test2");
                    command.Connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read()) {
                        //int senorId = reader.GetInt32(0);    // Weight int
                        string sensorData = reader.GetString(0);  // Name string
                        //string breed = reader.GetString(2); // Breed string
                        sensors.Add(sensorData);
                    }
                } catch (SqlException ex) {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    return "Error";
                }
            }
            /*IList<SensorData> sensorDatas;
            SensorData a;
            String measuredData;
            IList test;
            using (ISession session = OpenSession()) {
                //NHibernate query
                //a = session.Get<SensorData>(8);
                IQuery q = session.CreateQuery("from SensorData");
                test = q.List();
                //var test = q.SetResultTransformer(NHibernate.Transform.Transformers.AliasToEntityMap).List<Hashtable>();
                //IQuery query = session.CreateQuery("SELECT * FROM SensorData");
                //sensorDatas = query.List<SensorData>();
                //measuredData = (string)test[0]["MeasuredData"];
            }

            a = (SensorData)test[0];*/

            return sensors[0];
        }

        // POST api/values
        public void Post([FromBody]string value) {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value) {
        }

        // DELETE api/values/5
        public void Delete(int id) {
        }
    }
}