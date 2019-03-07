using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Text;
using System.Threading.Tasks;

namespace Labirint_Game
{
    class Game
    {
        //params
        public struct GameParams
        {
            public static int height, width;
            public static readonly int BlockFrequncy = 35;
            public static readonly int mobChanse = 5;
            public static readonly int WidthOfExit = 3;
            public static int power = 2;
            public static int biomeSmoothnes = 4;

        }
        /*
        public struct FileAdress
        {
            public static string biomesFile = "biomes.xml";
            public static string mobsFile = "mobs.xml";
        }
        
        static int height, width;
        static readonly int BlockFrequncy = 35;
        static readonly int mobChanse = 5;
        static readonly int WidthOfExit = 3;
        static int power = 2;
        static int biomeSmoothnes = 4;
*/
        // vars
        //static ConsoleColor[] forColors = new ConsoleColor[2];
        //static ConsoleColor[] backColors = new ConsoleColor[2];
        static char[,] map;

        static char wall = '#';
        static char Exit = '?';

        static ColorBlock[,] colorMap;
        public static List<Biome> readBiomes = new List<Biome>();
        static Mob[] mobsOnMap = new Mob[2];
        static Biome[] biomesOnMap = new Biome[2];
        public static List<Mob> readMobs = new List<Mob>();

        public struct ColorBlock
        {
            public ConsoleColor forColor;
            public ConsoleColor backColor;
        }
        public struct Biome
        {
            public ConsoleColor forColor;
            public ConsoleColor backColor;
            public int index;
            public string name;
        }
        public struct Mob
        {
            public string name;
            public int x, y;
            public char sym;
            public ConsoleColor color;
            public int Damage;
            public int index;
        }
        public struct PlayerCharacter
        {
            public static int x;
            public static int y;
            public static char sym = '$';
            public static ConsoleColor color = ConsoleColor.Red;
            public static int HP = 10;
        }
        /*
        public enum Mobs : Int32
        {
            Scorpion = 0,
            Horse = 1,
            Druid = 2,
            Snowman = 3,
            Yeti = 4,
        }*/
        static Random Evgen;

        public static void Process()
        {

            while (true)
            {
                Init();
                ConsoleKey input;

                while (!OnExit() && !IfDied())
                {
                    Draw();
                    input = Input();


                    Controll(input);
                    MobControler(ref mobsOnMap[0]);
                    MobControler(ref mobsOnMap[1]);

                }
                if (IfDied())
                    Die();
                else
                    End();
            }
        }

        private static void Init()
        {
            Evgen = new Random();
            //BiomeSet();
            //MobsSet();
            GenerateMobs();
            CreateMap();
            SetColorMap();
            GeneratePlayer();
            GameParams.power = 2;
            //BiomeSet(ref forColors[1], ref backColors[1], ref mobsOnMap[1].index);
        }

