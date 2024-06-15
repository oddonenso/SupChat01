using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ClientChat.Pages
{
    public partial class ResetPasswordPage : Page
    {
        public ResetPasswordPage()
        {
            InitializeComponent();
        }

        private async void ResetPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            var newPassword = NewPasswordTextBox.Text;
            var token = TokenTextBox.Text; // Токен, введенный пользователем

            using (var httpClient = new HttpClient())
            {
                var requestBody = JsonSerializer.Serialize(new { Token = token, NewPassword = newPassword });
                var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("https://localhost:7267/api/PasswordReset/reset", content);

                if (response.IsSuccessStatusCode)
                {
                    StatusMessageTextBlock.Text = "Пароль успешно сброшен.";
                    NavigationService.Navigate(new Autho());

                }
                else
                {
                    StatusMessageTextBlock.Text = "Произошла ошибка при сбросе пароля.";
                }
            }
        }
    }
}
