using Client.CheckersServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
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
        public CheckersServiceClient Client { get; internal set; }
        public ClientCallback Callback { get; internal set; }

        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void cleanBtn_Click(object sender, RoutedEventArgs e)
        {
            passwordTextBox.Password = null;
        }

        private void createBtn_Click(object sender, RoutedEventArgs e)
        {
            //after create and add to database
            string usrName = usrNameTextBox.Text.Trim();
            string password = passwordTextBox.Password.Trim();
            try {
            Client.Register(usrName, HashValue(password));
                MessageBox.Show("Your registration was successful");
                MenuWindow window = new MenuWindow();
                window.User = usrName;
                window.Callback = Callback;
                window.Client = Client;
                window.Show();
                this.Close();
            }
            catch (FaultException<UserNameAlreadyExistsFault> ex)
            {
                {
                    MessageBox.Show(ex.Detail.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void haveAnAccount_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LoginWindow window = new LoginWindow();
            window.Client = Client;
            window.Callback = Callback;
            window.Show();
            this.Close();
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            WelcomeWindow window = new WelcomeWindow();
            window.Show();
            this.Close();
        }

        private string HashValue(string password)
        {
            using (SHA256 hashObject = SHA256.Create())
            {
                byte[] hashBytes = hashObject.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
