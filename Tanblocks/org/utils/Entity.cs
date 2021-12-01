using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanblocks.org.utils
{
    public abstract class Entity
    {
        private string name;
        public Entity(string name) { this.name = name; }
        public string GetName() { return name; }
        public Color Color { get; set; }
        public Point Point { get; set; }
        public Size Size { get; set; }
        public Rectangle ClientRectangle { get; set; }
    }
}
