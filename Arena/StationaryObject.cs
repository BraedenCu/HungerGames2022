using DongUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arena
{
    abstract public class StationaryObject : ArenaObject
    {
        public StationaryObject(int graphicCode, int layer, double width, double height)
            : base(graphicCode, layer, width, height)
        { }

        public override bool IsUpdating => false;
    }
}
