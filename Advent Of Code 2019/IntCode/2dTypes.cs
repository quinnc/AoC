
using System;

namespace TwodTypes
{
    class Location : Tuple<int, int>
    {
        public Location(int _x, int _y) : base(_x, _y)
        {
        }

        public int x => Item1;
        public int y => Item2;
    }

    enum Colour
    {
        Black = 0,
        White = 1
    }

    enum Direction
    {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3
    }

}