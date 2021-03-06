﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;

namespace Day11
{
    class CodeRunner
    {
        enum Commands
        {
            ADD = 1,
            MULT,
            STOR,
            DUMP,
            JIT,
            JIF,
            LT,
            EQ,
            REL,
            HALT = 99
        };

        enum AddrModes
        {
            ADDR = 0,
            IMMED = 1,
            REL = 2
        };


        private string[] data = null;
        private Dictionary<Int64, Int64> ram = new Dictionary<long, long>();

        protected BlockingCollection<string> externalInput = new BlockingCollection<string>();
        protected BlockingCollection<string> externalOutput = null;

        private Int64 currentInput = 0;
        private Int64 relativeOffset = 0;

        public BlockingCollection<string> ExternalInput => externalInput;

        public BlockingCollection<string> ExternalOutput
        {
            get => externalOutput;
            set => externalOutput = value;
        }

        public bool Run()
        {
            Int64 execptr = 0;

            Int64 szData = data.Length;
            bool ok = false;

            if (ExternalOutput == null)
            {
                Debugger.Break();
                return false;
            }

            if (data == null)
            {
                Debugger.Break();
                return false;
            }

            string haltStr = Commands.HALT.ToString();
            relativeOffset = 0;
            Commands lastCommand = Commands.DUMP;


            while (execptr < szData && lastCommand != Commands.HALT)
            {
                ok = Command(execptr, out Int64 cmdSize, out lastCommand);
                if (!ok)
                {
                    Debugger.Break();
                    throw new Exception($"command failed! position {execptr}");
                }

                execptr += cmdSize;
            }

            return ok;
        }

        public string Code
        {
            get => string.Join(", ", data);
            set
            {
                currentInput = 0;
                relativeOffset = 0;
                data = Regex.Split(value, ",|\r\n|\r|\n");
            }
        }

