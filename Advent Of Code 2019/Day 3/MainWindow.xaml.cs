using Day_3.Data;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Day_3
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

        WireRoute wire1 = new WireRoute();
        WireRoute wire2 = new WireRoute();
        Overlaps overlaps = new Overlaps();

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            wire1.MakeRoute(tbWire1.Text);
            wire2.MakeRoute(tbWire2.Text);

            overlaps.Find (wire1, wire2);

            tbOverlaps.Text = overlaps.ToString();
        }

        private void BtnFindShortest_Click(object sender, RoutedEventArgs e)
        {
            overlaps.CalcDistances();
            overlaps.Shortest(out int x, out int y, out double dist);

            tbShortestDist.Text = $"Overlap closest to home is x={x} y={y} distance={dist}.";
        }
    }
}
