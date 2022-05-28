using DongUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HungerGamesCore.Animals
{
    public class Sound
    {
        public double Time { get; }
        public Vector2D Direction { get; }
        public double Magnitude { get; }
        // The code distinguishing different vocalizations.  Code 0 is for walking noise and may not be vocalized
        public byte SoundCode { get; }

        internal Sound(double time, Vector2D direction, double magnitude, byte soundCode)
        {
            Time = time;
            Direction = direction;
            Magnitude = magnitude;
            SoundCode = soundCode;
        }
    }
}
