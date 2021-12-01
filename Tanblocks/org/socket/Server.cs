using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Tanblocks.socket
{
    public class Server
    {
        public IPEndPoint ipServer { get { return endPoint; } }
        public bool IsRunning { get; private set; }
        public delegate void EventConnect(string username, Socket socket);
        public event EventConnect Connect;
        private Socket socket;
        private IPEndPoint endPoint;
        /// <summary>
        /// Construtor inicial do servidor
        /// </summary>
        /// <param name="endPoint">Ip do servidor</param>
        public Server(IPEndPoint endPoint)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.endPoint = endPoint;
        }
        /// <summary>
        /// Destinado a iniciar o servidor
        /// </summary>
        public void Start()
        {
            if (IsRunning) return;
            socket.Bind(endPoint);
            socket.Listen(0);
            socket.BeginAccept(socket.ReceiveBufferSize, OnConnect, null);
            this.IsRunning = true;
        }
        /// <summary>
        /// Destinado a parar o servidor
        /// </summary>
        public void Stop()
        {
            if (!IsRunning) return;
            socket.Close();
            socket.Dispose();
            this.IsRunning = false;
        }
        /// <summary>
        /// Representa quando o cliente se conecta ao servidor
        /// </summary>
        /// <param name="asyncResult"></param>
        private void OnConnect(IAsyncResult asyncResult)
        {
            try
            {
                byte[] buffer = new byte[this.socket.ReceiveBufferSize];
                Socket socket = this.socket.EndAccept(out buffer, asyncResult);

                if (Connect != null)
                {
                    string username = Encoding.Default.GetString(buffer);
                    Connect(username, socket);
                }
                this.socket.BeginAccept(this.socket.ReceiveBufferSize, OnConnect, null);
            }
            catch { }
        }
    }
}
