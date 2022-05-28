using System;
using System.Collections.Generic;
using System.Text;

namespace GraphData
{
    public class HybridGraphDataInterface : IGraphDataInterface
    {
        private IGraphDataInterface fileInterface;
        private IGraphDataInterface realTimeInterface;

        public HybridGraphDataInterface(IGraphDataInterface fileInterface, IGraphDataInterface realTimeInterface)
        {
            this.fileInterface = fileInterface;
            this.realTimeInterface = realTimeInterface;
        }

        public IEnumerable<IGraphPrototype> Graphs
        {
            get
            {
                foreach (var graph in fileInterface.Graphs)
                {
                    yield return graph;
                }
                foreach (var graph in realTimeInterface.Graphs)
                {
                    yield return graph;
                }
            }
        }

        public GraphDataPacket GetData()
        {
            return GraphDataPacket.Combine(fileInterface.GetData(), realTimeInterface.GetData());
        }
    }
}
