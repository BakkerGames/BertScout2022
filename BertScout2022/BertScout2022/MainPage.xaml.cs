using System;
using Xamarin.Forms;

namespace BertScout2022
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void MenuButton_Clicked(object sender, EventArgs e)
        {
            bool matchScreen = !MatchEntryView.IsVisible;
            if (matchScreen)
            {
                MatchEntryView.IsVisible = true;
                MatchEntryView.IsEnabled = true;
                MenuButton.Text = ". . .";
            }
            else
            {
                MatchEntryView.IsVisible = false;
                MatchEntryView.IsEnabled = false;
                MenuButton.Text = "\u25c0\u2013"; // <--
            }
        }
    }
}
