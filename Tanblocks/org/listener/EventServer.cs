using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using Tanblocks.entity;
using Tanblocks.org.entity;
using Tanblocks.org.utils;
using Tanblocks.socket;

namespace Tanblocks.org.listener
{
    public class EventServer
    {
        private Dictionary<string, Player> dicPlayers;
        private List<(string user, Directions direction, SolidBrush brush, Rectangle[] rectangles)> rectangles;
        private Form1 screenGame;
        private Tank tankServer;
        public EventServer(Form1 screenGame)
        {
            this.screenGame = screenGame;
            dicPlayers = new Dictionary<string, Player>();
            Server server = new Server(InfoConexion.IPServer);
            server.Connect += new Server.EventConnect(OnConnect);
            server.Start();

            tankServer = new Tank(Color.Blue, new Size(8, 8), new Point(4, 4), screenGame.ClientRectangle);
            rectangles = new List<(string user, Directions direction, SolidBrush, Rectangle[])>();
            rectangles.Add(("servidor", tankServer.Direction, new SolidBrush(tankServer.Color), tankServer.GetAllRectangles()));
        }

        public virtual void OnConnect(string username, Socket socket)
        {
            var infoUsername = JsonConvert.DeserializeObject<(string username, bool owner, bool op)>(username);
            Player player = new Player(infoUsername.username, socket);
            player.Receive += new Player.EventReceive(OnCommandAllClients);
            dicPlayers.Add(infoUsername.username, player);
        }

        public virtual void OnCommandAllClients(string username, int instr, StringBuilder sbData)
        {
            object obj = null;
            try { obj = JsonConvert.DeserializeObject(sbData.ToString()); }
            catch { return; }

            switch (instr)
            {
                case 0: // visual tank
                    VisualStyleTank styleTank = JsonConvert.DeserializeObject<VisualStyleTank>(sbData.ToString());
                    dicPlayers[username].tank = new Tank
                        (
                        styleTank.Color,
                        styleTank.Size,
                        styleTank.Point,
                        screenGame.ClientRectangle
                        );
                    break;
                case 1: // tank change
                    DirectionTank direction = JsonConvert.DeserializeObject<DirectionTank>(sbData.ToString());
                    dicPlayers[username].tank.MoveDirection(direction.Directions);
                    Refresh();
                    break;
                default:
                    break;
            }
        }

        public void MoveTankServer(Directions directions)
        {
            tankServer.MoveDirection(directions);
            Refresh();
        }

        public void Refresh()
        {
            rectangles = new List<(string user, Directions direction, SolidBrush, Rectangle[])>();
            if (dicPlayers.Count > 0)
            {
                int i = 0;
                foreach (var player in dicPlayers.Values)
                {
                    rectangles.Add((player.Username, player.tank.Direction, new SolidBrush(player.tank.Color), player.tank.GetAllRectangles()));
                    if (i + 1 >= dicPlayers.Values.Count)
                    {
                        rectangles.Add(("servidor", tankServer.Direction, new SolidBrush(tankServer.Color), tankServer.GetAllRectangles()));
                        player.SendObjectToPlayer(rectangles, 0);
                        i = 0;
                        break;
                    }
                    i++;
                }
            }
            else
            {
                rectangles.Add(("servidor",tankServer.Direction, new SolidBrush(tankServer.Color), tankServer.GetAllRectangles()));
            }
            screenGame.Invalidate();
        }

        public List<(string user, Directions direction, SolidBrush brush, Rectangle[] rectangles)> GetAllBlocksTank()
        {
            return rectangles;
        }

        public (string user, Directions directions, SolidBrush brush, Rectangle[] rectangles) GetArgsPlayer(string user)
        {
            int i = rectangles.FindIndex(x => x.user == user);
            if (i != -1)
            {
                return rectangles[i];
            }
            return (null, Directions.Down, null, null);
        }

        public string[] GetPlayers()
        {
            List<string> list = new List<string>(dicPlayers.Keys);
            list.Add("servidor");
            return list.ToArray();
        }
    }
}
