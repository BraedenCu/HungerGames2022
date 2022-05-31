using Arena;
using DongUtility;
using HungerGames.Animals;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HungerGames.Interface
{
    public class LynxIntelligenceBraedenCullen : LynxIntelligence
    {
        public override Color Color { get { return Color.Black; } }
        public override string Name { get { return "MyLynx"; } }
        public override string BitmapFilename { get { return "default.png"; } }
        //list of animal ids that i save
        static List<int> IDs = new List<int>();
        static List<int> badIDs = new List<int>();

        public override Turn ChooseTurn()
        {
            var animals = GetAnimalsSorted().ToList();
            if (animals.Count > 0)
            {
                foreach (var ani in animals)
                {
                    if(!IDs.Contains(ani.ID))
                    {
                        if (!ani.IsLynx)
                        {
                            var sounds = Listen().ToList();
                            if (sounds[0].SoundCode == 192)
                            {
                                IDs.Add(ani.ID);
                                //badIDs.Remove(ani.ID);
                            }
                            Vector2D direction = ani.Position - Position;
                            double distance = Vector2D.Distance2(ani.Position, Position);

                            if (badIDs.Contains(ani.ID))
                            {
                                return ChangeVelocity(direction.UnitVector() * 4);
                            }
                            else if (distance < 1 || !badIDs.Contains(ani.ID))
                            {
                                return Vocalize(1, 192);
                                badIDs.Add(ani.ID);
                            }

                            return ChangeVelocity(direction.UnitVector() * 4);
                        }
                    }
                }
            }
            return ChangeVelocity(Vector2D.PolarVector(1, Random.NextDouble(0, 2 * Math.PI)));
        }
    }
}

