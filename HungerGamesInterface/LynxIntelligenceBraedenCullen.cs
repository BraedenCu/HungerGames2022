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
        public static int spiciesId = -1;
        public override Color Color { get { return Color.Black; } }
        public override string Name { get { return "MyLynx"; } }
        public override string BitmapFilename { get { return "default.png"; } }
        //list of animal ids that i save
        static List<int> IDs = new List<int>();
        static List<int> badIDs = new List<int>();
        static List<int> codes = new List<int>();
        static List<int> myHaresId = new List<int>();

        /*public override Turn ChooseTurn()
        {
            var animals = GetAnimalsSorted().ToList();
            if (animals.Count > 0)
            {
                foreach (var ani in animals)
                {
                    //if (ani.GetType() == typeof(HungerGames.Animals.IntelligentHare<HungerGames.HareIntelligenceBraedenCullen>))
                    //{
                    //    Console.WriteLine("test");
                    //}
                    if (!ani.IsLynx && (ani.Species != 0 && ani.Species != 3))
                    {
                        Vector2D direction = ani.Position - Position;
                        return ChangeVelocity(direction.UnitVector() * 4);
                    }
                }
            }

            return ChangeVelocity(Vector2D.PolarVector(1, Random.NextDouble(0, 2 * Math.PI)));
        }*/

        
        public override Turn ChooseTurn()
        {

            //TODO there is no perceptron for Lynx
            var animals = GetAnimalsSorted().ToList();
            if (animals.Count > 0)
            {
                var sounds = Listen().ToList();//not sure if needed
                if (sounds.Count > 0 && sounds[0].SoundCode == HareIntelligenceBraedenCullen.voice)
                {
                    if(!animals[0].IsLynx){
                        double distance = Vector2D.Distance2(animals[0].Position, Position);
                        if(distance<HareIntelligenceBraedenCullen.soundDistance){//should be close enough or probably false positive
                        //if(sounds[0].Magnitude>.0004){
                            HareIntelligenceBraedenCullen.spiciesId = animals[0].Species;
                            Console.WriteLine("Hare Species Found:"+HareIntelligenceBraedenCullen.spiciesId);
                        }
                    }
                }

            }

            foreach(var ani in animals){
                if(!ani.IsLynx){//is hare
                    Vector2D direction = ani.Position - Position;
                    double distance = Vector2D.Distance2(ani.Position, Position);
                    if(HareIntelligenceBraedenCullen.spiciesId !=-1 && ani.Species!=HareIntelligenceBraedenCullen.spiciesId){
                        return ChangeVelocity(direction.UnitVector()*4);//attach it
                    }
                    
                    double soundDistance = HareIntelligenceBraedenCullen.soundDistance;
                    if (HareIntelligenceBraedenCullen.spiciesId==-1 && distance < soundDistance){
                        return Vocalize(soundDistance, 192);
                    }

                    if(HareIntelligenceBraedenCullen.spiciesId ==-1){ //i dont know the species yet
                        return ChangeVelocity(direction.UnitVector()*4); //move towards it
                    }

                    if(HareIntelligenceBraedenCullen.spiciesId == ani.Species){//My hare move away
                        return ChangeVelocity(direction.UnitVector()*-4);
                    }
                }

            }
            return ChangeVelocity(Vector2D.PolarVector(1, Random.NextDouble(0, 2 * Math.PI)));
        
       }
    }
}

