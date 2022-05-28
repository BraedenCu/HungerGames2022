using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arena;
using DongUtility;
using HungerGames.Animals;
using HungerGames.Interface;
using HungerGamesCore.Interface;
using HungerGamesCore.Terrain;

namespace HungerGames
{
    public class ChooserYOURNAME : LocationChooserTemplateIntermediate<HareIntelligenceYOURNAME, LynxIntelligenceYOURNAME>
    {
        protected override Vector2D UserDefinedChooseLocation(VisibleArena arena, bool hare, int organismNumber)
        {
            //EDITING THIS FILE IS NOT NECESSARY, IT IS FULLY FUNCITONAL THE WAY IT IS BUT REPRESENTS A BONUS FEATURE OF SORTS

            // Fill in your decision algorithm here.
            // The organismNumber variable counts from 0 to the total number of organisms.  This can be useful for
            // spreading your organisms out a bit
            var water = arena.GetObstacles<Water>();
            //return new Vector2D(ArenaEngine.Random.NextDouble(0, 15), ArenaEngine.Random.NextDouble(0, 15));
            return RandomLocation(arena, hare);
        }
    }
}
