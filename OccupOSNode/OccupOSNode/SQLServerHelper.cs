using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Collections;

namespace OccupOSNode
{
    class SQLServerHelper
    {
        private SqlDataReader queryResult;
        private string userName = "";
        private string password = "";
        private string dataSource = "";
        private string databaseName = "";
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

        public void sendData(ArrayList data)
        {
            using (SqlConnection connection = new SqlConnection(connectionString.ConnectionString))
            {

                /*SqlCommandBuilder sqlCommand = new SqlCommandBuilder();
                sqlCommand.
                sqlCommand.Connection = connection;
                connection.Open();
                queryResult = sqlCommand.ExecuteReader();*/
           
            }
        }
        
    }
}
