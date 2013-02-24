using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OccupOSNode;
using System.Net.Sockets;
using System.Threading;

namespace OccupOSNode
{
    class Program
    {
        static Listener l;

        static void Main(string[] args)
        {
          /*  l = new Listener(1333);
            l.SocketAccepted += new Listener.SocketAcceptedHandler(l_SocketAccepted);
            
            l.Start();
            */
            Console.Read();



            SQLServerHelper helper = new SQLServerHelper("dndo40zalb.database.windows.net", "comp2014", "20041908kjH", "TestSQLDB");
          if(helper.sendSensorData(1, 1, 1, "test", DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now)>0)
              Console.WriteLine("Updated successfully");
          else
              Console.Write("Error");
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
