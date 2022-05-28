using DongUtility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arena.GraphicTurns
{
    public class AddObject : GraphicTurn
    {
        private readonly Registry registry;
        private readonly int layer;
        private readonly int objCode;
        private readonly int graphicCode;
        private Vector2D coord;

        public AddObject(ArenaObject obj)
        {
            registry = obj.Arena.Registry;
            layer = obj.Layer;
            objCode = obj.Code;
            graphicCode = obj.GraphicCode;
            coord = obj.Position;
        }

        public AddObject(Registry registry, int layer, int objCode, int graphicCode, Vector2D coord)
        {
            this.registry = registry;
            this.layer = layer;
            this.objCode = objCode;
            this.graphicCode = graphicCode;
            this.coord = coord;
        }

        protected override GraphicTurnTypes GraphicType => GraphicTurnTypes.AddObject;

        public override void Do(IArenaDisplay display)
        {
            display.AddObject(registry, layer, graphicCode, objCode, coord);
        }

        private static readonly Dictionary<BinaryWriter, HashSet<int>> codesWritten 
            = new Dictionary<BinaryWriter, HashSet<int>>();

        protected override void WriteContent(BinaryWriter bw)
        {
            bw.Write(layer);
            bw.Write(objCode);
            bw.Write(graphicCode);
            bw.Write(coord);
            if (!codesWritten.ContainsKey(bw))
                codesWritten.Add(bw, new HashSet<int>());

            if (!codesWritten[bw].Contains(graphicCode))
            {
                var info = registry.GetInfo(graphicCode);
                info.WriteToFile(bw);
                codesWritten[bw].Add(graphicCode);
            }
        }

        private static readonly Dictionary<BinaryReader, HashSet<int>> codesRead
            = new Dictionary<BinaryReader, HashSet<int>>();

        internal AddObject(BinaryReader br, Registry registry)
        {
            this.registry = registry;
            layer = br.ReadInt32();
            objCode = br.ReadInt32();
            graphicCode = br.ReadInt32();
            coord = br.ReadVector2D();
            if (!codesRead.ContainsKey(br))
                codesRead.Add(br, new HashSet<int>());

            if (!codesRead[br].Contains(graphicCode))
            {
                var info = new GraphicInfo(br);
                registry.AddEntryWithIndex(info, graphicCode);
                codesRead[br].Add(graphicCode);
            }
        }
    }
}
