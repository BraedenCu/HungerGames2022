using DongUtility;
using HungerGames.Animals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HungerGames
{
    public class VisibleAnimal
    {
        internal Animal Animal { get; }

        public VisibleAnimal(Animal animal)
        {
            Animal = animal;
            ID = counter++;
        }

        // Just so you can tell animals apart
        static private int counter = 0;
        public int ID { get; }

        public double Time => Animal.Arena.Time;
        public Vector2D Position => Animal.Position;
        public Vector2D Velocity => Animal.Velocity;
        public Rectangle Size => Animal.Size;
        public int Species => Animal.SpeciesCode;
        public bool IsLynx => Animal is Lynx;
        public bool IsDead => Animal.Dead;
    }
}
