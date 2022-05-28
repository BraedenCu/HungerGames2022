using Arena;
using HungerGames.Animals;
using HungerGames.Interface;
using HungerGames.Turns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DongUtility;
using HungerGamesCore.Interface;
using HungerGamesCore.Terrain;
using HungerGamesCore.Animals;
using HungerGamesCore.Turns;

namespace HungerGames.Interface
{
    abstract public class AnimalIntelligence : Intelligence
    {
        private protected Animal Animal => (Animal)Organism;

        protected VisibleAnimal VisibleAnimal => new VisibleAnimal(Animal);

        public bool IsSameSpecies(VisibleAnimal ani)
        {
            return ani.Species == Animal.SpeciesCode;
        }

        public bool IsBattleRoyale => ((HungerGamesArena)(Animal.Arena)).IsBattleRoyale;

        protected Vector2D Position => Animal.Position;
        protected Rectangle Size => Animal.Size;
        protected Vector2D Velocity => Animal.Velocity;
        protected double Stamina => Animal.Stamina;

        protected IEnumerable<VisibleAnimal> GetOtherAnimals<T>() where T : Animal
        {
            foreach (var ani in Animal.GetVisibleObjects<T>())
            {
                if (ani != Animal)
                    yield return ani.VisibleAnimal;
            }
        }

        protected IEnumerable<VisibleObstacle> GetObstacles<T>() where T : Obstacle
        {
            return from obstacles in Animal.GetVisibleObjects<T>()
                   select obstacles.VisibleObstacle;
        }

        protected IEnumerable<Sound> Listen()
        {
            foreach (var sound in Animal.Sounds)
                yield return sound;
        }

        protected IEnumerable<VisibleAnimal> GetAnimalsSorted()
        {
            var response = new List<VisibleAnimal>();
            foreach (var ani in Animal.GetVisibleObjects<Animal>())
            {
                if (!ani.Dead && ani != Animal)
                {
                    response.Add(new VisibleAnimal(ani));
                }
            }

            response.Sort(new AnimalComparer(Animal));
            return response;
        }

        private class AnimalComparer : IComparer<VisibleAnimal>
        {
            private readonly Animal thisAnimal;

            public AnimalComparer(Animal thisAnimal)
            {
                this.thisAnimal = thisAnimal;
            }

            public int Compare(VisibleAnimal one, VisibleAnimal two)
            {
                double d1 = Distance2(one, thisAnimal);
                double d2 = Distance2(two, thisAnimal);

                return d1.CompareTo(d2);
            }

            private double Distance2(VisibleAnimal one, Animal two)
            {
                return Vector2D.Distance2(one.Position, two.Position);
            }
        }

        protected Turn ChangeVelocity(Vector2D deltaV)
        {
            return new ChangeVelocity(Animal, deltaV);
        }

        protected Turn Vocalize(double volume, byte soundCode)
        {
            return new Vocalize(Animal, volume, soundCode);
        }
    }
}
