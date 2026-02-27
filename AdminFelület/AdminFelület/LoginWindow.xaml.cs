using AdminFelület.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AdminFelület
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly ApiService _apiService;
        public LoginWindow()
        {
            InitializeComponent();
            _apiService = new ApiService();
        }

        private void ShowError(string message)
        {
            ErrorText.Text = message;
            ErrorText.Visibility = Visibility.Visible;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorText.Visibility = Visibility.Collapsed;

            var username = UsernameBox.Text.Trim();
            var password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowError("Kérem, adja meg a felhasználónevet és jelszót.");
                return;
            }

            IsEnabled = false;

            var result = await _apiService.LoginAsync(username, password);

            if (result != null && result.User.Role == "Admin")
            {
                var mainWindow = new MainWindow(_apiService);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                ShowError("Hibás felhasználónév vagy jelszó, vagy nincs admin jogosultság.");
                IsEnabled = true;
            }
        }
    }
}
