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
        public override string Name { get { return "MyHares"; } }
        public override string BitmapFilename { get { return "pink.png"; } }

        public Perceptron Perceptron { get; set; } = new Perceptron(5, 3);

        //static string fileNameForSavingBest = Directory.GetCurrentDirectory() + "\\" + "bestPerceptron-great-spring";
        //static string fileNameForSavingBest = "C:\\Users\\dev\\dev\\finalProject\\HungerGames2022-master\\HungerGamesInterface\\bestPerceptron-great-spring";
        static string fileNameForSavingBest = @"C:\\Users\\dev\\dev\finalProject\\HungerGames2022-master\\HungerGamesInterface\\bestPerceptron-good-circle copy";

        /*
        public override Turn ChooseTurn()
        { 
            return ChangeVelocity(Vector2D.PolarVector(1, Random.NextDouble(0, 2 * Math.PI)));
        }
        */

        public override Turn ChooseTurn()
        {
            var animals = GetAnimalsSorted().ToList();
            var nearestLynx = animals[0];

            foreach(var ani in animals)
            {
                if(ani.IsLynx == true)
                {
                    nearestLynx = ani;
                }
            }
            Perceptron.Reset();

            //position of current hare
            //distance too nearest lynx
            //distance to nearest bush
            //stamina

            Perceptron.AddInput(0, Position.X);
            Perceptron.AddInput(1, Position.Y);
            Perceptron.AddInput(2, nearestLynx.Position.X);
            Perceptron.AddInput(3, nearestLynx.Position.Y);
            Perceptron.AddInput(4, Stamina);
            
            Perceptron.Run();

            /*
            double x = Perceptron.GetOutput(0);
            double y = Perceptron.GetOutput(1);
            Vector2D returnMovement = new Vector2D(x, y);

            if (!UtilityFunctions.IsValid(x) || !UtilityFunctions.IsValid(y))
            {
                returnMovement = new Vector2D(0, 0);   
                return ChangeVelocity(returnMovement);
            }

            return ChangeVelocity(returnMovement);
            */

            double move = Perceptron.GetOutput(0);
            double hide = Perceptron.GetOutput(1);
            double wait = Perceptron.GetOutput(2);

            if (move == UtilityFunctions.Max(move, hide, wait))
            {
                return DefaultMovementAwayFromLynx();
            }
            else if (hide == UtilityFunctions.Max(move, hide, wait))
            {
                Hide();
            }
            return Wait();
        }

        public Turn Hide()
        {
            //return ChangeVelocity(Vector2D.PolarVector(0, 0));
            return DefaultMovementAwayFromLynx();
        }

        public Turn DefaultMovementAwayFromLynx()
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

        public Turn Wait()
        {
            //velocity 0
            return ChangeVelocity(Vector2D.PolarVector(0, 0));
        }
    }
}