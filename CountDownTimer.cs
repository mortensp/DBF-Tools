using DBF.UserControls;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace DBF
{
    public class CountDownTimer
    {
        private DispatcherTimer timer;
        private TimeSpan remainingTime;
        private bool isPaused;
        private TimeSpan startTime;
        private static TimeSpan oneHour = new TimeSpan(1, 0, 0);

        public CountDownTimer()
        {
            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += Timer_Tick;
        }

        public TimeSpan StartTime
        {
            get => startTime;
            set
            {
                startTime = value;
                remainingTime = (TimeSpan)value;
                setTime();
            }
        }

        public bool IsPaused => isPaused;

        public string Time { get; set; }

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

        public void StopCountdown()
        {
            timer.Stop();
            isPaused = false;
            remainingTime = StartTime;
            setTime();
        }
        #endregion

        #region Private Timer Events
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!isPaused && remainingTime.TotalSeconds > 0)
            {
                remainingTime = remainingTime.Subtract(TimeSpan.FromSeconds(1));
                setTime();
            }
            else
                if (remainingTime.TotalSeconds <= 0)
                timer.Stop();
        }

        private void setTime()
        {
            Time = remainingTime < oneHour
                 ? remainingTime.ToString(@"mm\:ss")
                 : remainingTime.ToString(@"mm\:ss");
        }
        #endregion
    }
}
