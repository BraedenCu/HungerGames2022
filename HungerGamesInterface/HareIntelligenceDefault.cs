using Arena;
using DongUtility;
using HungerGamesCore.Terrain;
using System;
using System.Drawing;
using System.Linq;

namespace HungerGames.Interface
{
    public class HareIntelligenceDefault : HareIntelligence
    {
        public override Color Color { get { return Color.HotPink; } }
        public override string Name { get { return "Default Hare"; } }
        public override string BitmapFilename { get { return "rabbit.jpg"; } }

        public override Turn ChooseTurn()
        {
            const double distanceLimit2 = 25;

            var animals = GetAnimalsSorted().ToList();
            foreach (var ani in animals)
            {
                if (ani.IsLynx && Vector2D.Distance2(Position, ani.Position) < distanceLimit2)
                {
                    Vector2D direction = ani.Position - Position;
                    return ChangeVelocity(-direction * 5);
                }
            }
            
            return ChangeVelocity(Vector2D.PolarVector(1, Random.NextDouble(0, 2 * Math.PI)));
        }
    }
}
