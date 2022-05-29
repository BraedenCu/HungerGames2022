using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arena;
using DongUtility;
using HungerGames;
using HungerGames.Interface;
using HungerGamesCore.Interface;
using HungerGamesCore.Terrain;
using HungerGames;

namespace HungerGames.Animals
{
    abstract public class Hare : Animal
    {
        //perceptron given to each hare during TRAINING PROCESS
        //TODO test without this code because dong wont have it when running the simulation
        public Perceptron Perceptron { get; set; } = new Perceptron(4, 2);

        static private readonly AnimalStats hareStats = new AnimalStats()
        {
            BaseWalkingVolume = 1e-8,
            HearingThreshold = 1e-9,
            MaxAcceleration = 3,
            MaxSpeed = 12,
            MaxStamina = 75,
            MaxVocalizationVolume = 1e-4,
            MaxWalkingVolume = 1e-6,
            StaminaPerSecondAtTopSpeed = 7,
            StaminaRestoredPerSecond = 2,
            StepTime = .25,
            WalkingSpeed = .5,
            VisionBase = 40
        };

        private const double hareWidth = .2;
        private const double hareLength = .2;

        public Hare(HungerGamesArena arena, HareIntelligence intel) :
            base(arena, intel, hareStats, hareLength, hareWidth) // So the hare is horizontal
        { }
    }
}
