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
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void cleanBtn_Click(object sender, RoutedEventArgs e)
        {
            emailTextBox.Text = null;
            passwordTextBox.Password = null;
        }

        private void createBtn_Click(object sender, RoutedEventArgs e)
        {
            //after create and add to database

            MenuWindow window = new MenuWindow();
            window.Show();
            this.Close();
        }


        private void haveAnAccount_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LoginWindow window = new LoginWindow();
            window.Show();
            this.Close();
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            WelcomeWindow window = new WelcomeWindow();
            window.Show();
            this.Close();
        }
    }
}
