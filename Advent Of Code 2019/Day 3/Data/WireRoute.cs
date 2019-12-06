using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;


namespace Day_3.Data
{
    class WireRoute
    {
        void Reset()
        {
            route.Clear();
        }
        public bool MakeRoute(string line)
        {
            Reset();
            var steps = SplitLine(line);

            int currX = 0;
            int currY = 0;
            route.Clear();
            bool ok = true;

            foreach (var step in steps)
            {
                ok &= SplitStep(step, out char dir, out int count);
                if (!ok)
                    continue;
                ok &= Walk(ref currX, ref currY, dir, count);
            }
            return ok;
        }

        private bool Walk(ref int currX, ref int currY, char dir, int count)
        {
            for (int i = 0; i < count; i++)
            {
                switch (dir)
                {
                    case 'U':
                        currY++;
                        break;

                    case 'D':
                        currY--;
                        break;

                    case 'L':
                        currX--;
                        break;

                    case 'R':
                        currX++;
                        break;

                    default:
                        MessageBox.Show($"Unknown direction: {dir}");
                        return false;
                }

                route.Add(new Tuple<int, int>(currX, currY));
            }
            return true;

        }

        private bool SplitStep(string step, out char dir, out int count)
        {
            dir = 'U';
            count = 0;

            if (step.Length < 2)
            {
                MessageBox.Show($"String was too small: {step}");
                return false;
            }
            dir = step[0];

            var countStr = step.Substring(1);
            count = 0;
            var ok = Int32.TryParse(countStr, out count);

            if (!ok)
                MessageBox.Show($"Failed to parse -{step}- -{countStr}-");

            return ok;
        }

        private string[] SplitLine(string line)
        {
            return Regex.Split(line, ",");
        }

      

        private XYPairList route = new XYPairList();

        public XYPairList Route => route;
    }
}
