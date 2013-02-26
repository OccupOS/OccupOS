using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Collections;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Configuration;


namespace OccupOSCloud
{
   public class SQLServerHelper
    {
        private SqlConnectionStringBuilder connectionStringb;

        private string connectionString;

        public SQLServerHelper(String dataSource, string userName, string password, string databaseName)
        {
            connectionStringb = new SqlConnectionStringBuilder();
            connectionStringb.DataSource = dataSource;
            connectionStringb.Encrypt = true;
            connectionStringb.Password = password;
            connectionStringb.UserID = userName;
            connectionStringb.InitialCatalog = databaseName;
            connectionStringb.TrustServerCertificate = false;
        }

        public SQLServerHelper(String connectionString)
        {
            this.connectionString = connectionString;
        }

        public int insertAppUser(string Username,string Email, string Password, DateTime createdAt, DateTime updatedAt, int creatorId, int updaterId, string FirstName, string LastName)
        {
            using (SqlConnection connection = new SqlConnection(connectionStringb.ConnectionString))
            {
                string queryString = string.Format("INSERT INTO AppUser (Username, Email, Password, createdAt, updatedAt, creatorId, updaterId, FirstName, LastName) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}');", Username, Email, Password, updatedAt.ToLongDateString() + " " + updatedAt.ToLongTimeString(), createdAt.ToLongDateString() + " " + createdAt.ToLongTimeString(), creatorId, updaterId, FirstName, LastName);
                SqlCommand command = new SqlCommand(queryString, connection);
                StringBuilder errorMessages = new StringBuilder();

                try
                {
                    command.Connection.Open();
                    int res = command.ExecuteNonQuery();
                    command.Connection.Close();
                    return res;
                }
                catch (SqlException ex)
                {
                    for (int i = 0; i < ex.Errors.Count; i++)
                    {
                        errorMessages.Append("Index #" + i + "\n" +
                            "Message: " + ex.Errors[i].Message + "\n" +
                            "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                            "Source: " + ex.Errors[i].Source + "\n" +
                            "Procedure: " + ex.Errors[i].Procedure + "\n");
                    }
                    Console.WriteLine(errorMessages.ToString());
                    Console.Read();
                    return 0;
                }

            }
        }

        public int insertSensorData(int SensorMetadataId,int IntermediateHwMetadataId,string MeasuredData,DateTime MeasuredAt,DateTime SendAt,DateTime PolledAt,DateTime UpdatedAt,DateTime CreatedAt)
        {
         
           using (SqlConnection connection = new SqlConnection(connectionStringb.ConnectionString))
            {
                string queryString = string.Format("INSERT INTO SensorData (SensorMetadataId, IntermediateHwMedadataId, MeasuredData, MeasuredAt, SendAt, PolledAt, UpdatedAt, CreatedAt) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}');", SensorMetadataId, IntermediateHwMetadataId, MeasuredData,MeasuredAt.ToLongDateString()+" "+MeasuredAt.ToLongTimeString() , SendAt.ToLongDateString()+" "+SendAt.ToLongTimeString(), PolledAt.ToLongDateString()+" "+PolledAt.ToLongTimeString(), UpdatedAt.ToLongDateString()+" "+UpdatedAt.ToLongTimeString(), CreatedAt.ToLongDateString()+" "+CreatedAt.ToLongTimeString()); 
                SqlCommand command = new SqlCommand(queryString,connection);
                StringBuilder errorMessages = new StringBuilder();

                try
                {
                     command.Connection.Open(); 
                     int res = command.ExecuteNonQuery();
                     return res;
                }
                catch (SqlException ex)
                {
                    for (int i = 0; i < ex.Errors.Count; i++)
                    {
                        errorMessages.Append("Index #" + i + "\n" +
                            "Message: " + ex.Errors[i].Message + "\n" +
                            "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                            "Source: " + ex.Errors[i].Source + "\n" +
                            "Procedure: " + ex.Errors[i].Procedure + "\n");
                    }
                    Console.WriteLine(errorMessages.ToString());
                    Console.Read();
                    return 0;
                }
             
            }
        }

