using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;

namespace day7.Compiler
{
    class ParallelCodeRunner //: INotifyPropertyChanged
    {
        private const string HALT_CMD = "99";
        private const string ADD_CMD = "01";
        private const string MULT_CMD = "02";
        private const string STOR_CMD = "03";
        private const string DUMP_CMD = "04";
        private const string JIT_CMD = "05"; // jump if true
        private const string JIF_CMD = "06"; // jump if false
        private const string LT_CMD = "07"; // if a < b, set c=1
        private const string EQ_CMD = "08"; // if a==b, set c=1

        private const char ADDR = '0';
        private const char IMMED = '1';

        //public event PropertyChangedEventHandler PropertyChanged;

        //// This method is called by the Set accessor of each property.  
        //// The CallerMemberName attribute that is applied to the optional propertyName  
        //// parameter causes the property name of the caller to be substituted as an argument.  
        //private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}

        public bool Run()
        {
            int execptr = 0;

            int szData = data.Length;
            bool ok = false;

            while (execptr < szData && data[execptr] != HALT_CMD)
            {
                ok = Command(execptr, out int cmdSize);
                if (!ok)
                {
                    Debugger.Break();
                    //externalOutput = data.ToString();
                    throw new Exception($"command failed! position {execptr}");
                    //break;
                }

                execptr += cmdSize;
            }

            return ok;
        }

        private bool threadedResult = false;
        Thread th;

        private static void Threadify (object inst)
        {
            if (inst is ParallelCodeRunner crInst)
            {
                crInst.threadedResult = crInst.Run();
            }
            else
            {
                Debugger.Break();
            }
        }

        public void RunInThread()
        {

            th = new Thread(Threadify);
            th.Start(this);

            Thread.Sleep(0);
        }

        public bool ThreadedResult()
        { 
            th.Join();
            return this.threadedResult;
        }

        private bool Command(int curr, out int jumpAmt)
        {
            string cmdStr = data[curr];
            int cmdInt = -1;

            if (cmdStr.Length == 0)
            {
                jumpAmt = 1;
                return true;
            }

            bool ok = Int32.TryParse(cmdStr, out cmdInt);

            if (!ok)
            {
                jumpAmt = 0;
                return false;
            }

            cmdInt += 100000; // force zero if there would be some in front of the number
            cmdStr = cmdInt.ToString();

            char modeParam3 = cmdStr[1];
            char modeParam2 = cmdStr[2];
            char modeParam1 = cmdStr[3];
            string opcode = cmdStr.Substring(4);


            switch (opcode)
            {
                case ADD_CMD:
                {
                    // addition case
                    ok = GetInputs(curr, 2, modeParam1, out int input1, modeParam2, out int input2);
                    if (ok)
                    {
                        int sum = input1 + input2;

                        ok = SetOutput(curr, 3, modeParam3, sum);
                        jumpAmt = 4;
                    }
                    else
                        jumpAmt = 1000;

                }
                break;

                case MULT_CMD:
                {
                    // multiple case
                    ok = GetInputs(curr, 2, modeParam1, out int input1, modeParam2, out int input2);
                    if (ok)
                    {
                        int sum = input1 * input2;

                        ok = SetOutput(curr, 3, modeParam3, sum);
                        jumpAmt = 4;
                    }
                    else
                        jumpAmt = 1000;
                }
                break;

                case STOR_CMD:
                {
                    // set address to current external input
                   // if (currentInput >= externalInput.Length)
                   if (externalInput.Count == 0)
                    {
                        //Debugger.Break();
                        //throw new ArgumentOutOfRangeException("currentInput " + currentInput.ToString() + " > externalInput.Length " + externalInput.Length.ToString());
                        // will block
                    }


                    var val = externalInput.Take();
                    ok = SetOutput(curr, 1, modeParam1, val);
                    currentInput++;
                    jumpAmt = 2;
                    break;
                }

                case DUMP_CMD:
                {

                    if(externalOutput == null)
                    {
                        Debugger.Break();
                    }

                    ok = GetInputs(curr, 1, modeParam1, out int externalInt, modeParam2, out int _);
                    externalOutput.Add(externalInt.ToString());
           //         NotifyPropertyChanged("ExternalOutput");
                    jumpAmt = 2;
                    break;
                }

                case JIT_CMD:
                {
                    ok = GetInputs(curr, 2, modeParam1, out int input1, modeParam2, out int input2);
                    if (ok)
                    {
                        if (input1 != 0)
                        {
                            jumpAmt = input2 - curr;
                        }
                        else
                        {
                            jumpAmt = 3;
                        }

                    }
                    else
                    {
                        jumpAmt = 1;
                    }
                    break;
                }

                case JIF_CMD:
                {
                    ok = GetInputs(curr, 2, modeParam1, out int input1, modeParam2, out int input2);
                    if (ok)
                    {
                        if (input1 == 0)
                        {
                            jumpAmt = input2 - curr;
                        }
                        else
                        {
                            jumpAmt = 3;
                        }

                    }
                    else
                    {
                        jumpAmt = 1;
                    }
                    break;
                }

                case LT_CMD:
                {
                    ok = GetInputs(curr, 2, modeParam1, out int input1, modeParam2, out int input2);
                    if (ok)
                    {
                        if (input1 < input2)
                        {
                            ok = SetOutput(curr, 3, modeParam3, "1");
                        }
                        else
                        {
                            ok = SetOutput(curr, 3, modeParam3, "0");
                        }

                        jumpAmt = 4;
                    }
                    else
                    {
                        jumpAmt = 1;
                    }
                    break;
                }

                case EQ_CMD:
                {
                    ok = GetInputs(curr, 2, modeParam1, out int input1, modeParam2, out int input2);
                    if (ok)
                    {
                        if (input1 == input2)
                        {
                            ok = SetOutput(curr, 3, modeParam3, "1");
                        }
                        else
                        {
                            ok = SetOutput(curr, 3, modeParam3, "0");
                        }

                        jumpAmt = 4;
                    }
                    else
                    {
                        jumpAmt = 1;
                    }
                    break;
                }

                case HALT_CMD:
                    // code done case
                    MessageBox.Show($"Found code 99 at pos {curr}.");
                    jumpAmt = 1;
                    ok = true;
                    break;

                default:
                    MessageBox.Show($"Unknown instruction {data[curr]}");
                    jumpAmt = 1;
                    ok = false;
                    break;
            }

            return ok;
        }

