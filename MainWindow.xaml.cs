﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Win32;
using System.IO;
using System.Globalization;
using System.Windows.Markup;
using CsvHelper;

namespace TeamManager
{
    /// <summary>
    /// The program main window with DataGrid for presenting data stored in the database as well as buttons transfering to diferent CRUD windows
    /// </summary>
    public partial class MainWindow : Window
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["TeamManager.Properties.Settings.TeamManagerConnectionString"].ConnectionString;
        
        DataClasses1DataContext dataContext;

        private string currentButton = string.Empty;

        private object selectedRow;

        public MainWindow(int userRole)
        {
            InitializeComponent();
            DataBaseConnect();
            
            dataContext = new DataClasses1DataContext(connectionString);

            RoleUsableButtons(userRole);

            //MessageBox.Show($"Data can be sorted by pressing the column name", "Information", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public void DataBaseConnect()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                }
                catch (SqlException sqlex)
                {
                    MessageBox.Show($"There's been a problem with establishing the database connection {sqlex.Message}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        public void RoleUsableButtons(int userRole)
        {
            if (userRole == 2) // user
            {
                BtnAdd.Visibility = Visibility.Hidden;
                BtnDelete.Visibility = Visibility.Hidden;
                BtnUpdate.Visibility = Visibility.Hidden;
            }
        }

        #region SetUp
        /*
        private void TxtBoxTeam_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TxtBoxTeam.Text == "Enter team name")
            {
                TxtBoxTeam.Text = "";
                TxtBoxTeam.Foreground = Brushes.Black;
            }
        }

        private void TxtBoxTeam_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtBoxTeam.Text))
            {
                TxtBoxTeam.Text = "Enter team name";
                TxtBoxTeam.Foreground = Brushes.Gray;
            }
        }
        */
        #endregion

        #region ButtonShow
        private void BtnTeam_Click(object sender, RoutedEventArgs e)
        {
            currentButton = "team";
            ShowTeams();
        }

        private void BtnPlayer_Click(object sender, RoutedEventArgs e)
        {
            currentButton = "player";
            ShowPlayers();
        }

        private void BtnMatch_Click(object sender, RoutedEventArgs e)
        {
            currentButton = "match";
            ShowMatches();
        }

        private void BtnStat_Click(object sender, RoutedEventArgs e)
        {
            currentButton = "stat";
            ShowStats();
        }
        #endregion
        private void BtnExportCSV_Click(object sender, RoutedEventArgs e)
        {
            if(currentButton == string.Empty)
            {
                MessageBox.Show("Please select a table.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                FileName = "output.csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (var writer = new StreamWriter(saveFileDialog.FileName))
                    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        csv.WriteRecords(DataGrid.ItemsSource);
                    }

                    MessageBox.Show("Expport succesful.","Information",MessageBoxButton.OK,MessageBoxImage.Information);
                }
                catch(Exception)
                {
                    MessageBox.Show("There's been an error in writing the csv file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
        }
        #region ButtonModification
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            switch (currentButton)
            {
                case "team":
                    Window1 window1 = new Window1(currentButton, null);
                    window1.Show();
                    window1.Closed += (s, args) => ShowTeams();
                    break;

                case "player":
                    Window2 window2 = new Window2(currentButton, null);
                    window2.Show();
                    window2.Closed += (s, args) => ShowPlayers();
                    break;

                case "match":
                    Window3 window3 = new Window3(currentButton, null);
                    window3.Show();
                    window3.Closed += (s, args) => ShowMatches();
                    break;

                case "stat":
                    Window4 window4 = new Window4(currentButton, null);
                    window4.Show();
                    window4.Closed += (s, args) => ShowStats();
                    break;

                default:
                    MessageBox.Show("Choose a table");
                    break;
            }
        }
        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (currentButton == string.Empty || selectedRow == null)
            {
                MessageBox.Show("Select a table and a record to update.", "Record not selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                var selectedRecord = selectedRow as dynamic;
                int selectedRecordId = selectedRecord.ID;

                if (selectedRow != null)
                {
                    if (currentButton == "team")
                    {
                        Window1 window1 = new Window1(currentButton,selectedRecordId);
                        window1.Show();
                        window1.Closed += (s, args) => ShowTeams();                     // reakcja na zamkniecie okna (WYMAGA SUBSKRYBOWANIA I OBSLUGI PRZEZ DELEGATA)
                    }
                    else if (currentButton == "player")
                    {
                        Window2 window2 = new Window2(currentButton, selectedRecordId);
                        window2.Show();
                        window2.Closed += (s, args) => ShowPlayers();
                    }
                    if (currentButton == "match")
                    {
                        Window3 window3 = new Window3(currentButton, selectedRecordId);
                        window3.Show();
                        window3.Closed += (s, args) => ShowMatches();
                    }
                    if (currentButton == "stat")
                    {
                        Window4 window4 = new Window4(currentButton, selectedRecordId);
                        window4.Show();
                        window4.Closed += (s, args) => ShowStats();
                    }
                }
                else
                {
                    MessageBox.Show("Select a record to update.", "Record not selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (SqlException)
            {
                MessageBox.Show("Couldn't connect to the database", "Connection error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Something went wrong: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (currentButton == string.Empty || selectedRow == null)
            {
                MessageBox.Show("Select a table and a record to update.", "Record not selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                var selectedRecord = selectedRow as dynamic;
                int selectedRecordId = selectedRecord.ID;

                if (selectedRow != null)
                {
                    var confirmResult = MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (confirmResult == MessageBoxResult.Yes)
                    {
                        if (currentButton == "team")
                        {
                            var teamToDelete = dataContext.Teams.SingleOrDefault(t => t.ID == selectedRecordId);
                            if (teamToDelete != null)
                            {
                                dataContext.Teams.DeleteOnSubmit(teamToDelete);
                                dataContext.SubmitChanges();

                                ShowTeams();
                            }
                        }
                        else if (currentButton == "player")
                        {
                            var playerToDelete = dataContext.Players.SingleOrDefault(p => p.ID == selectedRecordId);
                            if (playerToDelete != null)
                            {
                                dataContext.Players.DeleteOnSubmit(playerToDelete);
                                dataContext.SubmitChanges();

                                ShowPlayers();
                            }
                        }
                        else if (currentButton == "match")
                        {
                            var matchToDelete = dataContext.Matches.SingleOrDefault(p => p.ID == selectedRecordId);
                            if (matchToDelete != null)
                            {
                                dataContext.Matches.DeleteOnSubmit(matchToDelete);
                                dataContext.SubmitChanges();

                                ShowMatches();
                            }
                        }
                        else if (currentButton == "stat")
                        {
                            var statToDelete = dataContext.Statistics.SingleOrDefault(p => p.ID == selectedRecordId);
                            if (statToDelete != null)
                            {
                                dataContext.Statistics.DeleteOnSubmit(statToDelete);
                                dataContext.SubmitChanges();

                                ShowStats();
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Select a record to delete.", "Record not selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (SqlException)
            {
                MessageBox.Show("Couldn't connect to the database", "Connection error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Something went wrong: {ex.Message}","Error",MessageBoxButton.OK,MessageBoxImage.Warning);
            }
        }
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedRow = DataGrid.SelectedItem;
        }

        #endregion

        #region Methods
        private void ShowTeams()
        {
            /*                                                      NO LINQ
            try
            {
                string query = "SELECT * FROM dbo.team ";

                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, connection))
                {

                    DataTable teamTable = new DataTable();
                    sqlDataAdapter.Fill(teamTable);

                    DataGrid.ItemsSource = teamTable.DefaultView;
                }
            }
            */
            try
            {
                var teams = from t in dataContext.Teams                                 // LINQ
                            select new
                            {
                                ID = t.ID,
                                Name = t.Name,
                                Coach = t.Coach,
                                Founded = t.Founded_date,
                                League = t.League,
                                HomeTown = t.Home_town,
                            };

                DataGrid.ItemsSource = teams.ToList();

                foreach (var column in DataGrid.Columns)
                {
                    column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
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

        private void ShowPlayers()
        {
            try
            {
                var players = from p in dataContext.Players
                              join t in dataContext.Teams on p.Team_id equals t.ID
                              select new
                            {
                                ID = p.ID,
                                Name = p.Name,
                                Team = p.Team.Name,
                                Position = p.Position,
                                BithDate = p.Date_of_birth,
                                Nationality = p.Nationality,
                                Salary = p.Salary,
                                ContractStart = p.Contract_start_date,
                                ContractEnd = p.Contract_end_date
                            };

                DataGrid.ItemsSource = players.ToList();

                foreach (var column in DataGrid.Columns)
                {
                    column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
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
        private void ShowMatches()
        {
            try
            {
                var matches = from m in dataContext.Matches
                              join t in dataContext.Teams on m.Team_away_id equals t.ID 
                            select new
                            {
                                ID = m.ID,
                                HomeTeam = m.Team.Name,
                                AwayTeam = m.Team1.Name,
                                Date = m.Match_date,
                                Location = m.Location,
                                ScoreHome = m.Score_home,
                                ScoreAway = m.Score_away,
                            };

                DataGrid.ItemsSource = matches.ToList();

                foreach (var column in DataGrid.Columns)
                {
                    column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
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

        private void ShowStats()
        {
            try
            {
                var stats = from s in dataContext.Statistics
                              join m in dataContext.Matches on s.Match_id equals m.ID
                              join p in dataContext.Players on s.Player_id equals p.ID
                            select new
                                {
                                    ID = s.ID,
                                    Match_id = s.Match.ID,
                                    Player = s.Player.Name,
                                    Team = s.Player.Team.Name,
                                    Goals = s.Goals,
                                    Assists = s.Assists,
                                    Yellow_cards = s.Yellow_cards,
                                    Red_carts = s.Red_cards,
                                    Minutes = s.Minutes_played,
                                };

                DataGrid.ItemsSource = stats.ToList();

                foreach (var column in DataGrid.Columns)
                {
                    column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
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
        #endregion
    }
}
