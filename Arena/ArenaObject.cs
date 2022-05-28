using DongUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arena
{
    abstract public class ArenaObject
    {
        public ArenaEngine Arena { get; set; }

        public Vector2D Position
        {
            get
            {
                return Size.Center;
            }
            set
            {
                Size = new Rectangle(value, Size.Width, Size.Height);
            }
        }

        public Rectangle Size { get; internal set; }

        abstract public string Name { get; }

        private static int counter = 0;

        static private object locker = new object();

        public int Code { get; }
        public int GraphicCode { get; protected set; }
        public int Layer { get; }

        abstract public bool IsUpdating { get; }
        abstract public bool IsPassable(ArenaObject mover = null);

        public ArenaObject(int graphicCode, int layer, double width, double height)
        {
            GraphicCode = graphicCode;
            Layer = layer;
            Size = new Rectangle(Vector2D.NullVector(), width, height);
            lock (locker)
            {
                Code = counter++;
            }
        }

        public bool Occupies(Vector2D coordinate, ArenaObject mover)
        {
            if (!IsPassable(mover))
            {
                if (mover == null)
                {
                    return Size.Contains(coordinate);
                }
                else
                {
                    var newRect = new Rectangle(coordinate, mover.Size.Width, mover.Size.Height);
                    return Size.Overlaps(newRect);
                }
            }
            else
            {
                return false;
            }
        }

        public bool Overlaps(ArenaObject other)
        {
            return Size.Overlaps(other.Size);
        }
    }
}
