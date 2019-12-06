using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace Advent_of_Code_1a
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var txt = inputTextBox.Text;
            //var result = txt.Split(new[] { '\r', '\n' });
            var result = Regex.Split(txt, "\r\n|\r|\n");

            int totalMass = 0;
            progressTextBox.Text = "";
            foreach (var line in result)
            {
                bool ok = Int32.TryParse(line, out int lineAsInt);
                if (!ok)
                    continue;

                progressTextBox.Text += "CL {" + line + "}, ";
                progressTextBox.Text += " INT {" + lineAsInt + "}, ";

                double div3 = (lineAsInt) / 3.0;
                double floored = Math.Floor(div3);
                double minus2 = floored - 2;

                progressTextBox.Text += $" CALCS {div3}, {floored}, {minus2}, ";
                totalMass += (int)minus2;

                progressTextBox.Text += $" Total mass = {totalMass}\n";
              
            }

            resultsTextBox.Text = totalMass.ToString();
        }

        private void CalcFuelForFuel_Click(object sender, RoutedEventArgs e)
        {
 
            var txt = inputTextBox.Text;
            var result = Regex.Split(txt, "\r\n|\r|\n");

            int totalFuel = 0;
            progressTextBox.Text = "";

            foreach (var line in result)
            {
                bool ok = Int32.TryParse(line, out int lineAsInt);
                if (!ok)
                    continue;

                double currentMass = lineAsInt;
                int currentFuel = 0;

                do
                {
                    double div3 = currentMass / 3.0;
                    double floored = Math.Floor(div3);
                    double minus2 = floored - 2;

                    if (minus2 > 0)
                    {
                        currentFuel += (int)minus2;
                        
                    }
                    currentMass = minus2;

                } while (currentMass > 0);

                totalFuel += currentFuel;

            }

            tbFuelForFuelResult.Text = totalFuel.ToString();
        }
    }
}
