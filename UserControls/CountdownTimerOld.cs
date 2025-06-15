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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DBF.UserControls
{
    /// <summary>
    /// Interaction logic for CountdownTimerOld.xaml
    /// </summary>
    public partial class CountdownTimerOld : UserControl
    {
        private DispatcherTimer timer;
        private TimeSpan        remainingTime;
        private bool            isPaused;

        #region Constructors
            public CountdownTimerOld()
            {
                InitializeComponent();
                timer       = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
                timer.Tick += Timer_Tick;
            }
        #endregion

        #region Dependency Property: StartTime
            public TimeSpan StartTime
            {
                get { return (TimeSpan)GetValue(StartTimeProperty); }
                set { SetValue(StartTimeProperty, value); }
            }

            public static readonly DependencyProperty StartTimeProperty = 
                                   DependencyProperty.Register(nameof(StartTime), typeof(TimeSpan), typeof(CountdownTimerOld), new PropertyMetadata(TimeSpan.Zero, OnStartTimeChanged));
        #endregion


        #region Dependency Property: Time
        public string Time
        {
            get { return (string)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        public static readonly DependencyProperty TimeProperty =
                               DependencyProperty.Register(nameof(Time), typeof(string), typeof(CountdownTimerOld), new PropertyMetadata("00:11:00"));
        #endregion

        #region Public Methods
        public void StartCountdown()
            {
                if (isPaused)
                    isPaused = false;
                else
                    remainingTime = StartTime;

                timer.Start();
            }

            public void PauseCountdown()
            {
                isPaused = true;
            }

            //public void ResumeCountdown()
            //{
            //    isPaused = false;
            //}

            public void StopCountdown()
            {
                timer.Stop();
                isPaused      = false;
                remainingTime = StartTime;
                UpdateDisplay();
            }
        #endregion

        #region Private Timer Events
            private static void OnStartTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                var control           = (CountdownTimerOld)d;
                control.remainingTime = (TimeSpan)e.NewValue;
                control.UpdateDisplay();
            }

            private void Timer_Tick(object sender, EventArgs e)
            {
                if (!isPaused && remainingTime.TotalSeconds >  0)
                {
                    remainingTime = remainingTime.Subtract(TimeSpan.FromSeconds(1));
                    UpdateDisplay();
                }
                else
                    if (remainingTime.TotalSeconds <= 0)
                        timer.Stop();
            }

            private void UpdateDisplay()
            {
                Time = remainingTime.ToString(@"mm\:ss");
            }
        #endregion
    }
}
