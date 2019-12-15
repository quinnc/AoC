using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day9.Compiler
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
        string savedCode;

        public void Program (string code)
        {
            savedCode = code;
            //foreach (var c in compilers)
            for (int i =- 0; i < 5; i++)
            {
                compilers[i] = new CodeRunner();
                compilers[i].Code = code;
            }
        }

        public string Execute()
        {
            SortedSet<int> inputs0 = new SortedSet<int> { 0, 1, 2, 3, 4 };
            //Parallel.ForEach(inputs0,
            //    stage0input =>
            //    {
            //        RunCurrentCompiler("0", stage0input, inputs0, 0);
            //    });

            string r = "";

            for (var i=0; i < 5; i++)
            {
                r += RunCurrentCompiler("0", i, inputs0, 0) + Environment.NewLine;
            }


            //return string.Join (Environment.NewLine,_results.OrderBy(x => x));
            return _results.OrderByDescending(x => x).FirstOrDefault().ToString();
        }

        private string RunCurrentCompiler(string previousResult, int currentSetting, SortedSet<int> settingsList, int depth)
        {
            Debug.Assert(depth < compilers.Length);

            compilers[depth].Code = savedCode;
            compilers[depth].ExternalInput = new string[] { currentSetting.ToString(), previousResult };
            compilers[depth].Run();

            SortedSet<int> inputList = new SortedSet<int>(settingsList);
            inputList.Remove(currentSetting);

            if (inputList.Count == 0)
            {
                int finalout = 0;
                bool ok = false;
                ok = Int32.TryParse(compilers[depth].ExternalOutput, out finalout);
                _results.Add(finalout);
                return depth.ToString() + ":" + compilers[depth].ExternalOutput;  // when there is no more
            }

            string ret = "";
            foreach (var nextStageInput in inputList)
            {
                ret += RunCurrentCompiler(compilers[depth].ExternalOutput, nextStageInput, inputList, depth + 1) + Environment.NewLine;
            }

            return ret;
        }

        
    }
}
