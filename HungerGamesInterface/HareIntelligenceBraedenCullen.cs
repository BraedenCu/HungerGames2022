using Arena;
using DongUtility;
using HungerGames.Animals;
using HungerGames.Interface;
using HungerGamesCore.Terrain;
using System;
using System.Drawing;
using System.Linq;
using HungerGames.Animals;
using HungerGamesCore.Interface;

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
            //TODO defect check animals not null
            VisibleAnimal nearestLynx = animals[0];

            double distanceToNearestLynx = Vector2D.Distance2(nearestLynx.Position, Position);
            var sounds = Listen().ToList();
            if (sounds.Count > 0 && sounds[0].SoundCode == 191)
            {
                //Console.WriteLine("vocalizing");
                return Vocalize(10, 192);
            }

            bool lynxFound = false;
            foreach (var ani in animals)
            {
                if (ani.IsLynx == true)
                {
                    lynxFound = true;
                    nearestLynx = ani;
                }
            }
            if (animals == null || animals.Count() < 1 || lynxFound == false)
            {
                //nearestLynx = null;
                return Wait();
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
            //adding one more imput for attacking state -> TODO check the distance from the DIRECTION OF VELOCITY to the hare, like to see if the hare is being targetted
            //nearestLynx.Velocity.Magnitude

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
                return Hide();
            }
            return Wait();
        }

        public Turn Hide()
        {
            return ChangeVelocity(Vector2D.PolarVector(0, 0));
            VisibleObstacle closestGrass = null;
            double bestDistance = -1;
            foreach (var obstacle in GetObstacles<Grass>().ToList())
            {
                double distance = Vector2D.Distance2(obstacle.Position, Position);
                if (closestGrass == null || bestDistance == -1 || distance < bestDistance)
                {
                    closestGrass = obstacle;
                }
            }
            if(closestGrass != null)
            {
                Vector2D direction = closestGrass.Position - Position;
                return ChangeVelocity(-direction * 2); //*5
            }
            else
            {
                return DefaultMovementAwayFromLynx();
            }
        }

        public Turn DefaultMovementAwayFromLynx()
        {
            const double distanceLimit2 = 40; //default 25 5

            var animals = GetAnimalsSorted().ToList();
            foreach (var ani in animals)
            {
                if (ani.IsLynx && Vector2D.Distance2(Position, ani.Position) < distanceLimit2)
                { 
                    Vector2D direction = ani.Position - Position;
                    double distance = Vector2D.Distance2(ani.Position, Position);
                    double velocity = 10 / (distance / 3);
                    //return Vocalize(5, 10);
                    //TODO -> make a new perceptron for velocity (copy the one from homework nine) that takes in the position of nearest lynx and current hare and outputs x and y velocity
                    //TODO -> hardcode so hares never go to walls
                    //Console.WriteLine("Distance: " + distance + "  velocity: " + velocity);
                    return ChangeVelocity(-direction * velocity); //default 5 
                }
            }
            return Wait();
            //return ChangeVelocity(Vector2D.PolarVector(1, Random.NextDouble(0, 2 * Math.PI)));
        }

        public Turn Wait()
        {
            //velocity 0
            return ChangeVelocity(Vector2D.PolarVector(0, 0));
        }
    }
}