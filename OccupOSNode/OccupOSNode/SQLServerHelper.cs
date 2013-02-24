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


namespace OccupOSNode
{
   public class SQLServerHelper
    {
     //   private SqlDataReader queryResult;
      /*  private string userName = "";
        private string password = "";
        private string dataSource = "";
        private string databaseName = "";*/
        private SqlConnectionStringBuilder connectionStringb;
        private CloudStorageAccount account;
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

        public int connect()
        {
         //  account = CloudStorageAccount.Parse(connectionStringb.ConnectionString);
            string str = connectionStringb.ConnectionString;
           // Console.WriteLine(str);
           // Console.Read();
           // account = CloudStorageAccount.Parse(str); 
            if (account == null) return 0;
            return 1;
        }

        public int sendSensorData(int SensorMetadataId,int IntermediateHwMetadataId,string MeasuredData,DateTime MeasuredAt,DateTime SendAt,DateTime PolledAt,DateTime UpdatedAt,DateTime CreatedAt)
  // public void sendSensorData(SensorDataTest data)    
   {
          //  CloudTableClient tableClient = account.CreateCloudTableClient();
            //CloudTable sensorDataTable = tableClient.GetTableReference("SensorData");

            //TableOperation insertData = TableOperation.Insert(data);

//            sensorDataTable.Execute(insertData);
  //          Console.WriteLine("Entity inserted");
           using (SqlConnection connection = new SqlConnection(connectionStringb.ConnectionString))
            {
                string queryString = string.Format("INSERT INTO SensorData (SensorMetadataId, IntermediateHwMedadataId, MeasuredData, MeasuredAt, SendAt, PolledAt, UpdatedAt, CreatedAt) VALUES ('{0}','{1}','{2}',CONVERT(datetime,'{3}',102),'{4}','{5}','{6}','{7}');", SensorMetadataId, IntermediateHwMetadataId, MeasuredData, DateTime.Now.ToLongDateString(), SendAt.ToLongDateString(), PolledAt.ToLongDateString(), UpdatedAt.ToLongDateString(), CreatedAt.ToLongDateString()); 
                SqlCommand command = new SqlCommand(queryString,connection);
                StringBuilder errorMessages = new StringBuilder();

                try
                {
                    Console.WriteLine(DateTime.Now.ToString());
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                   
                    Console.Read();
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
                }
               return 1;
           /*     SqlCommandBuilder sqlCommand = new SqlCommandBuilder();
                sqlCommand.
                sqlCommand.Connection = connection;
                connection.Open();
                queryResult = sqlCommand.ExecuteReader();
           */
            }
        }
        
    }
}
