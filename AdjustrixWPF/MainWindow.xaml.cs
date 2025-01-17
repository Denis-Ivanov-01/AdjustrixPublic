using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AdjustrixWPF.ViewModel;
using MaterialDesignThemes.Wpf;
using System.Drawing;

namespace AdjustrixWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _mRestoreForDragMove;

        public MainWindow()
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            this.DataContext = new MainWindowViewModel();
            //ThemeViewModel.RegisterObject(DataTab);
        }

        // Not using this because of weird behaviour of DragMove()
        //private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    DragMove();
        //}

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Restore_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            
            Application.Current.Shutdown();
        }

        private void OnAppWindowWindowOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (this.ResizeMode != ResizeMode.CanResize &&
                    this.ResizeMode != ResizeMode.CanResizeWithGrip)
                {
                    return;
                }

                this.WindowState = this.WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;
            }
            else
            {
                _mRestoreForDragMove = this.WindowState == WindowState.Maximized;

                SafeDragMoveCall(e);
            }
        }

        private void SafeDragMoveCall(MouseEventArgs e)
        {
            Task.Delay(100).ContinueWith(_ =>
            {
                Dispatcher.BeginInvoke((Action)
                    delegate
                    {
                        if (Mouse.LeftButton == MouseButtonState.Pressed)
                        {
                            this.DragMove();
                            RaiseEvent(new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Left)
                            {
                                RoutedEvent = MouseLeftButtonUpEvent
                            });
                        }
                    });
            });
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_mRestoreForDragMove)
            {
                _mRestoreForDragMove = false;

                var point = PointToScreen(e.MouseDevice.GetPosition(this));

                this.Left = point.X - (this.RestoreBounds.Width * 0.5);
                this.Top = point.Y;

                this.WindowState = WindowState.Normal;

                this.DragMove();

                SafeDragMoveCall(e);
            }
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _mRestoreForDragMove = false;
        }
    }
}
