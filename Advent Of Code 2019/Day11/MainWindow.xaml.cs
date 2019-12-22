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

        private void BtnPart2_Click(object sender, RoutedEventArgs e)
        {
            EmergencyHullPaintingRobot ehpr = new EmergencyHullPaintingRobot();

            ehpr.SetCode(tbCode.Text);

            // part 2 starts off on a white square
            ehpr.Paint(1);
            // paint will block until complete

            // generate the image
            // show the image.
            tbResult.Text = ehpr.IdImage;
        }
    }
}
