using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Arena.GraphicTurns
{
    public class SetWindowDimensions : GraphicTurn
    {
        double windowWidth;
        double windowHeight;
        double arenaWidth;
        double arenaHeight;
        protected override GraphicTurnTypes GraphicType => GraphicTurnTypes.SetWindowDimensions;

        public override void Do(IArenaDisplay display)
        {
            display.SetWindowDimensions(windowWidth, windowHeight, arenaWidth, arenaHeight);
        }

        protected override void WriteContent(BinaryWriter bw)
        {
            bw.Write(windowWidth);
            bw.Write(windowHeight);
            bw.Write(arenaWidth);
            bw.Write(arenaHeight);
        }

        internal SetWindowDimensions(BinaryReader br)
        {
            windowWidth = br.ReadDouble();
            windowHeight = br.ReadDouble();
            arenaWidth = br.ReadDouble();
            arenaHeight = br.ReadDouble();
        }
    }
}
