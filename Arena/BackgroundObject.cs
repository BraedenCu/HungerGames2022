using DongUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arena
{
    public class BackgroundObject : StationaryObject
    {
        public override bool IsPassable(ArenaObject mover) => true;

        private const int backLayer = 0;

        public override string Name => "Background";

        public BackgroundObject(int graphicCode) :
            base(graphicCode, backLayer, 0, 0)
        { }
    }
}
