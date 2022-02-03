using BertScout2022.Data.Models;
using BertScout2022.Airtable;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Text;

namespace BertScout2022
{
    public partial class MainPage : ContentPage
    {
        private TeamMatch teamMatch;
        private int _state;
        private string deleteMatchPassword = "bert133";
        private string deleteAllMatchesPassword = "don't tell john";
        Boolean greenMode = false;
        Boolean darkGreenMode = false;
        Boolean darkMode = false;
        public static Color UnselectedButtonColor = Color.FromHex("#bfbfbf");
        public static Color SelectedButtonColor = Color.FromHex("#008000");

        public MainPage()
        {
            InitializeComponent();
            SetState(0);
        }

        private void MenuButton_Clicked(object sender, EventArgs e)
        {
            if (MenuButton.Text == "Back")
            {
                Delete_Match_Popup.IsVisible = false;
                return;
            }
            if (_state == 0)
            {
                SetState(1);
            }
            else if (_state == 2)
            {
                Back_Popup.IsVisible = true;
            }
            else
            {
                SetState(0);
            }
        }

        private async void MatchButton_Clicked(object sender, EventArgs e)
        {
            Delete_Match_Popup.IsVisible = false;
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
                    if (ScouterName.Text != null)
                    {
                        if (ScouterName.Text.ToUpper() == "DELETE")
                        {
                            if (teamMatch != null)
                            {
                                DeleteMatchPopup();
                            }
                            return;
                        }
                    }
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
                    if (ScouterName.Text == null || ScouterName.Text.ToUpper() == "DELETE")
                    {
                        ScouterName.Text = teamMatch.ScouterName;
                    }
                    SaveAllFields(teamMatch);
                    try
                    {
                        _ = await App.Database.SaveTeamMatchAsync(teamMatch);
                    }
                    catch (Exception)
                    {
                        //todo add error message
                    }
                    ClearAllFields();
                    SetState(0);
                    break;
                default:
                    break;
            }
        }

        private void ClearAllFields()
        {
            Moved_Off_Start(0);
            Climbed_Button_Background(-1);
            Win_Tie_Lost_Button_Background(-1);
            Rating_Button_Background(-1);
            Auto_Lower_Hub_Output(0);
            Auto_Upper_Hub_Output(0);
            Human_Upper_Hub_Output(0);
            Human_Lower_Hub_Output(0);
            Teleop_Lower_Hub_Output(0);
            Teleop_Upper_Hub_Output(0);
            Comments.Text = "";
            ClimbRP_Output(0);
            CargoRP_Output(0);
        }

        private void FillAllFields(TeamMatch item)
        {
            ScouterName.Text = item.ScouterName;
            Moved_Off_Start(item.LeftTarmac);
            Climbed_Button_Background(item.ClimbLevel);
            Win_Tie_Lost_Button_Background(item.MatchRP);
            Rating_Button_Background(item.ScouterRating);
            Auto_Lower_Hub_Output(item.AutoLowGoals);
            Auto_Upper_Hub_Output(item.AutoHighGoals);
            Human_Upper_Hub_Output(item.HumanHighGoals);
            Human_Lower_Hub_Output(item.HumanLowGoals);
            Teleop_Lower_Hub_Output(item.TeleLowGoals);
            Teleop_Upper_Hub_Output(item.TeleHighGoals);
            Comments.Text = item.Comments;
            ClimbRP_Output(teamMatch.ClimbRP);
            CargoRP_Output(teamMatch.CargoRP);
        }

