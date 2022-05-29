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
            bool training = false;

            if(training)
            {
                RunWithTraining();
                return;
            }

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





        //homework 10 copy paste -> work for training in homework ten
        static private Perceptron bestPerceptron = new Perceptron(4, 2);//Hare
        static private Perceptron bestLynxPerceptron = new Perceptron(4, 2);

        static private void RunWithTraining()
        {
            Process();
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
            bestPerceptron = getBestPerceptron(RunArenaHare, "Hare", new Perceptron(bestPerceptron.InputNodes.Count, bestPerceptron.OutputNodes.Count));
            lastBestHareScore = bestTimes[0];
            //bestLynxPerceptron = getBestLynxPerceptron();
            bestLynxPerceptron = getBestPerceptron(RunArenaHare, "Lynx", new Perceptron(bestPerceptron.InputNodes.Count, bestPerceptron.OutputNodes.Count));
            lastBestLynxScore = bestTimes[0];

            while (bestHareLynxScore > lastBestLynxScore || bestHareLynxScore > lastBestHareScore)
            {
                Console.WriteLine("Best Hare Score:" + bestHareLynxScore + "\tLast Hare Score:" + lastBestLynxScore);
                Console.WriteLine("Running Training Cycle for both Hare and Lynx");
                lastBestLynxScore = bestHareLynxScore;
                lastBestHareScore = bestHareLynxScore;
                bestPerceptron = getBestPerceptron(RunArenaHare, "Hare", bestPerceptron);
                lastBestHareScore = bestTimes[0];
                bestLynxPerceptron = getBestPerceptron(RunArenaHare, "Lynx", bestLynxPerceptron);
                lastBestLynxScore = bestTimes[0];
            }

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
        const int maxRunTime = 200;
        static Perceptron[] topPreceptrons = new Perceptron[numberOfTopPerceptronsToStore];
        static double[] bestTimes = new double[numberOfTopPerceptronsToStore];

        static private Perceptron getBestHarePerceptron()
        {
            Console.WriteLine("**Training Hares**");
            double bestScore = 0;
            int numberOfTrainingRuns = 1000;
            int InitalStandardDeviation = 6;
            double score = 0.0;
            //Perceptron bestHarePerceptron = bestPerceptron.Clone();
            topPreceptrons = new Perceptron[numberOfTopPerceptronsToStore];
            bestTimes = new double[numberOfTopPerceptronsToStore];

            var arena = SetupArena();
            int generations = 1;
            for (int i = 0; i < numberOfTrainingRuns; i++)
            {

                arena = SetupArena();

                var newPerceptron = new Perceptron(bestPerceptron.InputNodes.Count, bestPerceptron.OutputNodes.Count);

                // Here is where you do stuff to the Perceptron
                newPerceptron.RandomWeights(InitalStandardDeviation);

                score = RunArena(arena, newPerceptron, bestLynxPerceptron, maxRunTime);

                UpdateBestPerceptronsGreatThen(newPerceptron, score);

                if (i == numberOfTrainingRuns - 1)
                {
                    if (!isTopScoresFound())
                    {
                        InitalStandardDeviation++;
                        Console.WriteLine("Increasing Stadard Deviation to:" + InitalStandardDeviation);
                        i = i - 1000;
                    }
                }
            }
            Console.WriteLine("Final Standard Dev Used:" + InitalStandardDeviation);
            Console.WriteLine("Best Hare Score:" + score);


            double fineStandarDeviation = .5;
            double lastBestTopScore = bestTimes[0] - .5;//-.5 so it does at least 1 generation
            while (bestTimes[0] > lastBestTopScore)
            {
                lastBestTopScore = bestTimes[0];
                generations++;
                foreach (var topHarePerc in topPreceptrons)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        Perceptron testPerceptron = topHarePerc.RandomClone(fineStandarDeviation);
                        arena = SetupArena();
                        score = RunArena(arena, testPerceptron, bestLynxPerceptron, maxRunTime);
                        //Console.WriteLine("Fine Tuned Hare Score:"+score);
                        UpdateBestPerceptronsGreatThen(testPerceptron, score);
                    }
                }
            }
            Console.WriteLine("Final Best Hare Top Score:" + bestTimes[0]);
            Console.WriteLine("Number Of Generation:" + generations);

            arena = SetupArena();
            Console.WriteLine("Reran best and scored:" + RunArena(arena, topPreceptrons[0], bestLynxPerceptron, maxRunTime));
            //PrintWeights(topPreceptrons[0]);
            //memory is being eaten up... suggestion from stack overflow
            GC.Collect();
            GC.WaitForPendingFinalizers();
            //TODO possibly check the best again to make sure they are reliable?
            return topPreceptrons[0];
        }

        static private Perceptron getBestLynxPerceptron()
        {
            Console.Write("**Training Lynx**");
            topPreceptrons = new Perceptron[numberOfTopPerceptronsToStore];
            bestTimes = new double[numberOfTopPerceptronsToStore];

            double bestScore = 0;
            int numberOfTrainingRuns = 1000;
            int InitalStandardDeviation = 6;
            double score = 0.0;

            var arena = SetupArena();
            for (int i = 0; i < numberOfTrainingRuns; i++)
            {
                arena = SetupArena();

                var newPerceptron = new Perceptron(bestPerceptron.InputNodes.Count, bestPerceptron.OutputNodes.Count);

                // Here is where you do stuff to the Perceptron
                newPerceptron.RandomWeights(InitalStandardDeviation);

                score = RunArena(arena, bestPerceptron, newPerceptron, maxRunTime);

                UpdateBestPerceptronsGreatThen(newPerceptron, score);

                if (i == numberOfTrainingRuns - 1)
                {
                    if (!isTopScoresFound())
                    {
                        InitalStandardDeviation++;
                        Console.WriteLine("Increasing Stadard Deviation to:" + InitalStandardDeviation);
                        i = i - 1000;
                    }
                }
            }
            Console.WriteLine("Final Standard Dev Used:" + InitalStandardDeviation);
            Console.WriteLine("Best Lynx Score:" + score);

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
                        score = RunArena(arena, testPerceptron, bestPerceptron, maxRunTime);
                        UpdateBestPerceptronsGreatThen(testPerceptron, score);
                    }
                }
            }
            Console.WriteLine("Final Best Lynx Top Score:" + bestTimes[0]);
            Console.WriteLine("Number Of Generation:" + generations);
            arena = SetupArena();
            Console.WriteLine("Reran best and scored:" + RunArena(arena, bestPerceptron, topPreceptrons[0], maxRunTime));
            //PrintWeights(topPreceptrons[0]);
            //memory is being eaten up... suggestion from stack overflow
            GC.Collect();
            GC.WaitForPendingFinalizers();
            //TODO possibly check the best again to make sure they are reliable?
            return topPreceptrons[0];
        }

        static Perceptron bestGenericPerc;

        static private Perceptron getBestPerceptron(Func<HungerGamesArena, Perceptron, double> runArenaCall, String type, Perceptron startingPerceptron)
        {
            Console.Write("**Training " + type + "**");
            bestGenericPerc = startingPerceptron;
            topPreceptrons = new Perceptron[numberOfTopPerceptronsToStore];
            bestTimes = new double[numberOfTopPerceptronsToStore];

            double bestScore = 0;
            int numberOfTrainingRuns = 1000;
            int InitalStandardDeviation = 6;
            double score = 0.0;

            var arena = SetupArena();
            for (int i = 0; i < numberOfTrainingRuns; i++)
            {
                arena = SetupArena();

                var newPerceptron = new Perceptron(bestPerceptron.InputNodes.Count, bestPerceptron.OutputNodes.Count);

                // Here is where you do stuff to the Perceptron
                newPerceptron.RandomWeights(InitalStandardDeviation);

                score = RunArena(arena, bestPerceptron, newPerceptron);
                Console.WriteLine("SCORE: " + score);

                UpdateBestPerceptronsGreatThen(newPerceptron, score);

                if (i == numberOfTrainingRuns - 1)
                {
                    if (!isTopScoresFound())
                    {
                        InitalStandardDeviation++;
                        Console.WriteLine("Most Recent Score: " + score);
                        Console.WriteLine("Increasing Stadard Deviation to:" + InitalStandardDeviation);
                        i = i - 1000;
                    }
                }
            }
            Console.WriteLine("Final Standard Dev Used:" + InitalStandardDeviation);
            Console.WriteLine("Best Score:" + bestTimes[0]);

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

            return RunArena(arena, bestPerceptron, newPerceptronToRun, 300);
        }

        static private double RunArenaHare(HungerGamesArena arena, Perceptron newPerceptronToRun)
        {
            return RunArenaHareMaximize(arena, newPerceptronToRun, bestLynxPerceptron, maxRunTime);
        }
        static private double RunArena(HungerGamesArena arena, Perceptron harePerceptron, Perceptron lynxPerceptron)
        {
            return RunArena(arena, harePerceptron, lynxPerceptron, 300);
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
                    //PrintWeights(perceptronIn);
                    //PrintWeights(topPreceptrons[i]);                    
                    return true;
                }
            }
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
            Console.WriteLine("Number of hares: " + arena.CountObjects("MyHares"));
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

            while (keepRunning && arena.CountObjects("MyHares") > 0 && arena.Time < MaxRuntime)
            //while (keepRunning && arena.Time < totalTime)
            {
                arena.Tick(arena.Time + 1);
            }

            //return arena.CountObjects("Hare");
            //TODO Change to get time for hare being alive....
            return arena.Time;
        }


        /*
        static private void Display()
        {
            var arena = SetupArena();
            AddPerceptrons(arena, bestPerceptron, bestLynxPerceptron);

            var sim = new EcologySim(arena);
            sim.TimePerTurn = .1;

            sim.Arena.Manager.AddGraph(new List<TimelineInfo>()
            { new TimelineInfo(new TimelinePrototype("Hare", Color.SandyBrown), new BasicFunctionPair(() => arena.Time, () => arena.GetObjectsOfType<Hare>().Count())),
            new TimelineInfo(new TimelinePrototype("Lynx", Color.PaleVioletRed), new BasicFunctionPair(() => arena.Time, () => arena.GetObjectsOfType<Lynx>().Count()))
            }, "Time (days)", "Population");

            sim.Show();
        }
        */

        static private void AddPerceptrons(HungerGamesArena arena, Perceptron perceptron, Perceptron perceptronLynx)
        {
            foreach (var animal in arena.GetObjectsOfType<Hare>())
            {
                animal.Perceptron = perceptron.Clone();
                //Console.WriteLine("should be running");
            }
            foreach (var animal in arena.GetObjectsOfType<Lynx>())
            {
                animal.Perceptron = perceptron.Clone();
                //Console.WriteLine("should be running");
            }
        }
    }
}
