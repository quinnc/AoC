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

        public ConcurrentBag<VisibleAsteroidsFrom> visiblesList = new ConcurrentBag<VisibleAsteroidsFrom>();

        public bool SetMap(string mapStrs)
        {
            rawMap = Regex.Split(mapStrs, ",|\r\n|\r|\n");

            asteroids_in_space = new int[rawMap.Count(), rawMap[0].Length];

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
            int maxY = asteroids_in_space.GetLength(0);
            int maxX = asteroids_in_space.GetLength(1);
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
                                     SearchMultipliers(x, y, skipX, skipY, maxX, maxY, ref visited);

                                 }

                                 // going left from point
                                 for (int skipX = 0; skipX >= (-1 * x); skipX--)
                                 {
                                     SearchMultipliers(x, y, skipX, skipY, maxX, maxY, ref visited);

                                 }
                             }

                             // going down from the point
                             for (int skipY = 0; skipY < (maxY - y); skipY++)
                             {
                                 // going right from point
                                 for (int skipX = 0; skipX < (maxX - x); skipX++)
                                 {
                                     SearchMultipliers(x, y, skipX, skipY, maxX, maxY, ref visited);

                                 }
                                 // going left from point
                                 for (int skipX = 0; skipX >= (-1 * x); skipX--)
                                 {
                                     SearchMultipliers(x, y, skipX, skipY, maxX, maxY, ref visited);

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

        private void SearchMultipliers(int x, int y, int skipX, int skipY, int maxX, int maxY, ref Dictionary<Tuple<int, int>, bool> visited)
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
    }
}
