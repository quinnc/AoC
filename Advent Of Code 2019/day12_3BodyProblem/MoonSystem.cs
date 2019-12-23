using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace day12_3BodyProblem
{
    class MoonSystem
    {
        Moon[] moons = new Moon[4];
        private readonly List<long> States = new List<long>();

        public void SetLocations(string input)
        {
            string[] lines = Regex.Split(input, "\r\n|\n|\r");
            int currMoon = 0;
            char[] separatingChars = { '=', ',', '<', '>' };

            // for each line, get out the coordinates
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;
                string[] words = line.Split(separatingChars, System.StringSplitOptions.RemoveEmptyEntries);

                //MessageBox.Show(String.Join(" || ", words));

                moons[currMoon] = new Moon(words[1], words[3], words[5]);
                currMoon++;

                if (currMoon >= moons.Length)
                    break;
            }
        }



        private void UpdateSpeedPair(Moon m1, Moon m2)
        {
            UpdateSpeedPairAxis(m1.location.x, m2.location.x, ref m1.speed.x, ref m2.speed.x);
            UpdateSpeedPairAxis(m1.location.y, m2.location.y, ref m1.speed.y, ref m2.speed.y);
            UpdateSpeedPairAxis(m1.location.z, m2.location.z, ref m1.speed.z, ref m2.speed.z);
        }

        private void UpdateSpeedPairAxis(int x1, int x2, ref int x3, ref int x4)
        {
            if (x1 < x2)
            { x3++; x4--; }
            else if (x1 > x2)
            { x3--; x4++; }
            //else speed stays the same
        }

        private void RunStep()
        {
            moons[0].UpdateSpeed(moons[1], moons[2], moons[3]);
            moons[1].UpdateSpeed(moons[0], moons[2], moons[3]);
            moons[2].UpdateSpeed(moons[1], moons[0], moons[3]);
            moons[3].UpdateSpeed(moons[1], moons[2], moons[0]);

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


        private void RunStepOnePerPair()
        {
            Parallel.For(0, 2,
                n =>
                {
                    // 0,1 then 2,3
                    UpdateSpeedPair(moons[2 * n], moons[2 * n + 1]);
                });

            Parallel.For(0, 2,
                n =>
                {
                    // 0, 2 then 1, 3
                    UpdateSpeedPair(moons[n], moons[n + 2]);
                });

            Parallel.For(0, 2,
                n =>
                {
                    // 0, 3 then 1, 2
                    UpdateSpeedPair(moons[n], moons[3 - n]);
                });


            Parallel.For(0, 4,
                n => { moons[n].UpdateLocation(); });
        }

        public void SimulateWithRepeats(out ulong stepsUntilRepeats)
        {
            bool foundRepeat = false;
            stepsUntilRepeats = 0;

            int[] InitialStates = new int[4];

            for (int i = 0; i < 4; i++)
            {
                InitialStates[i] = moons[i].GetHashCode();
            }

            while (!foundRepeat)
            {
                stepsUntilRepeats++;
                RunStepOnePerPair();

                foundRepeat = IsMatchedState(InitialStates);

                if (stepsUntilRepeats >= System.UInt32.MaxValue)
                    break;
            }
        }

        private bool IsMatchedState(int[] initialStates)
        {
            bool match = true;
            for (int i = 0; i < 4; i++)
            {
                match &= (initialStates[i] == moons[i].GetHashCode());
            }
            return match;
        }


        private ulong Hash(ulong currHash, int b, int shift)
        {

            /* 
             * public int GetHashCode()
{
    return a.GetHashcode() ^ b.GetHashcode().RotateLeft(16);
}

public static uint RotateLeft(this uint value, int count)
{
    return (value << count) | (value >> (32 - count))
}
*/

            if /*(a > 1<<8 ||*/(b > 1 << 16)
            {
                MessageBox.Show($"numbers are too big! {b}");
                return 0;
            }
            if (shift > 56)
            {
                MessageBox.Show($"shift is too high (max 56) {shift}");
                return 0;
            }

            return currHash ^ (((ulong)b << shift) | ((ulong)b >> (64 - shift)));
        }


        private void GetHashes(ulong[] xHash, ulong[] yHash, ulong[] zHash)
        {
            xHash[0] = 0; xHash[1] = 0; // = { 0,0};
            yHash[0] = 0; yHash[1] = 0; //yHash = 0;
            zHash[0] = 0; zHash[1] = 0; //zHash = 0;

            int sft = 0;

            for (int i = 0; i < 4; i++)
            {
                sft = i * 16;

                xHash[0] = Hash(xHash[0], moons[i].location.x, sft);
                xHash[1] = Hash(xHash[1], moons[i].speed.x, sft);

                yHash[0] = Hash(yHash[0], moons[i].location.y, sft);
                yHash[1] = Hash(yHash[1], moons[i].speed.y, sft);

                zHash[0] = Hash(zHash[0], moons[i].location.z, sft);
                zHash[1] = Hash(zHash[1], moons[i].speed.z, sft);
            }

        }


        public void FindByLeastCommonMultiple(out ulong stepsUntilRepeat)
        {
            stepsUntilRepeat = 0;
            long xRep = -1, yRep = -1, zRep = -1;

            ulong[] xInit = new ulong[2];
            ulong[] yInit = new ulong[2];
            ulong[] zInit = new ulong[2];

            GetHashes(xInit, yInit, zInit);

            while (stepsUntilRepeat < System.UInt32.MaxValue &&
                (xRep == -1 || yRep == -1 || zRep == -1))
            {
                stepsUntilRepeat++;
                RunStepOnePerPair();

                ulong[] xCurr = new ulong[2], yCurr = new ulong[2], zCurr = new ulong[2];
                GetHashes(xCurr, yCurr, zCurr);

                if (xRep == -1 && xCurr[0] == xInit[0] && xCurr[1] == xInit[1])
                {
                    xRep = (long)stepsUntilRepeat;
                }

                if (yRep == -1 && yCurr[1] == yInit[1] && yCurr[0] == yInit[0])
                {
                    yRep = (long)stepsUntilRepeat;
                }

                if (zRep == -1 && zCurr[0] == zInit[0] && zCurr[1] == zInit[1])
                {
                    zRep = (long)stepsUntilRepeat;
                }

            }

            long[] rep = new long[] { xRep, yRep, zRep };

            long mult = MathUtils.LCM(rep);

            stepsUntilRepeat = (ulong)mult;

        }

        public int TotalEnergy => moons[0].TotalEnergy + moons[1].TotalEnergy + moons[2].TotalEnergy + moons[3].TotalEnergy;

        public string SystemDescription
            => moons[0].ToString() + Environment.NewLine +
             moons[1].ToString() + Environment.NewLine +
             moons[2].ToString() + Environment.NewLine +
             moons[3].ToString() + Environment.NewLine;
    }
}
