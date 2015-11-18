using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UltimateChess
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public Coordinate firstClick;
        public Coordinate secondClick;

        public MainPage()
        {
            this.InitializeComponent();
            GridModel Grid = new GridModel();
            Grid.Start();

            LayoutGridSetUp();
        }

        private void LayoutGridSetUp()
        {
            layoutGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(Window.Current.Bounds.Width / 5) });
            layoutGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            layoutGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(Window.Current.Bounds.Width / 5) });
            layoutGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(100) });
            layoutGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            layoutGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(100) });
        }

        private void CanvasSetUp()
        {
            if(CanvasBoard.ActualWidth < CanvasBoard.ActualHeight)
            {
                CanvasBoard.Height = CanvasBoard.Width;
            }
            else
            {
                CanvasBoard.Width = CanvasBoard.ActualHeight;
            }

            CanvasBoard.Children.Clear();

            int squareSize = (int)CanvasBoard.Width / 8;

            SolidColorBrush gray = new SolidColorBrush(Colors.Gray);
            SolidColorBrush white = new SolidColorBrush(Colors.White);

            bool flipflop = true;

            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    Rectangle square = new Rectangle();

                    if(flipflop)
                    {
                        square.Fill = white;
                    }
                    else
                    {
                        square.Fill = gray;
                    }

                    flipflop = !flipflop;

                    square.Width = square.Height = squareSize;

                    int x = c * squareSize;
                    int y = r * squareSize;

                    Canvas.SetTop(square, y);
                    Canvas.SetLeft(square, x);

                    CanvasBoard.Children.Add(square);
                }
                flipflop = !flipflop;
            }


        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnColor_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CanvasSetUp();
        }
    }
}
