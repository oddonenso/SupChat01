using System;
using System.Windows;
using System.Windows.Input;

namespace ClientChat
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            FrmMain.Navigate(new Uri("Pages/Autho.xaml", UriKind.Relative));

            // Событие для перемещения окна при нажатии на верхнюю панель
            this.MouseLeftButtonDown += (sender, e) =>
            {
                if (e.ClickCount == 2)
                {
                    ToggleWindowState();
                }
                else
                {
                    this.DragMove();
                }
            };
        }

        private void FrmMain_ContentRendered(object sender, EventArgs e)
        {
        }

        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeWindow(object sender, RoutedEventArgs e)
        {
            ToggleWindowState();
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ToggleWindowState()
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }
    }
}
