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
        static List<int> codes = new List<int>();
        static List<int> myHaresId = new List<int>();

        public override Turn ChooseTurn()
        {
            var animals = GetAnimalsSorted().ToList();
            if (animals.Count > 0)
            {
                foreach (var ani in animals)
                {
                    //if (ani.GetType() == typeof(HungerGames.Animals.IntelligentHare<HungerGames.HareIntelligenceBraedenCullen>))
                    //{
                    //    Console.WriteLine("test");
                    //}
                    if (!ani.IsLynx && (ani.Species != 0 && ani.Species != 3))
                    {
                        Vector2D direction = ani.Position - Position;
                        return ChangeVelocity(direction.UnitVector() * 4);
                    }
                }
            }

            return ChangeVelocity(Vector2D.PolarVector(1, Random.NextDouble(0, 2 * Math.PI)));
        }

        /*
        public override Turn ChooseTurn()
        {
            var animals = GetAnimalsSorted().ToList();
            if (animals.Count > 0)
            {
                var sounds = Listen().ToList();
                if (sounds.Count > 0 && sounds[0].SoundCode == 192)
                {
                    //Console.WriteLine(myHaresId);
                    if (myHaresId.Count < 1)
                    {
                        foreach(var ani in animals)
                        {
                            Console.WriteLine("run thru: " + ani.Species);
                            if(ani.IsLynx == false)
                            {
                                if(myHaresId.Contains(ani.Species))
                                {
                                    break;
                                }
                                else
                                {
                                    myHaresId.Add(ani.Species);
                                    Console.WriteLine("added: " + (ani.Species));
                                }
                                break;
                            }
                        }
                    }
                    IDs.Add(animals[0].ID);
                    //badIDs.Remove(ani.ID);
                }

                foreach (var ani in animals)
                {
                    /*
                    if (!IDs.Contains(ani.ID))
                    {
                        if (!ani.IsLynx)
                        {
                            Vector2D direction = ani.Position - Position;
                            double distance = Vector2D.Distance2(ani.Position, Position);

                            if (badIDs.Contains(ani.ID))
                            {
                                return ChangeVelocity(direction.UnitVector() * 4);
                            }
                            else if (distance < 6 && !badIDs.Contains(ani.ID))
                            {
                                return Vocalize(6, 192);
                                badIDs.Add(ani.ID);
                            }

                            return ChangeVelocity(direction.UnitVector() * 4);
                        }
                    }*/
        /*
                    if (!ani.IsLynx)
                    {
                        if(myHaresId.Contains(ani.Species))
                        {
                            continue;
                        }
                        else
                        {
                            Vector2D direction = ani.Position - Position;
                            double distance = Math.Sqrt(Vector2D.Distance2(ani.Position, Position));
                            if (distance < 1 && myHaresId.Count < 1) //  && myHaresId == -1
                            {
                                return Vocalize(5, 191);
                            }
                            else
                            {
                                return ChangeVelocity(direction.UnitVector() * 4);
                            }
                        }
                        //return ChangeVelocity(direction.UnitVector() * 4);
                    }
                }
            }
            return ChangeVelocity(Vector2D.PolarVector(1, Random.NextDouble(0, 2 * Math.PI)));
        }
        */
    }
}

