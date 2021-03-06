﻿using UltimateChess.Common;
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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace UltimateChess
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class ColorPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public string teamOneColor;
        public string teamTwoColor;

        public ColorPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
            var obj = App.Current as App;
            teamOneColor = obj.passedColors.TeamOne;
            teamTwoColor = obj.passedColors.TeamTwo;
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);

            var obj = App.Current as App;
            switch (obj.passedColors.TeamOne)
            {
                case "White":
                    teamOneRdoWhite.IsChecked = true;
                    break;
                case "Black":
                    teamOneRdoBlack.IsChecked = true;
                    break;
                case "Green":
                    teamOneRdoGreen.IsChecked = true;
                    break;
                case "Blue":
                    teamOneRdoBlue.IsChecked = true;
                    break;
                case "Yellow":
                    teamOneRdoYellow.IsChecked = true;
                    break;
                case "Purple":
                    teamOneRdoPurple.IsChecked = true;
                    break;
                case "Red":
                    teamOneRdoRed.IsChecked = true;
                    break;
                case "Orange":
                    teamOneRdoOrange.IsChecked = true;
                    break;
            }

            switch (obj.passedColors.TeamTwo)
            {
                case "White":
                    teamTwoRdoWhite.IsChecked = true;
                    break;
                case "Black":
                    teamTwoRdoBlack.IsChecked = true;
                    break;
                case "Green":
                    teamTwoRdoGreen.IsChecked = true;
                    break;
                case "Blue":
                    teamTwoRdoBlue.IsChecked = true;
                    break;
                case "Yellow":
                    teamTwoRdoYellow.IsChecked = true;
                    break;
                case "Purple":
                    teamTwoRdoPurple.IsChecked = true;
                    break;
                case "Red":
                    teamTwoRdoRed.IsChecked = true;
                    break;
                case "Orange":
                    teamTwoRdoOrange.IsChecked = true;
                    break;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void teamOneRdo_Checked(object sender, RoutedEventArgs e)
        {
            if (teamOneLabel != null)
            {
            RadioButton r = sender as RadioButton;
            string[] split = new string[] { "Rdo" };
            teamOneColor = r.Name.Split(split, StringSplitOptions.RemoveEmptyEntries)[1];

               
            teamOneLabel.Text = teamOneColor + " Team";

            var obj = App.Current as App;
            obj.passedColors.TeamOne = teamOneColor;
            }
        }

        private void teamTwoRdo_Checked(object sender, RoutedEventArgs e)
        {
            if (teamTwoLabel != null)
            {
                RadioButton r = sender as RadioButton;
                string[] split = new string[] { "Rdo" };
                teamTwoColor = r.Name.Split(split, StringSplitOptions.RemoveEmptyEntries)[1];


                teamTwoLabel.Text = teamTwoColor + " Team";
                var obj = App.Current as App;
                obj.passedColors.TeamTwo = teamTwoColor;
            }
        }


    }
}
