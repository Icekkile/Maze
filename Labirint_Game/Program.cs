using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using BaseData;

namespace Labirint_Game
{
    class Program
    {
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

        static void fillReadBiomes (List<Biome> biomes)
        {
            foreach (Biome biome in biomes)
            {
                GameBiome gameBiome = new GameBiome();
                gameBiome.name = biome.name;
                gameBiome.forColor = biome.forColor;/////////////////////////////////////////
                gameBiome.backColor = biome.backColor;/////////////////////////////////////////
                game.readBiomes.Add(gameBiome);
            }
        }

        static void fillReadMobs(List<Mob> mobs)
        {
            foreach (Mob mob in mobs)
            {
                bool biomeFound = false;
                foreach (GameBiome biome in game.readBiomes)
                    if (biome.name == mob.biome)
                        biomeFound = true;

                if (!biomeFound) continue;

                GameMob gameMob = new GameMob();
                gameMob.name = mob.name;
                gameMob.color = mob.color;
                gameMob.sym = mob.sym;
                gameMob.Damage = mob.Damage;
                gameMob.biome = mob.biome;
                game.readMobs.Add(gameMob);
            }
        }

        static void BiomeSet()
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<Biome>));
            FileStream fs = new FileStream(FileData.biomesFile, FileMode.OpenOrCreate);
            List<Biome> biomeList = (List<Biome>)ser.Deserialize(fs);
            fillReadBiomes(biomeList);
        }

        static void ModSet()
        {
            StreamReader sr = new StreamReader(FileData.modsFile);
            string supp = sr.ReadToEnd();
            if (supp == "" || supp == " ")
                return;
            string[] file = sr.ReadToEnd().Split('\n');
            foreach(string line in file)
            {
                XmlSerializer Bser = new XmlSerializer(typeof(List<Biome>));
                XmlSerializer Mser = new XmlSerializer(typeof(List<Mob>));
                FileStream fs = new FileStream(line.Substring(line.IndexOf('-')), FileMode.OpenOrCreate);

                if (line.Substring(0, line.IndexOf('-')) == "biome")
                {
                    List<Biome> biomeList = (List<Biome>)Bser.Deserialize(fs);
                    fillReadBiomes(biomeList);
                }

                else if (line.Substring(0, line.IndexOf('-')) == "mob")
                {
                    List<Mob> mobList = (List<Mob>)Mser.Deserialize(fs);
                    fillReadMobs(mobList);
                }
                    
            }
        }

        static void MobsSet()
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<Mob>));
            FileStream fs = new FileStream(FileData.mobsFile, FileMode.OpenOrCreate);
            List<Mob> mobList = (List<Mob>)ser.Deserialize(fs);
            fillReadMobs(mobList);
        }
    }
}
