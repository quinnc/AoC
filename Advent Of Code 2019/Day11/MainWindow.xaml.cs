using System.Windows;

namespace Day11
{
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
