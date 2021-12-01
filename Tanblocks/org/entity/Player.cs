using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tanblocks.org.entity;

namespace Tanblocks.entity
{
    public class Player
    {
        private string username;
        private Socket socket;
        public delegate void EventReceive(string username, int instr, StringBuilder sbData);
        public delegate void EventDisconnect(string username, StringBuilder sbInfo);
        public event EventReceive Receive;
        public event EventDisconnect Disconnect;
        public string Username { get { return username; } }
        public bool IsOp { get; set; }
        public bool IsOwner { get; set; }
        public Tank tank { get; set; }

        public Player(string username, Socket socket)
        {
            this.username = username;
            this.socket = socket;
            socket.BeginReceive(new byte[] { 0 }, 0, 0, 0, OnReceive, null);
        }

        private void OnReceive(IAsyncResult asyncResult)
        {
            try
            {
                socket.EndReceive(asyncResult);
                if (Receive != null)
                {
                    byte[] buffer = new byte[2000];
                    socket.Receive(buffer, buffer.Length, 0);
                    StringBuilder receivedMessage = new StringBuilder(Encoding.Default.GetString(buffer).Length);
                    receivedMessage.Append(Encoding.Default.GetString(buffer));
                    if (receivedMessage.Length > 0)
                    {
                        Regex rg = new Regex(@"\d+%");
                        Match m = rg.Match(receivedMessage.ToString());
                        if (m.Success)
                        {
                            Receive(username, int.Parse(m.Value.Replace("%", "")), receivedMessage.Replace(m.Value, ""));
                        }
                        else
                        {
                            Receive(username, -1, receivedMessage);
                        }
                    }
                }
                socket.BeginReceive(new byte[] { 0 }, 0, 0, 0, OnReceive, null);
            }
            catch { throw; }
        }

        public void SendToPlayer(string msg)
        {
            socket.Send(Encoding.Default.GetBytes(msg));
        }

        public void SendObjectToPlayer(object obj, int id)
        {
            string json = JsonConvert.SerializeObject(obj);
            json = json.Insert(0, $"{id}%");
            SendToPlayer(json);
        }
    }
}
