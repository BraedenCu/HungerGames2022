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
            WPFUtility.ConsoleManager.ShowConsoleWindow();
            Console.WriteLine("RUNNING LOOP");
            bool training = true;

            HungerGamesArena arena = new HungerGamesArena(arenaWidth, arenaHeight);

            GameMaster master = new GameMaster(arena);

            if (training)
            {
                arena = new HungerGamesArena(arenaWidth, arenaHeight);

                master = new GameMaster(arena);

                master.AddChooser(new ChooserBraedenCullen());
                master.AddChooser(new ChooserDefault());

                master.AddAllAnimals(nHare, nLynx);

                RunWithTraining();
                return;
            }

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

        //homework 10 copy paste -> work for training in homework ten
        static private Perceptron bestPerceptron = new Perceptron(5, 3);//Hare
        static private Perceptron bestLynxPerceptron = new Perceptron(5, 3);

        static private void RunWithTraining()
        {
            Process();
            Display();
        }

        static private void Process()
        {
            //List<double> hareScores = new List<double>();
            //List<double> lynxScores = new List<double>();
            double lastBestLynxScore = 0;
            double lastBestHareScore = 0;
            double bestLynxScore = 0;
            double bestHareLynxScore = 0;


            Console.WriteLine("entering training loop");
            //bestPerceptron = getBestHarePerceptron();
            bestPerceptron = getBestPerceptron(RunArenaHare, "MyHares", new Perceptron(bestPerceptron.InputNodes.Count, bestPerceptron.OutputNodes.Count));
            lastBestHareScore = bestTimes[0];
            //bestLynxPerceptron = getBestLynxPerceptron();
            //bestLynxPerceptron = getBestPerceptron(RunArenaHare, "Lynx", new Perceptron(bestPerceptron.InputNodes.Count, bestPerceptron.OutputNodes.Count));
            //lastBestLynxScore = bestTimes[0];

            /*  
            while (bestHareLynxScore > lastBestLynxScore || bestHareLynxScore > lastBestHareScore)
            {
                Console.WriteLine("Best Hare Score:" + bestHareLynxScore + "\tLast Hare Score:" + lastBestLynxScore);
                Console.WriteLine("Running Training Cycle for both Hare and Lynx");
                lastBestLynxScore = bestHareLynxScore;
                lastBestHareScore = bestHareLynxScore;
                bestPerceptron = getBestPerceptron(RunArenaHare, "Hare", bestPerceptron);
                lastBestHareScore = bestTimes[0];
                //bestLynxPerceptron = getBestPerceptron(RunArenaHare, "Lynx", bestLynxPerceptron);
                //lastBestLynxScore = bestTimes[0];
            }
            */

            /*while(bestHareLynxScore>lastBestLynxScore || bestHareLynxScore>lastBestHareScore){
                Console.WriteLine("Best Hare Score:" + bestHareLynxScore + "\tLast Hare Score:" + lastBestLynxScore);
                Console.WriteLine("Running Training Cycle for both Hare and Lynx");
                lastBestLynxScore = bestHareLynxScore;
                lastBestHareScore = bestHareLynxScore;
                bestPerceptron = getBestHarePerceptron();
                lastBestHareScore = bestTimes[0];
                bestLynxPerceptron = getBestLynxPerceptron();
                lastBestLynxScore = bestTimes[0];
            }*/
        }

        const int numberOfTopPerceptronsToStore = 5;
        const int maxRunTime = 4000;
        static Perceptron[] topPreceptrons = new Perceptron[numberOfTopPerceptronsToStore];
        static double[] bestTimes = new double[numberOfTopPerceptronsToStore];


        static Perceptron bestGenericPerc;

        static private Perceptron getBestPerceptron(Func<HungerGamesArena, Perceptron, double> runArenaCall, String type, Perceptron startingPerceptron)
        {
            Console.Write("**Training " + type + "**");
            bestGenericPerc = startingPerceptron;
            topPreceptrons = new Perceptron[numberOfTopPerceptronsToStore];
            bestTimes = new double[numberOfTopPerceptronsToStore];

            double bestScore = 0;
            int numberOfTrainingRuns = 10;
            int InitalStandardDeviation = 6;
            double score = 0.0;
            
            var arena = SetupArena();
            for (int i = 0; i < numberOfTrainingRuns; i++)
            {
                arena = SetupArena();

                var newPerceptron = new Perceptron(bestPerceptron.InputNodes.Count, bestPerceptron.OutputNodes.Count);

                // Here is where you do stuff to the Perceptron
                newPerceptron.RandomWeights(InitalStandardDeviation);

                //TODO -> fix the below issue
                score = runArenaCall(arena, newPerceptron);
                //score = RunArena(arena, bestPerceptron, newPerceptron);

                Console.WriteLine("SCORE: " + score);
                
                UpdateBestPerceptronsGreatThen(newPerceptron, score);

                if (i == numberOfTrainingRuns - 1)
                {
                    if (!isTopScoresFound() && false)
                    {
                        InitalStandardDeviation++;
                        Console.WriteLine("Most Recent Score: " + score);
                        Console.WriteLine("Increasing Stadard Deviation to:" + InitalStandardDeviation);
                        i = i - 3;
                    }
                }
            }
            Console.WriteLine("Final Standard Dev Used:" + InitalStandardDeviation);
            Console.WriteLine("Best Score:" + bestTimes[0]);

            //TODO change this to a different situation so its not just returning the first thing it finds
            return topPreceptrons[0];

            int generations = 1;
            double fineStandarDeviation = .5;
            double lastBestTopScore = bestTimes[0] - .5;//-.5 so it does at least 1 generation
            while (bestTimes[0] > lastBestTopScore)
            {
                lastBestTopScore = bestTimes[0];
                generations++;
                foreach (var topLynxPerc in topPreceptrons)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        Perceptron testPerceptron = topLynxPerc.RandomClone(fineStandarDeviation);
                        arena = SetupArena();
                        score = runArenaCall(arena, testPerceptron);
                        UpdateBestPerceptronsGreatThen(testPerceptron, score);
                    }
                }
            }
            Console.WriteLine("Final Best Top Score:" + bestTimes[0]);
            Console.WriteLine("Number Of Generation:" + generations);
            arena = SetupArena();
            //**because the field is randomly resetup each time, it sometimes does not repeat good times
            Console.WriteLine("Reran best and scored:" + runArenaCall(arena, topPreceptrons[0]));
            //PrintWeights(topPreceptrons[0]);
            //memory is being eaten up... suggestion from stack overflow
            GC.Collect();
            GC.WaitForPendingFinalizers();
            //TODO possibly check the best again to make sure they are reliable?
            return topPreceptrons[0];
        }

        static private double RunArenaLynx(HungerGamesArena arena, Perceptron newPerceptronToRun)
        {

            return RunArena(arena, bestPerceptron, newPerceptronToRun, maxRunTime);
        }

        static private double RunArenaHare(HungerGamesArena arena, Perceptron newPerceptronToRun)
        {
            return RunArenaHareMaximize(arena, newPerceptronToRun, bestLynxPerceptron, maxRunTime);
        }
        static private double RunArena(HungerGamesArena arena, Perceptron harePerceptron, Perceptron lynxPerceptron)
        {
            return RunArena(arena, harePerceptron, lynxPerceptron, maxRunTime);
        }

        //all slots have contain a Perceptron
        static bool isTopScoresFound()
        {
            foreach (var per in topPreceptrons)
            {
                if (per == null)
                    return false;
            }
            return true;
        }


        //TODO review the below function. minimal score and bestTimes[i] < score only works for lynxes -> optimizing decreasing the score
        static bool UpdateBestPerceptronsGreatThen(Perceptron perceptronIn, double score)
        {
            int minimalScore = 100;

            for (int i = 0; i < bestTimes.Count(); i++)
            {
                if (score > minimalScore && bestTimes[i] < score)
                {
                    Console.WriteLine("New Top Score:" + score + " Added to Index:" + i);
                    bestTimes[i] = score;
                    topPreceptrons[i] = perceptronIn.Clone();
                    PrintWeights(perceptronIn);
                    //PrintWeights(topPreceptrons[i]);                    
                    return true;
                }
            }
            //return true;
            return false;
        }

        static private HungerGamesArena SetupArena()
        {
            /*
            var arena = new HungerGamesArena(size, size);
            arena.AddAnimals<NeuralNetHare>(nHares);
            arena.AddAnimals<Lynx>(nLynx);
            return arena;
            */
            HungerGamesArena arena = new HungerGamesArena(arenaWidth, arenaHeight);
            GameMaster master = new GameMaster(arena);

            master.AddChooser(new ChooserBraedenCullen());
            master.AddChooser(new ChooserDefault());

            master.AddAllAnimals(nHare, nLynx);
            return arena;
        }

        static private void PrintWeights(Perceptron perc)
        {
            foreach (var input in perc.InputNodes)
            {
                foreach (var connector in input.Connectors)
                {
                    Console.Write(":" + connector.Weight);
                }

            }
            Console.WriteLine(":Perceptron Weights");
        }

        static private double RunArena(HungerGamesArena arena, Perceptron harePerceptron, Perceptron lynxPerceptron, int MaxRuntime)
        {
            AddPerceptrons(arena, harePerceptron, lynxPerceptron);
            bool keepRunning = true;
            //Console.WriteLine("Number of hares: " + arena.CountObjects("MyHares"));
            while (keepRunning && arena.CountObjects("MyHares") > 0 && arena.Time < MaxRuntime)
            //while (keepRunning && arena.CountObjects("Lynx") > 0 && arena.Time < MaxRuntime)
            //while (keepRunning && arena.Time < totalTime)
            {
                arena.Tick(arena.Time += 1);
            }

            //return arena.CountObjects("Hare");
            //TODO Change to get time for hare being alive....
            return arena.Time;
        }

        static private double RunArenaHareMaximize(HungerGamesArena arena, Perceptron harePerceptron, Perceptron lynxPerceptron, int MaxRuntime)
        {
            AddPerceptrons(arena, harePerceptron, lynxPerceptron);
            bool keepRunning = true;
            var myHares = arena.GetObjects("MyHares");

            //TODO -> figure out why this loop is taking so long
            while (keepRunning && arena.CountObjects("MyHares") > 0 && arena.Time < MaxRuntime)
            //while (keepRunning && arena.Time < totalTime)
            {
                myHares = arena.GetObjects("MyHares");
                arena.Tick(arena.Time + 1);
            }

            if (arena.GetObjects("Default Hare").Count() > myHares.Count())
            {
                Console.WriteLine("Time before discarding:  " + arena.Time);
                Console.WriteLine("**Discarding time since my hares did not do better than the default hares");
                return 0;
            }

            //return arena.CountObjects("Hare");
            //TODO Change to get time for hare being alive....
            return arena.Time;
        }


        
        static private void Display()
        {

            HungerGamesArena arena = new HungerGamesArena(arenaWidth, arenaHeight);
            GameMaster master = new GameMaster(arena);

            master.AddChooser(new ChooserBraedenCullen());
            master.AddChooser(new ChooserDefault());

            master.AddAllAnimals(nHare, nLynx);

            AddPerceptrons(arena, bestPerceptron, bestLynxPerceptron);


            var sim = new HungerGamesTestWindow(arena);

            sim.Manager.AddLeaderBoard(GetLeaderBars(master, true),
                () => GetLeaderBoardScores(arena, master));
            sim.Manager.AddLeaderBoard(GetLeaderBars(master, false),
                () => GetLynxScores(arena, master));

            sim.Show();
        }
        

        static private void AddPerceptrons(HungerGamesArena arena, Perceptron perceptron, Perceptron perceptronLynx)
        {
            foreach (var animal in arena.GetObjectsOfType<Hare>())
            {
                //only add perceptron to MyHares
                if(animal.Name == "MyHares")
                {
                    //Console.WriteLine("updating hare perceptron");
                    animal.Perceptron = perceptron.Clone();
                }
                else
                {
                    //Console.WriteLine("wat not of myhare type");

                }
            }
            /*
            foreach (var animal in arena.GetObjectsOfType<Lynx>())
            {
                animal.Perceptron = perceptronLynx.Clone();
                //Console.WriteLine("should be running");
            }
            */
        }
    }
}
