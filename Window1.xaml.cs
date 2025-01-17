﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
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
    /// Window for performig team insertions and updating the selected row in MainWindow
    /// </summary>
    public partial class Window1 : Window
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["TeamManager.Properties.Settings.TeamManagerConnectionString"].ConnectionString;

        DataClasses1DataContext dataContext;

        private int? teamId;

        public Window1(string currentButton, int? id)
        {
            InitializeComponent();
            
            dataContext = new DataClasses1DataContext(connectionString);

            if (id != null)
            {
                teamId = id;
                ShowTeam(id.Value);
            }
        }

        private void ShowTeam(int id)
        {
            try
            {
                var teamUpdate = dataContext.Teams.SingleOrDefault(team => team.ID == id);

                if (teamUpdate != null)
                {
                    TxtBoxData00.Text = teamUpdate.Name;
                    TxtBoxData01.Text = teamUpdate.Coach;
                    DatePickerFounded.SelectedDate = teamUpdate.Founded_date;
                    TxtBoxData10.Text = teamUpdate.League;
                    TxtBoxData11.Text = teamUpdate.Home_town;
                }
                else
                {
                    MessageBox.Show("Team not found for the provided ID.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
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

            if (DatePickerFounded.SelectedDate == null)
            {
                MessageBox.Show($"Please select a valid date.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!regex.IsMatch(TxtBoxData00.Text) || !regex.IsMatch(TxtBoxData01.Text) || !regex.IsMatch(TxtBoxData10.Text) || !regex.IsMatch(TxtBoxData11.Text))
            {
                MessageBox.Show($"Only letters are accepted", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (teamId == null)
            {
                try
                {
                    Team insertTeam = new Team();
                    
                    insertTeam.Name = TxtBoxData00.Text;
                    insertTeam.Coach = TxtBoxData01.Text;
                    insertTeam.Founded_date = DatePickerFounded.SelectedDate.Value;
                    insertTeam.League = TxtBoxData10.Text;
                    insertTeam.Home_town = TxtBoxData11.Text;

                    dataContext.Teams.InsertOnSubmit(insertTeam);
                    dataContext.SubmitChanges();

                    MessageBox.Show("Team added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

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
            else if (teamId != null)
            {
                try
                {
                    var teamUpdate = dataContext.Teams.First(t => t.ID == teamId);
                    if (teamUpdate != null)
                    {
                        teamUpdate.Name = TxtBoxData00.Text;
                        teamUpdate.Coach = TxtBoxData01.Text;
                        teamUpdate.Founded_date = DatePickerFounded.SelectedDate.Value;
                        teamUpdate.League = TxtBoxData10.Text;
                        teamUpdate.Home_town = TxtBoxData11.Text;

                        dataContext.SubmitChanges();

                        MessageBox.Show("Team updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Team not found for the provided ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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
