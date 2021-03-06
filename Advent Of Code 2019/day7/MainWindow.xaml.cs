﻿using day7.Compiler;
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

namespace day7
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
            Amplifier amp = new Amplifier();

            amp.Program(tbCode.Text);

            var r = amp.Execute();

            tbMatches.Text = r;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            FeedbackAmplifier famp = new FeedbackAmplifier();

            famp.Program(tbCode.Text);

            var r = famp.Execute();
            tbMatches.Text = r;
        }
    }
}
