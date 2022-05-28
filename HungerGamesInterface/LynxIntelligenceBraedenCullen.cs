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
        public override string Name { get { return "Your Predator name"; } }
        public override string BitmapFilename {  get { return "default.png"; } }

        public override Turn ChooseTurn()
        {
            return ChangeVelocity(Vector2D.PolarVector(1, Random.NextDouble(0, 2 * Math.PI)));
        }
    }
}
