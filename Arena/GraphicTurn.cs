using Arena.GraphicTurns;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualizerBaseClasses;

namespace Arena
{
    abstract public class GraphicTurn : ICommand<IArenaDisplay>
    {
        public GraphicTurn()
        { }

        abstract public void Do(IArenaDisplay display);

        protected enum GraphicTurnTypes : byte { SetWindowDimensions, AddObject, RemoveObject, MoveObject, ChangeObjectGraphic };

        abstract protected void WriteContent(BinaryWriter bw);

        abstract protected GraphicTurnTypes GraphicType { get; }

        public void WriteToFile(BinaryWriter bw)
        {
            bw.Write((byte)GraphicType);
            WriteContent(bw);
        }

        static public GraphicTurn ReadFromFile(BinaryReader br, Registry registry)
        {
            byte typeCode = br.ReadByte();
            GraphicTurnTypes type = (GraphicTurnTypes)typeCode;

            return type switch
            {
                GraphicTurnTypes.AddObject => new AddObject(br, registry),
                GraphicTurnTypes.MoveObject => new MoveObject(br),
                GraphicTurnTypes.RemoveObject => new RemoveObject(br),
                GraphicTurnTypes.SetWindowDimensions => new SetWindowDimensions(br),
                GraphicTurnTypes.ChangeObjectGraphic => new ChangeObjectGraphic(br),
                _ => throw new NotImplementedException("Should never reach here"),
            };
        }
    }
}
