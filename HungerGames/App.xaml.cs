using Arena;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace HungerGames
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            HungerGamesTest.Run();
            //HungerGamesDriver.RunHungerGames();
            //HungerGamesDriver.RunCastOfCharacters();
            //HungerGamesDriver.RunAll();
            //HungerGamesDriver.OneOnOne("Xander", "Advait", 100);
            //HungerGamesDriver.PreprocessHungerGames();
            //HungerGamesDriver.PlayFromFile("Teodor_vs_Evie.hg", false);
            //HungerGamesDriver.PlayFromFile("hungerGamesBattleRoyale.hg", true);
            //HungerGamesDriver.PreprocessRound();
        }
    }
}
