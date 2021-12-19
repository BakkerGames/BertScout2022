using BertScout2022.Data.Models;
using System;
using Xamarin.Forms;

namespace BertScout2022
{
    public partial class MainPage : ContentPage
    {
        private TeamMatch teamMatch;

        private int _state;

        public MainPage()
        {
            InitializeComponent();
            SetState(0);
        }

        private void MenuButton_Clicked(object sender, EventArgs e)
        {
            if (_state == 0)
            {
                SetState(1);
            }
            else
            {
                SetState(0);
            }
        }

        private async void MatchButton_Clicked(object sender, EventArgs e)
        {
            switch (MatchButton.Text)
            {
                case "Start":
                    //if (string.IsNullOrEmpty(TeamNumber.Text) ||
                    //    string.IsNullOrEmpty(MatchNumber.Text))
                    //{
                    //    return;
                    //}
                    int team;
                    int match;
                    if (!int.TryParse(TeamNumber.Text, out team) ||
                        !int.TryParse(MatchNumber.Text, out match))
                    {
                        return;
                    }
                    if (team <= 0 || match <= 0)
                    {
                        return;
                    }
                    teamMatch = await App.Database.GetTeamMatchAsync(team, match);
                    if (teamMatch == null)
                    {
                        if (string.IsNullOrEmpty(ScouterName.Text))
                        {
                            return;
                        }
                        teamMatch = new TeamMatch
                        {
                            TeamNumber = team,
                            MatchNumber = match
                        };
                        ClearAllFields();
                    }
                    else
                    {
                        FillAllFields(teamMatch);
                    }
                    SetState(2);
                    break;
                case "Save":
                    SaveAllFields(teamMatch);
                    _ = await App.Database.SaveTeamMatchAsync(teamMatch);
                    ClearAllFields();
                    SetState(0);
                    break;
                default:
                    break;
            }
        }

        private void ClearAllFields()
        {
            MovedOffStartCheckbox.IsChecked = false;
            ClimbedCheckbox.IsChecked = false;
            WonCheckbox.IsChecked = false;
            LostCheckbox.IsChecked = false;
            TiedCheckbox.IsChecked = false;
        }

        private void FillAllFields(TeamMatch item)
        {
            ScouterName.Text = item.ScouterName;
            MovedOffStartCheckbox.IsChecked = item.MovedOffStart;
            ClimbedCheckbox.IsChecked = item.Climbed;
            WonCheckbox.IsChecked = item.Won;
            TiedCheckbox.IsChecked = item.Tied;
            LostCheckbox.IsChecked = item.Lost;
        }

        private void SaveAllFields(TeamMatch item)
        {
            if (!string.IsNullOrWhiteSpace(ScouterName.Text))
            {
                item.ScouterName = ScouterName.Text;
            }
            item.MovedOffStart = MovedOffStartCheckbox.IsChecked;
            item.Climbed = ClimbedCheckbox.IsChecked;
            item.Won = WonCheckbox.IsChecked;
            item.Tied = TiedCheckbox.IsChecked;
            item.Lost = LostCheckbox.IsChecked;
        }

        private void SetState(int stateNumber)
        {
            switch (stateNumber)
            {
                case 0:
                    MenuButton.Text = "Menu";
                    MenuButton.IsVisible = true;
                    MatchButton.Text = "Start";
                    MatchButton.IsVisible = true;
                    TeamNumber.Text = "";
                    MatchNumber.Text = "";
                    TeamNumber.IsEnabled = true;
                    MatchNumber.IsEnabled = true;
                    ScouterName.IsEnabled = true;
                    MatchEntryView.IsVisible = true;
                    MatchEntryBody.IsVisible = false;
                    break;
                case 1:
                    MatchEntryView.IsVisible = false;
                    MenuButton.Text = "\u25c0\u2013"; // <--
                    MatchButton.IsVisible = false;
                    break;
                case 2:
                    TeamNumber.IsEnabled = false;
                    MatchNumber.IsEnabled = false;
                    ScouterName.IsEnabled = true;
                    MenuButton.IsVisible = false;
                    MatchEntryBody.IsVisible = true;
                    MatchButton.Text = "Save";
                    break;
                default:
                    break;
            }
            _state = stateNumber;
        }

        private void WonCheckbox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (WonCheckbox.IsChecked)
            {
                TiedCheckbox.IsChecked = false;
                LostCheckbox.IsChecked = false;
            }
        }

        private void TiedCheckbox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (TiedCheckbox.IsChecked)
            {
                WonCheckbox.IsChecked = false;
                LostCheckbox.IsChecked = false;
            }
        }

        private void LostCheckbox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (LostCheckbox.IsChecked)
            {
                WonCheckbox.IsChecked = false;
                TiedCheckbox.IsChecked = false;
            }
        }
    }
}
