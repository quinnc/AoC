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

namespace Day11
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
            EmergencyHullPaintingRobot ehpr = new EmergencyHullPaintingRobot();

            ehpr.SetCode(tbCode.Text);
            ehpr.Paint(0);
            // paint will block until complete

            tbResult.Text = $"The robot visited {ehpr.PaintedSquares} unique squares.";

        }
    }
}
