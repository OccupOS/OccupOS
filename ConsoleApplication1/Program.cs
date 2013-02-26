using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OccupOSNode;
using System.Net.Sockets;
using System.Threading;
using Microsoft.WindowsAzure.Storage;

namespace OccupOSCloud
{
    class Program
    {
        static Listener l;

        static void Main(string[] args)
        {

            // This is the server part
          /*  l = new Listener(1333);
            l.SocketAccepted += new Listener.SocketAcceptedHandler(l_SocketAccepted);
            
            l.Start();
            */
          //  Console.Read();

            //testing the SQLServerHelper
            SensorDataTest testData = new SensorDataTest(1,1);
            testData.CreatedAt = DateTime.Now;
            testData.IntermediateHwMetadataId = 1;
            testData.MeasuredAt = DateTime.Now;
            testData.MeasuredData = "testData";
            testData.PolledAt = DateTime.Now;
            testData.SendAt = DateTime.Now;
            testData.UpdatedAt = DateTime.Now;
            SQLServerHelper helper = new SQLServerHelper("tcp:dndo40zalb.database.windows.net,1433", "comp2014@dndo40zalb", "20041908kjH", "TestSQLDB");
            helper.insertSensorData(1, 1, "test", DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now);

        }
        static void l_SocketAccepted(System.Net.Sockets.Socket e)
        {
            Client client = new Client(e);
            client.Received += new Client.ClientReceivedHandler(client_Received);
            client.Disconnected += new Client.ClientDisconnectedHandler(client_Disconnected);   
        }

        static void client_Disconnected(Client sender)
        {
        }

        static void client_Received(Client sender, byte[] data)
        {
            Console.WriteLine("Message from {0}: {1}",sender.ID, Encoding.Default.GetString(data));
        }
    }
}
