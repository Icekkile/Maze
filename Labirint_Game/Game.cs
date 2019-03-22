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
        public int x;
        public int y;
        public char sym;
        public ConsoleColor color;
        public int HP;
        public int power;
        public PlayerCharacter(int _x, int _y, char _sym, ConsoleColor _color, int _HP, int _power)
        {
            x = _x;
            y = _y;
            sym = _sym;
            color = _color;
            HP = _HP;
            power = _power;
        }
    }
    public struct GlobalGameParams
    {
        public static readonly int BlockFrequncy = 35;
        public static readonly int WidthOfExit = 3;
    }

    public struct GameParams
    {
        public int height, width;
        public int biomeSmoothnes;
    }

    class Game
    {
        //params
        
        char[,] map;

        char wall = '#';
        char Exit = '?';
        char nullCell = ' ';

        ColorBlock[,] colorMap;
        public List<Biome> readBiomes = new List<Biome>();
        Mob[] mobsOnMap = new Mob[2];
        Biome[] biomesOnMap = new Biome[2];
        public List<Mob> readMobs = new List<Mob>();

        PlayerCharacter player;
        GameParams gameParams;
        Random Evgen;

        public void Process()
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

        private void Init()
        {
            Evgen = new Random();
            GeneratePlayer();
            GenerateMobs();
            CreateMap();
            SetColorMap();
            
            player.power = 2;
        }

        void SetBiomeSmoothnes ()
        {
            gameParams.biomeSmoothnes = gameParams.height / 5;
        }

        private void CreateMap()
        {
            gameParams.height = Evgen.Next(15, 20);
            gameParams.width = Evgen.Next(20, 30);
            SetBiomeSmoothnes();

            //CreateMap walls
            map = new char[gameParams.height, gameParams.width];
            for (int i = 0; i < gameParams.height; i++)
                for (int j = 0; j < gameParams.width; j++)
                {
                    int GeneratedChance = Evgen.Next(100);
                    map[i, j] = GeneratedChance < GlobalGameParams.BlockFrequncy ? wall : ' ';
                }

            //create color map
            colorMap = new ColorBlock[gameParams.height, gameParams.width];

            //create bounds of map
            for (int i = 0; i < gameParams.height; i++)
            {
                map[i, 0] = wall;
                map[i, gameParams.width - 1] = wall;
            }

            for (int i = 0; i < gameParams.width; i++)
            {
                map[0, i] = wall;
                map[gameParams.height - 1, i] = wall;
            }

            //create biomes
            biomesOnMap[0] = readBiomes[Evgen.Next(0, readBiomes.Count)];
            biomesOnMap[1] = readBiomes[Evgen.Next(0, readBiomes.Count)];

            //create mobs
            mobsOnMap[0].x = Evgen.Next(1, gameParams.width - 1);
            mobsOnMap[0].y = Evgen.Next(2, gameParams.height / 2);
            mobsOnMap[1].x = Evgen.Next(1, gameParams.width - 1);
            mobsOnMap[1].y = Evgen.Next(gameParams.height / 2, gameParams.height - 1);

            //create exit
            int l = Evgen.Next(1, gameParams.width - GlobalGameParams.WidthOfExit);
            for (int i = 0; i < GlobalGameParams.WidthOfExit; i++, l++)
            {
                map[gameParams.height - 1, l] = Exit;
            }
        }
        
        void SetColorMap()
        {

            for (int i = 0; i < (gameParams.height / 2) - gameParams.biomeSmoothnes; i++)
                for (int j = 0; j < gameParams.width; j++)
                {
                    colorMap[i, j].forColor = biomesOnMap[0].forColor;
                    colorMap[i, j].backColor = biomesOnMap[0].backColor;
                }
            for (int i = gameParams.height - 1; i > gameParams.height / 2; i--)
                for (int j = 0; j < gameParams.width; j++)
                {
                    colorMap[i, j].forColor = biomesOnMap[1].forColor;
                    colorMap[i, j].backColor = biomesOnMap[1].backColor;
                }

            int randomOfSmoth = 0;
            for (int j = 0; j < gameParams.width; j++)
            {
                randomOfSmoth = Evgen.Next(1, gameParams.biomeSmoothnes + 1);
                for (int i = (gameParams.height / 2) - gameParams.biomeSmoothnes; i < (gameParams.height / 2) - randomOfSmoth; i++)
                {
                    colorMap[i, j].forColor = biomesOnMap[0].forColor;
                    colorMap[i, j].backColor = biomesOnMap[0].backColor;
                }
                for (int i = (gameParams.height / 2) - randomOfSmoth; i <= gameParams.height / 2; i++)
                {
                    colorMap[i, j].forColor = biomesOnMap[1].forColor;
                    colorMap[i, j].backColor = biomesOnMap[1].backColor;
                }
            }
        }

        void Coloring(int pointX, int pointY)
        {
            

            Console.ForegroundColor = colorMap[pointY, pointX].forColor;
            Console.BackgroundColor = colorMap[pointY, pointX].backColor;
        }

        void GeneratePlayer()
        {
            player = new PlayerCharacter(1, 1, '$', ConsoleColor.Red, 10, 2);
        }
        

        void GenerateMobs()
        {
            mobsOnMap[0] = readMobs[Evgen.Next(0, readMobs.Count)];
            mobsOnMap[1] = readMobs[Evgen.Next(0, readMobs.Count)];
        }

        void MobControler(ref Mob mob)
        {
            int xRand = Evgen.Next(-1, 2);
            int yRand = Evgen.Next(-1, 2);
            if (CanMove(xRand, yRand, mob.x, mob.y))
                Move(xRand, yRand, ref mob.x, ref mob.y);
        }

        private void Draw()
        {
            Console.Clear();


            // draw HEIGHT rows
            for (int i = 0; i < gameParams.height; i++)
            {
                //draw WIDTH symbols
                for (int j = 0; j < gameParams.width; j++)
                {
                    Coloring(j, i);
                    DrawTile(j, i);
                }
                Console.WriteLine();
            }
            Console.ResetColor();
        }

        void DrawTile(int x, int y)
        {
            if (player.y == y && player.x == x)
            {
                //forward
                Console.ForegroundColor = player.color;
                Console.Write(player.sym);
            }
            else if ((mobsOnMap[0].x == x && mobsOnMap[0].y == y) || (mobsOnMap[1].x == x && mobsOnMap[1].y == y))
            {
                Console.ForegroundColor = mobsOnMap[y < gameParams.height / 2 ? 0 : 1].color;
                Console.Write(mobsOnMap[y < gameParams.height / 2 ? 0 : 1].sym);
            }
            else
            {
                //back
                Console.Write(map[y, x]);
            }
        }

        private ConsoleKey Input()
        {
            ConsoleKey playerInput = Console.ReadKey().Key;
            return playerInput;
        }

        void Controll(ConsoleKey key)
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

        void TryMove(int x, int y)
        {
            if (OnMob(x, y, out Mob mob))
                player.HP -= mob.Damage;
            else if (CanMove(x, y, player.x, player.y))
                Move(x, y, ref player.x, ref player.y);
        }

        bool CanMove(int x, int y, int xObj, int yObj)
        {
            return (map[yObj + y, xObj + x] == nullCell || map[yObj + y, xObj + x] == Exit);
        }

        bool OnMob(int x, int y, out Mob mob)
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

        bool IfDied()
        {
            return player.HP <= 0;
        }

        void TryBreakWall(int x, int y)
        {
            if (IfBreakWall(x, y))
            {
                map[GetWY(y), GetWX(x)] = nullCell;
                player.power--;
            }
        }

        void Move(int x, int y, ref int xObj, ref int yObj)
        {
            xObj += x;
            yObj += y;
        }

        bool OnExit()
        {
            if (player.y == gameParams.height - 1) return true;
            else return false;
        }

        void End()
        {
            Console.Clear();
            Console.WriteLine("Congratulations!");
            Console.WriteLine("You escaped maze!");
            Console.WriteLine("If you want again press: Enter");
            if (Console.ReadKey().Key != ConsoleKey.Enter)
                System.Environment.Exit(1);

        }

        void Die()
        {
            Console.Clear();
            Console.WriteLine("You died!");
            Console.WriteLine("If you want again press: Enter");
            if (Console.ReadKey().Key != ConsoleKey.Enter)
                System.Environment.Exit(1);
        }

        //some nead methods

        bool IfBreakWall(int x, int y)
        {
            if ((GetWY(y) != gameParams.height - 1 && GetWX(x) != gameParams.width - 1) &&
               (GetWY(y) != 0 && GetWX(x) != 0))
            {
                if (map[GetWY(y), GetWX(x)] == wall)
                {
                    if (player.power > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        int GetWY(int y)
        {
            return player.y + y;
        }

        int GetWX(int x)
        {
            return player.x + x;
        }
    }
}
