using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace OccupOSNode.Micro
{
    class TestServer
    {
        Socket socket;
        public TestServer(string hostName, int port)
        {
            socket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
            socket.Bind(new IPEndPoint(IPAddress.Any, port));
           // socket.Bind(new IPEndPoint(IPAddress.Parse(hostName),port));

        }
         void ListenForConnections()
        {
            while (true)
            {
                using (Socket newConnection = socket.Accept())
                {
                    using (NetworkStream stream = new NetworkStream(newConnection))
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string data = reader.ReadLine();

                    }
                }
            }
        }

         public void start()
         {
             socket.Listen(10);
             Thread listener = new Thread(new ThreadStart(ListenForConnections));
             listener.Start();
         }
    }
}
