using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace IntCode
{
    interface IThreadible
    {
        bool StartMember();
    }

    class Threadify
    {

        private bool threadedResult = false;
        private Thread th;
        private IThreadible _inst;

        public IThreadible Instance
        {
            get => _inst;
            set => _inst = value;
        }

        // Write the code to call the subordinate class here
        //protected abstract bool StartMember();

        protected static void ThreadStart(object inst)
        {
            if (inst is Threadify crInst)
            {
                crInst.threadedResult = crInst.Instance.StartMember();
            }
            else
            {
                Debugger.Break();
            }
        }


        public void RunInThread()
        {
            if (_inst == null)
                Debugger.Break();

            th = new Thread(ThreadStart);
            th.Start(this);

            Thread.Yield();
            Thread.Sleep(0);
        }

        public bool ThreadedResult()
        {
            th.Join();
            return threadedResult;
        }
    }
}