        public int insertControllerMetadata(string ExternalId, string DepartmentName, string BuildingName, string RoomId, int FloorNr,  DateTime UpdatedAt, DateTime CreatedAt, int UpdaterId, int CreatorId)
        {
            using (SqlConnection connection = new SqlConnection(connectionStringb.ConnectionString))
            {
                string queryString = string.Format("INSERT INTO SensorMetadata (ExternalId, DepartmentName, BuildingName, RoomId, FloorNr,  UpdatedAt, CreatedAt, UpdaterId, CreatorId) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}');", ExternalId, DepartmentName, BuildingName, RoomId, FloorNr, UpdatedAt.ToLongDateString() + " " + UpdatedAt.ToLongTimeString(), CreatedAt.ToLongDateString() + " " + CreatedAt.ToLongTimeString(), UpdaterId, CreatorId);
                SqlCommand command = new SqlCommand(queryString, connection);
                StringBuilder errorMessages = new StringBuilder();

                try
                {
                    command.Connection.Open();
                    int res = command.ExecuteNonQuery();
                    command.Connection.Close();
                    return res;
                }
                catch (SqlException ex)
                {
                    for (int i = 0; i < ex.Errors.Count; i++)
                    {
                        errorMessages.Append("Index #" + i + "\n" +
                            "Message: " + ex.Errors[i].Message + "\n" +
                            "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                            "Source: " + ex.Errors[i].Source + "\n" +
                            "Procedure: " + ex.Errors[i].Procedure + "\n");
                    }
                    Console.WriteLine(errorMessages.ToString());
                    Console.Read();
                    return 0;
                }

            }
        }

        public int insertSensorMetadata(string ExternalId, string SensorName, string RoomId, int FloorNr, int GeoLongitude, int GeoLatidude, DateTime UpdatedAt, DateTime CreatedAt, int UpdaterId, int CreatorId, int HwControllerMetadataId)
        {
            using (SqlConnection connection = new SqlConnection(connectionStringb.ConnectionString))
            {
                string queryString = string.Format("INSERT INTO SensorMetadata (ExternalId, SensorName, RoomId, FloorNr, GeoLongitude, GeoLatidude, UpdatedAt, CreatedAt, UpdaterId, CreatorId, HwControllerMetadataId) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}');", ExternalId, SensorName, RoomId, FloorNr,GeoLongitude, GeoLatidude, UpdatedAt.ToLongDateString() + " " + UpdatedAt.ToLongTimeString(), CreatedAt.ToLongDateString() + " " + CreatedAt.ToLongTimeString(), UpdaterId, CreatorId, HwControllerMetadataId);
                SqlCommand command = new SqlCommand(queryString, connection);
                StringBuilder errorMessages = new StringBuilder();

                try
                {
                    command.Connection.Open();
                    int res = command.ExecuteNonQuery();
                    command.Connection.Close();
                    return res;
                }
                catch (SqlException ex)
                {
                    for (int i = 0; i < ex.Errors.Count; i++)
                    {
                        errorMessages.Append("Index #" + i + "\n" +
                            "Message: " + ex.Errors[i].Message + "\n" +
                            "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                            "Source: " + ex.Errors[i].Source + "\n" +
                            "Procedure: " + ex.Errors[i].Procedure + "\n");
                    }
                    Console.WriteLine(errorMessages.ToString());
                    Console.Read();
                    return 0;
                }

            }
        }

        public void inserDataIntoStorage(SensorDataTest data)
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = account.CreateCloudTableClient();
            CloudTable sensorDataTable = tableClient.GetTableReference("SensorData");

            TableOperation insertData = TableOperation.Insert(data);

            sensorDataTable.Execute(insertData);
            Console.WriteLine("Entity inserted");
        }
        
    }
}
