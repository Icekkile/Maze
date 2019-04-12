using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Labirint_Game
{
    class Program
    {
        /*
        //params
        public struct GameParams
        {
            public static int height, width;
            public static readonly int BlockFrequncy = 35;
            public static readonly int mobChanse = 5;
            public static readonly int WidthOfExit = 3;
            public static int power = 2;
            public static int biomeSmoothnes = 4;

        }*/
        
        public struct FileData
        {
            public static string biomesFile = "biomes.xml";
            public static int fileBiomesCount = 5;
            public static string mobsFile = "mobs.xml";
            public static int fileMobsCount = 5;
            public static string modsFile = @"E:\MyCode\kirill_strong\Labirint_Game\Labirint_Game\Mods\modsList.txt";
        }

        static Game game = new Game();

        static void Main (string[] args)
        {
            
            BiomeSet();
            MobsSet();
            ModSet();
            game.Process();
        }

        static void BiomeSet()
        {
            XmlSerializer ser = new XmlSerializer(typeof(GameBiome[]));
            FileStream fs = new FileStream(FileData.biomesFile, FileMode.OpenOrCreate);
            game.readBiomes = (List<GameBiome>)ser.Deserialize(fs);

        }

        static void ModSet()
        {
            
            StreamReader sr = new StreamReader(FileData.modsFile);
            string[] file = sr.ReadToEnd().Split('\n');
            foreach(string line in file)
            {
                XmlSerializer ser = new XmlSerializer(typeof(GameBiome[]));
                FileStream fs = new FileStream(line.Substring(line.IndexOf('-')), FileMode.OpenOrCreate);

                if (line.Substring(0, line.IndexOf('-')) == "biome")
                    foreach (GameBiome gb in (List<GameBiome>)ser.Deserialize(fs))
                    {
                        game.readBiomes.Add(gb);
                    }

                else if (line.Substring(0, line.IndexOf('-')) == "mob")
                    foreach (GameMob gm in (List<GameMob>)ser.Deserialize(fs))
                    {
                        game.readMobs.Add(gm);
                    }
            }
        }

        static void MobsSet()
        {
            XmlSerializer ser = new XmlSerializer(typeof(GameMob[]));
            FileStream fs = new FileStream(FileData.mobsFile, FileMode.OpenOrCreate);
            game.readMobs = (List<GameMob>)ser.Deserialize(fs);
        }
    }
}
