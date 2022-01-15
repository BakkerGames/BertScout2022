using BertScout2022.Data.Models;
using BertScout2022.Airtable;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace BertScout2022
{
    public partial class MainPage : ContentPage
    {
        private TeamMatch teamMatch;
        private int _state;
        public static Color UnselectedButtonColor = Color.FromHex("#bfbfbf");
        public static Color SelectedButtonColor = Color.FromHex("#008000");

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
            Climbed1.IsChecked = false;
            Climbed2.IsChecked = false;
            Climbed3.IsChecked = false;
            Climbed4.IsChecked = false;
            Win_Tie_Lost_Button_Background(-1);
            Rating_Button_Background(-1);
        }

        private void FillAllFields(TeamMatch item)
        {
            ScouterName.Text = item.ScouterName;
            MovedOffStartCheckbox.IsChecked = item.LeftTarmac == 1;
            FillClimbedCheckBoxes(item.ClimbLevel);
            Win_Tie_Lost_Button_Background(item.MatchRP);
            Rating_Button_Background(item.ScouterRating);
        }

        private void FillClimbedCheckBoxes(int climbLevel)
        {
            Climbed1.IsChecked = (climbLevel == 1);
            Climbed2.IsChecked = (climbLevel == 2);
            Climbed3.IsChecked = (climbLevel == 3);
            Climbed4.IsChecked = (climbLevel == 4);
        }

        private void SaveAllFields(TeamMatch item)
        {
            if (!string.IsNullOrWhiteSpace(ScouterName.Text))
            {
                item.ScouterName = ScouterName.Text;
            }
            item.LeftTarmac = MovedOffStartCheckbox.IsChecked ? 1 : 0;
            item.ClimbLevel = 0;
            if (Climbed1.IsChecked) item.ClimbLevel = 1;
            if (Climbed2.IsChecked) item.ClimbLevel = 2;
            if (Climbed3.IsChecked) item.ClimbLevel = 3;
            if (Climbed4.IsChecked) item.ClimbLevel = 4;
            item.Changed = true;
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
                    MatchMenuView.IsVisible = false;
                    MatchMenuView.IsEnabled = false;
                    break;
                case 1:
                    MatchEntryView.IsVisible = false;
                    MenuButton.Text = "\u25c0\u2013"; // <--
                    MatchButton.IsVisible = false;
                    MatchMenuView.IsVisible = true;
                    MatchMenuView.IsEnabled = true;
                    break;
                case 2:
                    TeamNumber.IsEnabled = false;
                    MatchNumber.IsEnabled = false;
                    ScouterName.IsEnabled = true;
                    MenuButton.IsVisible = false;
                    MatchEntryBody.IsVisible = true;
                    MatchMenuView.IsVisible = false;
                    MatchMenuView.IsEnabled = false;
                    MatchButton.Text = "Save";
                    break;
                default:
                    break;
            }
            _state = stateNumber;
        }

        private bool _climbedChanging = false;

        private void Climbed1_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (_climbedChanging) return;
            _climbedChanging = true;
            if (Climbed1.IsChecked)
                FillClimbedCheckBoxes(1);
            _climbedChanging = false;
        }

        private void Climbed2_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (_climbedChanging) return;
            _climbedChanging = true;
            if (Climbed2.IsChecked)
                FillClimbedCheckBoxes(2);
            _climbedChanging = false;
        }

        private void Climbed3_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (_climbedChanging) return;
            _climbedChanging = true;
            if (Climbed3.IsChecked)
                FillClimbedCheckBoxes(3);
            _climbedChanging = false;
        }

        private void Climbed4_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (_climbedChanging) return;
            _climbedChanging = true;
            if (Climbed4.IsChecked)
                FillClimbedCheckBoxes(4);
            _climbedChanging = false;
        }
        private void Won_Button_Clicked(object sender, EventArgs e)
        {
            teamMatch.MatchRP = 2;
            Win_Tie_Lost_Button_Background(teamMatch.MatchRP);
        }

        private void Tied_Button_Clicked(object sender, EventArgs e)
        {
            teamMatch.MatchRP = 1;
            Win_Tie_Lost_Button_Background(teamMatch.MatchRP);
        }

        private void Lost_Button_Clicked(object sender, EventArgs e)
        {
            teamMatch.MatchRP = 0;
            Win_Tie_Lost_Button_Background(teamMatch.MatchRP);
        }
        private void Win_Tie_Lost_Button_Background(int value)
        {
            WonButton.Background = (value == 2) ? SelectedButtonColor : UnselectedButtonColor;
            TiedButton.Background = (value == 1) ? SelectedButtonColor : UnselectedButtonColor;
            LostButton.Background = (value == 0) ? SelectedButtonColor : UnselectedButtonColor;
        }
        private void Button_Rate0Box_Clicked(object sender, EventArgs e)
        {
            teamMatch.ScouterRating = 0;
            Rating_Button_Background(teamMatch.ScouterRating);
        }
        private void Button_Rate1Box_Clicked(object sender, EventArgs e)
        {
            teamMatch.ScouterRating = 1;
            Rating_Button_Background(teamMatch.ScouterRating);
        }
        private void Button_Rate2Box_Clicked(object sender, EventArgs e)
        {
            teamMatch.ScouterRating = 2;
            Rating_Button_Background(teamMatch.ScouterRating);
        }
        private void Button_Rate3Box_Clicked(object sender, EventArgs e)
        {
            teamMatch.ScouterRating = 3;
            Rating_Button_Background(teamMatch.ScouterRating);
        }
        private void Button_Rate4Box_Clicked(object sender, EventArgs e)
        {
            teamMatch.ScouterRating = 4;
            Rating_Button_Background(teamMatch.ScouterRating);
        }
        private void Button_Rate5Box_Clicked(object sender, EventArgs e)
        {
            teamMatch.ScouterRating = 5;
            Rating_Button_Background(teamMatch.ScouterRating);
        }
        private void Rating_Button_Background(int value)
        {
            Rate0Button.BackgroundColor = (value == 0) ? SelectedButtonColor : UnselectedButtonColor;
            Rate1Button.BackgroundColor = (value == 1) ? SelectedButtonColor : UnselectedButtonColor;
            Rate2Button.BackgroundColor = (value == 2) ? SelectedButtonColor : UnselectedButtonColor;
            Rate3Button.BackgroundColor = (value == 3) ? SelectedButtonColor : UnselectedButtonColor;
            Rate4Button.BackgroundColor = (value == 4) ? SelectedButtonColor : UnselectedButtonColor;
            Rate5Button.BackgroundColor = (value == 5) ? SelectedButtonColor : UnselectedButtonColor;
        }

        private async void Button_SendToAirtable(object sender, EventArgs e)
        {
            List<TeamMatch> matches = await App.Database.GetTeamMatchesAsync();
            await AirtableDB.AirtableSendRecords(matches);
            foreach (TeamMatch match in matches)
            {
                match.Changed = false;
                await App.Database.SaveTeamMatchAsync(match);
            }
        }
    }
}
