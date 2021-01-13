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
    /// Interaction logic for MenuWindow.xaml
    /// </summary>
    public partial class MenuWindow : Window
    {
        private int chosenSize;
        private int chosenLevel;

        public MenuWindow()
        {
            InitializeComponent();
        }

        private void backPlayerBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            playerBtn.Visibility = Visibility.Visible;
        }

        private void backCPUBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            cmpBtn.Visibility = Visibility.Visible;
        }

        private void chooseSizeTableClick(object sender, RoutedEventArgs e)
        {
            chosenSize = Int32.Parse(this.Tag.ToString());
            tableSizeCPUGrid.Visibility = Visibility.Hidden;
            gameLevelGrid.Visibility = Visibility.Visible;
        }

        //handle level
        private void chooseLevelGameClick(object sender, RoutedEventArgs e)
        {
            chosenLevel = Int32.Parse(this.Tag.ToString());

            GameWindow window = new GameWindow(chosenSize, Level.Easy,true);
            window.Show();
            this.Close();
        }

        private void gameLevelGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            gameLevelGrid.Visibility = Visibility.Hidden;
            cmpBtn.Visibility = Visibility.Visible;
        }

        private void backLevelBtn_Click(object sender, RoutedEventArgs e)
        {
            gameLevelGrid.Visibility = Visibility.Hidden;
            cmpBtn.Visibility = Visibility.Visible;
        }

        private void tableSizeGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            cmpBtn.Visibility = Visibility.Visible;
        }

        private void vsComputer_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            tableSizeCPUGrid.Visibility = Visibility.Visible;
        }

        private void vsPlayer_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            tableSizePlayerGrid.Visibility = Visibility.Visible;
        }

        private void tableSizePlayerGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            playerBtn.Visibility = Visibility.Visible;
        }

        //handle level
        private void chooseSizeTablePlayerClick(object sender, RoutedEventArgs e)
        {
            chosenSize = Int32.Parse(this.Tag.ToString());

            //chosen level should be null
            GameWindow window = new GameWindow(chosenSize,Level.Easy, false);
            window.Show();
            this.Close();
        }

        private void watchPrevGame_Click(object sender, RoutedEventArgs e)
        {
            PrevsGamesWindow window = new PrevsGamesWindow();
            window.Show();
            this.Close();
        }
    }
}
