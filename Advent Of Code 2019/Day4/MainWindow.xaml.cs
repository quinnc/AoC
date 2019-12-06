using Day4.Calculators;
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

namespace Day4
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
            PasswordGuesser pg = new PasswordGuesser();

            int lower = 0;
            bool ok = false;

            ok = Int32.TryParse(tbLower.Text, out lower);
            if (!ok)
                MessageBox.Show("Failed to parse lower bounds " + tbLower.Text);

            int upper = 0;
            bool ok2;
            ok2 = Int32.TryParse(tbUpper.Text, out upper);
            if (!ok2)
                MessageBox.Show("Failed to parse upper bounds " + tbUpper.Text);

            if (!ok || !ok2)
                return;

            pg.Guess(lower, upper);
            tbMatches.Text = pg.ToString();
        }
    }
}
