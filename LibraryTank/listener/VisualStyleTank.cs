using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryTank.listener
{
    public sealed class VisualStyleTank
    {
        public Color Color { get; set; }
        public Point Point { get; set; }
        public Size Size { get; set; }
        public Rectangle ClientRectangle { get; set; }
    }
}
