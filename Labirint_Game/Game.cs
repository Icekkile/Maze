using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Text;
using System.Threading.Tasks;
using BaseData;
using System.Xml.Serialization;

namespace Labirint_Game
{
    interface IMovable
    {
        //int x; int y;
        void Move(int _x, int _y);
    }

    public abstract class Character
    {
        public int x { get; set; }
        public int y { get; set; }

        //public abstract void Move(int _x, int _y);
    }

    [Serializable]
    public class GameMob : Character, IMovable
    {
        public Mob commonHalf { get; set; }

        public void Move (int _x, int _y)
        {
            x += _x;
            y += _y;
        }

        public GameMob()
        {

        }
    }

    [Serializable]
    public class GameBiome
    {
        public Biome commonHalf;
        public GameBiome()
        {

        }
    }

    public struct ColorBlock
    {
        public ConsoleColor forColor;
        public ConsoleColor backColor;
    }

    public class PlayerCharacter : Character, IMovable
    {
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

        public void Move (int _x, int _y)
        {
            x += _x;
            y += _y;
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
        public List<GameBiome> readBiomes = new List<GameBiome>();
        GameMob[] mobsOnMap = new GameMob[2];
        GameBiome[] biomesOnMap = new GameBiome[2];
        public List<GameMob> readMobs = new List<GameMob>();

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
            CreateMap();
            SetColorMap();
        }

        void SetBiomeSmoothnes()
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
            GenerateMobs();

            //create exit
            int l = Evgen.Next(1, gameParams.width - GlobalGameParams.WidthOfExit);
            for (int i = 0; i < GlobalGameParams.WidthOfExit; i++, l++)
            {
                map[gameParams.height - 1, l] = Exit;
            }
        }

        void SetColorMap()
        {

            for (int i = 0; i < (gameParams.height / 2) - gameParams.biomeSmoothnes / 2; i++)
                for (int j = 0; j < gameParams.width; j++)
                {
                    colorMap[i, j].forColor = biomesOnMap[0].commonHalf.forColor;
                    colorMap[i, j].backColor = biomesOnMap[0].commonHalf.backColor;
                }
            for (int i = gameParams.height - 1; i > gameParams.height / 2 + gameParams.biomeSmoothnes / 2; i--)
                for (int j = 0; j < gameParams.width; j++)
                {
                    colorMap[i, j].forColor = biomesOnMap[1].commonHalf.forColor;
                    colorMap[i, j].backColor = biomesOnMap[1].commonHalf.backColor;
                }

            int randomOfSmoth = 0;
            for (int j = 0; j < gameParams.width; j++)
            {
                randomOfSmoth = Evgen.Next(gameParams.biomeSmoothnes / 2 * -1, gameParams.biomeSmoothnes / 2 + 1);
                for (int i = (gameParams.height / 2) - gameParams.biomeSmoothnes / 2; i < (gameParams.height / 2) - randomOfSmoth; i++)
                {
                    colorMap[i, j].forColor = biomesOnMap[0].commonHalf.forColor;
                    colorMap[i, j].backColor = biomesOnMap[0].commonHalf.backColor;
                }
                for (int i = (gameParams.height / 2) - gameParams.biomeSmoothnes / 2 - randomOfSmoth; i <= gameParams.height / 2 + gameParams.biomeSmoothnes / 2; i++)
                {
                    colorMap[i, j].forColor = biomesOnMap[1].commonHalf.forColor;
                    colorMap[i, j].backColor = biomesOnMap[1].commonHalf.backColor;
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

            mobsOnMap[0].x = Evgen.Next(1, gameParams.width - 1);
            mobsOnMap[0].y = Evgen.Next(2, gameParams.height / 2);
            mobsOnMap[1].x = Evgen.Next(1, gameParams.width - 1);
            mobsOnMap[1].y = Evgen.Next(gameParams.height / 2, gameParams.height - 1);
        }

        void MobControler(ref GameMob mob)
        {
            int xRand = Evgen.Next(-1, 2);
            int yRand = Evgen.Next(-1, 2);
            if (CanMove(xRand, yRand, mob.x, mob.y))
                mob.Move(xRand, yRand);
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
                Console.ForegroundColor = mobsOnMap[y < gameParams.height / 2 ? 0 : 1].commonHalf.color;
                Console.Write(mobsOnMap[y < gameParams.height / 2 ? 0 : 1].commonHalf.sym);
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
            MobControler(ref mobsOnMap[0]);
            MobControler(ref mobsOnMap[1]);
        }

        void TryMove(int x, int y)
        {
            if (OnMob(x, y, out GameMob mob))
                player.HP -= mob.commonHalf.Damage;
            else if (CanMove(x, y, player.x, player.y))
                player.Move(x, y);
        }

        bool CanMove(int x, int y, int xObj, int yObj)
        {
            return (map[yObj + y, xObj + x] == nullCell || map[yObj + y, xObj + x] == Exit);
        }

        bool OnMob(int x, int y, out GameMob mob)
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
            mob = new GameMob();
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

        /*void Move(int x, int y, ref object obj)
        {
            obj.x += x;
            yObj += y;
        }*/

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