        private bool SetOutput(int curr, int paramOffset, char mode, int val)
        {
            string valStr = val.ToString();
            return SetOutput(curr, paramOffset, mode, valStr);
        }

        private bool SetOutput(int curr, int paramOffset, char mode, string val)
        {
            bool ok = false;

            switch (mode)
            {
                case ADDR:
                {
                    int address = 0;
                    ok = Int32.TryParse(data[curr + paramOffset], out address);

                    if (ok)
                        if (address < data.Length)
                        {
                            ok = true;
                            //data[address] = val;
                            SetData(address, val);
                        }
                        else
                            ok = false;
                    break;
                }

                case IMMED:
                {
                    ok = true;
                    //data[curr + paramOffset] = val;
                    SetData(curr + paramOffset, val);
                    break;
                }

                default:
                {
                    MessageBox.Show($"Invalid output mode = {mode}");
                    break;
                }
            }

            return ok;
        }

        private bool GetInputs(int curr, int numParams, char modeParam1, out int input1, char modeParam2, out int input2)
        {
            input1 = 0;
            input2 = 0;

            if ((curr + numParams) >= data.Length)
            {
                MessageBox.Show("Error: Would go outside the code to get the input parameters.");
                return false;
            }

            bool ok = true;

            if (ok && numParams >= 1)
                ok = GetInput(curr + 1, modeParam1, out input1);

            if (ok && numParams >= 2)
                ok = GetInput(curr + 2, modeParam2, out input2);

            return ok;
        }

        private bool GetInput(int param, char mode, out int val)
        {
            bool ok = false;
            val = 0;

            switch (mode)
            {
                case ADDR:
                {
                    int address = 0;
                    ok = Int32.TryParse(data[param], out address);

                    if (ok)
                        if (address < data.Length)
                            ok = Int32.TryParse(data[address], out val);
                        else
                            ok = false;
                    break;
                }

                case IMMED:
                {
                    ok = Int32.TryParse(data[param], out val);
                    break;
                }

                default:
                {
                    MessageBox.Show($"Invalid mode = {mode}");
                    break;
                }
            }

            return ok;
        }

        private string[] data = new string[] {"99"};
        private void SetData(int address, string val)
        {
            if (address >= data.Length)
            {
                MessageBox.Show($"not a valid address {address}");
                return;
            }

            if (data[address] == val)
                return;

            data[address] = val;
            //NotifyPropertyChanged("Code");
        }

        private int currentInput = 0;
        private BlockingCollection<string> externalInput = new BlockingCollection<string>();
        private BlockingCollection<string> externalOutput = null;


        private bool GetAddresses(int currpos, int numParams, out int input1loc, out int input2loc, out int outputloc)
        {
            input1loc = 0;
            input2loc = 0;
            outputloc = 0;

            if ((currpos + numParams) >= data.Length)
            {
                MessageBox.Show("Error: Would go outside the code to get the input parameters.");
                return false;
            }

            bool ok = true;

            if (ok && numParams >= 1)
                ok = Int32.TryParse(data[currpos + 1], out input1loc);

            if (ok && numParams >= 2)
                ok = Int32.TryParse(data[currpos + 2], out input2loc);

            if (ok && numParams >= 3)
                ok = Int32.TryParse(data[currpos + 3], out outputloc);

            return ok;
        }

        public string Code
        {
            get => string.Join(", ", data);
            set
            {
                currentInput = 0;
                
                data = Regex.Split(value, ",|\r\n|\r|\n");
            }
        }

        public BlockingCollection<string> ExternalInput => externalInput;

        public BlockingCollection<string> ExternalOutput
        {
            get => externalOutput;
            set => externalOutput = value;
        }

    }
}
