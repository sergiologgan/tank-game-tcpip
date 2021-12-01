using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tanblocks.socket
{
    public class Client
    {
        public Socket Socket { get { return socket; } internal set { socket = value; } }
        public bool IsConnected { get; private set; }
        public delegate void EventReceive(string username, int instr, StringBuilder stringBuilder);
        public event EventReceive Receive;
        private (string username, bool owner, bool op) infoUsername;
        private Socket socket;
        private EndPoint ipServer;

        /// <summary>
        /// Representa quando o jogador do outro lado se conecta ao servidor
        /// </summary>
        /// <param name="username"></param>
        /// <param name="ipServer"></param>
        public Client(string username, IPEndPoint ipServer)
        {
            this.infoUsername = (username, false, false);
            this.ipServer = ipServer;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        /// <summary>
        /// Representa quando o jogador do outro lado se conecta ao servidor e é OP ou Owner
        /// </summary>
        /// <param name="username"></param>
        /// <param name="ipServer"></param>
        public Client(string username, IPEndPoint ipServer, bool isOwner, bool isOp)
        {
            infoUsername = (username, isOwner, isOp);
            this.ipServer = ipServer;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// Representa quando quer se conectar com o servidor
        /// </summary>
        public void Connect()
        {
            try
            {
                if (!IsConnected)
                {
                    byte[] buffer = new byte[2000];
                    socket.BeginConnect(ipServer, new AsyncCallback(OnConnect), null);
                    socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None,
                        ref ipServer, new AsyncCallback(OnReceive), buffer);
                    IsConnected = true;
                }
            }
            catch
            {
                IsConnected = false;
            }
        }
        /// <summary>
        /// Dá um override no connect principal
        /// </summary>
        /// <param name="asyncResult"></param>
        private void OnConnect(IAsyncResult asyncResult)
        {
            try
            {
                string json = JsonConvert.SerializeObject(infoUsername);
                byte[] buffer = Encoding.Default.GetBytes(json);
                socket.Send(buffer);
                socket.EndConnect(asyncResult);
            }
            catch { }
        }
        /// <summary>
        /// Representa quando recebe mensagem do servidor
        /// </summary>
        /// <param name="asyncResult"></param>
        private void OnReceive(IAsyncResult asyncResult)
        {
            try
            {
                socket.EndReceive(asyncResult);
                byte[] receiveData = new byte[1500];
                byte[] buffer = new byte[2000];
                if (Receive != null)
                {
                    receiveData = (byte[])asyncResult.AsyncState;
                    StringBuilder receivedMessage = new StringBuilder(Encoding.Default.GetString(receiveData).Length);
                    receivedMessage.Append(Encoding.Default.GetString(receiveData));
                    if (receivedMessage.Length > 0)
                    {
                        Regex rg = new Regex(@"\d+%");
                        Match m = rg.Match(receivedMessage.ToString());
                        if (m.Success)
                        {
                            Receive(infoUsername.username, int.Parse(m.Value.Replace("%", "")), receivedMessage.Replace(m.Value, ""));
                        }
                        else
                        {
                            Receive(infoUsername.username, -1, receivedMessage);
                        }
                    }
                }
                socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None,
                    ref ipServer, new AsyncCallback(OnReceive), buffer);
            }
            catch { throw; }
        }
        /// <summary>
        /// Envia mensagem para o servidor
        /// </summary>
        /// <param name="data"></param>
        public void SendToServer(string data)
        {
            socket.Send(Encoding.Default.GetBytes(data));
        }
        /// <summary>
        /// Envia objeto para o servidor
        /// </summary>
        /// <param name="obj"></param>
        public void SendObjectToServer(Object obj, int id)
        {
            string json = JsonConvert.SerializeObject(obj);
            json = json.Insert(0, $"{id}%");
            SendToServer(json);
        }
    }
}
