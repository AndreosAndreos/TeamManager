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
    }
}
