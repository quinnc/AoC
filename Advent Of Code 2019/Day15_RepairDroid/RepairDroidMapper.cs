using GalaSoft.MvvmLight;
using IntCode;
using Overby.Collections;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
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
            sectionMap.Add(new Location(0, 0), '+');
            repairDroid.ExternalOutput = droidOutput;
            repairDroid.RunInThread();
        }

        public RepairDroidMapper(string _code)
        {
            sectionMap.Add(new Location(0, 0), '+');
            repairDroid.ExternalOutput = droidOutput;
            Code = _code;
        }


        public void Start()
        {
            repairDroid.RunInThread();
        }

        public void Stop()
        {
            repairDroid.Stop();
        }

        public enum Direction
        {
            North = 1,
            South = 2,
            West = 3,
            East = 4
        }

        public enum Space
        {
            Home,
            Hall,
            Wall,
            Hole
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

        public void AutoSearch(out int distanceToHole, out int longestPathFromHole)
        {

            currX = 0;
            currY = 0;
            distanceToHole = 0;
            repairDroid.DefaultSleepTime = 0;

            TreeNode<Space> searchRoot = new TreeNode<Space>(Space.Home);

            distanceToHole = DepthFirstSearch(searchRoot, 0, 0);

            longestPathFromHole = LongestPath(searchRoot);
        }

        public int LongestPath(TreeNode<Space> root)
        {

#if OLDCODE
            List<int> depths = new List<int>();
            var flatnodes = root.FlattenNode();

            foreach (var n in flatnodes)
            {
                if (n.NumChildren == 0)
                {
                    depths.Add(n.Depth);
                }
            }

            depths.Sort();
            depths.Reverse();

            if (depths[0] == distanceToHole)
                longestOtherPath = depths[1];
            else
                longestOtherPath = depths[0];
#endif

            int maxpath = 0;
            object lockingVar = new object();


            Parallel.ForEach(root.Children, c =>
                {
                    if (c.Find(Space.Hole) == null)
                    {
                        int childMaxPath = 0;
                        c.MaxPath(ref childMaxPath);

                        lock (lockingVar)
                        {
                            if (childMaxPath > maxpath)
                                maxpath = childMaxPath;
                        }
                    }
                });

            return maxpath;
        }

        private int DepthFirstSearch(TreeNode<Space> currNode, int _currX, int _currY)
        {
            int foundDepth = Int32.MaxValue;

            foreach (var dir in Enum.GetValues(typeof(Direction)))
            {

                int attemptedX = _currX;
                int attemptedY = _currY;
                switch (dir)
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

                if (sectionMap.ContainsKey(attemptedLoc))
                {
                    // then already been here, could be loop or just going backwards
                    continue;
                }

                // else ask the droid to move
                repairDroid.ExternalInput.Add(((int)(dir)).ToString());
                // wait for reponse
                string _out = droidOutput.Take();

                if (_out == "0")
                {
                    // hit a wall, so add wall to map, but don't move droid
                    sectionMap[attemptedLoc] = '=';
                    currNode.AddChild(Space.Wall);


                    if (attemptedX < minX) minX = attemptedX;
                    if (attemptedX > maxX) maxX = attemptedX;
                    if (attemptedY < minY) minY = attemptedY;
                    if (attemptedY > maxY) maxY = attemptedY;
                    RaisePropertyChanged(nameof(Map));
                }
                else
                {
                    if (_out == "1")
                    {
                        // moved into open space
                        sectionMap[attemptedLoc] = '.';
                    }
                    else if (_out == "2")
                    {
                        sectionMap[attemptedLoc] = '!';
                    }
                    else
                    {
                        Debugger.Break();
                    }

                    currX = attemptedX;
                    currY = attemptedY;

                    if (attemptedX < minX) minX = attemptedX;
                    if (attemptedX > maxX) maxX = attemptedX;
                    if (attemptedY < minY) minY = attemptedY;
                    if (attemptedY > maxY) maxY = attemptedY;
                    RaisePropertyChanged(nameof(Map));

                    if (_out == "1")
                    {
                        var childnode = currNode.AddChild(Space.Hall);
                        int a = DepthFirstSearch(childnode, attemptedX, attemptedY);
                        if (a > 0 && a < foundDepth)
                        {
                            foundDepth = a;
                            //break;
                        }
                    }
                    else if (_out == "2")
                    {
                        var childnode = currNode.AddChild(Space.Hole);
                        if (childnode.Depth > 0 && childnode.Depth < foundDepth)
                        {
                            foundDepth = childnode.Depth;
                            //break;
                        }
                    }

                    // finished searching that depth, now back up 
                    int revDir = 0;
                    switch (dir)
                    {
                        case Direction.North:
                            revDir = (int)Direction.South;
                            break;

                        case Direction.South:
                            revDir = (int)Direction.North;
                            break;

                        case Direction.East:
                            revDir = (int)Direction.West;
                            break;

                        case Direction.West:
                            revDir = (int)Direction.East;
                            break;
                    }

                    repairDroid.ExternalInput.Add($"{revDir}");
                    // wait for reponse
                    droidOutput.Take();

                }
            }

            return foundDepth;
        }
    }
}
