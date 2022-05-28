using Arena;
using DongUtility;
using System;
using System.Drawing;
using System.Linq;

namespace HungerGames.Interface
{
    public class LynxIntelligenceDefault : LynxIntelligence
    {
        public override Color Color { get { return Color.Thistle; } }
        public override string Name { get { return "Default Lynx"; } }
        public override string BitmapFilename { get { return "lynx.png"; } }

        public override Turn ChooseTurn()
        {
 
            var animals = GetAnimalsSorted().ToList();
            if (animals.Count > 0)
            {
                foreach (var ani in animals)
                {
                    if (!ani.IsLynx)
                    {
                        Vector2D direction = ani.Position - Position;
                        return ChangeVelocity(direction.UnitVector() * 4);
                    }
                }
            }

            return ChangeVelocity(Vector2D.PolarVector(1, Random.NextDouble(0, 2 * Math.PI)));
        }

    }
}
