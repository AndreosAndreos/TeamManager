using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Data.Linq;

namespace TeamManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["TeamManager.Properties.Settings.TeamManagerConnectionString"].ConnectionString;
        
        DataClasses1DataContext dataContext;

        private string currentButton = string.Empty;

        private object selectedRow;

        public MainWindow()
        {
            InitializeComponent();
            DataBaseConnect();
            
            dataContext = new DataClasses1DataContext(connectionString);
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

        #region ButtonModification
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            switch (currentButton)
            {
                case "team":
                    Window1 window1 = new Window1(currentButton, null);
                    window1.Show();
                    break;

                case "player":
                    Window2 window2 = new Window2(currentButton, null);
                    window2.Show();
                    break;

                case "match":
                    Window3 window3 = new Window3(currentButton, null);
                    window3.Show();
                    break;

                case "stat":
                    Window4 window4 = new Window4(currentButton, null);
                    window4.Show();
                    break;

                default:
                    MessageBox.Show("Choose a table");
                    break;
            }
        }
        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectedRow != null)
                {
                    if (currentButton == "team")
                    {
                        var selectedRecord = selectedRow as dynamic;
                        int selectedRecordId = selectedRecord.ID;

                        Window1 window1 = new Window1(currentButton,selectedRecordId);
                        window1.Show();
                    }
                    else if (currentButton == "player")
                    {
                        var selectedRecord = selectedRow as dynamic;
                        int selectedRecordId = selectedRecord.ID;

                        Window2 window2 = new Window2(currentButton, selectedRecordId);
                        window2.Show();
                    }
                    if (currentButton == "match")
                    {
                        var selectedRecord = selectedRow as dynamic;
                        int selectedRecordId = selectedRecord.ID;

                        Window3 window3 = new Window3(currentButton, selectedRecordId);
                        window3.Show();
                    }
                    if (currentButton == "stat")
                    {
                        var selectedRecord = selectedRow as dynamic;
                        int selectedRecordId = selectedRecord.ID;

                        Window4 window4 = new Window4(currentButton, selectedRecordId);
                        window4.Show();
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
                MessageBox.Show($"Something went wrong: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectedRow != null)
                {
                    var confirmResult = MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (confirmResult == MessageBoxResult.Yes)
                    {
                        if (currentButton == "team")
                        {
                            var selectedRecord = selectedRow as dynamic;
                            int selectedRecordId = selectedRecord.ID;

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
                            var selectedRecord = selectedRow as dynamic;
                            int selectedRecordId = selectedRecord.ID;

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
                            var selectedRecord = selectedRow as dynamic;
                            int selectedRecordId = selectedRecord.ID;

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
                            var selectedRecord = selectedRow as dynamic;
                            int selectedRecordId = selectedRecord.ID;

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