        private bool Command(Int64 curr, out Int64 jumpAmt, out Commands foundCommand)
        {
            string cmdStr = data[curr];
            Int64 cmdInt = -1;

            foundCommand = Commands.HALT;

            if (cmdStr.Length == 0)
            {
                jumpAmt = 1;
                return true;
            }

            bool ok = Int64.TryParse(cmdStr, out cmdInt);

            if (!ok)
            {
                jumpAmt = 1;
                return false;
            }

            cmdInt += 100000; // force zero if there would be some in front of the number
            cmdStr = cmdInt.ToString();

            char modeParam3 = cmdStr[1];
            char modeParam2 = cmdStr[2];
            char modeParam1 = cmdStr[3];
            string opcode = cmdStr.Substring(4);
            Int64 opcodeInt = 0;

            ok = Int64.TryParse(opcode, out opcodeInt);

            Commands opcodeCmd = (Commands)opcodeInt;

            if (!ok)
            {
                jumpAmt = 1;
                return false;
            }

            foundCommand = opcodeCmd;

            switch (opcodeCmd)
            {
                case Commands.ADD:
                {
                    // addition case
                    ok = GetInputs(curr, 2, modeParam1, out Int64 input1, modeParam2, out Int64 input2);
                    if (ok)
                    {
                        Int64 sum = input1 + input2;

                        ok = SetOutput(curr, 3, modeParam3, sum);
                        jumpAmt = 4;
                    }
                    else
                        throw new Exception("command error"); // jumpAmt = 1000;

                }
                break;

                case Commands.MULT:
                {
                    // multiple case
                    ok = GetInputs(curr, 2, modeParam1, out Int64 input1, modeParam2, out Int64 input2);
                    if (ok)
                    {
                        Int64 sum = input1 * input2;

                        ok = SetOutput(curr, 3, modeParam3, sum);
                        jumpAmt = 4;
                    }
                    else
                        throw new Exception("command error"); //jumpAmt = 1000;
                }
                break;

                case Commands.STOR:
                {
                    // set address to current external input
                    var val = externalInput.Take();
                    ok = SetOutput(curr, 1, modeParam1, val);
                    currentInput++;
                    jumpAmt = 2;
                    break;
                }

                case Commands.DUMP:
                {

                    if (externalOutput == null)
                    {
                        Debugger.Break();
                    }

                    ok = GetInputs(curr, 1, modeParam1, out Int64 externalInt, modeParam2, out Int64 _);
                    externalOutput.Add(externalInt.ToString());
                    //         NotifyPropertyChanged("ExternalOutput");
                    jumpAmt = 2;
                    break;
                }

                case Commands.JIT:
                {
                    ok = GetInputs(curr, 2, modeParam1, out Int64 input1, modeParam2, out Int64 input2);
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

                case Commands.JIF:
                {
                    ok = GetInputs(curr, 2, modeParam1, out Int64 input1, modeParam2, out Int64 input2);
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

                case Commands.LT:
                {
                    ok = GetInputs(curr, 2, modeParam1, out Int64 input1, modeParam2, out Int64 input2);
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

                case Commands.EQ:
                {
                    ok = GetInputs(curr, 2, modeParam1, out Int64 input1, modeParam2, out Int64 input2);
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

                case Commands.REL:
                {
                    ok = GetInputs(curr, 1, modeParam1, out Int64 input1, modeParam2, out Int64 input2);

                    if (ok)
                    {
                        // got a value, now adjust the relative jump base by the amount
                        relativeOffset += input1;

                    }
                    else
                    {
                        Debugger.Break();
                    }

                    jumpAmt = 2;
                    break;
                }

                case Commands.HALT:
                    // code done case
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

        protected bool SetOutput(Int64 curr, Int64 paramOffset, char mode, Int64 val)
        {
            string valStr = val.ToString();
            return SetOutput(curr, paramOffset, mode, valStr);
        }

        protected bool SetOutput(Int64 curr, Int64 paramOffset, char mode, string val)
        {
            bool ok = false;
            AddrModes modeInt = (AddrModes)(mode - '0');

            switch (modeInt)
            {
                case AddrModes.ADDR:
                {
                    Int64 address = 0;
                    ok = Int64.TryParse(data[curr + paramOffset], out address);

                    if (ok)
                        ok = SetData(address, val);
                    else
                        throw new Exception("parse failed: " + data[curr + paramOffset].ToString()); //
                    break;
                }

                case AddrModes.IMMED:
                {
                    ok = SetData(curr + paramOffset, val);
                    break;
                }

                case AddrModes.REL:
                {
                    Int64 address = 0;
                    ok = Int64.TryParse(data[curr + paramOffset], out address);

                    address += relativeOffset;
                    if (ok)
                        ok = SetData(address, val);
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

        private bool GetInputs(Int64 curr, Int64 numParams, char modeParam1, out Int64 input1, char modeParam2, out Int64 input2)
        {
            input1 = 0;
            input2 = 0;

            bool ok = true;

            if (ok && numParams >= 1)
                ok = GetInput(curr + 1, modeParam1, out input1);

            if (!ok)
                throw new Exception("parse error: " + (curr+1).ToString()); //

            if (ok && numParams >= 2)
            {
                ok = GetInput(curr + 2, modeParam2, out input2);
                if (!ok)
                    throw new Exception("parse error: " + (curr + 2).ToString()); //
            }

            return ok;
        }

        private bool GetInput(Int64 param, char mode, out Int64 val)
        {
            bool ok = false;
            val = 0;

            AddrModes modeAM = (AddrModes)(mode - '0');

            switch (modeAM)
            {
                case AddrModes.ADDR:
                case AddrModes.REL:
                {
                    Int64 address = 0;
                    ok = Int64.TryParse(data[param], out address);

                    if (modeAM == AddrModes.REL)
                    {
                        // extra add the relative base
                        address += relativeOffset;
                    }

                    if (ok)
                        if (address < data.Length)
                        {
                            ok = Int64.TryParse(data[address], out val);
                            ram[address] = val;
                        }
                        else
                        {
                            if (ram.ContainsKey(address))
                                val = ram[address];
                            else
                            {
                                val = 0;
                                ram[address] = val;
                            }

                        }
                    else
                        throw new Exception("parse error: " + data[param]); //
                    break;
                }

                case AddrModes.IMMED:
                {
                    ok = Int64.TryParse(data[param], out val);
                    if (!ok)
                        throw new Exception("parse error: " + val); //
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
        private bool SetData(Int64 address, string val)
        {

            if (address < data.Length)
            {
                data[address] = val;
            }

            long ramVal = -1;

            bool ok;
            ok = Int64.TryParse(val, out ramVal);

            if (ok)
                ram[address] = ramVal;
            else
                throw new Exception("parse error: " + val); //

            return ok;
        }

        private bool GetAddresses(Int64 currpos, Int64 numParams, out Int64 input1loc, out Int64 input2loc, out Int64 outputloc)
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
                ok = Int64.TryParse(data[currpos + 1], out input1loc);

            if (ok && numParams >= 2)
                ok = Int64.TryParse(data[currpos + 2], out input2loc);

            if (ok && numParams >= 3)
                ok = Int64.TryParse(data[currpos + 3], out outputloc);

            return ok;
        }
    }

    class ParallelCodeRunner : CodeRunner
    {

        private bool threadedResult = false;
        private Thread th;

        private static void Threadify(object inst)
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
            return threadedResult;
        }
    }
}
