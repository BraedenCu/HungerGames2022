using Arena;
using DongUtility;
using HungerGames.Animals;
using HungerGames.Interface;
using HungerGamesCore.Terrain;
using System;
using System.Drawing;
using System.Linq;
using HungerGames.Animals;

namespace HungerGames
{
    public class HareIntelligenceBraedenCullen : HareIntelligence
    {
        public override Color Color { get { return Color.RosyBrown; } }
        public override string Name { get { return "dogs"; } }
        public override string BitmapFilename { get { return "default.png"; } }

        public Perceptron Perceptron { get; set; } = new Perceptron(4, 2);

        static string fileNameForSavingBest = Directory.GetCurrentDirectory() + "\\" + "bestPerceptron";

        /*
        public override Turn ChooseTurn()
        { 
            return ChangeVelocity(Vector2D.PolarVector(1, Random.NextDouble(0, 2 * Math.PI)));
        }*/

        public override Turn ChooseTurn()
        {
            var animals = GetAnimalsSorted().ToList();


            Perceptron readPerceptron = new Perceptron(fileNameForSavingBest);

            Perceptron.Reset();

            Perceptron.AddInput(0, Position.X);
            Perceptron.AddInput(1, Position.Y);
            Perceptron.AddInput(2, animals[0].Position.X);
            Perceptron.AddInput(3, animals[0].Position.Y);

            Perceptron.Run();

            double x = Perceptron.GetOutput(0);
            double y = Perceptron.GetOutput(1);
            Vector2D returnMovement = new Vector2D(x, y);

            if (!UtilityFunctions.IsValid(x) || !UtilityFunctions.IsValid(y))
            {
                returnMovement = new Vector2D(0, 0);   
                return ChangeVelocity(returnMovement);
            }

            return ChangeVelocity(returnMovement);
        }

    }
}