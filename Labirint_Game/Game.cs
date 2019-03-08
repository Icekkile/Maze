using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Text;
using System.Threading.Tasks;

namespace Labirint_Game
{
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
    public struct GameParams
    {
        public static int height, width;
        public static readonly int BlockFrequncy = 35;
        public static readonly int mobChanse = 5;
        public static readonly int WidthOfExit = 3;
        public static int power = 2;
        public static int biomeSmoothnes = 4;

    }

    class Game
    {
        //params
        
        static char[,] map;

        static char wall = '#';
        static char Exit = '?';
        static char nullCell = ' ';

        static ColorBlock[,] colorMap;
        public static List<Biome> readBiomes = new List<Biome>();
        static Mob[] mobsOnMap = new Mob[2];
        static Biome[] biomesOnMap = new Biome[2];
        public static List<Mob> readMobs = new List<Mob>();

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
            GenerateMobs();
            CreateMap();
            SetColorMap();
            GeneratePlayer();
            GameParams.power = 2;
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
            

            Console.ForegroundColor = colorMap[pointY, pointX].forColor;
            Console.BackgroundColor = colorMap[pointY, pointX].backColor;
        }

        static void GeneratePlayer()
        {
            PlayerCharacter.x = 1;
            PlayerCharacter.y = 1;
            PlayerCharacter.HP = 10;
        }
        

        static void GenerateMobs()
        {
            mobsOnMap[0] = readMobs[Evgen.Next(0, readMobs.Count)];
            mobsOnMap[1] = readMobs[Evgen.Next(0, readMobs.Count)];
        }

        static void MobControler(ref Mob mob)
        {
            int xRand = Evgen.Next(-1, 2);
            int yRand = Evgen.Next(-1, 2);
            if (CanMove(xRand, yRand, mob.x, mob.y))
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
            MobControler(ref mobsOnMap[0]);
            MobControler(ref mobsOnMap[1]);
            switch (key)
            {
                case ConsoleKey.W:
                    TryMove(0, -1);
                    break;

                case ConsoleKey.A:
                    TryMove(-1, 0);
                    break;

                case ConsoleKey.S:
                    TryMove(0, 1);
                    break;

                case ConsoleKey.D:
                    TryMove(1, 0);
                    break;

                case ConsoleKey.UpArrow:
                    TryBreakWall(0, -1);
                    break;

                case ConsoleKey.LeftArrow:
                    TryBreakWall(-1, 0);
                    break;

                case ConsoleKey.DownArrow:
                    TryBreakWall(0, 1);
                    break;

                case ConsoleKey.RightArrow:
                    TryBreakWall(1, 0);
                    break;
            }
        }

        static void TryMove(int x, int y)
        {
            if (OnMob(x, y, out Mob mob))
                PlayerCharacter.HP -= mob.Damage;
            else if (CanMove(x, y, PlayerCharacter.x, PlayerCharacter.y))
                Move(x, y, ref PlayerCharacter.x, ref PlayerCharacter.y);
        }

        static bool CanMove(int x, int y, int xObj, int yObj)
        {
            return (map[yObj + y, xObj + x] == nullCell);
        }

        static bool OnMob(int x, int y, out Mob mob)
        {
            if (GetWX(x) == mobsOnMap[0].x && GetWY(y) == mobsOnMap[0].y)
            {
                mob = mobsOnMap[0];
                return true;
            }
            if (GetWX(x) == mobsOnMap[1].x && GetWY(y) == mobsOnMap[1].y)
            {
                mob = mobsOnMap[0];
                return true;
            }
            mob = new Mob();
            return false;
        }

        static bool IfDied()
        {
            return PlayerCharacter.HP <= 0;
        }

        static void TryBreakWall(int x, int y)
        {
            if (IfBreakWall(x, y))
            {
                map[GetWY(y), GetWX(x)] = nullCell;
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
