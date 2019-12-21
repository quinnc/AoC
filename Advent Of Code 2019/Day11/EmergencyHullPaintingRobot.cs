using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day11
{
    class EmergencyHullPaintingRobot
    {

        private string initcode = null;
        //private string currentcode;
        private PaintTracker tracker = new PaintTracker();

        public void SetCode(string _code)
        {
            initcode = _code;
        }

        public void Paint (int inp)
        {
            ParallelCodeRunner intcomp = new ParallelCodeRunner();
            

            tracker.Output = intcomp.ExternalInput;

            intcomp.ExternalOutput = tracker.Input;
            intcomp.Code = initcode;
            intcomp.ExternalInput.Add(inp.ToString());

            intcomp.RunInThread();
            tracker.RunInThread();

            bool r = intcomp.ThreadedResult();

            // now that the computer is done, tell the tracker to stop too.
            tracker.Input.Add(PaintTracker.HALT);

            bool s = tracker.ThreadedResult();

        }

        public int PaintedSquares => tracker.PaintedSquares;
    }
}
