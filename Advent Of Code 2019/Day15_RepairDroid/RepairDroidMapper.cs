using GalaSoft.MvvmLight;
using IntCode;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using TwodTypes;

namespace Day15_RepairDroid
{
    internal class RepairDroidMapper : ViewModelBase
    {

        // use sequential execution engine
        ParallelCodeRunner repairDroid = new ParallelCodeRunner();

        public string Code
        {
            get => repairDroid.Code;
            set => repairDroid.Code = value;
        }

        Dictionary<Location, char> sectionMap = new Dictionary<Location, char>();
        int currX = 0;
        int currY = 0;

        int minX = 0, maxX = 0;
        int minY = 0, maxY = 0;

        BlockingCollection<string> droidOutput = new BlockingCollection<string>();



        public RepairDroidMapper()
        {
            sectionMap.Add(new Location(0, 0), '.');
            repairDroid.ExternalOutput = droidOutput;
            repairDroid.RunInThread();
        }

        public RepairDroidMapper(string _code)
        {
            sectionMap.Add(new Location(0, 0), '.');
            repairDroid.ExternalOutput = droidOutput;
            Code = _code;
        }


        public void Start()
        {
            repairDroid.RunInThread();
        }

        public enum Direction
        {
            North = 1,
            South = 2,
            West = 3,
            East = 4
        }

        public void Walk(Direction _dir)
        {
            repairDroid.ExternalInput.Add(((int)(_dir)).ToString());

            int attemptedX = currX;
            int attemptedY = currY;
            switch (_dir)
            {
                case Direction.North:
                {
                    attemptedY++;

                }
                break;

                case Direction.South:
                {
                    attemptedY--;
                }
                break;

                case Direction.East:
                {
                    attemptedX++;
                }
                break;

                case Direction.West:
                {
                    attemptedX--;
                }
                break;
            }

            Location attemptedLoc = new Location(attemptedX, attemptedY);

            // wait for reponse
            string _out = droidOutput.Take();

            if (_out == "0")
            {
                // hit a wall, so add wall to map, but don't move droid
                if (sectionMap.ContainsKey(attemptedLoc))
                {
                    Debug.Assert(sectionMap[attemptedLoc] == '=');
                }
                else
                {
                    sectionMap[attemptedLoc] = '=';
                }
            }
            else if (_out == "1")
            {
                // moved into open space
                if (sectionMap.ContainsKey(attemptedLoc))
                {
                    Debug.Assert(sectionMap[attemptedLoc] == '.');
                }
                else
                {
                    sectionMap[attemptedLoc] = '.';
                }
                currX = attemptedX;
                currY = attemptedY;
            }
            else if (_out == "2")
            {
                if (sectionMap.ContainsKey(attemptedLoc))
                {
                    Debug.Assert(sectionMap[attemptedLoc] == '$');
                }
                else
                {
                    sectionMap[attemptedLoc] = '$';

                }

                currX = attemptedX;
                currY = attemptedY;


                Debugger.Break();
            }
            else
            {
                Debugger.Break();
            }

            if (attemptedX < minX) minX = attemptedX;
            if (attemptedX > maxX) maxX = attemptedX;
            if (attemptedY < minY) minY = attemptedY;
            if (attemptedY > maxY) maxY = attemptedY;
            RaisePropertyChanged(nameof(Map));
        }

        public string Map
        {
            set
            {
                RaisePropertyChanged(nameof(Map));
            }
            get
            {
                string val = "";

                for (int y = maxY; y >= minY; y--)
                {
                    for (int x = minX; x <= maxX; x++)
                    {
                        Location loc = new Location(x, y);

                        if (sectionMap.ContainsKey(loc))
                        {
                            if (x == currX && y == currY)
                                val += '[';
                            else
                                val += ' ';
                            // else don't add space
                            val += sectionMap[loc];

                            if (x == currX && y == currY)
                                val += ']';
                            else
                                val += ' ';
                            // else don't add space
                        }
                        else
                            val += "   ";
                    }

                    val += Environment.NewLine;
                }

                return val;
            }

        }

    }
}