        private static void CreateMap()
        {
            GameParams.height = Evgen.Next(15, 20);
            GameParams.width = Evgen.Next(20, 30);


            //CreateMap walls
            map = new char[GameParams.height, GameParams.width];
            for (int i = 0; i < GameParams.height; i++)
                for (int j = 0; j < GameParams.width; j++)
                {
                    int GeneratedChance = Evgen.Next(100);
                    map[i, j] = GeneratedChance < GameParams.BlockFrequncy ? wall : ' ';
                }

            //create color map
            colorMap = new ColorBlock[GameParams.height, GameParams.width];

            //create bounds of map
            for (int i = 0; i < GameParams.height; i++)
            {
                map[i, 0] = wall;
                map[i, GameParams.width - 1] = wall;
            }

            for (int i = 0; i < GameParams.width; i++)
            {
                map[0, i] = wall;
                map[GameParams.height - 1, i] = wall;
            }

            //create biomes
            biomesOnMap[0] = readBiomes[Evgen.Next(0, readBiomes.Count)];
            biomesOnMap[1] = readBiomes[Evgen.Next(0, readBiomes.Count)];

            //create mobs
            mobsOnMap[0].x = Evgen.Next(1, GameParams.width - 1);
            mobsOnMap[0].y = Evgen.Next(2, GameParams.height / 2);
            mobsOnMap[1].x = Evgen.Next(1, GameParams.width - 1);
            mobsOnMap[1].y = Evgen.Next(GameParams.height / 2, GameParams.height - 1);

            //create exit
            int l = Evgen.Next(1, GameParams.width - GameParams.WidthOfExit);
            for (int i = 0; i < GameParams.WidthOfExit; i++, l++)
            {
                map[GameParams.height - 1, l] = Exit;
            }
        }
        /*
        static Mob MobXmlReader(XmlNode node)
        {
            Mob mob = new Mob();
            if (node.ChildNodes.Count == 2)
                if (node.FirstChild.Name == "ForColor" && node.LastChild.Name == "Sym")
                {
                    switch (node.FirstChild.InnerText)
                    {
                        case "White":
                            mob.color = ConsoleColor.White;
                            break;
                        case "DarkBlue":
                            mob.color = ConsoleColor.DarkBlue;
                            break;
                        case "DarkCyan":
                            mob.color = ConsoleColor.DarkCyan;
                            break;
                        case "DarkGreen":
                            mob.color = ConsoleColor.DarkGreen;
                            break;
                        case "DarkGrey":
                            mob.color = ConsoleColor.DarkGray;
                            break;
                        case "DarkMagenta":
                            mob.color = ConsoleColor.DarkMagenta;
                            break;
                        case "DarkYellow":
                            mob.color = ConsoleColor.DarkYellow;
                            break;
                        case "DarkRed":
                            mob.color = ConsoleColor.DarkRed;
                            break;
                        case "Red":
                            mob.color = ConsoleColor.Red;
                            break;
                        case "Yellow":
                            mob.color = ConsoleColor.Yellow;
                            break;
                        case "Magenta":
                            mob.color = ConsoleColor.Magenta;
                            break;
                        case "Grey":
                            mob.color = ConsoleColor.Gray;
                            break;
                        case "Green":
                            mob.color = ConsoleColor.Green;
                            break;
                        case "Cyan":
                            mob.color = ConsoleColor.Cyan;
                            break;
                        case "Blue":
                            mob.color = ConsoleColor.Blue;
                            break;
                        default:
                            mob.color = ConsoleColor.Black;
                            break;
                    }

                    mob.sym = Convert.ToChar(node.LastChild.InnerText);
                }
            mob.name = Convert.ToString(node.Attributes.GetNamedItem("name"));
            return mob;
        }

        static Biome BiomeXmlReader(XmlNode node)
        {
            Biome biome = new Biome();
            if (node.ChildNodes.Count == 2)
                if (node.FirstChild.Name == "ForColor" && node.LastChild.Name == "BackColor")
                {
                    switch (node.FirstChild.InnerText)
                    {
                        case "White":
                            biome.forColor = ConsoleColor.White;
                            break;
                        case "DarkBlue":
                            biome.forColor = ConsoleColor.DarkBlue;
                            break;
                        case "DarkCyan":
                            biome.forColor = ConsoleColor.DarkCyan;
                            break;
                        case "DarkGreen":
                            biome.forColor = ConsoleColor.DarkGreen;
                            break;
                        case "DarkGrey":
                            biome.forColor = ConsoleColor.DarkGray;
                            break;
                        case "DarkMagenta":
                            biome.forColor = ConsoleColor.DarkMagenta;
                            break;
                        case "DarkYellow":
                            biome.forColor = ConsoleColor.DarkYellow;
                            break;
                        case "DarkRed":
                            biome.forColor = ConsoleColor.DarkRed;
                            break;
                        case "Red":
                            biome.forColor = ConsoleColor.Red;
                            break;
                        case "Yellow":
                            biome.forColor = ConsoleColor.Yellow;
                            break;
                        case "Magenta":
                            biome.forColor = ConsoleColor.Magenta;
                            break;
                        case "Grey":
                            biome.forColor = ConsoleColor.Gray;
                            break;
                        case "Green":
                            biome.forColor = ConsoleColor.Green;
                            break;
                        case "Cyan":
                            biome.forColor = ConsoleColor.Cyan;
                            break;
                        case "Blue":
                            biome.forColor = ConsoleColor.Blue;
                            break;
                        default:
                            biome.forColor = ConsoleColor.Black;
                            break;
                    }

                    switch (node.LastChild.InnerText)
                    {
                        case "White":
                            biome.backColor = ConsoleColor.White;
                            break;
                        case "DarkBlue":
                            biome.backColor = ConsoleColor.DarkBlue;
                            break;
                        case "DarkCyan":
                            biome.backColor = ConsoleColor.DarkCyan;
                            break;
                        case "DarkGreen":
                            biome.backColor = ConsoleColor.DarkGreen;
                            break;
                        case "DarkGrey":
                            biome.backColor = ConsoleColor.DarkGray;
                            break;
                        case "DarkMagenta":
                            biome.backColor = ConsoleColor.DarkMagenta;
                            break;
                        case "DarkYellow":
                            biome.backColor = ConsoleColor.DarkYellow;
                            break;
                        case "DarkRed":
                            biome.backColor = ConsoleColor.DarkRed;
                            break;
                        case "Red":
                            biome.backColor = ConsoleColor.Red;
                            break;
                        case "Yellow":
                            biome.backColor = ConsoleColor.Yellow;
                            break;
                        case "Magenta":
                            biome.backColor = ConsoleColor.Magenta;
                            break;
                        case "Grey":
                            biome.backColor = ConsoleColor.Gray;
                            break;
                        case "Green":
                            biome.backColor = ConsoleColor.Green;
                            break;
                        case "Cyan":
                            biome.backColor = ConsoleColor.Cyan;
                            break;
                        case "Blue":
                            biome.backColor = ConsoleColor.Blue;
                            break;
                        default:
                            biome.backColor = ConsoleColor.Black;
                            break;
                    }
                }
                else
                {
                    biome.forColor = ConsoleColor.Black;
                    biome.backColor = ConsoleColor.Black;
                    biome.name = "ErrorBiome";
                }
            biome.name = node.Attributes.GetNamedItem("name").Value;
            return biome;
        }

        static void BiomeSet()
        {
            XmlDocument xD = new XmlDocument();
            xD.Load(FileAdress.biomesFile);
            XmlElement xmlMain = xD.DocumentElement;
            //readBiomes = new Biome[xmlMain.ChildNodes.Count];
            foreach (XmlNode node in xmlMain)
            {
                readBiomes.Add(BiomeXmlReader(node));
            }

        }
        */
        static void SetColorMap()
        {

            for (int i = 0; i < (GameParams.height / 2) - GameParams.biomeSmoothnes; i++)
                for (int j = 0; j < GameParams.width; j++)
                {
                    colorMap[i, j].forColor = biomesOnMap[0].forColor;
                    colorMap[i, j].backColor = biomesOnMap[0].backColor;
                }
            for (int i = GameParams.height - 1; i > GameParams.height / 2; i--)
                for (int j = 0; j < GameParams.width; j++)
                {
                    colorMap[i, j].forColor = biomesOnMap[1].forColor;
                    colorMap[i, j].backColor = biomesOnMap[1].backColor;
                }

            int randomOfSmoth = 0;
            for (int j = 0; j < GameParams.width; j++)
            {
                randomOfSmoth = Evgen.Next(1, GameParams.biomeSmoothnes + 1);
                for (int i = (GameParams.height / 2) - GameParams.biomeSmoothnes; i < (GameParams.height / 2) - randomOfSmoth; i++)
                {
                    colorMap[i, j].forColor = biomesOnMap[0].forColor;
                    colorMap[i, j].backColor = biomesOnMap[0].backColor;
                }
                for (int i = (GameParams.height / 2) - randomOfSmoth; i <= GameParams.height / 2; i++)
                {
                    colorMap[i, j].forColor = biomesOnMap[1].forColor;
                    colorMap[i, j].backColor = biomesOnMap[1].backColor;
                }
            }
        }