        private void SaveAllFields(TeamMatch item)
        {
            if (!string.IsNullOrWhiteSpace(ScouterName.Text))
            {
                item.ScouterName = ScouterName.Text;
            }
            item.Comments = Comments.Text;
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
                    Back_Popup.IsVisible = false;
                    Grid_Header.IsVisible = true;
                    frame.BackgroundColor = Color.FromHex("#008000");
                    Delete_Match_Password.Text = "";
                    Delete_Match_Popup.IsVisible = false;
                    break;
                case 1:
                    MatchEntryView.IsVisible = false;
                    MenuButton.Text = "Return"; // <-- \u25c0\u2013
                    MatchButton.IsVisible = false;
                    MatchMenuView.IsVisible = true;
                    MatchMenuView.IsEnabled = true;
                    Back_Popup.IsVisible = false;
                    Grid_Header.IsVisible = true;
                    frame.BackgroundColor = Color.FromHex("#008000");
                    break;
                case 2:
                    TeamNumber.IsEnabled = false;
                    MatchNumber.IsEnabled = false;
                    ScouterName.IsEnabled = true;
                    MenuButton.Text = "Cancel";
                    MatchEntryBody.IsVisible = true;
                    MatchEntryView.IsVisible = true;
                    MatchMenuView.IsVisible = false;
                    MatchMenuView.IsEnabled = false;
                    MatchButton.Text = "Save";
                    Back_Popup.IsVisible = false;
                    Grid_Header.IsVisible = true;
                    frame.BackgroundColor = Color.FromHex("#008000");
                    break;
                default:
                    break;
            }
            _state = stateNumber;
        }
        private void DeleteMatchPopup()
        {
            Delete_Match_Popup.IsVisible = true;
            ScouterName.Text = "";
            MenuButton.Text = "Back";
        }
        private async void Delete_All_Matches_Clicked(object sender, EventArgs e)
        {
            if (DeleteAllMatchesPassword.Text.ToLower() == deleteAllMatchesPassword)
            {
                List<TeamMatch> deleteMatches = await App.Database.GetTeamMatchesAsync();
                foreach (TeamMatch match in deleteMatches)
                {
                    teamMatch = await App.Database.GetTeamMatchAsync(match.TeamNumber, match.MatchNumber);
                    _ = await App.Database.DeleteTeamMatchAsync(teamMatch);
                }
                ResultsLabel.Text = "All matches deleted";
            }
            else if (DeleteAllMatchesPassword.Text.ToLower() == "hi")
            {
                ResultsLabel.Text = "hi";
            } else if (DeleteAllMatchesPassword.Text.ToLower() == "massimo")
            {
                Green_Mode();
            }
            else
            {
                ResultsLabel.Text = "Wrong password";
            }
            DeleteAllMatchesPassword.Text = "";
        }
        private void Green_Mode()
        {
            greenMode = !greenMode;
            if (greenMode)
            {
                darkGreenMode = darkMode;
                if (!darkGreenMode)
                {
                    frame.BackgroundColor = Color.FromHex("#008000");
                    LayoutBackground.BackgroundColor = Color.DarkGreen;
                    MatchEntryView.BackgroundColor = Color.DarkGreen;
                    title.TextColor = Color.LightGreen;
                    ScouterName.BackgroundColor = Color.LightGreen;
                    MatchNumber.BackgroundColor = Color.LightGreen;
                    TeamNumber.BackgroundColor = Color.LightGreen;
                    MatchMenuView.BackgroundColor = Color.DarkGreen;
                    ResultsLabel.Text = "Disclaimer: (dark)Green Mode was Massimo's idea";
                }
                else
                {
                    frame.BackgroundColor = Color.FromHex("#990099");
                    LayoutBackground.BackgroundColor = Color.FromHex("#220022");
                    MatchEntryView.BackgroundColor = Color.FromHex("#220022");
                    title.TextColor = Color.FromHex("#dd00dd");
                    ScouterName.BackgroundColor = Color.FromHex("#dd00dd");
                    MatchNumber.BackgroundColor = Color.FromHex("#dd00dd");
                    TeamNumber.BackgroundColor = Color.FromHex("#dd00dd");
                    MatchMenuView.BackgroundColor = Color.FromHex("#220022");
                    ResultsLabel.Text = "Disclaimer: Green Mode was Massimo's idea";
                }
            }
            else
            {
                LayoutBackground.BackgroundColor = Color.White;
                MatchEntryView.BackgroundColor = Color.White;
                title.TextColor = Color.White;
                ScouterName.BackgroundColor = Color.White;
                MatchNumber.BackgroundColor = Color.White;
                TeamNumber.BackgroundColor = Color.White;
                MatchMenuView.BackgroundColor = Color.White;
            }
        }
        private void Dark_Mode_Clicked(object sender, EventArgs e)
        {
            darkMode = !darkMode;
            if (greenMode)
            {
                greenMode = false;
                Green_Mode();
            }
            if (darkMode)
            {
                frame.BackgroundColor = Color.FromHex("#008000");
                DarkMode.BackgroundColor = Color.FromHex("#bfbfbf");
                LayoutBackground.BackgroundColor = Color.Black;
                title.TextColor = Color.Black;
                MatchEntryView.BackgroundColor = Color.Black;
                ScouterName.BackgroundColor = Color.FromHex("#eeeeee");
                MatchNumber.BackgroundColor = Color.FromHex("#eeeeee");
                TeamNumber.BackgroundColor = Color.FromHex("#eeeeee");
                MatchMenuView.BackgroundColor = Color.Black;
            }
            else
            {
                frame.BackgroundColor = Color.FromHex("#008000");
                DarkMode.BackgroundColor = Color.White;
                LayoutBackground.BackgroundColor = Color.White;
                MatchEntryView.BackgroundColor = Color.White;
                title.TextColor = Color.White;
                ScouterName.BackgroundColor = Color.White;
                MatchNumber.BackgroundColor = Color.White;
                TeamNumber.BackgroundColor = Color.White;
                MatchMenuView.BackgroundColor = Color.White;
            }
        }
        private void Verified_Back_Clicked(object sender, EventArgs e)
        {
            SetState(0);
        }
        private void Cancel_Back_Clicked(object sender, EventArgs e)
        {
            SetState(2);
        }
        private async void Delete_Match_Clicked(object sender, EventArgs e)
        {
            if (Delete_Match_Password.Text == deleteMatchPassword)
            {
                Climbed_Button_Background(-1);
                Win_Tie_Lost_Button_Background(-1);
                Rating_Button_Background(-1);
                Comments.Text = "";
                Delete_Match_Password.Text = "";
                Auto_Upper_Hub_Output(0);
                Auto_Lower_Hub_Output(0);
                Human_Upper_Hub_Output(0);
                Human_Lower_Hub_Output(0);
                Teleop_Upper_Hub_Output(0);
                Teleop_Lower_Hub_Output(0);
                Moved_Off_Start(0);
                ClimbRP_Output(0);
                CargoRP_Output(0);
                await App.Database.DeleteTeamMatchAsync(teamMatch);
                SetState(0);
            }
        }
        private void Moved_Off_Start_Clicked(object sender, EventArgs e)
        {
            teamMatch.LeftTarmac = (-teamMatch.LeftTarmac) + 1;
            Moved_Off_Start(teamMatch.LeftTarmac);
        }
        private void Moved_Off_Start(int value)
        {
            Moved_Off_Start_Button.Background = (value == 1) ? SelectedButtonColor : UnselectedButtonColor;
        }
        private void Auto_Lower_Hub_Plus_Clicked(object sender, EventArgs e)
        {
            teamMatch.AutoLowGoals++;
            if (teamMatch.AutoLowGoals < 0)
            {
                teamMatch.AutoLowGoals = 0;
            }
            Auto_Lower_Hub_Output(teamMatch.AutoLowGoals);
        }
        private void Auto_Lower_Hub_Minus_Clicked(object sender, EventArgs e)
        {
            teamMatch.AutoLowGoals--;
            if (teamMatch.AutoLowGoals < 0)
            {
                teamMatch.AutoLowGoals = 0;
            }
            Auto_Lower_Hub_Output(teamMatch.AutoLowGoals);
        }
        private void Auto_Lower_Hub_Output(int value)
        {
            Auto_Lower_Hub.Text = ("Lower Hub: " + value);
        }
        private void Auto_Upper_Hub_Plus_Clicked(object sender, EventArgs e)
        {
            teamMatch.AutoHighGoals++;
            if (teamMatch.AutoHighGoals < 0)
            {
                teamMatch.AutoHighGoals = 0;
            }
            Auto_Upper_Hub_Output(teamMatch.AutoHighGoals);
        }
        private void Auto_Upper_Hub_Minus_Clicked(object sender, EventArgs e)
        {
            teamMatch.AutoHighGoals--;
            if (teamMatch.AutoHighGoals < 0)
            {
                teamMatch.AutoHighGoals = 0;
            }
            Auto_Upper_Hub_Output(teamMatch.AutoHighGoals);
        }
        private void Auto_Upper_Hub_Output(int value)
        {
            Auto_Upper_Hub.Text = ("Upper Hub: " + value);
        }
        private void Human_Upper_Hub_Plus_Clicked(object sender, EventArgs e)
        {
            teamMatch.HumanHighGoals++;
            if (teamMatch.HumanHighGoals < 0)
            {
                teamMatch.HumanHighGoals = 0;
            }
            Human_Upper_Hub_Output(teamMatch.HumanHighGoals);
        }
        private void Human_Upper_Hub_Minus_Clicked(object sender, EventArgs e)
        {
            teamMatch.HumanHighGoals--;
            if (teamMatch.HumanHighGoals < 0)
            {
                teamMatch.HumanHighGoals = 0;
            }
            Human_Upper_Hub_Output(teamMatch.HumanHighGoals);
        }
        private void Human_Upper_Hub_Output(int value)
        {
            Human_Upper_Hub.Text = ("Upper Hub: " + value);
        }
        private void Human_Lower_Hub_Plus_Clicked(object sender, EventArgs e)
        {
            teamMatch.HumanLowGoals++;
            if (teamMatch.HumanLowGoals < 0)
            {
                teamMatch.HumanLowGoals = 0;
            }
            Human_Lower_Hub_Output(teamMatch.HumanLowGoals);
        }
        private void Human_Lower_Hub_Minus_Clicked(object sender, EventArgs e)
        {
            teamMatch.HumanLowGoals--;
            if (teamMatch.HumanLowGoals < 0)
            {
                teamMatch.HumanLowGoals = 0;
            }
            Human_Lower_Hub_Output(teamMatch.HumanLowGoals);
        }
        private void Human_Lower_Hub_Output(int value)
        {
            Human_Lower_Hub.Text = ("Lower Hub: " + value);
        }
        private void Teleop_Upper_Hub_Plus_Clicked(object sender, EventArgs e)
        {
            teamMatch.TeleHighGoals++;
            if (teamMatch.TeleHighGoals < 0)
            {
                teamMatch.TeleHighGoals = 0;
            }
            Teleop_Upper_Hub_Output(teamMatch.TeleHighGoals);
        }
        private void Teleop_Upper_Hub_Minus_Clicked(object sender, EventArgs e)
        {
            teamMatch.TeleHighGoals--;
            if (teamMatch.TeleHighGoals < 0)
            {
                teamMatch.TeleHighGoals = 0;
            }
            Teleop_Upper_Hub_Output(teamMatch.TeleHighGoals);
        }
        private void Teleop_Upper_Hub_Output(int value)
        {
            Teleop_Upper_Hub.Text = ("Upper Hub: " + value);
        }
        private void Teleop_Lower_Hub_Plus_Clicked(object sender, EventArgs e)
        {
            teamMatch.TeleLowGoals++;
            if (teamMatch.TeleLowGoals < 0)
            {
                teamMatch.TeleLowGoals = 0;
            }
            Teleop_Lower_Hub_Output(teamMatch.TeleLowGoals);
        }
        private void Teleop_Lower_Hub_Minus_Clicked(object sender, EventArgs e)
        {
            teamMatch.TeleLowGoals--;
            if (teamMatch.TeleLowGoals < 0)
            {
                teamMatch.TeleLowGoals = 0;
            }
            Teleop_Lower_Hub_Output(teamMatch.TeleLowGoals);
        }
        private void Teleop_Lower_Hub_Output(int value)
        {
            Teleop_Lower_Hub.Text = ("Lower Hub: " + value);
        }
        private void Climbed_None_Clicked(object sender, EventArgs e)
        {
            teamMatch.ClimbLevel = 0;
            Climbed_Button_Background(teamMatch.ClimbLevel);
        }
        private void Climbed1_Clicked(object sender, EventArgs e)
        {
            teamMatch.ClimbLevel = 1;
            Climbed_Button_Background(teamMatch.ClimbLevel);
        }

        private void Climbed2_Clicked(object sender, EventArgs e)
        {
            teamMatch.ClimbLevel = 2;
            Climbed_Button_Background(teamMatch.ClimbLevel);
        }

        private void Climbed3_Clicked(object sender, EventArgs e)
        {
            teamMatch.ClimbLevel = 3;
            Climbed_Button_Background(teamMatch.ClimbLevel);
        }

        private void Climbed4_Clicked(object sender, EventArgs e)
        {
            teamMatch.ClimbLevel = 4;
            Climbed_Button_Background(teamMatch.ClimbLevel);
        }
        private void Climbed_Button_Background(int value)
        {
            Climbed0.Background = (value == 0) ? SelectedButtonColor : UnselectedButtonColor;
            Climbed1.Background = (value == 1) ? SelectedButtonColor : UnselectedButtonColor;
            Climbed2.Background = (value == 2) ? SelectedButtonColor : UnselectedButtonColor;
            Climbed3.Background = (value == 3) ? SelectedButtonColor : UnselectedButtonColor;
            Climbed4.Background = (value == 4) ? SelectedButtonColor : UnselectedButtonColor;
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
        private void CargoRP_Clicked(object sender, EventArgs e)
        {
            teamMatch.CargoRP = -(teamMatch.CargoRP) + 1;
            CargoRP_Output(teamMatch.CargoRP);
        }
        private void CargoRP_Output(int value)
        {
            CargoRP.Background = (teamMatch.CargoRP == 1) ? SelectedButtonColor : UnselectedButtonColor;
        }
        private void ClimbRP_Clicked(object sender, EventArgs e)
        {
            teamMatch.ClimbRP = -(teamMatch.ClimbRP) + 1;
            ClimbRP_Output(teamMatch.ClimbRP);
        }
        private void ClimbRP_Output(int value)
        {
            ClimbRP.Background = (teamMatch.ClimbRP == 1) ? SelectedButtonColor : UnselectedButtonColor;
        }
        private async void Button_SendToAirtable(object sender, EventArgs e)
        {
            string result;
            List<TeamMatch> matches = await App.Database.GetTeamMatchesAsync();
            try
            {
                result = await AirtableDB.AirtableSendRecords(matches);
            }
            catch (Exception ex)
            {
                ResultsLabel.Text = ex.Message + "\n" + ex.InnerException.Message;
                return;
            }
            ResultsLabel.Text = result; // show everything sent
            foreach (TeamMatch match in matches)
            {
                match.Changed = false;
                await App.Database.SaveTeamMatchAsync(match);
            }
        }

        private async void Button_ShowMatches(object sender, EventArgs e)
        {
            StringBuilder result = new StringBuilder();
            List<TeamMatch> matches = await App.Database.GetTeamMatchesAsync();
            SortedList<string, TeamMatch> sorted = new SortedList<string, TeamMatch>();
            foreach (TeamMatch match in matches)
            {
                sorted.Add($"{match.MatchNumber:000}-{match.TeamNumber:0000}", match);
            }
            foreach (TeamMatch match in sorted.Values)
            {
                string sent = match.AirtableId == null ? "" : "* ";
                result.AppendLine($"{sent}Match: {match.MatchNumber,3} - Team: {match.TeamNumber,4} - Scouter: {match.ScouterName}");
            }
            ResultsLabel.Text = result.ToString();
        }
    }
}
