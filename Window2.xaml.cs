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
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["TeamManager.Properties.Settings.TeamManagerConnectionString"].ConnectionString;

        DataClasses1DataContext dataContext;

        private int? playerId;
        public Window2(string currentButton, int? id)
        {
            InitializeComponent();
            
            dataContext = new DataClasses1DataContext(connectionString);

            if (id != null)
            {
                playerId = id;
                ShowPlayer(id.Value);
            }
            CmbBoxTeamData();
        }

        private void CmbBoxTeamData()
        {
            try
            {
                var teams = dataContext.Teams
                               .Select(t => new { t.ID, t.Name })
                               .ToList();

                if (playerId != null)
                {
                    var player = dataContext.Players.SingleOrDefault(p => p.ID == playerId.Value);
                    if (player != null)
                    {
                        var selectedTeam = teams.SingleOrDefault(t => t.ID == player.Team_id);
                        if (selectedTeam != null)
                        {
                            CmbBoxTeam.SelectedValue = selectedTeam.ID;
                        }
                    }
                }
                CmbBoxTeam.ItemsSource = teams;
                CmbBoxTeam.DisplayMemberPath = "Name";
                CmbBoxTeam.SelectedValuePath = "ID";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading team data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowPlayer(int id)
        {
            try
            {
                var playerUpdate = dataContext.Players.SingleOrDefault(player => player.ID == id);

                if (playerUpdate != null)
                {
                    TxtBoxData00.Text = playerUpdate.Name;
                    TxtBoxData01.Text = playerUpdate.Position;
                    DatePickerBirth.SelectedDate = playerUpdate.Date_of_birth;
                    TxtBoxData10.Text = playerUpdate.Nationality;
                    TxtBoxData11.Text = playerUpdate.Salary.ToString();
                    DatePickerContractStart.SelectedDate = playerUpdate.Contract_start_date;
                    DatePickerContractEnd.SelectedDate = playerUpdate.Contract_end_date;
                }
                else
                {
                    MessageBox.Show("Player not found for the provided ID.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
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

            if (DatePickerBirth.SelectedDate == null || DatePickerContractStart.SelectedDate == null || DatePickerContractEnd.SelectedDate == null)
            {
                MessageBox.Show($"Please select a valid date.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!regex.IsMatch(TxtBoxData00.Text) || !regex.IsMatch(TxtBoxData01.Text) || !regex.IsMatch(TxtBoxData10.Text))
            {
                MessageBox.Show($"Only letters are accepted", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if(!regexDecimal.IsMatch(TxtBoxData11.Text))
            {
                MessageBox.Show($"Only number are accepted in salary box", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if(DatePickerContractStart.SelectedDate > DatePickerContractEnd.SelectedDate)
            { 
                MessageBox.Show($"Contract start date cannot be newer than contract end date", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (playerId == null)
            {
                try
                {
                    Player insertPlayer = new Player();

                    insertPlayer.Team_id = (int)CmbBoxTeam.SelectedValue;
                    insertPlayer.Name = TxtBoxData00.Text;
                    insertPlayer.Position = TxtBoxData01.Text;
                    insertPlayer.Date_of_birth =  DatePickerBirth.SelectedDate.Value;
                    insertPlayer.Nationality = TxtBoxData10.Text;
                    insertPlayer.Salary = Convert.ToDecimal(TxtBoxData11.Text);
                    insertPlayer.Contract_start_date = DatePickerContractStart.SelectedDate.Value;
                    insertPlayer.Contract_end_date = DatePickerContractEnd.SelectedDate.Value;

                    dataContext.Players.InsertOnSubmit(insertPlayer);
                    dataContext.SubmitChanges();

                    MessageBox.Show("Player added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

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
            else if (playerId != null)
            {
                try
                {
                    var insertPlayer = dataContext.Players.First(t => t.ID == playerId);
                    if (insertPlayer != null)
                    {
                        insertPlayer.Team_id = (int)CmbBoxTeam.SelectedValue;
                        insertPlayer.Name = TxtBoxData00.Text;
                        insertPlayer.Position = TxtBoxData01.Text;
                        insertPlayer.Date_of_birth = DatePickerBirth.SelectedDate.Value;
                        insertPlayer.Nationality = TxtBoxData10.Text;
                        insertPlayer.Salary = Convert.ToDecimal(TxtBoxData11.Text);
                        insertPlayer.Contract_start_date = DatePickerContractStart.SelectedDate.Value;
                        insertPlayer.Contract_end_date = DatePickerContractEnd.SelectedDate.Value;

                        dataContext.SubmitChanges();

                        MessageBox.Show("Player updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

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
