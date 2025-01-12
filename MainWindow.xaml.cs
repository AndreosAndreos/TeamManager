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
        }

        public static void DataBaseConnect()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["TeamManager.Properties.Settings.TeamManagerConnectionString"].ConnectionString;

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
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }

        }

        #region SetUp

        private void MainWindowSetup(object sender, EventArgs e)
        {

        }
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

        private void BtnTeam_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnPlayer_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnMatch_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnStat_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            Window1 window1 = new Window1();
            window1.Show();
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
