using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Collections;

namespace OccupOSNode
{
   public class SQLServerHelper
    {
        private SqlDataReader queryResult;
      /*  private string userName = "";
        private string password = "";
        private string dataSource = "";
        private string databaseName = "";*/
        private SqlConnectionStringBuilder connectionString;

        public SQLServerHelper(String dataSource, string userName, string password, string databaseName)
        {
            connectionString = new SqlConnectionStringBuilder();
            connectionString.DataSource = dataSource;
            connectionString.Encrypt = true;
            connectionString.Password = password;
            connectionString.UserID = userName;
            connectionString.InitialCatalog = databaseName;
            connectionString.TrustServerCertificate = false;
        }

        public void getData(string command)
        {
            using (SqlConnection connection = new SqlConnection(connectionString.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand(command);
                sqlCommand.Connection = connection;
                connection.Open();
                queryResult = sqlCommand.ExecuteReader();
                while (queryResult.Read())
                {

                }
            }
        }

        public int sendSensorData(int Id,int SensorMetadataId,int IntermediateHwMetadataId,string MeasuredData,DateTime MeasuredAt,DateTime SendAt,DateTime PolledAt,DateTime UpdatedAt,DateTime CreatedAt)
        {
            using (SqlConnection connection = new SqlConnection(connectionString.ConnectionString))
            {
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = string.Format("INSERT INTO SensorData (Id, SensorMetadataId, IntermediateHwMetadataId, MeasuredData, MeasuredAt, SendAt, PolledAt, UpdatedAt, CreatedAt) VALUES ({0},{1},{2},{3},{4},{5},{6},{7},{8}", Id, SensorMetadataId, IntermediateHwMetadataId, MeasuredData, MeasuredAt, SendAt, PolledAt, UpdatedAt, CreatedAt);
                connection.Open();
                command.ExecuteNonQuery();
                return 1;
                /*SqlCommandBuilder sqlCommand = new SqlCommandBuilder();
                sqlCommand.
                sqlCommand.Connection = connection;
                connection.Open();
                queryResult = sqlCommand.ExecuteReader();*/
           
            }
        }
        
    }
}
