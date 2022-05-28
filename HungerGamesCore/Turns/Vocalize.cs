using Arena;
using HungerGames.Animals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HungerGamesCore.Turns
{
    internal class Vocalize : Turn
    {
        private double volume;
        private byte soundCode;

        public Vocalize(Animal animal, double volume, byte soundCode) :
            base(animal)
        {
            this.volume = volume;
            this.soundCode = soundCode;
        }

        public override bool DoTurn()
        {
            if (soundCode == 0)
                return false;

            ((Animal)Owner).Vocalize(volume, soundCode);
            return true;
        }
    }
}
