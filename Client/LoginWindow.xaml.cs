using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
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
using System.Windows.Shapes;
using System.Windows.Threading;
using Client.CheckersServiceReference;
namespace Client
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public ClientCallback Callback { set; get; }
        public CheckersServiceClient Client { get; set; }

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            string usrName = usrNameTextBox.Text.Trim();
            string pass = passwordTextBox.Password.Trim();
            if (string.IsNullOrEmpty(usrName))
            {
                MessageBox.Show("Please enter a username..","Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Please enter a password..", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            try
            {
                Client.Connect(usrName, HashValue(pass));
            }
            catch (FaultException<UserAlreadyLoginFault>) {
                MessageBox.Show("The User already connected", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            catch (FaultException<UserNotExistsFault>) {
                MessageBox.Show("Something got wrong with the userName..", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            catch (FaultException<WrongPasswordFault>) {
                MessageBox.Show("Something got wrong with the password..", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            catch (Exception fault) {
                MessageBox.Show(fault.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Client = new CheckersServiceClient(new InstanceContext(Callback));
                return;
            }
            MenuWindow window = new MenuWindow();
            window.User = usrName;
            window.Callback = Callback;
            window.Client = Client;
            window.Show();
            this.Close();
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            WelcomeWindow window = new WelcomeWindow();
            window.Client = Client;
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
