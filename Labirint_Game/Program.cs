using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labirint_Game
{
    class Program
    {

        //params
        static int height, width;
        static int BlockFrequncy = 40;
        static int WidthOfExit = 3;
        static int power = 1;

        // vars
        static char[,] map;
        static char wall = '#';
        static char Exit = '?';
        public struct PlayerCharacter
        {
            public static int x;
            public static int y;
            public static char sym;
        }

        //private
        static Random Evgen;

        static void Main(string[] args)
        {    
            Init();
            ConsoleKey input;

            while (!OnExit())
            {
                Console.Clear();
                Draw();
                input = Input();
                Controll(input);
                
            }

            Congratulations();
        }

        private static void Init()
        {
            Evgen = new Random();

            CreateMap();
            GeneratePlayer();
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

            //create exit
            int l = Evgen.Next(1, width - 3);
            for(int i = 0; i < WidthOfExit; i++, l++)
            {
                map[height - 1, l] = Exit;
            }
        }


        static void GeneratePlayer ()
        {
            PlayerCharacter.x = 1;
            PlayerCharacter.y = 1;
            PlayerCharacter.sym = '$';
        }

        private static void Draw()
        {
            // draw HEIGHT rows
            for (int i = 0; i < height; i++)
            {
                //draw WIDTH symbols
                for (int j = 0; j < width; j++)
                    Console.Write(map[i, j]);
                Console.WriteLine();
            }

            //Console.ForegroundColor
            //Console.BackgroundColor  
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
                    if (!OnWall(0, -1)) 
                        Move(0, -1);
                    break;

                case ConsoleKey.A:
                    if (!OnWall(-1, 0))
                        Move(-1, 0);
                    break;

                case ConsoleKey.S:
                    if (!OnWall(0, 1))
                        Move(0, 1);
                    break;

                case ConsoleKey.D:
                    if (!OnWall(1, 0))
                        Move(1, 0);
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

        static bool OnWall(int x, int y)
        {
            if (map[GetWY(y), GetWX(x)] == wall) return true;
            else return false;
        }

        static void BreakWall(int x, int y)
        {
            //ok?
            if (BreakWallSupport(x, y))
            {
                map[GetWY(y), GetWX(x)] = ' ';
                power--;
            }
        }

        static void Move (int x, int y)
        {
            //deleting previous
            map[PlayerCharacter.y, PlayerCharacter.x] = ' ';
            //moving
            PlayerCharacter.x += x;
            PlayerCharacter.y += y;
            map[PlayerCharacter.y, PlayerCharacter.x] = PlayerCharacter.sym;
        }

        static bool OnExit()
        {
            if (PlayerCharacter.y == height - 1) return true;
            else return false;
        }

        static void Congratulations()
        {
            Console.Clear();
            Console.WriteLine("Congratulations!");
            Console.WriteLine("You escaped maze!");
            Console.ReadKey();
        }

        //some nead methods

        static bool BreakWallSupport (int x, int y)
        {
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
