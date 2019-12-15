using Day9.Compiler;
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

namespace Day9
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
            ParallelCodeRunner pcr = new ParallelCodeRunner();
            tbMatches.Text = "";
            pcr.Code = tbCode.Text;
            pcr.ExternalOutput = new System.Collections.Concurrent.BlockingCollection<string>();
            pcr.ExternalInput.Add("1");
            pcr.Run();
            string outstr = string.Join("\n", pcr.ExternalOutput);

            tbMatches.Text = "result = \n" + outstr;
        }
    }
}
