using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Arena
{
    public class GraphicInfo
    {
        public string Filename { get; }
        public double XSize { get; }
        public double YSize { get; }

        public GraphicInfo(string pictureFile, double xsize, double ysize)
        {
            XSize = xsize;
            YSize = ysize;
            Filename = pictureFile;
        }

        public override string ToString()
        {
            return $"{Filename}_{XSize}_{YSize}";
        }

        static public bool operator==(GraphicInfo lhs, GraphicInfo rhs)
        {
            return lhs.Filename == rhs.Filename && lhs.XSize == rhs.XSize && lhs.YSize == rhs.YSize;
        }

        static public bool operator!=(GraphicInfo lhs, GraphicInfo rhs)
        {
            return !(lhs == rhs);
        }

        public void WriteToFile(BinaryWriter bw)
        {
            bw.Write(Filename);
            bw.Write(XSize);
            bw.Write(YSize);
        }

        public GraphicInfo(BinaryReader br)
        {
            Filename = br.ReadString();
            XSize = br.ReadDouble();
            YSize = br.ReadDouble();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            return obj is GraphicInfo info && this == info;
        }

        public override int GetHashCode()
        {
            return Filename.GetHashCode() ^ XSize.GetHashCode() ^ YSize.GetHashCode();
        }
    }
}
