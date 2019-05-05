using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;
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
using TypingApp.Utilities;
using System.Windows.Forms;
using System.Drawing;

namespace TypingApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KeyboardHook keyboard;
        public static NotifyIcon notifyIcon = new NotifyIcon();

        int noOfChars = 0;
        bool hasSpecialKey = false;
        string typedText;
        bool skipWord = false;

        public MainWindow()
        {
            InitializeComponent();

            lblUser.Content = "Welcome " + Environment.UserName.ToUpper() + " on " + Environment.MachineName.ToUpper();

            this.Closing += MainWindow_Closing;

            notifyIcon.DoubleClick += new EventHandler(NotifyIcon_DoubleClick);
            notifyIcon.Click += new EventHandler(NotifyIcon_Click);
            notifyIcon.Icon = Resource.favicon;
            notifyIcon.BalloonTipTitle = "TypingPattern";
            notifyIcon.BalloonTipText = "Watching your keyboard";
            notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon.Text = "TypingPattern";
            notifyIcon.Visible = true;
            //notifyIcon.Click += NotifyIcon_Click;
            //notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
            //notifyIcon.MouseDoubleClick += NotifyIcon_DoubleClick;

            Watch.SetupWatch();

            Hide();
            //this.ShowInTaskbar = false;
            //this.WindowState = WindowState.Minimized;

            keyboard = new KeyboardHook();
            keyboard.KeyPressEvent += Keyboard_KeyPressEvent;
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            if (IsVisible)
            {
                Activate();
            }
            else
            {
                Show();
            }
            //this.Visibility = Visibility.Visible;
            //this.WindowState = WindowState.Normal;
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            notifyIcon.ShowBalloonTip(2000);
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            keyboard?.Dispose();
        }

        private void Keyboard_KeyPressEvent(object sender, KeyPressedArgs e)
        {
            if (e.KeyPressed == Key.LeftShift ||
                e.KeyPressed == Key.RightShift ||
                e.KeyPressed == Key.LeftCtrl ||
                e.KeyPressed == Key.RightCtrl)
            {
                hasSpecialKey = true;
            }

            if (e.KeyPressed >= Key.A && e.KeyPressed <= Key.Z)
            {
                if (!Watch.watch.IsRunning)
                    Watch.watch.Start();
                typedText += e.KeyPressed.ToString();
                noOfChars++;
            }
            else if ((e.KeyPressed >= Key.NumPad0 && e.KeyPressed <= Key.NumPad9) ||
                (e.KeyPressed >= Key.D0 && e.KeyPressed <= Key.D9))
            {
                //reset; alphanumeric data
                skipWord = true;
            }

            switch (e.KeyPressed)
            {
                case Key.Return:
                case Key.Escape:
                case Key.Space:
                case Key.Tab:
                case Key.OemComma:
                case Key.Decimal:
                case Key.OemMinus:
                case Key.OemPeriod:
                case Key.OemQuestion:
                case Key.OemSemicolon:
                    if (0 != noOfChars && false == skipWord)
                    {
                        DataPoints sample = new DataPoints(
                                                            typedText,
                                                            noOfChars,
                                                            Watch.watch.ElapsedMilliseconds,
                                                            hasSpecialKey,
                                                            DateTime.UtcNow);

                        lock (DataPoints.dataSample)
                        {
                            DataPoints.dataSample.Add(sample);
                        }
                        notifyIcon.BalloonTipText = DataPoints.dataSample.Count + " Samples collected";
                    }
                    Watch.watch.Reset();
                    typedText = "";
                    noOfChars = 0;
                    hasSpecialKey = false;
                    break;

                default:
                    break;
            }
        }

    }
}
