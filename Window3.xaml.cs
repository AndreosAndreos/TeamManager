using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TeamManager
{
    /// <summary>
    /// Window for performig matches insertions and updating the selected row in MainWindow
    /// </summary>
    public partial class Window3 : Window
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["TeamManager.Properties.Settings.TeamManagerConnectionString"].ConnectionString;

        DataClasses1DataContext dataContext;

        private int? matchId;
        public Window3(string currentButton, int? id)
        {
            InitializeComponent();

            dataContext = new DataClasses1DataContext(connectionString);

            if (id != null)
            {
                matchId = id;
                ShowMatch(id.Value);
            }
            CmbBoxTeamHomeData();
            CmbBoxTeamAwayData();
        }

        private void CmbBoxTeamHomeData()
        {
            try
            {
                var teams = dataContext.Teams
                               .Select(t => new { t.ID, t.Name })
                               .ToList();

                if (matchId != null)
                {
                    var match = dataContext.Matches.SingleOrDefault(p => p.ID == matchId.Value);
                    if (match != null)
                    {
                        var selectedMatch = teams.SingleOrDefault(t => t.ID == match.Team_home_id);
                        if (selectedMatch != null)
                        {
                            CmbBoxTeamHome.SelectedValue = selectedMatch.ID;
                        }
                    }
                }
                CmbBoxTeamHome.ItemsSource = teams;
                CmbBoxTeamHome.DisplayMemberPath = "Name";
                CmbBoxTeamHome.SelectedValuePath = "ID";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading team data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void CmbBoxTeamAwayData()
        {
            try
            {
                var teams = dataContext.Teams
                               .Select(t => new { t.ID, t.Name })
                               .ToList();

                if (matchId != null)
                {
                    var match = dataContext.Matches.SingleOrDefault(p => p.ID == matchId.Value);
                    if (match != null)
                    {
                        var selectedMatch = teams.SingleOrDefault(t => t.ID == match.Team_away_id);
                        if (selectedMatch != null)
                        {
                            CmbBoxTeamAway.SelectedValue = selectedMatch.ID;
                        }
                    }
                }
                CmbBoxTeamAway.ItemsSource = teams;
                CmbBoxTeamAway.DisplayMemberPath = "Name";
                CmbBoxTeamAway.SelectedValuePath = "ID";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading team data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ShowMatch(int id)
        {
            try
            {
                var matchUpdate = dataContext.Matches.SingleOrDefault(match => match.ID == id);

                if (matchUpdate != null)
                {
                    CmbBoxTeamHome.SelectedItem = matchUpdate.Team_home_id;
                    CmbBoxTeamAway.SelectedItem = matchUpdate.Team_away_id;
                    DatePickerMatch.SelectedDate = matchUpdate.Match_date;
                    TxtBoxData10.Text = matchUpdate.Location;
                    TxtBoxData11.Text = matchUpdate.Score_home.ToString();
                    TxtBoxData12.Text = matchUpdate.Score_away.ToString();
                }
                else
                {
                    MessageBox.Show("Match not found for the provided ID.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Couldn't connect to database: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Something went wrong: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Regex regex = new Regex(@"^[a-zA-Z\s]+$");
            Regex regexData = new Regex(@"^\d{4}-\d{2}-\d{2}$");
            Regex regexDecimal = new Regex(@"^\d+(\.\d{2})?$");
            Regex regexInt = new Regex("^[0-9]+$");

            if (DatePickerMatch.SelectedDate == null)
            {
                MessageBox.Show($"Please select a valid date.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!regex.IsMatch(TxtBoxData10.Text))
            {
                MessageBox.Show($"Only letters are accepted", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!regexInt.IsMatch(TxtBoxData11.Text) || !regexInt.IsMatch(TxtBoxData12.Text))
            {
                MessageBox.Show($"Score can only be a number", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if(CmbBoxTeamHome.SelectedIndex == CmbBoxTeamAway.SelectedIndex)
            {
                MessageBox.Show($"The team cannot play against itself", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int scoreHome = Convert.ToInt32(TxtBoxData11.Text);
            int scoreAway = Convert.ToInt32(TxtBoxData12.Text);
            if (scoreHome < 0 || scoreAway < 0)
            {
                MessageBox.Show($"Invalid score number", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (matchId == null)
            {
                try
                {
                    Match insertMatch = new Match();

                    insertMatch.Team_home_id = (int)CmbBoxTeamHome.SelectedValue;
                    insertMatch.Team_away_id = (int)CmbBoxTeamAway.SelectedValue;
                    insertMatch.Match_date = DatePickerMatch.SelectedDate.Value;
                    insertMatch.Location = TxtBoxData10.Text;
                    insertMatch.Score_home = scoreHome;
                    insertMatch.Score_away = scoreAway;

                    dataContext.Matches.InsertOnSubmit(insertMatch);
                    dataContext.SubmitChanges();

                    MessageBox.Show("Match added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    this.Close();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Couldn't connect to database: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Something went wrong: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else if (matchId != null)
            {
                try
                {
                    var insertMatch = dataContext.Matches.First(t => t.ID == matchId);
                    if (insertMatch != null)
                    {
                        insertMatch.Team_home_id = (int)CmbBoxTeamHome.SelectedValue;
                        insertMatch.Team_away_id = (int)CmbBoxTeamAway.SelectedValue;
                        insertMatch.Match_date = DatePickerMatch.SelectedDate.Value;
                        insertMatch.Location = TxtBoxData10.Text;
                        insertMatch.Score_home = Convert.ToInt32(TxtBoxData11.Text);
                        insertMatch.Score_away = Convert.ToInt32(TxtBoxData12.Text);

                        dataContext.SubmitChanges();

                        MessageBox.Show("Match updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Player not found for the provided ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Couldn't connect to database: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Something went wrong: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void BtnAbort_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
