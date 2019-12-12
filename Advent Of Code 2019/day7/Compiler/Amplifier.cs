using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace day7.Compiler
{
    //struct AmplifyResult
    //    {
    //    string desc;
    //    int outVal;
    //}

    class Amplifier
    {
        CodeRunner[] compilers = new CodeRunner[5];

        //ConcurrentBag<AmplifyResult> results;
        ConcurrentBag<int> _results = new ConcurrentBag<int>();

        public void Program (string code)
        {
            foreach (var c in compilers)
            {
                c.Code = code;
            }
        }

        public string Execute()
        {
            SortedSet<int> inputs0 = new SortedSet<int> { 0, 1, 2, 3, 4 };
            Parallel.ForEach(inputs0,
                stage0input =>
                {
                    RunCurrentCompiler("0", stage0input, inputs0, 0);
                });

            return string.Join (Environment.NewLine,_results.OrderBy(x => x));
        }

        private void RunCurrentCompiler(string previousResult, int currentSetting, SortedSet<int> settingsList, int depth)
        {
            Debug.Assert(depth < compilers.Length);

            compilers[depth].ExternalInput = new string[] { previousResult, currentSetting.ToString() };
            compilers[depth].Run();

            SortedSet<int> inputList = new SortedSet<int>(settingsList);
            inputList.Remove(currentSetting);

            if (inputList.Count == 0)
            {
                int finalout = 0;
                bool ok = false;
                ok = Int32.TryParse(compilers[depth].ExternalOutput, out finalout);
                _results.Add(finalout);
                return;  // when there is no more
            }

            foreach (var nextStageInput in inputList)
            {
                RunCurrentCompiler(compilers[depth].ExternalOutput, nextStageInput, inputList, depth + 1);
            }
        }

        
    }
}
