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
using System.Windows.Threading;

namespace MemoryUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Button mFirstSelectedButton;
        private Button mSecondSelectedButton;
        private DateTime mTimeGameStart;

        public DispatcherTimer Timer
        {
            get
            {
                if (mTimer is null)
                {
                    mTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 100),
                        DispatcherPriority.Render, (_, _) => lblTime.Text = $"Time: {(DateTime.Now - mTimeGameStart).TotalSeconds.ToString()}", Dispatcher.CurrentDispatcher);
                }
                return mTimer;
            }
        }
        private DispatcherTimer mTimer;

        private int mPoints;
        public int Points
        {
            get { return mPoints; }
            set
            {
                mPoints = value;
                tblPoints.Text = "Points: " + mPoints;
            }
        }
        private int mTurns;
        public int Turns
        {
            get { return mTurns; }
            set
            {
                mTurns = value;
                tblTurns.Text = "Turns: " + mTurns;
            }
        }


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnField_Click(object sender, RoutedEventArgs e)
        {
            if (!Timer.IsEnabled)
            {
                mTimeGameStart = DateTime.Now;
                Timer.Start();
            }
            if (mFirstSelectedButton is not null && mSecondSelectedButton is not null)
            {
                (mFirstSelectedButton.Content as Image).Visibility = Visibility.Collapsed;
                (mSecondSelectedButton.Content as Image).Visibility = Visibility.Collapsed;

                mFirstSelectedButton = null;
                mSecondSelectedButton = null;
                // show selected button
                ((sender as Button).Content as Image).Visibility = Visibility.Visible;
                return;
            }


            if (mFirstSelectedButton is null)
            {
                // first selection
                mFirstSelectedButton = sender as Button;
            }
            else
            {
                // second selection
                mSecondSelectedButton = sender as Button;
                Turns++;

                // comparison
                if ((mFirstSelectedButton.Content as Image).Source != (mSecondSelectedButton.Content as Image).Source) 
                {
                    return;
                }

                mFirstSelectedButton.IsEnabled = false;
                mSecondSelectedButton.IsEnabled = false;
                mFirstSelectedButton = null;
                mSecondSelectedButton = null;
                Points++;
                Timer.Stop();
            }

            //if first selection empty
            //  save selected button as first selection
            //else (first NOT empty, second empty)
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
