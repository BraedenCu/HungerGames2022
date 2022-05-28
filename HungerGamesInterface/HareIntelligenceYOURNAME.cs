using Arena;
using DongUtility;
using HungerGames.Animals;
using HungerGames.Interface;
using HungerGamesCore.Terrain;
using System;
using System.Drawing;
using System.Linq;

namespace HungerGames
{
    public class HareIntelligenceYOURNAME : HareIntelligence
    {
        public override Color Color { get { return Color.HotPink; } }
        public override string Name { get { return "Your Herbivore name"; } }
        public override string BitmapFilename { get { return "default.png"; } }

        public override Turn ChooseTurn()
        { 
            return ChangeVelocity(Vector2D.PolarVector(1, Random.NextDouble(0, 2 * Math.PI)));
        }

    }
}
