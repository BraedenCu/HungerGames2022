using Arena;
using HungerGames;
using HungerGames.Animals;
using HungerGamesCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HungerGamesCore.Terrain
{
    abstract public class Obstacle : StationaryObject
    {
        private const int obstacleLayer = 1;

        public Obstacle(HungerGamesArena arena, string filename, double width, double height) :
            base(0, obstacleLayer, width, height)
        {
            Arena = arena;
            GraphicCode = arena.GetGraphicsCode(GetType(), filename, width, height);
            VisibleObstacle = new VisibleObstacle(this);
        }

        abstract public double VisionReductionPerMeter { get; }

        abstract public double VelocityReduction { get; }
        
        public VisibleObstacle VisibleObstacle { get; }
    }
}
