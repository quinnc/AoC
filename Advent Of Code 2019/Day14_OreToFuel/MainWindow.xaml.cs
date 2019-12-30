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

namespace Day14_OreToFuel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ReactionList rl;

        public MainWindow()
        {
            InitializeComponent();
            rl = new ReactionList();
            this.DataContext = rl;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var numOre = rl.OresForOneFuel();
            tbResult.Text = numOre.ToString();
            double totalOre = 1e12;
            double oreUsedRatio = totalOre / numOre;

            tbResult.Text += Environment.NewLine + oreUsedRatio.ToString();

            tbResult.Text += Environment.NewLine + (oreUsedRatio * rl.NumFuelsToMake).ToString();
        }
    }
}
