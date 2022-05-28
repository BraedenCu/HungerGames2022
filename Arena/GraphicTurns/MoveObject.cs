using DongUtility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arena.GraphicTurns
{
    public class MoveObject : GraphicTurn
    {
        private readonly int layer;
        private readonly int objCode;
        private readonly Vector2D coord;

        public MoveObject(ArenaObject obj, Vector2D newLocation) :
            this(obj.Layer, obj.Code, newLocation)
        { }

        public MoveObject(int layer, int objCode, Vector2D coord)
        {
            this.layer = layer;
            this.objCode = objCode;
            this.coord = coord;
        }

        protected override GraphicTurnTypes GraphicType => GraphicTurnTypes.MoveObject;

        public override void Do(IArenaDisplay display)
        { 
            display.MoveObject(layer, objCode, coord);
        }

        protected override void WriteContent(BinaryWriter bw)
        {
            bw.Write(layer);
            bw.Write(objCode);
            bw.Write(coord);
        }

        internal MoveObject(BinaryReader br)
        {
            layer = br.ReadInt32();
            objCode = br.ReadInt32();
            coord = br.ReadVector2D();
        }
    }
}
