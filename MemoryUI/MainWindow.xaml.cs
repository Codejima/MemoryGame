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
            ResetGame();

            mTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 100),
                        DispatcherPriority.Render, (_, _) => lblTime.Text = $"Time: {(DateTime.Now - mTimeGameStart).TotalSeconds.ToString("N1")}", Dispatcher.CurrentDispatcher);
            mTimer.Stop();
        }

        //switch visibility of image cardBack/cardFront on click( "flip card" )
        void CardFlip_Click(object sender, RoutedEventArgs e)
        {
            //start round timer on first card pick
            if (!mTimer.IsEnabled)
            {
                mTimeGameStart = DateTime.Now;
                mTimer.Start();
            }
            ///////////////////////////////
            if (mFirstSelectedButton is not null && mSecondSelectedButton is not null)
            {
                CardTurnaround(mFirstSelectedButton);
                CardTurnaround(mSecondSelectedButton);

                mFirstSelectedButton = sender as Button;
                mSecondSelectedButton = null;

                CardTurnaround(mFirstSelectedButton);
                return;
            }

            if (mFirstSelectedButton is null)
            {
                // first selection
                mFirstSelectedButton = sender as Button;
                CardTurnaround(mFirstSelectedButton);
            }
            else
            {
                // second selection
                mSecondSelectedButton = sender as Button;
                CardTurnaround(mSecondSelectedButton);
                Turns++;

                // comparison
                //if ((mFirstSelectedButton.Content as Image).Name != (mSecondSelectedButton.Content as Image).Name)
                if (((mFirstSelectedButton.Content as StackPanel).Children[0] as Image).Name != ((mSecondSelectedButton.Content as StackPanel).Children[0] as Image).Name)
                {
                    return;
                }
                // hide & reset
                mFirstSelectedButton.IsEnabled = false;
                mSecondSelectedButton.IsEnabled = false;
                mFirstSelectedButton = null;
                mSecondSelectedButton = null;
                Points++;
                //TODO: add Timer.Stop(); to end of game
                // if (endofgame)
            }
        }

        private void CardTurnaround(Button card)
        {
            Image cardBack = (card.Content as StackPanel).Children[1] as Image;
            Image cardFront = (card.Content as StackPanel).Children[0] as Image;

            if (cardBack.Visibility == Visibility.Visible)
            {
                cardBack.Visibility = Visibility.Collapsed;
                cardFront.Visibility = Visibility.Visible;
            }
            else
            {
                cardBack.Visibility = Visibility.Visible;
                cardFront.Visibility = Visibility.Collapsed;
            }
        }
        private void ResetGame()
        {
            //enables card buttons (if disabled)
            foreach (var item in FieldGrid.Children)
            {
                (item as Button).IsEnabled = true;
            }
            
            // resets selected buttons
            mFirstSelectedButton = null;
            mSecondSelectedButton = null;

            //shuffle cards / fill board again
            
            List<String> Images = new() { "/Images/1.png", "/Images/2.png", "/Images/3.png", "/Images/4.png", "/Images/5.png", "/Images/6.png", "/Images/7.png", "/Images/8.png" };
            string cardBackground = "/Images/back.png";
            for (int imgID = Images.Count - 1; imgID > -1; imgID--)
            {
                Images.Add(Images[imgID]);
            }
            Random rndGen = new();
            for (int btnID = 0; btnID < FieldGrid.Children.Count; btnID++)
            {
                ((FieldGrid.Children[btnID] as Button).Content as StackPanel).Children.Clear();

                int tempInt = rndGen.Next(Images.Count);
                Uri uriFront = new(Images[tempInt], UriKind.Relative);
                Image cardFrontReset = new();
                cardFrontReset.Source = new BitmapImage(uriFront);
                cardFrontReset.Width = 150;
                cardFrontReset.Height = 150;
                cardFrontReset.Visibility = Visibility.Collapsed;
                ((FieldGrid.Children[btnID] as Button).Content as StackPanel).Children.Add(cardFrontReset);
                Images.RemoveAt(tempInt);

                Uri uriBack = new(cardBackground, UriKind.Relative);
                Image cardBackReset = new();
                cardBackReset.Source = new BitmapImage(uriBack);
                cardBackReset.Width = 150;
                cardBackReset.Height = 150;
                cardBackReset.Visibility = Visibility.Visible;
                ((FieldGrid.Children[btnID] as Button).Content as StackPanel).Children.Add(cardBackReset);
            }

        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            ResetGame();
        }
    }
}
