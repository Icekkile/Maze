using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseData
{
    [Serializable]
    public class Biome
    {
        public ConsoleColor forColor;
        public ConsoleColor backColor;
        public string name;

        public Biome() { }
    }

    [Serializable]
    public class Mob
    {
        public string name;
        public char sym;
        public ConsoleColor color;
        public int Damage;

        public Mob() { }
    }

    public class BaseData
    {
    }
}
