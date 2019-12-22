using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Day11
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

    class PaintTracker
    {
        public static string HALT = "halt";
        protected BlockingCollection<string> externalInput = new BlockingCollection<string>();
        protected BlockingCollection<string> externalOutput = null;

        protected Dictionary<Location, Colour> visited = new Dictionary<Location, Colour>();

        Location current = new Location(0, 0);
        Direction facing = Direction.Up;

        private int minX, maxX, minY, maxY;

        // takes in the output from the painting program
        // -> first output is the colour to paint the current square
        // -> second output is the directory to turn (then it moves forward one square in that direction) 
        public BlockingCollection<string> Input => externalInput;

        public BlockingCollection<string> Output
        {
            get => externalOutput;
            set => externalOutput = value;
        }

        private bool threadedResult = false;
        private Thread th;

        private bool Run()
        {
            if (Output == null)
            {
                Debugger.Break();
                return false;
            }

            var val = externalInput.Take();
            bool isColour = true;

            while (val != HALT)
            {
                int valInt = 0;
                bool ok = false;

                ok = Int32.TryParse(val, out valInt);

                if (!ok)
                {
                    Debugger.Break();
                    return false;
                }

                if (isColour)
                {
                    PaintSquare(valInt);
                    // next will be a move
                    isColour = false;
                }
                else
                {
                    MoveRobot(valInt);

                    // next will be a colour
                    isColour = true;
                }

                val = externalInput.Take();
            }


            return true;
        }

        private void MoveRobot(int turnType)
        {
            Turn(turnType);

            current = StepOne();

            // report the current colour to compiler
            if (visited.ContainsKey(current))
            {
                Output.Add(((int)visited[current]).ToString());
            }
            else
            {
                // if haven't been here before, then it is black
                Output.Add("0");
            }
        }

        private Location StepOne()
        {
            // move the robot in that direction
            Location l = current;

            switch (facing)
            {
                case Direction.Up:
                    l = new Location(current.x, current.y + 1);
                    break;

                case Direction.Right:
                    l = new Location(current.x + 1, current.y);
                    break;

                case Direction.Down:
                    l = new Location(current.x, current.y - 1);
                    break;

                case Direction.Left:
                    l = new Location(current.x - 1, current.y);
                    break;
            }


            if (l.x < minX)
                minX = l.x;

            if (l.x > maxX)
                maxX = l.x;

            if (l.y < minY)
                minY = l.y;

            if (l.y > maxY)
                maxY = l.y;

            return l;
        }

        private void Turn(int turnType)
        {
            // update the way the robot is facing
            switch (facing)
            {
                case Direction.Up:
                {
                    if (turnType == 0)
                        facing = Direction.Left;
                    else
                        facing = Direction.Right;
                }
                break;

                case Direction.Left:
                {
                    if (turnType == 1)
                        facing = Direction.Up;
                    else
                        facing = Direction.Down;
                }
                break;

                default:
                {
                    // for the middle directions
                    if (turnType == 0)
                        facing--;
                    else
                        facing++;

                }
                break;
            }
        }

        private void PaintSquare(int colourInt)
        {
            Colour colour = Colour.Black;
            if (colourInt == 1)
                colour = Colour.White;

            // re-reading is sounds like black-on-black counts as a point
            //if (!visited.ContainsKey(current))
            //{
            //    if (colour == Colour.White)
            //    {
            //        visited[current] = colour;
            //    }
            //    // else do nothing
            //}
            //else
            {
                // we already changed the colour once so we don't count repaints, but track current colour anyway
                visited[current] = colour;
            }

        }

        private static void Threadify(object inst)
        {
            if (inst is PaintTracker crInst)
            {
                crInst.threadedResult = crInst.Run();
            }
            else
            {
                Debugger.Break();
            }
        }

        public void RunInThread()
        {

            th = new Thread(Threadify);
            th.Start(this);

            Thread.Sleep(0);
        }

        public bool ThreadedResult()
        {
            th.Join();
            return threadedResult;
        }


        public int PaintedSquares => visited.Count;

        public IReadOnlyList<string> Image
        {
            get
            {
                var _image = new List<string>();
                for (int x = minX; x <= maxX; x++)
                {
                    string s = "";

                    for (int y = minY; y <= maxY; y++)
                    {
                        var l = new Location(x, y);

                        if (visited.ContainsKey(l))
                        {
                            s += (visited[l] == Colour.Black) ? ' ' : '*';
                        }
                        else
                        {
                            if (x == 0 && y == 0)
                                Debugger.Break();
                            s += ' ';
                        }
                    }

                    _image.Add(s);
                }

                return _image;
            }
        }

        public IReadOnlyList<string> ImageRotate
        {
            get
            {
                var _image = new List<string>();
                for (int y = maxY; y >= minY; y--)
                {
                    string s = "";
                    for (int x = minX; x <= maxX; x++)
                    {
                        var l = new Location(x, y);

                        if (visited.ContainsKey(l))
                        {
                            s += (visited[l] == Colour.Black) ? ' ' : '*';
                        }
                        else
                        {
                            if (x == 0 && y == 0)
                                Debugger.Break();
                            s += ' ';
                        }
                    }

                    _image.Add(s);
                }

                return _image;
            }
        }
    }
}
