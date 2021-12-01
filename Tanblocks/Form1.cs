using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Net;
using System.Windows.Forms;
using Tanblocks.entity;
using Tanblocks.org;
using Tanblocks.org.entity;
using Tanblocks.org.listener;

namespace Tanblocks
{
    public partial class Form1 : Form
    {
        private bool emula;
        private bool lockGame = true;
        private Directions directions;
        private EventServer eventServer;
        private EventClient eventClient;
        public Form1()
        {
            InitializeComponent();
            InfoConexion.IPServer = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345);
            fontNickName = new Font(new FontFamily("Times New Roman"), 12, FontStyle.Regular, GraphicsUnit.Pixel);
            brushNickName = new SolidBrush(Color.Black);
        }
        

        private void btnEmular(object sender, EventArgs e)
        {            
            eventServer = new EventServer(this);
            emula = true;
            lockGame = false;
            this.Text = "192.168.0.105:12345";
        }
        
        private void btnConectar(object sender, EventArgs e)
        {            
            eventClient = new EventClient(this);
            lockGame = false;
            emula = false;
        }

        private Font fontNickName;
        private SolidBrush brushNickName;
        private Point locationNickame;
        private Rectangle r;
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (emula && !lockGame)
            {
                if (eventServer.GetAllBlocksTank() != null)
                {
                    foreach (var p in eventServer.GetPlayers())
                    {

                        var args = eventServer.GetArgsPlayer(p);
                        if (args.user != null)
                        {
                            e.Graphics.FillRectangles(args.brush, args.rectangles);

                            switch (args.directions)
                            {
                                case Directions.Up:
                                    r = args.rectangles[2];
                                    locationNickame = new Point(r.X - 12, r.Y + 8);
                                    e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                                    e.Graphics.DrawString(args.user, fontNickName, brushNickName, locationNickame);
                                    break;
                                case Directions.Left:
                                    r = args.rectangles[3];
                                    locationNickame = new Point(r.X - 12 , r.Y - 16);
                                    e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                                    e.Graphics.DrawString(args.user, fontNickName, brushNickName, locationNickame);
                                    break;
                                case Directions.Right:
                                    r = args.rectangles[0];
                                    locationNickame = new Point(r.X - 12, r.Y - 16);
                                    e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                                    e.Graphics.DrawString(args.user, fontNickName, brushNickName, locationNickame);
                                    break;
                                case Directions.Down:
                                    r = args.rectangles[0];
                                    locationNickame = new Point(r.X - 12, r.Y - 16);
                                    e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                                    e.Graphics.DrawString(args.user, fontNickName, brushNickName, locationNickame);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            else if (!lockGame)
            {
                if (eventClient.GetAllBlocksTank() != null)
                {
                    foreach (var blockTank in eventClient.GetAllBlocksTank())
                    {
                        var args = eventClient.GetArgsPlayers(blockTank.user);
                        if (args.user != null)
                        {
                            e.Graphics.FillRectangles(args.brush, args.rectangles);

                            switch (args.directions)
                            {
                                case Directions.Up:
                                    r = args.rectangles[2];
                                    locationNickame = new Point(r.X - 12, r.Y + 8);
                                    e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                                    e.Graphics.DrawString(args.user, fontNickName, brushNickName, locationNickame);
                                    break;
                                case Directions.Left:
                                    r = args.rectangles[3];
                                    locationNickame = new Point(r.X - 12, r.Y - 16);
                                    e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                                    e.Graphics.DrawString(args.user, fontNickName, brushNickName, locationNickame);
                                    break;
                                case Directions.Right:
                                    r = args.rectangles[0];
                                    locationNickame = new Point(r.X - 12, r.Y - 16);
                                    e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                                    e.Graphics.DrawString(args.user, fontNickName, brushNickName, locationNickame);
                                    break;
                                case Directions.Down:
                                    r = args.rectangles[0];
                                    locationNickame = new Point(r.X - 12, r.Y - 16);
                                    e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                                    e.Graphics.DrawString(args.user, fontNickName, brushNickName, locationNickame);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }                    
                }
            }
        }

        private bool left, down, up, right;
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    up = true;
                    break;
                case Keys.S:
                    down = true;
                    break;
                case Keys.D:
                    right = true;
                    break;
                case Keys.A:
                    left = true;
                    break;
                default:
                    break;
            }
            if (!(up || down || left || right))
            {
                timer1.Stop();
            }
        }
        
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    up = false;
                    break;
                case Keys.S:
                    down = false;
                    break;
                case Keys.D:
                    right = false;
                    break;
                case Keys.A:
                    left = false;
                    break;
                default:
                    break;
            }
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (emula)
            {
                if (up)
                {
                    directions = Directions.Up;
                    eventServer.MoveTankServer(directions);
                }
                else if (down)
                {
                    directions = Directions.Down;
                    eventServer.MoveTankServer(directions);
                }
                else if (left)
                {
                    directions = Directions.Left;
                    eventServer.MoveTankServer(directions);
                }
                else if (right)
                {
                    directions = Directions.Right;
                    eventServer.MoveTankServer(directions);
                } 
            }
            else
            {
                if (up)
                {
                    directions = Directions.Up;
                    eventClient.MoveTankClient(directions);
                }
                else if (down)
                {
                    directions = Directions.Down;
                    eventClient.MoveTankClient(directions);
                }
                else if (left)
                {
                    directions = Directions.Left;
                    eventClient.MoveTankClient(directions);
                }
                else if (right)
                {
                    directions = Directions.Right;
                    eventClient.MoveTankClient(directions);
                }                
            }
        }
    }
}
