using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Tanblocks.org.entity
{
    public enum Directions { Up, Left, Right, Down }
    public sealed partial class Tank
    {
        public Tank(Color colorStart, Size sizePixelStart, Point pointUniform, Rectangle areaRectangle)
        {
            incX = sizePixelStart.Width;
            incY = sizePixelStart.Height;
            walkDirection = new Dictionary<Directions, bool>();
            this.Color = colorStart;
            this.areaRectangle = areaRectangle;

            r = new Rectangle[3, 3]
            {
                { new Rectangle(),new Rectangle(),new Rectangle()},
                { new Rectangle(),new Rectangle(),new Rectangle()},
                { new Rectangle(),new Rectangle(),new Rectangle()}
            };

            r_parts = new Rectangle[3, 3]
            {
                { new Rectangle(),new Rectangle(),new Rectangle()},
                { new Rectangle(),new Rectangle(),new Rectangle()},
                { new Rectangle(),new Rectangle(),new Rectangle()}
            };

            walkDirection.Add(Directions.Left, true);
            walkDirection.Add(Directions.Right, true);
            walkDirection.Add(Directions.Down, true);
            walkDirection.Add(Directions.Up, true);

            int sumX = 0;
            int sumY = 0;
            for (int x = 0; x < r.GetLength(0); x++)
            {
                for (int y = 0; y < r.GetLength(1); y++)
                {
                    r_parts[x, y].Size = sizePixelStart;
                    r_parts[x, y].Location = new Point(sumX, sumY);
                    if (!(x == 0 && y == 0) || !(x == 2 && y == 0) || !(x == 1 && y == 0))
                    {
                        r[x, y].Size = sizePixelStart;
                        r[x, y].Location = new Point(sumX, sumY);
                        if (y > 3) sumY = 0;
                        sumY += incY;
                    }
                }
                if (x > 3) sumX = 0;
                sumX += incX;
                sumY = 0;
            }
        }

        #region ações do tanque
        public void MoveDirection(Directions direction)
        {
            this.Direction = direction;
            switch (direction)
            {
                case Directions.Left:
                    if (LastDirection != direction)
                    {
                        ViewDirection(direction);
                        LastDirection = direction;
                        return;
                    }
                    LastDirection = direction;
                    for (int y = 0; y < r.GetLength(1); y++)
                    {
                        for (int x = 0; x < r.GetLength(0); x++)
                        {
                            r_parts[x, y].X -= incX;
                        }
                    }
                    ViewDirection(direction);
                    break;
                case Directions.Up:
                    if (LastDirection != direction)
                    {
                        if (ViewDirection(direction)) return;
                        LastDirection = direction;
                        return;
                    }
                    LastDirection = direction;
                    for (int x = 0; x < r.GetLength(0); x++)
                    {
                        for (int y = 0; y < r.GetLength(1); y++)
                        {
                            r_parts[x, y].Y -= incY;
                        }
                    }
                    ViewDirection(direction);
                    break;
                case Directions.Right:
                    if (LastDirection != direction)
                    {
                        if (ViewDirection(direction)) return;
                        LastDirection = direction;
                        return;
                    }
                    LastDirection = direction;
                    for (int y = 0; y < r.GetLength(1); y++)
                    {
                        for (int x = 0; x < r.GetLength(0); x++)
                        {
                            r_parts[x, y].X += incX;
                        }
                    }
                    ViewDirection(direction);
                    break;
                case Directions.Down:
                    if (LastDirection != direction)
                    {
                        ViewDirection(direction);
                        LastDirection = direction;
                        return;
                    }
                    LastDirection = direction;
                    for (int x = 0; x < r.GetLength(0); x++)
                    {
                        for (int y = 0; y < r.GetLength(1); y++)
                        {
                            r_parts[x, y].Y += incY;
                        }
                    }
                    ViewDirection(direction);
                    break;
                default:
                    break;
            }            
        }
        public bool ViewDirection(Directions direction)
        {
            bool interserctLeft =
                r[1, 0].IntersectsWith(new Rectangle()) ||
                r[2, 0].IntersectsWith(new Rectangle()) ||
                r[0, 1].IntersectsWith(new Rectangle()) ||
                r[1, 1].IntersectsWith(new Rectangle()) ||
                r[1, 2].IntersectsWith(new Rectangle()) ||
                r[2, 2].IntersectsWith(new Rectangle());

            bool interserctRight =
                r[0, 0].IntersectsWith(new Rectangle()) ||
                r[1, 0].IntersectsWith(new Rectangle()) ||
                r[1, 1].IntersectsWith(new Rectangle()) ||
                r[2, 1].IntersectsWith(new Rectangle()) ||
                r[0, 2].IntersectsWith(new Rectangle()) ||
                r[1, 2].IntersectsWith(new Rectangle());

            bool interserctUp =
                r[1, 0].IntersectsWith(new Rectangle()) ||
                r[0, 1].IntersectsWith(new Rectangle()) ||
                r[1, 1].IntersectsWith(new Rectangle()) ||
                r[2, 1].IntersectsWith(new Rectangle()) ||
                r[0, 2].IntersectsWith(new Rectangle()) ||
                r[2, 2].IntersectsWith(new Rectangle());

            bool interserctDown =
                r[0, 0].IntersectsWith(new Rectangle()) ||
                r[2, 0].IntersectsWith(new Rectangle()) ||
                r[0, 1].IntersectsWith(new Rectangle()) ||
                r[1, 1].IntersectsWith(new Rectangle()) ||
                r[2, 1].IntersectsWith(new Rectangle()) ||
                r[1, 2].IntersectsWith(new Rectangle());

            for (int x = 0; x < r_parts.GetLength(0); x++)
            {
                for (int y = 0; y < r_parts.GetLength(1); y++)
                {
                    r[x, y].Location = r_parts[x, y].Location;
                    r[x, y].Size = r_parts[x, y].Size;
                }
            }

            switch (direction)
            {
                case Directions.Left:
                    if (interserctLeft) return !interserctLeft;
                    r[0, 0] = new Rectangle();
                    r[2, 1] = new Rectangle();
                    r[0, 2] = new Rectangle();
                    break;
                case Directions.Up:
                    if (interserctLeft) return !interserctUp;
                    r[0, 0] = new Rectangle();
                    r[2, 0] = new Rectangle();
                    r[1, 2] = new Rectangle();
                    break;
                case Directions.Right:
                    if (interserctLeft) return !interserctRight;
                    r[2, 0] = new Rectangle();
                    r[0, 1] = new Rectangle();
                    r[2, 2] = new Rectangle();
                    break;
                case Directions.Down:
                    if (interserctLeft) return !interserctDown;
                    r[1, 0] = new Rectangle();
                    r[0, 2] = new Rectangle();
                    r[2, 2] = new Rectangle();
                    break;
                default:
                    break;
            }
            return false;
        }

        #endregion        

        public Rectangle[] GetAllRectangles()
        {
            return r.Cast<Rectangle>().ToArray();
        }
    }

    public sealed partial class Tank
    {
        private Dictionary<Directions, bool> walkDirection;
        private Rectangle[,] r;
        private Rectangle[,] r_parts;
        private Rectangle areaRectangle;
        private Rectangle[,] r_preview;
        private int incX;
        private int incY;

        public Directions Direction { get; private set; }
        public Directions LastDirection { get; set; }
        public (int X, int Y) Speed { get; internal set; }
        public Color Color { get; }
        public Rectangle[,] Rectangles { get { return r; } }
    }
}