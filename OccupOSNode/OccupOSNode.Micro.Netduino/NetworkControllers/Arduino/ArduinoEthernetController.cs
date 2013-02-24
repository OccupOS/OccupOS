using System.Net;
using System.Net.Sockets;
using System.Text;

namespace OccupOSNode.Micro.NetworkControllers.Arduino
{
   public class ArduinoEthernetController
    {
        
        private Socket socket;
        IPHostEntry hostEntry; 
        IPAddress hostAddress;
        IPEndPoint remoteEndPoint;
        string address;
        public ArduinoEthernetController(string hostName, int port)
        {
            address = hostName;
           //hostEntry = Dns.GetHostEntry(hostName);
           //hostAddress = hostEntry.AddressList[0];
            hostAddress = IPAddress.Parse(hostName); 
            remoteEndPoint = new IPEndPoint(hostAddress, port);
           
            

        }

        public Socket connect()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
           socket.Connect(remoteEndPoint);
            socket.SetSocketOption(SocketOptionLevel.Tcp,
            SocketOptionName.NoDelay, true);
            socket.SendTimeout = 5000;
            return socket;
        }
        public int sendData(string data)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(data);
           return socket.Send(buffer);
        }

    }
}
