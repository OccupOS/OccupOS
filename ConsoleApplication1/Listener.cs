using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace OccupOSNode
{
    class Listener
    {
        Socket s;

        public bool Listening
        {
            get;
            private set;
        }

        public int Port
        {
            get;
            private set;
        }

        public Listener(int port)
        {
            Port = port;
            s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start()
        {
            if (Listening)
                return;
            s.Bind(new IPEndPoint(IPAddress.Any, Port));
            s.Listen(0);
            s.BeginAccept(callback, null);
            Listening = true;
        }

        public void Stop()
        {
            if (!Listening)
                return;
            s.Close();
            s.Dispose();
            s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        void callback(IAsyncResult ar)
        {
            try
            {
                Socket s = this.s.EndAccept(ar);
                if (SocketAccepted != null)
                    SocketAccepted(s);
                this.s.BeginAccept(callback,null);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public delegate void SocketAcceptedHandler(Socket e);
        public event SocketAcceptedHandler SocketAccepted;
    }
}
