using System.Diagnostics;
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

        public IThreadible Instance { get; set; }

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
            if (Instance == null)
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
