using Arena;
using DongUtility;
using HungerGames.Animals;
using HungerGamesCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HungerGames.Interface
{
    abstract public class LocationChooser
    {
        public Vector2D ChooseLocation(VisibleArena arena, bool hare, int organismNumber)
        {
            try
            {
                Vector2D coord = UserDefinedChooseLocation(arena, hare, organismNumber);
                return Check(arena, coord, hare);
            }
            catch
            {
                return RandomLocation(arena, hare);
            }
        }

        private Vector2D Check(VisibleArena arena, Vector2D coord, bool hare)
        {
            if (CheckPoint(arena, coord, hare))
            {
                return coord;
            }
            else
            {
                return RandomLocation(arena, hare);
            }
        }

        protected virtual Vector2D UserDefinedChooseLocation(VisibleArena arena, bool hare, int organismNumber)
        {
            return RandomLocation(arena, hare);
        }

        private Rectangle GetSize(bool hare)
        {
            var animal = MakeOrganism(null, hare);
            return animal.Size;
        }

        protected Vector2D RandomLocation(VisibleArena arena, bool hare)
        {
            Vector2D center;
            do
            {
                center = new Vector2D(ArenaEngine.Random.NextDouble(0, arena.Width),
                    ArenaEngine.Random.NextDouble(0, arena.Height));
            } while (!CheckPoint(arena, center, hare));
            return center;
        }

        static protected Random Random => ArenaEngine.Random;

        protected bool CheckPoint(VisibleArena arena, Vector2D location, bool hare)
        {
            Rectangle size = GetSize(hare);

            var rect = new Rectangle(location, size.Width, size.Height);

            return arena.TestArea(rect);
        }

        abstract public Animal MakeOrganism(ArenaEngine arena, bool hare);

        public string GetName(bool hare)
        {
            var org = MakeOrganism(null, hare);
            return org.Name;
        }

        protected Animal MakeOrganism<T>(ArenaEngine arena, bool hare) where T : Intelligence
        {
            if (hare)
                return MakeOrganism(arena, typeof(IntelligentHare<>), typeof(T));
            else
                return MakeOrganism(arena, typeof(IntelligentLynx<>), typeof(T));
        }

        private Animal MakeOrganism(ArenaEngine arena, Type org, Type intel)
        {
            var constructed = org.MakeGenericType(new Type[] { intel });
            var response = Activator.CreateInstance(constructed, new object[] {arena});
            return (Animal)response;
        }
    }
}
