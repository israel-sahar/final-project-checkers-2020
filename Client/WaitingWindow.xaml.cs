using Client.CheckersServiceReference;
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
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for WaitingWindow.xaml
    /// </summary>
    public partial class WaitingWindow : Window
    {
        public WaitingWindow()
        {
            InitializeComponent();
        }

        public WaitingWindow(int chosenSize, Level human, bool v)
        {
            ChosenSize = chosenSize;
            Human = human;
            V = v;
        }

        public int ChosenSize { get; }
        public Level Human { get; }
        public bool V { get; }
        public ClientCallback Callback { get; internal set; }
        public CheckersServiceClient Client { get; internal set; }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
