using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
        private double canvasSize;

        public MainPage()
        {
            this.InitializeComponent();
            GridModel Grid = new GridModel();
            Grid.Start();

            if(Window.Current.Bounds.Height < Window.Current.Bounds.Width)
            {
                canvasSize = Window.Current.Bounds.Height - 128;
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
    }
}
