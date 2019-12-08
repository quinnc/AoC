using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace Day5.Compiler
{
    class Compiler
    {
        private const string HALT_CMD = "99";
        private const string ADD_CMD = "01";
        private const string MULT_CMD = "02";
        private const string STOR_CMD = "03";
        private const string DUMP_CMD = "04";

        private const char ADDR = '0';
        private const char IMMED = '1';
        


        public bool Run(string code, string _input, out string _output)
        {

            int execptr = 0;

            data = Regex.Split(code, ",|\r\n|\r|\n");
            externalIO = _input;

            int szData = data.Length;
            bool ok = false;
            int cmdSize;

            while (execptr < szData && data[execptr] != HALT_CMD)
            {
                ok = Command(execptr, out cmdSize);
                if (!ok)
                {
                    MessageBox.Show($"command failed! {execptr}");
                    break;
                }

                execptr += cmdSize;
            }

            _output = output;
            return ok;
        }

        private bool Command(int curr, out int size)
        {

            string cmdStr = data[curr];
            int cmdInt = -1;

            bool ok = Int32.TryParse(cmdStr, out cmdInt);

            if (!ok)
            {
                size = 0;
                return false;
            }

            cmdInt += 100000; // force zero if there would be some in front of the number
            cmdStr = cmdInt.ToString();

            char modeParam3 = cmdStr[1];
            char modeParam2 = cmdStr[2];
            char modeParam1 = cmdStr[3];
            string opcode = cmdStr.Substring(4);
            size = 1000;


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
                        size = 4;
                    }
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
                        size = 4;
                    }
                }
                break;

                case STOR_CMD:
                {
                    ok = SetOutput(curr, 1, modeParam1, externalIO);
                    break;
                }

                case DUMP_CMD:
                {

                    ok = GetInput(curr+1, modeParam1, out int externalInt);
                    externalIO = externalInt.ToString();
                    break;
                }


                case HALT_CMD:
                    // code done case
                    MessageBox.Show($"Found code 99 at pos {curr}.");
                    break;

                default:
                    MessageBox.Show($"Unknown instruction {data[curr]}");
                    break;

            }


            return false;
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
                    ok = Int32.TryParse(data[curr+paramOffset], out address);

                    if (ok)
                        if (address < data.Length)
                        {
                            ok = true;
                            data[address] = val; ;
                            }
                        else
                            ok = false;
                    break;
                }

                case IMMED:
                {
                    ok = true;
                    data[curr + paramOffset] = val;
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
                ok = GetInput(curr + 2, modeParam1, out input1);

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

        private string[] data;
        private string externalIO = "TBD";
        private readonly string output = "FAIL";


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

    }
}
