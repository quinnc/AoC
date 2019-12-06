﻿using System;
using System.Collections.Generic;

namespace Day_3.Data
{

    class Overlaps
    {
        void Reset()
        {
            overlaps.Clear();
            distances.Clear();
        }

        public void Find(WireRoute w1, WireRoute w2)
        {
            Reset();

            foreach (var pointIn2 in w1.Route)
            {
                foreach (var pointIn1 in w2.Route)
                {
                    if (pointIn1.Item1 == pointIn2.Item1 &&
                        pointIn1.Item2 == pointIn2.Item2)
                    {
                        overlaps.Add(pointIn1);
                        // don't search anymore in case there are loops
                        break;
                    }
                }
            }
        }

        public override string ToString()
        {
            //return base.ToString();

            string outStr = "";
            foreach(var overlap in overlaps)
            {
                outStr += $" {overlap.Item1},{overlap.Item2} ; ";
            }

            return outStr;

        }

        private XYPairList overlaps = new XYPairList();
        private List<int> distances = new List<int>();

        public void CalcDistances()
        {
            distances.Clear();

            foreach (var overlap in overlaps)
            {
                distances.Add(Math.Abs(overlap.Item1) + Math.Abs(overlap.Item2));
            }
        }

        public void Shortest(out int x, out int y, out double dist)
        {
            x = 0;
            y = 0;
            dist = -10;

            int idxShortest = 0;


            for (int i = 0; i < distances.Count; i++)
            {
                if (dist <= 0)
                {
                    dist = distances[i];
                    idxShortest = i;
                }
                else if (distances[i] < dist)
                {
                    dist = distances[i];
                    idxShortest = i;

                }
            }

            x = overlaps[idxShortest].Item1;
            y = overlaps[idxShortest].Item2;
        }
    }
}
