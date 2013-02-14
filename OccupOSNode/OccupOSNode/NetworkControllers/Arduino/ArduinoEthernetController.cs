using System.Net;
using System.Net.Sockets;
using System.Text;

namespace OccupOSNode.NetworkControllers.Arduino
{
    class ArduinoEthernetController
    {
        private Socket socket;
        public ArduinoEthernetController(string hostName, int port)
        {
            IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
            IPAddress hostAddress = hostEntry.AddressList[0];
            IPEndPoint remoteEndPoint = new IPEndPoint(hostAddress, port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(remoteEndPoint);
            socket.SetSocketOption(SocketOptionLevel.Tcp,
            SocketOptionName.NoDelay, true);
            socket.SendTimeout = 5000;
        }

        public void sendData(string data)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            socket.Send(buffer);
        }

    }
}
