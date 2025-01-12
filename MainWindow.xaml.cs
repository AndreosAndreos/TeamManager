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

namespace TeamManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection con;

        public MainWindow()
        {
            InitializeComponent();
            DataBaseConnect();
            

<<<<<<< HEAD
=======
        }

        public static void DataBaseConnect()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["TeamManager.Properties.Settings.TeamManagerConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MessageBox.Show("The connection to database has been established.");

                }
                catch (SqlException sqlex) 
                {
                    MessageBox.Show($"There's been a problem with establishing the database connection {sqlex.Message}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
>>>>>>> 1987759f2a89d64f131ec859095436be61a41a70
        }

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

        public static void DataBaseConnect()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["TeamManager.Properties.Settings.TeamManagerConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MessageBox.Show("The connection to database has been established.");

                }
                catch (SqlException sqlex) 
                {
                    MessageBox.Show($"There's been a problem with establishing the database connection {sqlex.Message}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

        private void BtnTeam_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
