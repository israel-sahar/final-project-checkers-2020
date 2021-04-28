﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
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

namespace GameHost
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

        ServiceHost host;

        private void Window_Initialized(object sender, EventArgs e)
        {
            host = new ServiceHost(typeof(CheckersService.CheckersService));
            host.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true });
            try
            {
                host.Open();
                StatusLabel.Content = "RUNNING";
                StatusLabel.Foreground = Brushes.Green;
            }
            catch (Exception ex) { throw ex; }
        }

        private void onB_Click(object sender, RoutedEventArgs e)
        {
            if (host.State == CommunicationState.Opened) return;
            host = new ServiceHost(typeof(CheckersService.CheckersService));
            host.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true });
            try
            {
                host.Open();
                StatusLabel.Content = "RUNNING";
                StatusLabel.Foreground = Brushes.Green;
            }
            catch (Exception ex) { throw ex; }
        }

        private void restartB_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Process.GetCurrentProcess().MainModule.FileName);
            Application.Current.Shutdown();
        }

        private void offB_Click(object sender, RoutedEventArgs e)
        {
            if (host.State == CommunicationState.Closed) return;
            try
            {
                host.Close();
                StatusLabel.Content = "CLOSED";
                StatusLabel.Foreground = Brushes.Red;
            }
            catch (Exception ex) { throw ex; }
        }
    }
}
