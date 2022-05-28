using GraphData;
using System;
using System.Collections.Generic;
using System.Text;

namespace Arena
{
    public class ArenaStatisticsGraphManager
    {
        private GraphDataManager manager;
        private ArenaEngine engine;

        public ArenaStatisticsGraphManager(ArenaEngine engine, GraphDataManager manager)
        {
            this.manager = manager;
            this.engine = engine;
        }

        
    }
}
