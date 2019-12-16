using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Day10.Asteroids
{
    struct VisibleAsteroidsFrom
    {
        public int numAsteroidsVisible;
        // source asteroid
        public int x;
        public int y;

    }

    class Mapper
    {

        public string[] rawMap;
        public int[,] asteroids_in_space;
        int maxY;
        int maxX;

        public ConcurrentBag<VisibleAsteroidsFrom> visiblesList = new ConcurrentBag<VisibleAsteroidsFrom>();

        public bool SetMap(string mapStrs)
        {
            rawMap = Regex.Split(mapStrs, ",|\r\n|\r|\n");

            maxX = rawMap[0].Length;
            maxY = rawMap.Count();

            asteroids_in_space = new int[maxY, maxX];

            //// x is left to right, y is top to bottom
            //// 0,0 is the top left; 1,0 is the first space to the right
            //// but my arrays go the other way so [0,1] is the first space to the right
            //int x = 0, y = 0;

            //foreach (var row in rawMap)
            //{
            //    foreach (var c in row)
            //    {
            //        asteroids_in_space[y, x] = (c == '#') ? 1 : 0;
            //        x++;
            //    }
            //    y++;
            //}
            return true;
        }

        public void FindSightLines()
        {
            Parallel.For(0, maxY,
               y =>
                 {
                     // or do in parallel

                     // for each x position
                     for (int x = 0; x < maxX; x++)
                     {
                         // for each location

                         // is this position an asteroid?
                         if (rawMap[y][x] == '#')
                         {
                             // if so, then look for all the asteroids that it can see

                             Dictionary<Tuple<int, int>, bool> visited = new Dictionary<Tuple<int, int>, bool>();

                             // going up from point
                             for (int skipY = 0; skipY >= (-1 * y); skipY--)
                             {
                                 // going right from point
                                 for (int skipX = 0; skipX < (maxX - x); skipX++)
                                 {
                                     SearchMultipliers(x, y, skipX, skipY, ref visited);
                                 }

                                 // going left from point
                                 for (int skipX = 0; skipX >= (-1 * x); skipX--)
                                 {
                                     SearchMultipliers(x, y, skipX, skipY, ref visited);
                                 }
                             }

                             // going down from the point
                             for (int skipY = 0; skipY < (maxY - y); skipY++)
                             {
                                 // going right from point
                                 for (int skipX = 0; skipX < (maxX - x); skipX++)
                                 {
                                     SearchMultipliers(x, y, skipX, skipY,ref visited);

                                 }
                                 // going left from point
                                 for (int skipX = 0; skipX >= (-1 * x); skipX--)
                                 {
                                     SearchMultipliers(x, y, skipX, skipY, ref visited);

                                 }
                             }

                             // add to bag
                             var vaf = new VisibleAsteroidsFrom() { numAsteroidsVisible = asteroids_in_space[y, x] };
                             vaf.x = x;
                             vaf.y = y;
                             visiblesList.Add(vaf);
                         }
                     }
                 });
        }

        private void SearchMultipliers(int x, int y, int skipX, int skipY, ref Dictionary<Tuple<int, int>, bool> visited)
        {
            if (skipX == 0 && skipY == 0)
                return;

            // if this is multiple of a previous point(s), then only check if there wasn't an asteroid in those (e.g. blocking the view)
            int multiplier = 1;
            int checkX = x + skipX * multiplier;
            int checkY = y + skipY * multiplier;
            var checkTup = new Tuple<int, int>(checkX, checkY);

            // if we've already checked this square, skip
            if (visited.ContainsKey(checkTup) && visited[checkTup])
                return;

            bool foundAster = false;

            // else
            while (checkX >= 0 && checkX < maxX && checkY >= 0 && checkY < maxY)
            {
                if (!foundAster)
                {
                    // else if there is an asteroid here, add one to the viewable
                    if (rawMap[checkY][checkX] == '#')
                    {
                        asteroids_in_space[y, x]++;
                        foundAster = true;
                    }
                }

                visited[checkTup] = true;

                multiplier++;
                checkX = x + skipX * multiplier;
                checkY = y + skipY * multiplier;
                checkTup = new Tuple<int, int>(checkX, checkY);
            }
        }

        public void GetMostVisibile(out VisibleAsteroidsFrom best)
        {
            best = visiblesList.OrderBy(x => x.numAsteroidsVisible).Last();
        }

        public void MakeVaporizeList(int maxBlasts, out int lastX, out int lastY)
        {
            // as we vaporize asteroids, remove them from here
            var vaporizedMap = new string[rawMap.Length];

            rawMap.CopyTo(vaporizedMap, 0);

            int x = 0;
            int y = 0;

            VisibleAsteroidsFrom best = new VisibleAsteroidsFrom();
            GetMostVisibile(out best);
            x = best.x;
            y = best.y;

            // each time we vaporize an asteroid, add the x,y into here
            List<Tuple<int, int>> vaporedList = new List<Tuple<int, int>>();

            // if we blast an asteroid, set this to true as a safety
            bool blasted = false;

            while (vaporedList.Count < maxBlasts)
            {
                // keep track of places we've visited and the rays that extend through it, but reset on each loop (360 deg)
                Dictionary<Tuple<int, int>, bool> visited = new Dictionary<Tuple<int, int>, bool>();

                // go right & up
                for (int skipX = 0; skipX < (maxX - x); skipX++)
                    for (int skipY = 0; skipY >= (-1 * y); skipY--)
                    {
                        bool foundAster = false;
                        BlastThem(x, y, skipX, skipY, visited, vaporizedMap, vaporedList, ref foundAster);
                        blasted |= foundAster;
                    }

                // for down & right
                for (int skipY = 0; skipY < (maxY - y); skipY++)
                    for (int skipX = 0; skipX < (maxX - x); skipX++)
                    {
                        bool foundAster = false;
                        BlastThem(x, y, skipX, skipY, visited, vaporizedMap, vaporedList, ref foundAster);
                        blasted |= foundAster;
                    }

                // going left & down
                for (int skipX = 0; skipX >= (-1 * x); skipX--)
                    for (int skipY = 0; skipY < (maxY - y); skipY++)
                    {
                        bool foundAster = false;
                        BlastThem(x, y, skipX, skipY, visited, vaporizedMap, vaporedList, ref foundAster);
                        blasted |= foundAster;
                    }

                // going left & up
                for (int skipY = 0; skipY >= (-1 * y); skipY--)
                    for (int skipX = 0; skipX >= (-1 * x); skipX--)
                    {
                        bool foundAster = false;
                        BlastThem(x, y, skipX, skipY, visited, vaporizedMap, vaporedList, ref foundAster);
                        blasted |= foundAster;
                    }

                if (!blasted)
                    break;

                // else reset
                blasted = false;

            }

            lastX = -1;
            lastY = -1;

            if (vaporedList.Count >= maxBlasts)
            {
                lastX = vaporedList[maxBlasts - 1].Item1;
                lastY = vaporedList[maxBlasts - 1].Item2;
            }

        }

        private void BlastThem(int x, int y, int skipX, int skipY, Dictionary<Tuple<int, int>, bool> visited,  string[] vaporizedMap,  List<Tuple<int, int>> vaporedList, ref bool foundAster)
        {
            // don't blast ourselves
            if (skipX == 0 && skipY == 0)
                return;

            // if this is multiple of a previous point(s), then only check if there wasn't an asteroid in those (e.g. blocking the view)
            int multiplier = 1;
            int checkX = x + skipX * multiplier;
            int checkY = y + skipY * multiplier;
            var checkTup = new Tuple<int, int>(checkX, checkY);

            // if we've already checked this square, skip
            if (visited.ContainsKey(checkTup) && visited[checkTup])
                return;

 
            // else
            while (checkX >= 0 && checkX < maxX && checkY >= 0 && checkY < maxY)
            {
                if (!foundAster)
                {
                    // else if there is an asteroid here, add one to the viewable
                    if (vaporizedMap[checkY][checkX] == '#')
                    {
                        // remove the asteroid from the map
                        var ca = vaporizedMap[checkY].ToCharArray();
                        ca[checkX]= '.';
                        vaporizedMap[checkY] = new string(ca);

                        //add the destuction to the list
                        vaporedList.Add(new Tuple<int, int>(checkX, checkY));

                        // set found flag so that we don't blast anymore in this multiplier
                        foundAster = true;
                    }
                }

                visited[checkTup] = true;

                multiplier++;
                checkX = x + skipX * multiplier;
                checkY = y + skipY * multiplier;
                checkTup = new Tuple<int, int>(checkX, checkY);
            }
        }
    }
}
