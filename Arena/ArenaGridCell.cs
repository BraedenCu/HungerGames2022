using DongUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arena
{
    internal class ArenaGridCell
    {
        public LinkedList<ArenaObject> Objects { get; } = new LinkedList<ArenaObject>();

        public void AddObject(ArenaObject obj)
        {
            Objects.AddLast(obj);
        }

        public void RemoveObject(ArenaObject obj)
        {
            Objects.Remove(obj);
        }
    }
}
