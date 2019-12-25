using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
using TwodTypes;

namespace day13_pong
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Game game;
        GameDraw gameDrawer;

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
            if (game == null)
                return;

            switch (e.Key)
            {
                case Key.Left:
                    game.LeftPressed();
                    e.Handled = true;
                    break;
                case Key.Right:
                    game.RightPressed();
                    e.Handled = true;
                    break;
                case Key.Up:
                    game.StayPressed();
                    e.Handled = true;
                    break;
            }
        }

        private void BtnPart2_Click(object sender, RoutedEventArgs e)
        {

            game = new Game(tbCode.Text);
            game.PlayUnlimited();

            LeftButton.IsEnabled = true;
            StayButton.IsEnabled = true;
            RightButton.IsEnabled = true;

            gameDrawer = new GameDraw(ref tbResult, ref tbScore, ref game);
            gameDrawer.RunInThread();

        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            game?.LeftPressed();
        }

        private void StayButton_Click(object sender, RoutedEventArgs e)
        {
            game?.StayPressed();
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            game?.RightPressed();
        }

        private void EndButton_Click(object sender, RoutedEventArgs e)
        {
            gameDrawer?.Stop();
            game?.Stop();
        }
    }

    class GameDraw
    {

        private TextBox screen, scorebox;
        private Game _game;
        bool stop = false;

        public void Stop()
        {
            stop = true;
            
        }

        public GameDraw (ref TextBox output, ref TextBox score, ref Game game)
        {
            screen = output;
            scorebox = score;
            _game = game ;
        }

        private bool threadedResult = false;
        private Thread th;

        private static void Threadify(object inst)
        {
            if (inst is GameDraw crInst)
            {
                crInst.threadedResult = crInst.Run();
            }
            else
            {
                Debugger.Break();
            }
        }

        private bool Run()
        {
            while (!stop)
            {
                Thread.Sleep(100);
  
                screen.Text = _game.ToString();
                scorebox.Text = _game.Score;
            }

            return true;
        }

        public void RunInThread()
        {

            th = new Thread(Threadify);
            th.Start(this);

            Thread.Sleep(0);
        }

        public bool ThreadedResult()
        {
            th.Join();
            return threadedResult;
        }

    }
}
