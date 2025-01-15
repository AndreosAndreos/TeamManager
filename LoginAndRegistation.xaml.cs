using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace TeamManager
{
    /// <summary>
    /// Login and registration window
    /// </summary>
    public partial class LoginAndRegistation : Window
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["TeamManager.Properties.Settings.TeamManagerConnectionString"].ConnectionString;

        public LoginAndRegistation()
        {
            InitializeComponent();
        }

        private void BtnEnter_Click(object sender, RoutedEventArgs e)
        {
            string username = TxtboxLogin.Text;
            string password = TxtboxPassword.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please fill in both username and password.", "Login Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT PasswordHash, Role FROM Users WHERE UserName = @UserName";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserName", username);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedHash = reader["PasswordHash"].ToString();
                                int role = Convert.ToInt32(reader["Role"]);

                                if (VerifyPassword(password, storedHash))
                                {
                                    MessageBox.Show($"Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                                    MainWindow mainWindow = new MainWindow(role);
                                    mainWindow.Show();
                                    this.Close();
                                }
                                else
                                {
                                    MessageBox.Show("Incorrect password.", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("User not found.", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            string username = TxtboxLogin.Text;
            string password = TxtboxPassword.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please fill in both username and password.", "Registration Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string checkQuery = "SELECT COUNT(*) FROM Users WHERE UserName = @UserName";
                    using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@UserName", username);
                        int count = (int)checkCommand.ExecuteScalar();
                        if (count > 0)
                        {
                            MessageBox.Show("Username already exists. Please choose another one.", "Registration Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }

                    string hashedPassword = HashPassword(password);
                    string insertQuery = "INSERT INTO Users (UserName, PasswordHash, Role) VALUES (@UserName, @PasswordHash, @Role)";
                    using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@UserName", username);
                        insertCommand.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                        insertCommand.Parameters.AddWithValue("@Role", 2);

                        insertCommand.ExecuteNonQuery();
                        MessageBox.Show("Registration successful! You can now log in.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        TxtboxLogin.Clear();
                        TxtboxPassword.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            string enteredHash = HashPassword(enteredPassword);
            return enteredHash == storedHash;
        }
    }
}