        static void Coloring(int pointX, int pointY)
        {
            /*
            if (point < (height /  2) - biomeSmoothnes)
            {
                Console.ForegroundColor = biomesOnMap[0].forColor;
                Console.BackgroundColor = biomesOnMap[0].backColor;
            }
            else if (point >= (height / 2) + biomeSmoothnes)
            {
                Console.ForegroundColor = biomesOnMap[1].forColor;
                Console.BackgroundColor = biomesOnMap[1].backColor;
            }
            else if (point > (height / 2) - biomeSmoothnes && point <= (height / 2) + biomeSmoothnes)
            {
                int randomBiome = Evgen.Next(0, 2);
                Console.ForegroundColor = biomesOnMap[randomBiome].forColor;
                Console.BackgroundColor = biomesOnMap[randomBiome].backColor;
            }*/

            Console.ForegroundColor = colorMap[pointY, pointX].forColor;
            Console.BackgroundColor = colorMap[pointY, pointX].backColor;
        }

        static void GeneratePlayer()
        {
            PlayerCharacter.x = 1;
            PlayerCharacter.y = 1;
            /*for (int i = GameParams.width - 4; i >= 3; i--)
            {
                if (map[1, i] != wall)
                {
                    PlayerCharacter.x = i;
                    PlayerCharacter.y = 1;
                }
            }*/
        }
        /*
        static void MobsSet()
        {


            XmlDocument xD = new XmlDocument();
            xD.Load(FileAdress.mobsFile);
            XmlElement xmlMain = xD.DocumentElement;
            //readMobs = new Mob[xmlMain.ChildNodes.Count];
            foreach (XmlNode node in xmlMain)
            {
                readMobs.Add(MobXmlReader(node));
            }

            /*
            switch ((Mobs)mob.index)
            {
                case Mobs.Scorpion:
                    mob.sym = '~';
                    mob.Damage = 5;
                    mob.color = ConsoleColor.Black;
                    break;
                case Mobs.Horse:
                    mob.sym = 'P';
                    mob.Damage = 3;
                    mob.color = ConsoleColor.DarkYellow;
                    break;
                case Mobs.Druid:
                    mob.sym = 'W';
                    mob.Damage = 7;
                    mob.color = ConsoleColor.DarkGreen;
                    break;
                case Mobs.Snowman:
                    mob.sym = '8';
                    mob.Damage = 0;
                    mob.color = ConsoleColor.Cyan;
                    break;
                case Mobs.Yeti:
                    mob.sym = 'Y';
                    mob.Damage = 10;
                    mob.color = ConsoleColor.Blue;
                    break;
                default:
                    System.Diagnostics.Debug.Assert(false, "Program::GeneratedMob : Not set mob type");
                    break;
            }*



        }*/

