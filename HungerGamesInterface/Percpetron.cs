using DongUtility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HungerGames
{
    public class Perceptron
    {
        static public Random Random { get; } = new();

        public Perceptron(int nInputs, int nOutputs)
        {
            for (int i = 0; i < nOutputs; ++i)
            {
                OutputNodes.Add(new Node());
            }

            for (int i = 0; i < nInputs; ++i)
            {
                var node = new Node();
                foreach (var output in OutputNodes)
                {
                    node.Connectors.Add(new Connector() { Weight = 1, Node = output });
                }
                InputNodes.Add(node);
            }
        }

        public class Connector
        {
            public double Weight { get; set; }
            public Node Node { get; set; }
        }

        public class Node
        {
            public List<Connector> Connectors { get; set; } = new List<Connector>();

            private double total = 0;

            public void Reset()
            {
                total = 0;
            }

            public void AddData(double value, double weight = 1)
            {
                total += value * weight;
            }

            public void FeedForward()
            {
                Connectors.ForEach((x) => x.Node.AddData(total, x.Weight));
            }

            public double GetValue()
            {
                return total;
            }
        }

        public List<Node> InputNodes { get; } = new List<Node>();
        public List<Node> OutputNodes { get; } = new List<Node>();

        public IEnumerable<Connector> Connectors
        {
            get
            {
                foreach (var node in InputNodes)
                {
                    foreach (var connector in node.Connectors)
                    {
                        yield return connector;
                    }
                }
                foreach (var node in OutputNodes)
                {
                    foreach (var connector in node.Connectors)
                    {
                        yield return connector;
                    }
                }
            }
        }

        public void AddInput(int index, double value)
        {
            InputNodes[index].AddData(value);
        }

        public void Reset()
        {
            InputNodes.ForEach((x) => x.Reset());
            OutputNodes.ForEach((x) => x.Reset());
        }

        public void Run()
        {
            InputNodes.ForEach((x) => x.FeedForward());
        }

        public Perceptron Clone()
        {
            var clone = new Perceptron(InputNodes.Count, OutputNodes.Count);

            for (int inode = 0; inode < InputNodes.Count; ++inode)
            {
                for (int iconnect = 0; iconnect < InputNodes[inode].Connectors.Count; ++iconnect)
                {
                    clone.InputNodes[inode].Connectors[iconnect].Weight =
                        InputNodes[inode].Connectors[iconnect].Weight;
                }
            }

            return clone;
        }

        public Perceptron RandomClone(double standardDeviation)
        {
            var clone = Clone();

            clone.RandomWeights(standardDeviation);

            return clone;
        }

        public double GetOutput(int index)
        {
            return OutputNodes[index].GetValue();
        }

        public void RandomWeights(double standardDeviation)
        {
            foreach (var node in InputNodes)
            {
                foreach (var connector in node.Connectors)
                {
                    connector.Weight = Random.NextGaussian(connector.Weight, standardDeviation);
                }
            }
        }

        public void WriteToFile(string filename)
        {
            using var bw = new BinaryWriter(File.Create(filename));

            var nodeDict = new Dictionary<Node, int>();
            int counter = 0;

            // Input nodes
            bw.Write(InputNodes.Count);
            foreach (var node in InputNodes)
            {
                nodeDict.Add(node, counter++);
            }

            // Output nodes
            bw.Write(OutputNodes.Count);
            foreach (var node in OutputNodes)
            {
                nodeDict.Add(node, counter++);
            }

            // Connectors
            bw.Write(Connectors.Count());
            foreach (var node in InputNodes)
            {
                foreach (var connector in node.Connectors)
                {
                    bw.Write(nodeDict[node]);
                    bw.Write(nodeDict[connector.Node]);
                    bw.Write(connector.Weight);
                }
            }
        }

        public Perceptron(string filename)
        {
            using var br = new BinaryReader(File.OpenRead(filename));

            int nInputNodes = br.ReadInt32();
            int nOutputNodes = br.ReadInt32();

            var nodeDict = new Dictionary<int, Node>();
            for (int i = 0; i < nInputNodes; ++i)
            {
                var newNode = new Node();
                InputNodes.Add(newNode);
                nodeDict.Add(i, newNode);
            }
            for (int i = nInputNodes; i < nInputNodes + nOutputNodes; ++i)
            {
                var newNode = new Node();
                OutputNodes.Add(newNode);
                nodeDict.Add(i, newNode);
            }

            int nConnectors = br.ReadInt32();
            for (int i = 0; i < nConnectors; ++i)
            {
                int inputNode = br.ReadInt32();
                var connector = new Connector();
                int outputNode = br.ReadInt32();
                connector.Node = nodeDict[outputNode];
                connector.Weight = br.ReadDouble();

                nodeDict[inputNode].Connectors.Add(connector);
            }
        }
    }
}