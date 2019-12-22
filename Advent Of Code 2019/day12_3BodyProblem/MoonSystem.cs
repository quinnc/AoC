using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace day12_3BodyProblem
{
    class MoonSystem
    {
        Moon[] moons = new Moon[4];

        public void SetLocations (string input)
        {
            string[] lines = Regex.Split(input, "\r\n|\n|\r");
            int currMoon = 0;
            char[] separatingChars = { '=',',' };

            // for each line, get out the coordinates
            foreach (var line in lines)
            {
                string[] words = line.Split(separatingChars, System.StringSplitOptions.RemoveEmptyEntries);

                moons[currMoon] = new Moon(words[1], words[3], words[5]);
                currMoon++;
            }
        }

        private void RunStep ()
        {
            moons[0].UpdateSpeed(moons[1], moons[2], moons[3]);
            moons[1].UpdateSpeed(moons[0], moons[2], moons[3]);
            moons[2].UpdateSpeed(moons[1], moons[0], moons[3]);
            moons[2].UpdateSpeed(moons[1], moons[2], moons[0]);

            moons[0].UpdateLocation();
            moons[1].UpdateLocation();
            moons[2].UpdateLocation();
            moons[3].UpdateLocation();
        }

        public void RunSteps(int steps)
        {
            for (int i = 0; i < steps; i++)
            {
                RunStep();
            }
        }

        public int TotalEnergy => moons[0].TotalEnergy + moons[1].TotalEnergy + moons[2].TotalEnergy + moons[3].TotalEnergy;
    }
}
