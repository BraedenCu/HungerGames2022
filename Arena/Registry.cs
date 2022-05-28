using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arena
{
    public class Registry
    {
        private List<GraphicInfo> typeMap = new List<GraphicInfo>();
        private Dictionary<GraphicInfo, int> codeMap = new Dictionary<GraphicInfo, int>();

        static private object locker = new();

        public string ImageDirectory { get; private set; }

        public void Initialize(string directory, string suffix = "")
        {
            if (directory == "")
            {
                ImageDirectory = Directory.GetCurrentDirectory() + "\\" + suffix;
            }
            else
            {
                string dir = Directory.GetCurrentDirectory();
                string comparator = directory;
                var index = dir.LastIndexOf(comparator);
                var length = comparator.Length;
                ImageDirectory = dir.Substring(0, index + length) + suffix;
            }
        }

        public GraphicInfo GetInfo(int graphicCode)
        {
            return typeMap[graphicCode];
        }

        public int GetGraphicCode(GraphicInfo obj)
        {
            return codeMap[obj];
        }

        public List<GraphicInfo> GetAllGraphicInfo()
        {
            return typeMap;
        }

        public int AddEntry(GraphicInfo obj)
        {
            lock (locker)
            {
                if (codeMap.ContainsKey(obj))
                {
                    return GetGraphicCode(obj);
                }
                typeMap.Add(obj);
                codeMap.Add(obj, typeMap.Count - 1);
                return typeMap.Count - 1;
            }
        }

        internal void AddEntryWithIndex(GraphicInfo obj, int index)
        {
            lock (locker)
            {
                while (typeMap.Count <= index)
                {
                    typeMap.Add(null);
                }
                if (typeMap[index] != null && typeMap[index].Filename.Length != 0)
                {
                    var a = typeMap;
                    var b = typeMap[index];

                    // Maybe this is okay
                    //throw new ArgumentException("Something went wrong in reloading registries from file");
                }
                typeMap[index] = obj;
                codeMap.Add(obj, index);
            }
        }

        public void WriteToFile(BinaryWriter bw)
        {
            bw.Write("MAIN");
            bw.Write(typeMap.Count);

            for (int i = 0; i < typeMap.Count; ++i)
            {
                var gi = typeMap[i];
                bw.Write(gi.Filename);
                bw.Write(gi.XSize);
                bw.Write(gi.YSize);
            }
        }

        public void Clear()
        {
            typeMap.Clear();
            codeMap.Clear();
        }

        public void Read(BinaryReader br)
        {
            string header = br.ReadString();
            if (header != "MAIN")
            {
                throw new FileNotFoundException("Invalid file passed to MainRegistry.FillFromFile()!");
            }

            int length = br.ReadInt32();

            Clear();

            for (int i = 0; i < length; ++i)
            {
                string filename = br.ReadString();
                double xSize = br.ReadDouble();
                double ySize = br.ReadDouble();

                var gi = new GraphicInfo(filename, xSize, ySize);
                AddEntry(gi);
            }
        }
    }
}
