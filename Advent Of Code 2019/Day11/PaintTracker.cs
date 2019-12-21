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
                Output.Add(visited[current].ToString());
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
            switch (facing)
            {
                case Direction.Up:
                    return new Location(current.x, current.y + 1);

                case Direction.Right:
                    return new Location(current.x + 1, current.y);

                case Direction.Down:
                    return new Location(current.x, current.y - 1);

                case Direction.Left:
                    return new Location(current.x - 1, current.y);
            }

            Debugger.Break();
            return current;
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

    }
}
