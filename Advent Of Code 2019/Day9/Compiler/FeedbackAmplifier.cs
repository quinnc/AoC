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

    class FeedbackAmplifier
    {
        

        //ConcurrentBag<AmplifyResult> results;
        ConcurrentBag<Tuple<int, string>> _results = new ConcurrentBag<Tuple<int, string>>();
        string savedCode;

        public void Program (string code)
        {
            savedCode = code;

        }

        public string Execute()
        {
            SortedSet<int> inputs0 = new SortedSet<int> { 9,8,7,6,5 };
            //foreach (var i0 in inputs0)
            Parallel.ForEach(inputs0,
                i0 =>
            {
                int[] i = new int[5];
                i[0] = i0;
                SortedSet<int> input1 = new SortedSet<int>(inputs0);
                input1.Remove(i0);

                foreach (var i1 in input1)
                {
                    i[1] = i1;
                    SortedSet<int> inputs2 = new SortedSet<int>(input1);
                    inputs2.Remove(i1);

                    foreach (var i2 in inputs2)
                    {
                        i[2] = i2;

                        SortedSet<int> inputs3 = new SortedSet<int>(inputs2);
                        inputs3.Remove(i2);

                        foreach (var i3 in inputs3)
                        {
                            i[3] = i3;

                            SortedSet<int> inputs4 = new SortedSet<int>(inputs3);
                            inputs4.Remove(i3);

                            Debug.Assert(inputs4.Count == 1);
                            i[4] = inputs4.First();

                            // build the executions

                            ParallelCodeRunner[] compilers = new ParallelCodeRunner[5];

                            for (int idx = 0; idx < 5; idx++)
                            {
                                compilers[idx] = new ParallelCodeRunner();
                                compilers[idx].ExternalInput.Add(i[idx].ToString());
                                compilers[idx].Code = savedCode;

                                if (idx != 0)
                                {
                                    compilers[idx - 1].ExternalOutput = compilers[idx].ExternalInput;
                                }
                            }

                            compilers[4].ExternalOutput = compilers[0].ExternalInput;
                            compilers[0].ExternalInput.Add("0");

                            // now start them
                            for (int idx = 0; idx < 5; idx++)
                            {
                                compilers[idx].RunInThread();

                            }

                            bool ok = true;
                            // now wait for them to stop
                            for (int idx = 0; idx < 5; idx++)
                            {
                                ok &= compilers[idx].ThreadedResult();
                                if (!ok)
                                {
                                    Debugger.Break();
                                }
                            }

                            //

                            if (compilers[4].ExternalOutput == null || compilers[4].ExternalOutput.Count != 1)
                            {
                                Debugger.Break();
                                Debug.Assert(false);
                            }

                            // else

                            var opStr = compilers[4].ExternalOutput.Take();
                            int opInt = 0;

                            var convOk = Int32.TryParse(opStr, out opInt);

                            string fullStr = $"Code ok? {ok} Convsion ok? {convOk} Phases: {i[0]}  {i[1]} {i[2]} {i[3]} {i[4]}; Output: {opStr} aka: {opInt}";
                            _results.Add(new Tuple<int, string>(opInt, fullStr));

                        }
                    }
                }
            });
            
            //return string.Join (Environment.NewLine,_results.OrderBy(x => x));
            return _results.OrderByDescending(x => x.Item1).FirstOrDefault().Item2;
        }

        //private string RunCurrentCompiler(string previousResult, int currentSetting, SortedSet<int> settingsList, int depth)
        //{
        //    Debug.Assert(depth < compilers.Length);

        //    compilers[depth].Code = savedCode;
        //    compilers[depth].ExternalInput = new string[] { currentSetting.ToString(), previousResult };
        //    compilers[depth].Run();

        //    SortedSet<int> inputList = new SortedSet<int>(settingsList);
        //    inputList.Remove(currentSetting);

        //    if (inputList.Count == 0)
        //    {
        //        int finalout = 0;
        //        bool ok = false;
        //        ok = Int32.TryParse(compilers[depth].ExternalOutput, out finalout);
        //        _results.Add(finalout);
        //        return depth.ToString() + ":" + compilers[depth].ExternalOutput;  // when there is no more
        //    }

        //    string ret = "";
        //    foreach (var nextStageInput in inputList)
        //    {
        //        ret += RunCurrentCompiler(compilers[depth].ExternalOutput, nextStageInput, inputList, depth + 1) + Environment.NewLine;
        //    }

        //    return ret;
        //}

        
    }
}
