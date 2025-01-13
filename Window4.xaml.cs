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
    /// Interaction logic for Window4.xaml
    /// </summary>
    public partial class Window4 : Window
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["TeamManager.Properties.Settings.TeamManagerConnectionString"].ConnectionString;

        DataClasses1DataContext dataContext;

        private int? statId;
        public Window4(string currentButton, int? id)
        {
            InitializeComponent();
            dataContext = new DataClasses1DataContext(connectionString);

            if (id != null)
            {
                statId = id;
                ShowStat(id.Value);
            }
            CmbBoxMatchData();
            CmbBoxPlayerData();
        }
        private void CmbBoxMatchData()
        {
            try
            {
                var matches = dataContext.Matches
                               .Select(t => new { t.ID })
                               .ToList();

                if (statId != null)
                {
                    var stat = dataContext.Statistics.SingleOrDefault(m => m.ID == statId.Value);
                    if (stat != null)
                    {
                        var selectedStat = matches.SingleOrDefault(t => t.ID == stat.Match_id);
                        if (selectedStat != null)
                        {
                            CmbBoxMatch.SelectedValue = selectedStat.ID;
                        }
                    }
                }
                CmbBoxMatch.ItemsSource = matches;
                CmbBoxMatch.DisplayMemberPath = "ID";
                CmbBoxMatch.SelectedValuePath = "ID";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading match data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void CmbBoxPlayerData()
        {
            try
            {
                var players = dataContext.Players
                               .Select(t => new { t.ID,t.Name })
                               .ToList();

                if (statId != null)
                {
                    var stat = dataContext.Statistics.SingleOrDefault(m => m.ID == statId.Value);
                    if (stat != null)
                    {
                        var selectedStat = players.SingleOrDefault(t => t.ID == stat.Player_id);
                        if (selectedStat != null)
                        {
                            CmbBoxPlayer.SelectedValue = selectedStat.ID;
                        }
                    }
                }
                CmbBoxPlayer.ItemsSource = players;
                CmbBoxPlayer.DisplayMemberPath = "Name";
                CmbBoxPlayer.SelectedValuePath = "ID";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading player data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowStat(int id)
        {
            try
            {
                var statUpdate = dataContext.Statistics.SingleOrDefault(stat => stat.ID == id);

                if (statUpdate != null)
                {
                    CmbBoxMatch.SelectedItem = statUpdate.Match_id;
                    CmbBoxPlayer.SelectedItem = statUpdate.Player_id;
                    TxtBoxData02.Text = statUpdate.Goals.ToString();
                    TxtBoxData10.Text = statUpdate.Assists.ToString();
                    TxtBoxData11.Text = statUpdate.Yellow_cards.ToString();
                    TxtBoxData12.Text = statUpdate.Red_cards.ToString();
                    TxtBoxData20.Text = statUpdate.Minutes_played.ToString();
                }
                else
                {
                    MessageBox.Show("Stat not found for the provided ID.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
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

            Regex regexDecimal = new Regex(@"^\d+(\.\d{2})?$");
            Regex regexInt = new Regex("^[0-9]+$");

            if (!regexInt.IsMatch(TxtBoxData02.Text) || !regexInt.IsMatch(TxtBoxData10.Text) || !regexInt.IsMatch(TxtBoxData11.Text) || !regexInt.IsMatch(TxtBoxData12.Text))
            {
                MessageBox.Show($"It can only be a number", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if(!regexDecimal.IsMatch(TxtBoxData20.Text))
            {
                MessageBox.Show($"Enter time in format MM,SS", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            decimal totalMinutesPlayed = Convert.ToDecimal(TxtBoxData20.Text);

            if (totalMinutesPlayed > 120.00m || totalMinutesPlayed < 0)
            {
                MessageBox.Show($"The total minutes played exceded", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (statId == null)
            {
                try
                {
                    Statistic insertStatistic = new Statistic();

                    insertStatistic.Match_id = (int)CmbBoxMatch.SelectedValue;
                    insertStatistic.Player_id = (int)CmbBoxPlayer.SelectedValue;
                    insertStatistic.Goals = Convert.ToInt32(TxtBoxData02.Text);
                    insertStatistic.Assists = Convert.ToInt32(TxtBoxData10.Text);
                    insertStatistic.Yellow_cards = Convert.ToInt32(TxtBoxData11.Text);
                    insertStatistic.Red_cards = Convert.ToInt32(TxtBoxData12.Text);
                    insertStatistic.Minutes_played = totalMinutesPlayed;

                    dataContext.Statistics.InsertOnSubmit(insertStatistic);
                    dataContext.SubmitChanges();

                    MessageBox.Show("Stat added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

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
            else if (statId != null)
            {
                try
                {
                    var insertStatistic = dataContext.Statistics.First(t => t.ID == statId);
                    if (insertStatistic != null)
                    {
                        insertStatistic.Match_id = (int)CmbBoxMatch.SelectedValue;
                        insertStatistic.Player_id = (int)CmbBoxPlayer.SelectedValue;
                        insertStatistic.Goals = Convert.ToInt32(TxtBoxData02.Text);
                        insertStatistic.Assists = Convert.ToInt32(TxtBoxData10.Text);
                        insertStatistic.Yellow_cards = Convert.ToInt32(TxtBoxData11.Text);
                        insertStatistic.Red_cards = Convert.ToInt32(TxtBoxData12.Text);
                        insertStatistic.Minutes_played = totalMinutesPlayed;

                        dataContext.SubmitChanges();

                        MessageBox.Show("Stat updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Stat not found for the provided ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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
