using Arena;
using ArenaVisualizer;
using GraphData;
using HungerGames.Animals;
using HungerGames.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HungerGames
{
    class HungerGamesTest
    {
        private const double hareToLynxRatio = 10;

        private const int nLynx = 10;
        private const int nHare = (int)(nLynx * hareToLynxRatio);

        private const int arenaHeight = 50;
        private const int arenaWidth = 50;

        public static void Run()
        {
            HungerGamesArena arena = new HungerGamesArena(arenaWidth, arenaHeight);

            GameMaster master = new GameMaster(arena);

            master.AddChooser(new ChooserBraedenCullen());
            master.AddChooser(new ChooserDefault());

            master.AddAllAnimals(nHare, nLynx);

            var sim = new HungerGamesTestWindow(arena);

            sim.Manager.AddLeaderBoard(GetLeaderBars(master, true),
                () => GetLeaderBoardScores(arena, master));
            sim.Manager.AddLeaderBoard(GetLeaderBars(master, false),
                () => GetLynxScores(arena, master));

            sim.Show();

            
        }

        static private List<LeaderBarPrototype> GetLeaderBars(GameMaster gm, bool hare)
        {
            var leaderBars = new List<LeaderBarPrototype>();
            foreach (var chooser in gm.Choosers)
            {
                var color = chooser.MakeOrganism(null, hare).Color;
                var bar = new LeaderBarPrototype(chooser.GetName(hare), color);
                leaderBars.Add(bar);
            }
            return leaderBars;
        }

        static private List<double> GetLeaderBoardScores(ArenaEngine arena, GameMaster gm)
        {
            var data = new List<double>();
            foreach (var chooser in gm.Choosers)
            {
                var list = arena.GetObjects(chooser.GetName(true));
                data.Add(list.Count());
            }
            return data;
        }

        static private List<double> GetLynxScores(ArenaEngine arena, GameMaster gm)
        {
            var data = new List<double>();
            foreach (var chooser in gm.Choosers)
            {
                var list = arena.GetObjects(chooser.GetName(false));
                var sum = list.Sum((x) => ((Lynx)x).HaresEaten);
                data.Add(sum);
            }
            return data;
        }
    }
}
