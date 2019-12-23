using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace day12_3BodyProblem
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
            MoonSystem ms = new MoonSystem();

            ms.SetLocations(tbCode.Text);
            tbResult.Text = "";

            int i = 0;
            for (i = 0; i < 10; i++)
            {
                tbResult.Text += " Step " + (i * 100).ToString() + Environment.NewLine;
                tbResult.Text += ms.SystemDescription;
                tbResult.Text += ms.TotalEnergy.ToString() + Environment.NewLine + Environment.NewLine;

                ms.RunSteps(100);

            }

            tbResult.Text += " Step " + (i * 100).ToString() + Environment.NewLine;
            tbResult.Text += ms.SystemDescription;
            tbResult.Text += ms.TotalEnergy.ToString() + Environment.NewLine + Environment.NewLine;

        }
    }
}