        static void GenerateMobs()
        {
            mobsOnMap[0] = readMobs[Evgen.Next(0, readMobs.Count)];
            mobsOnMap[1] = readMobs[Evgen.Next(0, readMobs.Count)];
        }

        static void MobControler(ref Mob mob)
        {
            int xRand = Evgen.Next(-1, 2);
            int yRand = Evgen.Next(-1, 2);
            if (!OnWall(xRand, yRand, mob.x, mob.y))
                Move(xRand, yRand, ref mob.x, ref mob.y);
        }

        private static void Draw()
        {
            Console.Clear();


            // draw HEIGHT rows
            for (int i = 0; i < GameParams.height; i++)
            {
                //draw WIDTH symbols
                for (int j = 0; j < GameParams.width; j++)
                {
                    Coloring(j, i);
                    DrawTile(j, i);
                }
                Console.WriteLine();
            }
            Console.ResetColor();
        }

        static void DrawTile(int x, int y)
        {
            if (PlayerCharacter.y == y && PlayerCharacter.x == x)
            {
                //forward
                Console.ForegroundColor = PlayerCharacter.color;
                Console.Write(PlayerCharacter.sym);
            }
            else if ((mobsOnMap[0].x == x && mobsOnMap[0].y == y) || (mobsOnMap[1].x == x && mobsOnMap[1].y == y))
            {
                Console.ForegroundColor = mobsOnMap[y < GameParams.height / 2 ? 0 : 1].color;
                Console.Write(mobsOnMap[y < GameParams.height / 2 ? 0 : 1].sym);
            }
            else
            {
                //back
                Console.Write(map[y, x]);
            }
        }

        private static ConsoleKey Input()
        {
            ConsoleKey playerInput = Console.ReadKey().Key;
            return playerInput;
        }

