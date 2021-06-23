using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
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

            mTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 100),
                        DispatcherPriority.Render, (_, _) => lblTime.Text = $"Time: {(DateTime.Now - mTimeGameStart).TotalSeconds:N1}", Dispatcher.CurrentDispatcher);
            ResetGame();
        }
        //switch visibility of image cardBack/cardFront on click( "flip card" )
        int pairedCards;
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
                if (mFirstSelectedButton == sender)
                {
                    CardTurnaround(mFirstSelectedButton);
                    return;
                }
                Turns++; 
                mSecondSelectedButton = sender as Button;
                CardTurnaround(mSecondSelectedButton);


                // comparison
                if (((mFirstSelectedButton.Content as StackPanel).Children[0] as Image).Name != ((mSecondSelectedButton.Content as StackPanel).Children[0] as Image).Name)
                {
                    Points = Math.Max(0, Points - 2);
                    return;
                }
                // hide & reset
                mFirstSelectedButton.IsEnabled = false;
                mSecondSelectedButton.IsEnabled = false; 
                mFirstSelectedButton = null;
                mSecondSelectedButton = null;
                Points = Points + 5;
                pairedCards++;
                if (pairedCards == 8) //TODO: change 8 to list.count or something
                {
                    mTimer.Stop();
                    GameEndImage.Visibility = Visibility.Visible;
                }
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

            // fill board
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
                cardFrontReset.Name = "imageName" + Images[tempInt][8..^4];
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
            foreach (var item in FieldGrid.Children)
            {
                (item as Button).IsEnabled = true;
            }
            mTimer.Stop();
            pairedCards = 0;
            Points = 0;
            Turns = 0;
            lblTime.Text = "Time: 0";
            GameEndImage.Visibility = Visibility.Collapsed;
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            ResetGame();
        }
    }
}