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
                        DispatcherPriority.Render, (_, _) => lblTime.Text = $"Time: {(DateTime.Now - mTimeGameStart).TotalSeconds}", Dispatcher.CurrentDispatcher);
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

            List<String> Images = new() { "/Images/1.png", "/Images/2.png", "/Images/3.png", "/Images/4.png", "/Images/5.png", "/Images/6.png", "/Images/7.png", "/Images/8.png" };
            string cardBackground = "/Images/back.png";
            List<String> ImagesOnField = new();

            // duplicate content of List Images
            for (int imgID = Images.Count - 1; imgID > -1; imgID--)
            {
                Images.Add(Images[imgID]);
            }

            // fill FieldGrid with cardBack Image and random cardFront Image
            Random rndGen = new();
            for (int btnID = 0; btnID < FieldGrid.Children.Count; btnID++)
            {
                int tempInt = rndGen.Next(Images.Count);
                Uri uriFront = new(Images[tempInt], UriKind.Relative);
                ImagesOnField.Add(Images[tempInt]);
                Image cardFront = new();
                cardFront.Source = new BitmapImage(uriFront);
                cardFront.Width = 150;
                cardFront.Height = 150;
                cardFront.Visibility = Visibility.Collapsed;
                ((FieldGrid.Children[btnID] as Button).Content as StackPanel).Children.Add(cardFront);
                Images.RemoveAt(tempInt);

                Uri uriBack = new(cardBackground, UriKind.Relative);
                Image cardBack = new();
                cardBack.Source = new BitmapImage(uriBack);
                cardBack.Width = 150;
                cardBack.Height = 150;
                cardBack.Visibility = Visibility.Visible;
                ((FieldGrid.Children[btnID] as Button).Content as StackPanel).Children.Add(cardBack);
            }
            //for (int btnID = 0; btnID < FieldGrid.Children.Count; btnID++)
            //{
            //    Uri uri = new(cardBackground, UriKind.Relative);
            //    Image cardBack = new();
            //    cardBack.Source = new BitmapImage(uri);
            //    cardBack.Width = 100;
            //    cardBack.Height = 100;
            //    cardBack.Visibility = Visibility.Visible;
            //    ((FieldGrid.Children[btnID] as Button).Content as StackPanel).Children.Add(cardBack);
            //}
        }

        //switch visibility of image cardBack/cardFront on click( "flip card" )
        void CardFlip_Click(object sender, RoutedEventArgs e)
        {
            if (!Timer.IsEnabled)
            {
                mTimeGameStart = DateTime.Now;
                Timer.Start();
            }

            Image cardBack = ((sender as Button).Content as StackPanel).Children[0] as Image;
            Image cardFront = ((sender as Button).Content as StackPanel).Children[1] as Image;

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
