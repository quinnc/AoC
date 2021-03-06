﻿using IntCode;
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
            
            if (game != null || gameDrawer != null)
                EndGame();
            tbResult.Text = "";

            Thread.Sleep(1000);

            game = new Game(tbCode.Text);
            gameDrawer = new GameDraw(ref tbResult, ref tbScore, ref game);

            gameDrawer.Start();
            game.PlayThread();
        }



        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (game == null)
                return;

            switch (e.Key)
            {
                case Key.Left:
                    game?.LeftPressed();
                    e.Handled = true;
                    break;
                case Key.Right:
                    game?.RightPressed();
                    e.Handled = true;
                    break;
                case Key.Up:
                    game?.StayPressed();
                    e.Handled = true;
                    break;
                case Key.Escape:
                    EndGame();
                    e.Handled = true;
                    break;
            }
        }

        private void BtnPart2_Click(object sender, RoutedEventArgs e)
        {
            if (game != null || gameDrawer != null)
                EndGame();

            tbResult.Text = "";

            Thread.Sleep(1000);
            game = new Game(tbCode.Text);
            gameDrawer = new GameDraw(ref tbResult, ref tbScore, ref game);

            gameDrawer.Start();
            game.PlayUnlimited();

            LeftButton.IsEnabled = true;
            StayButton.IsEnabled = true;
            RightButton.IsEnabled = true;
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            game?.LeftPressed();
            gameDrawer.Joystick = -1;
        }

        private void StayButton_Click(object sender, RoutedEventArgs e)
        {
            game?.StayPressed();
            gameDrawer.Joystick = 0;
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            game?.RightPressed();
            gameDrawer.Joystick = 1;
        }

        private void EndButton_Click(object sender, RoutedEventArgs e)
        {
            EndGame();
        }

        private void EndGame()
        {
            gameDrawer?.Stop();
            game?.Stop();
            tbResult.Text += Environment.NewLine + Environment.NewLine + Environment.NewLine + $"There are {game?.Count(GamePiece.Block)} block tiles when the game is done.";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            EndGame();
        }
    }
}
