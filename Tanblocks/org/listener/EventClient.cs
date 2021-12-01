using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using Tanblocks.org.entity;
using Tanblocks.org.utils;
using Tanblocks.socket;

namespace Tanblocks.org.listener
{
    public class EventClient
    {
        private Form1 screenGame;
        private Client client;
        private List<(string user, Directions direction, SolidBrush brush, Rectangle[] rectangles)> rectangles;
        private Tank tankClient;
        public EventClient(Form1 screenGame)
        {
            this.screenGame = screenGame;
            client = new Client("cliente", InfoConexion.IPServer, false, false);
            client.Receive += new Client.EventReceive(OnReceive);
            client.Connect();

            Thread.Sleep(1000);
            VisualStyleTank styleTank = new VisualStyleTank();
            styleTank.Color = Color.Red;
            styleTank.Size = new Size(8, 8);
            styleTank.Point = new Point(4, 4);
            client.SendObjectToServer(styleTank, 0);
        }

        public void OnReceive(string username, int instr, StringBuilder sbData)
        {
            object obj = null;
            try { obj = JsonConvert.DeserializeObject(sbData.ToString()); }
            catch { return; }

            switch (instr)
            {
                case 0: // pegando todos retangulos do servidor
                    rectangles = JsonConvert.
                        DeserializeObject<List<(string user, Directions direction, SolidBrush brush, Rectangle[] rectangles)>>
                        (sbData.ToString());
                    screenGame.Invalidate();
                    break;
                default:
                    break;
            }
        }

        public void MoveTankClient(Directions directions)
        {
            DirectionTank direction = new DirectionTank();
            direction.Directions = directions;
            client.SendObjectToServer(direction, 1);
        }

        public List<(string user, Directions direction, SolidBrush brush, Rectangle[] rectangles)> GetAllBlocksTank()
        {
            return rectangles;
        }

        public (string user, Directions directions, SolidBrush brush, Rectangle[] rectangles) GetArgsPlayers(string user)
        {
            int i = rectangles.FindIndex(x => x.user == user);
            if (i != -1)
            {
                return rectangles[i];
            }
            return (null, Directions.Down, null, null);
        }
    }
}
