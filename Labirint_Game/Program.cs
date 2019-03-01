using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Labirint_Game
{
    class Program
    {
        


        //params
        static int height, width;
        static readonly int BlockFrequncy = 35;
        static readonly int mobChanse = 5;
        static readonly int WidthOfExit = 3;
        static int power = 2;
        static int biomeSmoothnes = 4;

        // vars
        //static ConsoleColor[] forColors = new ConsoleColor[2];
        //static ConsoleColor[] backColors = new ConsoleColor[2];
        static char[,] map;

        static char wall = '#';
        static char Exit = '?';

        static ColorBlock[,] colorMap;
        static Biome[] readBiomes;
        static Mob[] mobsOnMap = new Mob[2];
        static Biome[] biomesOnMap = new Biome[2];
        static Mob[] readMobs;

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

        static void Main(string[] args)
        {

            while (true)
            {
                Init();
                ConsoleKey input;

                while (!OnExit() && !IfDied())
                {
                    Console.Clear();
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
            BiomeSet();
            MobsSet();
            GenerateMobs();
            CreateMap();
            SetColorMap();
            GeneratePlayer();
 
            //BiomeSet(ref forColors[1], ref backColors[1], ref mobsOnMap[1].index);
        }

        private static void CreateMap()
        {
            height = Evgen.Next(15, 20);
            width = Evgen.Next(20, 30);


            //CreateMap walls
            map = new char[height, width];
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    int GeneratedChance = Evgen.Next(100);
                    map[i, j] = GeneratedChance < BlockFrequncy ? wall : ' ';
                }

            //create color map
            colorMap = new ColorBlock[height, width];

            //create bounds of map
            for(int i = 0; i < height; i++)
            {
                map[i, 0] = wall;
                map[i, width - 1] = wall;
            }

            for(int i = 0; i < width; i++)
            {
                map[0, i] = wall;
                map[height - 1, i] = wall;
            }

            //create biomes
            biomesOnMap[0] = readBiomes[Evgen.Next(0, readBiomes.Length)];
            biomesOnMap[1] = readBiomes[Evgen.Next(0, readBiomes.Length)];

            //create mobs
            mobsOnMap[0].x = Evgen.Next(1, width - 1);
            mobsOnMap[0].y = Evgen.Next(2, height / 2);
            mobsOnMap[1].x = Evgen.Next(1, width - 1);
            mobsOnMap[1].y = Evgen.Next(height / 2, height - 1);

            //create exit
            int l = Evgen.Next(1, width - WidthOfExit);
            for(int i = 0; i < WidthOfExit; i++, l++)
            {
                map[height - 1, l] = Exit;
            }
        }

        static void MobXmlReader(XmlNode node, ref Mob mob)
        {
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
        }

        static void BiomeXmlReader(XmlNode node, ref ConsoleColor forColor, ref ConsoleColor backColor, ref string name)
        {
            if (node.ChildNodes.Count == 2)
                if (node.FirstChild.Name == "ForColor" && node.LastChild.Name == "BackColor")
                {
                    switch (node.FirstChild.InnerText)
                    {
                        case "White":
                            forColor = ConsoleColor.White;
                            break;
                        case "DarkBlue":
                            forColor = ConsoleColor.DarkBlue;
                            break;
                        case "DarkCyan":
                            forColor = ConsoleColor.DarkCyan;
                            break;
                        case "DarkGreen":
                            forColor = ConsoleColor.DarkGreen;
                            break;
                        case "DarkGrey":
                            forColor = ConsoleColor.DarkGray;
                            break;
                        case "DarkMagenta":
                            forColor = ConsoleColor.DarkMagenta;
                            break;
                        case "DarkYellow":
                            forColor = ConsoleColor.DarkYellow;
                            break;
                        case "DarkRed":
                            forColor = ConsoleColor.DarkRed;
                            break;
                        case "Red":
                            forColor = ConsoleColor.Red;
                            break;
                        case "Yellow":
                            forColor = ConsoleColor.Yellow;
                            break;
                        case "Magenta":
                            forColor = ConsoleColor.Magenta;
                            break;
                        case "Grey":
                            forColor = ConsoleColor.Gray;
                            break;
                        case "Green":
                            forColor = ConsoleColor.Green;
                            break;
                        case "Cyan":
                            forColor = ConsoleColor.Cyan;
                            break;
                        case "Blue":
                            forColor = ConsoleColor.Blue;
                            break;
                        default:
                            forColor = ConsoleColor.Black;
                            break;
                    }

                    switch (node.LastChild.InnerText)
                    {
                        case "White":
                            backColor = ConsoleColor.White;
                            break;
                        case "DarkBlue":
                            backColor = ConsoleColor.DarkBlue;
                            break;
                        case "DarkCyan":
                            backColor = ConsoleColor.DarkCyan;
                            break;
                        case "DarkGreen":
                            backColor = ConsoleColor.DarkGreen;
                            break;
                        case "DarkGrey":
                            backColor = ConsoleColor.DarkGray;
                            break;
                        case "DarkMagenta":
                            backColor = ConsoleColor.DarkMagenta;
                            break;
                        case "DarkYellow":
                            backColor = ConsoleColor.DarkYellow;
                            break;
                        case "DarkRed":
                            backColor = ConsoleColor.DarkRed;
                            break;
                        case "Red":
                            backColor = ConsoleColor.Red;
                            break;
                        case "Yellow":
                            backColor = ConsoleColor.Yellow;
                            break;
                        case "Magenta":
                            backColor = ConsoleColor.Magenta;
                            break;
                        case "Grey":
                            backColor = ConsoleColor.Gray;
                            break;
                        case "Green":
                            backColor = ConsoleColor.Green;
                            break;
                        case "Cyan":
                            backColor = ConsoleColor.Cyan;
                            break;
                        case "Blue":
                            backColor = ConsoleColor.Blue;
                            break;
                        default:
                            backColor = ConsoleColor.Black;
                            break;
                    }
                } else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error xml");

                    forColor = ConsoleColor.Black;
                    backColor = ConsoleColor.Black;
                    name = "ErrorBiome";
                }
        }

        static void BiomeSet ()
        {
            XmlDocument xD = new XmlDocument();
            xD.Load("biomes.xml");
            XmlElement xmlMain = xD.DocumentElement;
            readBiomes = new Biome[xmlMain.ChildNodes.Count];
            int i = 0;
            foreach (XmlNode node in xmlMain)
            {
                BiomeXmlReader(node, ref readBiomes[i].forColor, ref readBiomes[i].backColor, ref readBiomes[i].name);
                i++;
            }

        }

        static void SetColorMap()
        {
            
            for (int i = 0; i < (height / 2) - biomeSmoothnes; i++)
                for (int j = 0; j < width; j++)
                {
                    colorMap[i, j].forColor = biomesOnMap[0].forColor;
                    colorMap[i, j].backColor = biomesOnMap[0].backColor;
                }
            for (int i = height - 1; i > height / 2; i--)
                for(int j = 0; j < width; j++)
                {
                    colorMap[i, j].forColor = biomesOnMap[1].forColor;
                    colorMap[i, j].backColor = biomesOnMap[1].backColor;
                }

            int randomOfSmoth = 0;
            for (int j = 0; j < width; j++)
            {
                randomOfSmoth = Evgen.Next(1, biomeSmoothnes + 1);
                for (int i = (height / 2) - biomeSmoothnes; i < (height / 2) - randomOfSmoth; i++)
                {
                    colorMap[i, j].forColor = biomesOnMap[0].forColor;
                    colorMap[i, j].backColor = biomesOnMap[0].backColor;
                }
                for (int i = (height / 2) - randomOfSmoth; i <= height / 2; i++)
                {
                    colorMap[i, j].forColor = biomesOnMap[1].forColor;
                    colorMap[i, j].backColor = biomesOnMap[1].backColor;
                }
            }
        }

        static void Coloring (int pointX, int pointY)
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

        static void GeneratePlayer ()
        {
            for (int i = width - 4; i >= 3; i--)
            {
                if (map[1, i] != wall)
                {
                    PlayerCharacter.x = i;
                    PlayerCharacter.y = 1;
                }
            }
        }

        static void MobsSet ()
        {


            XmlDocument xD = new XmlDocument();
            xD.Load("mobs.xml");
            XmlElement xmlMain = xD.DocumentElement;
            readMobs = new Mob[xmlMain.ChildNodes.Count];
            int i = 0;
            foreach (XmlNode node in xmlMain)
            {
                MobXmlReader(node, ref readMobs[i]);
                i++;
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
            }*/



        }

        static void GenerateMobs ()
        {
            mobsOnMap[0] = readMobs[Evgen.Next(0, readMobs.Length)];
            mobsOnMap[0] = readMobs[Evgen.Next(0, readMobs.Length)];
        }

        static void MobControler (ref Mob mob)
        {
            int xRand = Evgen.Next(-1, 2);
            int yRand = Evgen.Next(-1, 2);
            if (!OnWall(xRand, yRand, mob.x, mob.y))
                Move(xRand, yRand, ref mob.x, ref mob.y);
        }

        private static void Draw()
        {
            // draw HEIGHT rows
            for (int i = 0; i < height; i++)
            {
                //draw WIDTH symbols
                for (int j = 0; j < width; j++)
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
                Console.ForegroundColor = mobsOnMap[y < height / 2 ? 0 : 1].color;
                Console.Write(mobsOnMap[y < height / 2 ? 0 : 1].sym);
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

        static void Controll (ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.W:
                    if (OnMob(0, -1))
                        PlayerCharacter.HP -= mobsOnMap[PlayerCharacter.y + -1 < height / 2 ? 0 : 1].Damage;
                    if (!OnWall(0, -1, PlayerCharacter.x, PlayerCharacter.y)) 
                        Move(0, -1, ref PlayerCharacter.x, ref PlayerCharacter.y);
                    break;

                case ConsoleKey.A:
                    if (OnMob(-1, 0))
                        PlayerCharacter.HP -= mobsOnMap[PlayerCharacter.y + 0 < height / 2 ? 0 : 1].Damage;
                    if (!OnWall(-1, 0, PlayerCharacter.x, PlayerCharacter.y))
                        Move(-1, 0, ref PlayerCharacter.x, ref PlayerCharacter.y);
                    break;

                case ConsoleKey.S:
                    if (OnMob(0, 1))
                        PlayerCharacter.HP -= mobsOnMap[PlayerCharacter.y + 1 < height / 2 ? 0 : 1].Damage;
                    if (!OnWall(0, 1, PlayerCharacter.x, PlayerCharacter.y))
                        Move(0, 1, ref PlayerCharacter.x, ref PlayerCharacter.y);
                    break;

                case ConsoleKey.D:
                    if (OnMob(1, 0))
                        PlayerCharacter.HP -= mobsOnMap[PlayerCharacter.y + 0 < height / 2 ? 0 : 1].Damage;
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

        static bool IfDied ()
        {
            return PlayerCharacter.HP <= 0;
        }

        static void BreakWall(int x, int y)
        {
            //ok?
            if (IfBreakWall(x, y))
            {
                map[GetWY(y), GetWX(x)] = ' ';
                power--;
            }
        }

        static void Move (int x, int y, ref int xObj, ref int yObj)
        {
            xObj += x;
            yObj += y;
        }

        static bool OnExit()
        {
            if (PlayerCharacter.y == height - 1) return true;
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

        static bool IfBreakWall (int x, int y)
        {
            //ok?
            if ((GetWY(y) != height - 1 && GetWX(x) != width - 1) &&
               (GetWY(y) != 0 && GetWX(x) != 0))
            {
                if (map[GetWY(y), GetWX(x)] == wall)
                {
                    if (power > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        static int GetWY (int y)
        {
            return PlayerCharacter.y + y;
        }

        static int GetWX (int x)
        {
            return PlayerCharacter.x + x;
        }
    }
}
