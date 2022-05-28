using Arena;
using HungerGames.Animals;
using HungerGames.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DongUtility;
using System.Collections.Concurrent;
using HungerGamesCore.Interface;

namespace HungerGames
{
    public class GameMaster
    {
        private HungerGamesArena arena;
        public List<LocationChooser> Choosers { get; } = new List<LocationChooser>();
        private readonly VisibleArena va;
        private ConcurrentBag<Tuple<Vector2D, Animal>> allAnimals = new ConcurrentBag<Tuple<Vector2D, Animal>>();

        public GameMaster(HungerGamesArena arena)
        {
            this.arena = arena;
            va = new VisibleArena(arena);
        }

        public void AddChooser(LocationChooser lc)
        {
            Choosers.Add(lc);
        }

        public void AddAllAnimals(int nHare, int nLynx)
        {
            Parallel.ForEach(Choosers, (chooser) =>
            {
                for (int ihare = 0; ihare < nHare; ++ihare)
                {
                    DoChoice(chooser, true, ihare);
                }
                for (int ilynx = 0; ilynx < nLynx; ++ilynx)
                {
                    DoChoice(chooser, false, ilynx);
                }
            });

            foreach (var entry in allAnimals)
            {
                Vector2D position = entry.Item1;
                var animal = entry.Item2;
                var rectangle = new Rectangle(position, animal.Size.Width, animal.Size.Height);
                if (!arena.IsValidLocation(rectangle))
                {
                    position = MoveAnimalToRandomLocation(arena, entry.Item2);
                }
                arena.AddObject(animal, position);
            }

            // let arena know which lynx goes with which hare, for scoring
            foreach (var chooser in Choosers)
            {
                int harecode = -1, lynxcode = -1;
                var hares = arena.GetObjectsOfType<Hare>();
                foreach (var hare in hares)
                {
                    if (hare.Name == chooser.GetName(true))
                    {
                        harecode = hare.SpeciesCode;
                        break;
                    }
                }
                var lynxes = arena.GetObjectsOfType<Lynx>();
                foreach (var lynx in lynxes)
                {
                    if (lynx.Name == chooser.GetName(false))
                    {
                        lynxcode = lynx.SpeciesCode;
                        break;
                    }
                }
                if (harecode == -1 || lynxcode == -1)
                    throw new Exception("Something has gone seriously wrong.");

                arena.HareToLynxMapping.Add(harecode, lynxcode);
            }

            //foreach (var animal in arena.GetObjectsOfType<Lynx>())
            //{
            //    if (animal.Name == "Default Lynx")
            //        arena.RemoveObjectDelay(animal);
            //}
        }

        private Vector2D MoveAnimalToRandomLocation(HungerGamesArena arena, Animal animal)
        {
            Rectangle rect;
            do
            {
                Vector2D position = new Vector2D(ArenaEngine.Random.NextDouble(animal.Size.Width,
                    arena.Width - animal.Size.Width),
                    ArenaEngine.Random.NextDouble(animal.Size.Height, arena.Height - animal.Size.Height));
                rect = new Rectangle(position, animal.Size.Width, animal.Size.Height);
            } while (!arena.IsValidLocation(rect));
            return rect.Center;
        }

        private void DoChoice(LocationChooser lc, bool hare, int counter)
        {
            var location = lc.ChooseLocation(va, hare, counter);
            var animal = lc.MakeOrganism(arena, hare);
            // Store in a list first so that they can't see each others' choices
            allAnimals.Add(new Tuple<Vector2D, Animal>(location, animal));
        }
    }
}
