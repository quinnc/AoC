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

namespace Day15_RepairDroid
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

        RepairDroidMapper rdm = null;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            rdm = new RepairDroidMapper(tbCode.Text);
            this.DataContext = rdm;
            rdm.Code = tbCode.Text;

            rdm.Start();
        }

        private void TbResult_KeyDown(object sender, KeyEventArgs e)
        {
            if (rdm == null)
                return;

            switch (e.Key)
            {
                case Key.Up:
                    rdm.Walk(RepairDroidMapper.Direction.North);
                    break;

                case Key.Right:
                    rdm.Walk(RepairDroidMapper.Direction.East);
                    break;

                case Key.Down:
                    rdm.Walk(RepairDroidMapper.Direction.South);
                    break;

                case Key.Left:
                    rdm.Walk(RepairDroidMapper.Direction.West);
                    break;
            }

            e.Handled = true;

        }

        private void BtnPart2_Click(object sender, RoutedEventArgs e)
        {
            rdm = new RepairDroidMapper(tbCode.Text);
            this.DataContext = rdm;
            rdm.Code = tbCode.Text;
            rdm.Start();
            rdm.AutoSearch(out int dist, out int other);

            var t = tbResult.Text;

            t += Environment.NewLine + $"Minimum steps to hole = {dist}, furthest distance to another deadend {other}.";

            tbResult.Text = t;

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            rdm?.Stop();
        }
    }
}
