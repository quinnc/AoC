using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace day7.Compiler
{
    struct AmplifyResult
        {
        string desc;
        int outVal;
    }

    class Amplifier
    {
        CodeRunner[] compilers = new CodeRunner[5];

        ConcurrentBag<AmplifyResult> results;

        public void Program (string code)
        {
            foreach (var c in compilers)
            {
                c.Code = code;
            }
        }

        public void Execute ()
        {
            Parallel.For(0, 5,
                stage1input =>
                {
                    compilers[0].ExternalInput = new string[]{ "0", stage1input.ToString()};
                    compilers[0].Run();

                    SortedSet<int> inputs2 = new SortedSet<int> { 0, 1, 2, 3, 4, 5 };
                    inputs2.Remove(stage1input);

                    foreach (var stage2input in inputs2)
                    {
                        compilers[1].ExternalInput = new string[] { compilers[1], stage2input.ToString() };
                        compilers[1].Run();

                    }


                });
        }


    }
}
