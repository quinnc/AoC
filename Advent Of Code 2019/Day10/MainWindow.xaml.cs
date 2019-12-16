using Day10.Asteroids;
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

namespace Day10
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
            Mapper mapper = new Mapper();

            mapper.SetMap(tbCode.Text);
            mapper.FindSightLines();
            mapper.GetMostVisibile(out VisibleAsteroidsFrom best);

            tbBestLocation.Text = $"x = {best.x}, y = {best.y} asteroids={best.numAsteroidsVisible}";
        }

        private void BtnVaporize_Click(object sender, RoutedEventArgs e)
        {

            Mapper mapper = new Mapper();

            mapper.SetMap(tbCode.Text);
            mapper.FindSightLines();
            mapper.GetMostVisibile(out VisibleAsteroidsFrom best);

            mapper.MakeVaporizeList(200, out int x, out int y);

            tbBestLocation.Text = $"x = {best.x}, y = {best.y} val {x*100 + y}";
        }
    }
}
