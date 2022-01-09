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
            Climbed0.IsChecked = true;
            Climbed1.IsChecked = false;
            Climbed2.IsChecked = false;
            Climbed3.IsChecked = false;
            Climbed4.IsChecked = false;
            WonCheckbox.IsChecked = false;
            LostCheckbox.IsChecked = false;
            TiedCheckbox.IsChecked = false;
        }

        private void FillAllFields(TeamMatch item)
        {
            ScouterName.Text = item.ScouterName;
            MovedOffStartCheckbox.IsChecked = item.MovedOffStart;
            WonCheckbox.IsChecked = item.Won;
            TiedCheckbox.IsChecked = item.Tied;
            LostCheckbox.IsChecked = item.Lost;
            FillClimbedCheckBoxes(item.ClimbLevel);
        }

        private void FillClimbedCheckBoxes(int climbLevel)
        {
            Climbed0.IsChecked = climbLevel == 0;
            Climbed1.IsChecked = climbLevel == 1;
            Climbed2.IsChecked = climbLevel == 2;
            Climbed3.IsChecked = climbLevel == 3;
            Climbed4.IsChecked = climbLevel == 4;
        }

        private void SaveAllFields(TeamMatch item)
        {
            if (!string.IsNullOrWhiteSpace(ScouterName.Text))
            {
                item.ScouterName = ScouterName.Text;
            }
            item.MovedOffStart = MovedOffStartCheckbox.IsChecked;
            if (Climbed0.IsChecked) item.ClimbLevel = 0;
            if (Climbed1.IsChecked) item.ClimbLevel = 1;
            if (Climbed2.IsChecked) item.ClimbLevel = 2;
            if (Climbed3.IsChecked) item.ClimbLevel = 3;
            if (Climbed4.IsChecked) item.ClimbLevel = 4;
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

        bool _climbedChanging = false;

        private void Climbed0_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (_climbedChanging) return;
            _climbedChanging = true;
            FillClimbedCheckBoxes(0);
            _climbedChanging = false;
        }

        private void Climbed1_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (_climbedChanging) return;
            _climbedChanging = true;
            FillClimbedCheckBoxes(1);
            _climbedChanging = false;
        }

        private void Climbed2_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (_climbedChanging) return;
            _climbedChanging = true;
            FillClimbedCheckBoxes(2);
            _climbedChanging = false;
        }

        private void Climbed3_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (_climbedChanging) return;
            _climbedChanging = true;
            FillClimbedCheckBoxes(3);
            _climbedChanging = false;
        }

        private void Climbed4_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (_climbedChanging) return;
            _climbedChanging = true;
            FillClimbedCheckBoxes(4);
            _climbedChanging = false;
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
