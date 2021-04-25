using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LA2RTS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RTSServer server = new RTSServer(RTSServer.PORT);

        public MainWindow()
        {
            InitializeComponent();
            logTextBlock.Text = "";
            server.RegisterOnAllEvents(LogToConsole);
            server.ClientConnectedEvent += AddClientToList;
            server.ClientDisconnectedEvent += RemoveClientFromList;
            server.ClientConnectedEvent += (cl) =>
            {
                cl.SelfInfoPacketEvent += UpdateClientData;
                cl.QuickUpdatePacketEvent += UpdateClientData;
            };

            server.ClientConnectedEvent += UpdateClientCount;
            server.ClientDisconnectedEvent += UpdateClientCount;
            server.ClientDisconnectedEvent += LogClDisconnected;

            // Create a timer with a two second interval.
            //System.Timers.Timer aTimer = new System.Timers.Timer(1000);
            // Hook up the Elapsed event for the timer. 
            //aTimer.Elapsed += UpdateClientCount;
            //aTimer.AutoReset = true;
            //aTimer.Enabled = true;

        }

        private void UpdateClientCount(RTSClient cl)
        {
            Dispatcher.Invoke(() =>
            {
                clientCountLabel.Content = server.clients.Count.ToString();
            });
        }

        private void LogClDisconnected(RTSClient cl)
        {
            LogToConsole(cl, "Client disconnected event.");
        }

        private void UpdateClientData(RTSClient cl)
        {
            Dispatcher.Invoke(() =>
            {
                var ch = cl.UserChar;
                foreach (ListBoxItem item in clientsListBox.Items)
                {
                    if (item.Name == "client_" + cl.clientID.ToString())
                        item.Content = String.Format("{0} X:{1} Y:{2} Z:{3} ToX:{4} ToY:{5} ToZ:{6} Speed:{7}", ch.Name, ch.X, ch.Y, ch.Z, ch.ToX, ch.ToY, ch.ToZ, ch.Speed);
                }
            });
        }

        private void RemoveClientFromList(RTSClient cl)
        {
            Dispatcher.Invoke(() =>
            {
                ListBoxItem itemToRemove = null;
                foreach (ListBoxItem item in clientsListBox.Items)
                {
                    if (item.Name == "client_" + cl.clientID.ToString())
                        itemToRemove = item;
                }
                if (itemToRemove != null)
                    clientsListBox.Items.Remove(itemToRemove);
            });
        }

        private void AddClientToList(RTSClient cl)
        {
            Dispatcher.Invoke(() =>
            {
                var item = new ListBoxItem();
                //var ch = cl.UserChar;
                item.Name = "client_" + cl.clientID.ToString();
                item.Content = "Unknown";
                clientsListBox.Items.Add(item);
            });
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            server.Start();
            statusLabel.Content = "Server is running";
            LogToConsole(null, "Server started");
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            server.Stop();
            statusLabel.Content = "Server stopped";
            LogToConsole(null, "Server stopped");
        }

        public void LogToConsole(RTSClient client, string message)
        {
            Dispatcher.Invoke(() =>
            {
                logTextBlock.Text += DateTime.Now.ToString("HH:mm:ss") + " " + message + "\n";
            });
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // close all active threads
            Environment.Exit(0);
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (ListBoxItem item in clientsListBox.SelectedItems)
            {
                foreach (RTSClient cl in server.clients)
                {
                    if (item.Name == "client_" + cl.clientID.ToString())
                    {
                        cl.SendEnterCredentialsCommand("cschim_se1", "8PweCDQdJGVtoXo6");
                    }
                }
            }
        }

        private void enterWorldButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (ListBoxItem item in clientsListBox.SelectedItems)
            {
                foreach (RTSClient cl in server.clients)
                {
                    if (item.Name == "client_" + cl.clientID.ToString())
                    {
                        cl.SendEnterWorldCommand();
                    }
                }
            }
        }

        private void loadPosButton_Click(object sender, RoutedEventArgs e)
        {
            RTSClient firstSelectedClient = GetFirstSelectedClient();

            if (firstSelectedClient != null)
            {
                toXTextBox.Text = firstSelectedClient.UserChar.X.ToString();
                toYTextBox.Text = firstSelectedClient.UserChar.Y.ToString();
                toZTextBox.Text = firstSelectedClient.UserChar.Z.ToString();
            }
        }

        private RTSClient GetFirstSelectedClient()
        {
            RTSClient result = null;
            foreach (ListBoxItem item in clientsListBox.SelectedItems)
            {
                foreach (RTSClient cl in server.clients)
                {
                    if (item.Name == "client_" + cl.clientID.ToString())
                    {
                        result = cl;
                    }
                }
            }
            return result;
        }

        private void gotoButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (ListBoxItem item in clientsListBox.SelectedItems)
            {
                foreach (RTSClient cl in server.clients)
                {
                    if (item.Name == "client_" + cl.clientID.ToString())
                    {
                        try
                        {
                            int x = int.Parse(toXTextBox.Text);
                            int y = int.Parse(toYTextBox.Text);
                            int z = int.Parse(toZTextBox.Text);
                            cl.SendMoveToCommand(x, y, z);
                        }
                        catch (FormatException)
                        {
                            LogToConsole(cl, "Wrong target position format");
                        }

                    }
                }
            }
        }

        private void EnableBotButton_Click(object sender, RoutedEventArgs e)
        {

            foreach (ListBoxItem item in clientsListBox.SelectedItems)
            {
                foreach (RTSClient cl in server.clients)
                {
                    if (item.Name == "client_" + cl.clientID.ToString())
                    {
                        cl.SendEnableBotCommand(true);
                    }
                }
            }

        }

        private void DisableBotButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (ListBoxItem item in clientsListBox.SelectedItems)
            {
                foreach (RTSClient cl in server.clients)
                {
                    if (item.Name == "client_" + cl.clientID.ToString())
                    {
                        cl.SendEnableBotCommand(false);
                    }
                }
            }
        }
    }

}
