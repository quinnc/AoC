using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day11
{
    class EmergencyHullPaintingRobot
    {

        private string initcode;
        private string currentcode;
        private PaintTracker tracker;

        public void SetCode(string _code)
        {
            initcode = _code;
        }

        public void Paint (int inp)
        {
            ParallelCodeRunner intcomp = new ParallelCodeRunner();
            

            tracker.Output = intcomp.ExternalInput;
            intcomp.ExternalOutput = tracker.Input;
            intcomp.ExternalInput.Add("0");

        }

        public int PaintedSquares { get; private set; } = 0;
    }
}
