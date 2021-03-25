using Client.CheckersServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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
    /// Interaction logic for ResetPassword.xaml
    /// </summary>
    public partial class ResetPasswordWindow : Window
    {
        public ClientCallback Callback { get; internal set; }
        public CheckersServiceClient Client { get; internal set; }

        public ResetPasswordWindow()
        {
            InitializeComponent();
        }

        private void resetBtn_Click(object sender, RoutedEventArgs e)
        {
            string email = emailTextBox.Text.Trim();
            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Please Enter your Email..", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                Client.ResetPassword(email);
                MessageBox.Show("New password send to " + email, "Reset password", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (FaultException<UserNotExistsFault>)
            {
                MessageBox.Show("didnt find a user with this email..", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow window = new LoginWindow();
            window.Callback = Callback;
            window.Client = Client;
            window.Show();
            this.Close();
        }
    }
}
