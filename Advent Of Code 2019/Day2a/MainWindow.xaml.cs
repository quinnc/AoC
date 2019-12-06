using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace Day2a
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnRun_Click(object sender, RoutedEventArgs e)
        {
            var result = Regex.Split(tbInput.Text, ",|\r\n|\r|\n");

            // loop through the pieces and put into an array
            int[] code = new int[result.Length + 1];
            int[] codeSaved = new int[result.Length + 1];
            int codelen = 0;

            foreach (var str in result)
            {
                int input = 0;
                var ok = Int32.TryParse(str, out input);

                if (!ok)
                {
                    MessageBox.Show("Failed to parse: " + str);
                    continue;
                }

                // else ok, so add it to the array
                code[codelen] = input;
                codelen++;
            }

            for (int i = 0; i < codelen; i++)
            {
                codeSaved[i] = code[i];
            }


            int addr1sub = 0;
            int addr2sub = 0;

            while (code[0] != 19690720)
            {
                // reset the code
                for (int i = 0; i < codelen; i++)
                {
                    code[i] = codeSaved[i];
                }

                if (addr1sub < 100)
                {
                    addr1sub++;
                }
                else
                {
                    addr2sub++;
                    if (addr2sub > 99)
                    {
                        MessageBox.Show("Didn't find a solution in the first 100.");
                        break;
                    }

                    addr1sub = 0;
                }

                if (cb1202Error.IsChecked ?? false)
                {
                    code[1] = addr1sub;
                    code[2] = addr2sub;
                }


                int currpos = 0;
                while (currpos < codelen)
                {
                    if (code[currpos] == 99)
                    {
                        //MessageBox.Show($"Found code 99 at pos {currpos}.");
                        break;
                    }

                    switch (code[currpos])
                    {
                        case 1:
                        {
                            // addition case
                            bool goodAddrs = GetAddresses(codelen, currpos, code, out int input1loc, out int input2loc, out int outputloc);
                            if (!goodAddrs)
                                currpos = codelen;
                            else
                            {
                                int sum = code[input1loc] + code[input2loc];
                                code[outputloc] = sum;
                            }
                        }
                        break;

                        case 2:
                        {
                            // multiple case
                            bool goodAddrs = GetAddresses(codelen, currpos, code, out int input1loc, out int input2loc, out int outputloc);
                            if (!goodAddrs)
                                currpos = codelen;
                            else
                            {
                                int sum = code[input1loc] * code[input2loc];
                                code[outputloc] = sum;
                            }
                        }
                        break;

                        case 99:
                            // code done case
                            MessageBox.Show($"Found code 99 at pos {currpos}.");

                            // force it to end the while loop
                            currpos = codelen;
                            break;

                        default:
                            MessageBox.Show($"Unknown instruction {code[currpos]}");
                            break;

                    }

                    // move ahead 4
                    currpos += 4;

                }
            }
            tbOutput.Text = "";
            // now regenerate and output the final values

            for (int i = 0; i < codelen; i++)
            {
                tbOutput.Text += $"{code[i]},";
            }

        }

        private bool GetAddresses(int codelen, int currpos, int[] code, out int input1loc, out int input2loc, out int outputloc)
        {
            input1loc = 0;
            input2loc = 0;
            outputloc = 0;

            if ((currpos + 3) >= codelen)
            {
                MessageBox.Show("Error: Would go outside the code to get the input parameters.");
                return false;
            }
            input1loc = code[currpos + 1];
            input2loc = code[currpos + 2];
            outputloc = code[currpos + 3];

            if ( (input1loc >= codelen) || (input2loc >= codelen) || (outputloc >= codelen))
            {
                MessageBox.Show($"Error: At least one of the parameter addresses are outside the code {input1loc} {input2loc} {outputloc}.");
                return false;
            }

            return true;

        }
    }
}
