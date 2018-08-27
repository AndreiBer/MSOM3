using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Xvue.MSOT;
using Xvue.MSOT.ViewModels.Experiment;


namespace ViewMSOTc
{
	/// <summary>
	/// Interaction logic for SplashStartup.xaml
	/// </summary>
	public partial class SplashStartup : Window
	{
		public SplashStartup()
		{
			this.InitializeComponent();
            this.Height = System.Windows.SystemParameters.VirtualScreenHeight;
			// Insert code required on object creation below this point.
		}

        System.Windows.Threading.DispatcherTimer _delay;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int instances = 0;
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();
            foreach (System.Diagnostics.Process process in processes)
            {
                if (process.ProcessName.Contains("MSOT") && process.MainModule.FileVersionInfo.CompanyName.Contains("iThera"))
                {
                    if (++instances > 1)
                    {
                        break;
                    }
                }
            }

            if (instances > 1)
            {
                Hide();
                //cannot use ViewMSOTcSystem.NotifyUserOnError yet, so use the standard MessageBox
                MessageBox.Show("An MSOT application is still active.\nPlease close before proceeding.", this.Name, MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
            else
            {
                _delay = new System.Windows.Threading.DispatcherTimer();
                _delay.Tick += new EventHandler(delay_Tick);
                _delay.Interval = new TimeSpan(100);
                _delay.Start();
            }
        }

        void delay_Tick(object sender, EventArgs e)
        {
            ViewMSOTcSystem mainView = null;
            try
            {
                _delay.Stop();
                mainView = new ViewMSOTcSystem();
                mainView.StartMSOT(childWindowContainer);
                mainView.Show();
                Close();
                Application.Current.MainWindow = mainView;
                mainView.ShowPendingPushedMessages();
                mainView = null;
            }
            finally
            {
                mainView?.Dispose();
            }
        }

	}



}