        static void Controll(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.W:
                    if (OnMob(0, -1))
                        PlayerCharacter.HP -= mobsOnMap[PlayerCharacter.y + -1 < GameParams.height / 2 ? 0 : 1].Damage;
                    if (!OnWall(0, -1, PlayerCharacter.x, PlayerCharacter.y))
                        Move(0, -1, ref PlayerCharacter.x, ref PlayerCharacter.y);
                    break;

                case ConsoleKey.A:
                    if (OnMob(-1, 0))
                        PlayerCharacter.HP -= mobsOnMap[PlayerCharacter.y + 0 < GameParams.height / 2 ? 0 : 1].Damage;
                    if (!OnWall(-1, 0, PlayerCharacter.x, PlayerCharacter.y))
                        Move(-1, 0, ref PlayerCharacter.x, ref PlayerCharacter.y);
                    break;

                case ConsoleKey.S:
                    if (OnMob(0, 1))
                        PlayerCharacter.HP -= mobsOnMap[PlayerCharacter.y + 1 < GameParams.height / 2 ? 0 : 1].Damage;
                    if (!OnWall(0, 1, PlayerCharacter.x, PlayerCharacter.y))
                        Move(0, 1, ref PlayerCharacter.x, ref PlayerCharacter.y);
                    break;

                case ConsoleKey.D:
                    if (OnMob(1, 0))
                        PlayerCharacter.HP -= mobsOnMap[PlayerCharacter.y + 0 < GameParams.height / 2 ? 0 : 1].Damage;
                    if (!OnWall(1, 0, PlayerCharacter.x, PlayerCharacter.y))
                        Move(1, 0, ref PlayerCharacter.x, ref PlayerCharacter.y);
                    break;

                case ConsoleKey.UpArrow:
                    BreakWall(0, -1);
                    break;

                case ConsoleKey.LeftArrow:
                    BreakWall(-1, 0);
                    break;

                case ConsoleKey.DownArrow:
                    BreakWall(0, 1);
                    break;

                case ConsoleKey.RightArrow:
                    BreakWall(1, 0);
                    break;
            }
        }

        static bool OnWall(int x, int y, int xObj, int yObj)
        {
            return (map[yObj + y, xObj + x] == wall);
        }

        static bool OnMob(int x, int y)
        {
            return (GetWX(x) == mobsOnMap[0].x && GetWY(y) == mobsOnMap[0].y) || (GetWX(x) == mobsOnMap[1].x && GetWY(y) == mobsOnMap[1].y);
        }

        static bool IfDied()
        {
            return PlayerCharacter.HP <= 0;
        }

        static void BreakWall(int x, int y)
        {
            //ok?
            if (IfBreakWall(x, y))
            {
                map[GetWY(y), GetWX(x)] = ' ';
                GameParams.power--;
            }
        }

        static void Move(int x, int y, ref int xObj, ref int yObj)
        {
            xObj += x;
            yObj += y;
        }

        static bool OnExit()
        {
            if (PlayerCharacter.y == GameParams.height - 1) return true;
            else return false;
        }

        static void End()
        {
            Console.Clear();
            Console.WriteLine("Congratulations!");
            Console.WriteLine("You escaped maze!");
            Console.WriteLine("If you want again press: Enter");
            if (Console.ReadKey().Key != ConsoleKey.Enter)
                System.Environment.Exit(1);

        }

        static void Die()
        {
            Console.Clear();
            Console.WriteLine("You died!");
            Console.WriteLine("If you want again press: Enter");
            if (Console.ReadKey().Key != ConsoleKey.Enter)
                System.Environment.Exit(1);
        }

        //some nead methods

        static bool IfBreakWall(int x, int y)
        {
            //ok?
            if ((GetWY(y) != GameParams.height - 1 && GetWX(x) != GameParams.width - 1) &&
               (GetWY(y) != 0 && GetWX(x) != 0))
            {
                if (map[GetWY(y), GetWX(x)] == wall)
                {
                    if (GameParams.power > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        static int GetWY(int y)
        {
            return PlayerCharacter.y + y;
        }

        static int GetWX(int x)
        {
            return PlayerCharacter.x + x;
        }
    }
}
