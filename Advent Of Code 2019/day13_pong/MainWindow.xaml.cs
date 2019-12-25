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

namespace day13_pong
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Game game;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnPartA_Click(object sender, RoutedEventArgs e)
        {
            game = new Game(tbCode.Text);

            game.Play();

            tbResult.Text = $"There are {game.Count(GamePiece.Block)} block tiles when the game is done.";
        }



        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void BtnPart2_Click(object sender, RoutedEventArgs e)
        {

            game = new Game(tbCode.Text);
            game.PlayUnlimited();

            LeftButton.IsEnabled = true;
            StayButton.IsEnabled = true;
            RightButton.IsEnabled = true;



        }
    }
}
