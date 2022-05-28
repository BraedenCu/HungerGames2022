using Arena;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VisualizerBaseClasses;

namespace ArenaVisualizer
{
    public class ArenaCommandFileReader : ICommandFileReader<ArenaCoreInterface>
    {
        private Registry registry;

        public ArenaCommandFileReader(Registry registry)
        {
            this.registry = registry;
        }

        public ICommand<ArenaCoreInterface> ReadCommand(BinaryReader br)
        {
            var turn = GraphicTurn.ReadFromFile(br, registry);
            return new GraphicTurnAdapter(turn);
        }
    }
}
