using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HungerGames.Turns;
using Arena;
using System.IO;
using HungerGames.Interface;
using DongUtility;
using System.Drawing;

namespace HungerGames.Animals
{
    abstract public class IntelligentOrganism : MovingObject
    {
        private Intelligence intelligence;

        public override string Name { get { return intelligence.Name; } }
        public Color Color { get { return intelligence.Color; } }

        private const int orgLayer = 2;

        public IntelligentOrganism(HungerGamesArena arena, Intelligence intel, double width, double height) :
            base(0, orgLayer, width, height)
        {
            intelligence = intel;
            intelligence.Organism = this;
            if (arena != null)
            {
                Arena = arena;
                GraphicCode = arena.GetGraphicsCode(intel, width, height);
                SpeciesCode = arena.GetSpeciesCode(intel);
            }
        }

        public int SpeciesCode { get; }

        protected Turn HungerGamesChooseAction()
        {
            try
            {
                return intelligence.ChooseTurn();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
