using System;
using System.Collections.Generic;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
        private bool mCurrentPlayer;
        private DateTime mTimeGameStart;
        private DispatcherTimer mTimer;
        private readonly MediaPlayer mMusic;
        private readonly SoundPlayer mSound;

        private int mPoints;
        public int Points
        {
            get { return mPoints; }
            set
            {
                mPoints = value;
                tblPoints1.Text = "Points: " + mPoints;
            }
        }

        private int mTurns;
        public int Turns
        {
            get { return mTurns; }
            set
            {
                mTurns = value;
                tblTurns1.Text = "Turns: " + mTurns;
            }
        }

        public MainWindow()
        {
            mMusic = new();
            mMusic.Open(new Uri("Audio/Adiantum.mp3", UriKind.Relative));
            mMusic.Volume = 0.1;
            InitializeComponent();
            CenterWindowOnScreen();
            mMusic.Play();
            mMusic.MediaEnded += loop;
            mSound = new("Audio/bokara.wav");

            mTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 100),
                        DispatcherPriority.Render, (_, _) => lblTime.Text = $"Time: {(DateTime.Now - mTimeGameStart).TotalSeconds:N1}", Dispatcher.CurrentDispatcher);
            ResetGame("Alpaca");

            System.Reflection.PropertyInfo[] proplist = typeof(Brushes).GetProperties();
            foreach (System.Reflection.PropertyInfo item in proplist)
                brushList.Add((item.Name, item.GetValue(null) as SolidColorBrush));
        }


        private void loop(object sender, EventArgs e)
        {
            mMusic.Position = TimeSpan.Zero;
            mMusic.Play();
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
                    mCurrentPlayer = !mCurrentPlayer;
                    return;
                }
                // hide & reset
                mFirstSelectedButton.IsEnabled = false;
                mSecondSelectedButton.IsEnabled = false;
                mFirstSelectedButton = null;
                mSecondSelectedButton = null;
                Points = Points + 5;
                pairedCards++;
                mSound.Play();
                if (pairedCards == numberOfCards)
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


        int numberOfCards;
        private void ResetGame(string sender)
        {
            //enables card buttons (if disabled)
            foreach (var item in FieldGrid.Children)
            {
                (item as Button).IsEnabled = true;
            }
            // resets selected buttons
            mFirstSelectedButton = null;
            mSecondSelectedButton = null;
            List<string> Images;
            // fill board
            switch (sender)
            {
                case "Alpaca":
                    Images = new() { "/ImagesAlpaca/1.png", "/ImagesAlpaca/2.png", "/ImagesAlpaca/3.png", "/ImagesAlpaca/4.png", "/ImagesAlpaca/5.png", "/ImagesAlpaca/6.png", "/ImagesAlpaca/7.png", "/ImagesAlpaca/8.png" };
                    break;
                case "Sweets":
                    Images = new() { "/ImagesSweets/1.png", "/ImagesSweets/2.png", "/ImagesSweets/3.png", "/ImagesSweets/4.png", "/ImagesSweets/5.png", "/ImagesSweets/6.png", "/ImagesSweets/7.png", "/ImagesSweets/8.png" };
                    break;
                default:
                    Images = new() { "/ImagesAlpaca/1.png", "/ImagesAlpaca/2.png", "/ImagesAlpaca/3.png", "/ImagesAlpaca/4.png", "/ImagesAlpaca/5.png", "/ImagesAlpaca/6.png", "/ImagesAlpaca/7.png", "/ImagesAlpaca/8.png" };
                    break;
            }

            string cardBackground = "/ImagesAlpaca/back.png";

            numberOfCards = Images.Count;
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
                cardFrontReset.Name = "imageName" + Images[tempInt][14..^4];
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

        private void CenterWindowOnScreen()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2);
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            //ResetGame(cmbTheme.SelectedItem.ToString());
            ResetGame((cmbTheme.SelectedItem as ComboBoxItem).Content.ToString());
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            mMusic.Play();
            btnPlay.Visibility = Visibility.Collapsed;
            btnPause.Visibility = Visibility.Visible;
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            mMusic.Pause();
            btnPause.Visibility = Visibility.Collapsed;
            btnPlay.Visibility = Visibility.Visible;
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            mMusic.Position = TimeSpan.Zero;
            mMusic.Pause();
        }

        private int mCurrentColorIndex;
        private readonly List<(string, SolidColorBrush)> brushList = new();
        private void btnPlayer1_Click(object sender, RoutedEventArgs e)
        {
            Button s = sender as Button;
            s.Foreground = brushList[mCurrentColorIndex].Item2;
            tblPoints1.Foreground = brushList[mCurrentColorIndex].Item2;
            tblTurns1.Foreground = brushList[mCurrentColorIndex].Item2;
            lblTime.Foreground = brushList[mCurrentColorIndex].Item2;
            mCurrentColorIndex = mCurrentColorIndex == brushList.Count - 1 ? 0 : mCurrentColorIndex + 3;
        }

        private void btnPlayer2_Click(object sender, RoutedEventArgs e)
        {
            Button s = sender as Button;
            s.Foreground = brushList[mCurrentColorIndex].Item2;
            tblPoints2.Foreground = brushList[mCurrentColorIndex].Item2;
            tblTurns2.Foreground = brushList[mCurrentColorIndex].Item2;
            lblTime2.Foreground = brushList[mCurrentColorIndex].Item2; mCurrentColorIndex = mCurrentColorIndex == brushList.Count - 1 ? 0 : mCurrentColorIndex + 3;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //SliderStuff.Navigate(new SliderCanvasLineDemo());
        }
        static class ChangeThemeCommand
        {
            public static readonly RoutedUICommand ChangeTheme = new RoutedUICommand("Upload Command", "Upload", typeof(ChangeThemeCommand), new InputGestureCollection() { new KeyGesture(Key.C, ModifierKeys.Control) });
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mMusic.Volume = VolumeSlider.Value;
            if (mMusic.Volume == 0)
            {
                btnPause.Visibility = Visibility.Collapsed;
                btnPlay.Visibility = Visibility.Visible;
            }
            if (mMusic.Volume > 0)
            {
                btnPlay.Visibility = Visibility.Collapsed;
                btnPause.Visibility = Visibility.Visible;
            }
        }
    }
}