// WriteEmail.xaml.cs
using System;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ClientChat.ViewModel;

namespace ClientChat.Pages
{
    public partial class WriteEmail : Page
    {
        private readonly WriteEmailViewModel _viewModel;
        private string _generatedCode;

        public WriteEmail(HttpClient httpClient)
        {
            InitializeComponent();
        }

        private async void SendEmailButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var email = EmailTextBox.Text; 

                    var requestBody = JsonSerializer.Serialize(new { Email = email });
                    var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync("https://localhost:7267/api/PasswordReset/request", content);

                    if (response.IsSuccessStatusCode)
                    {
                        
                        _generatedCode = GenerateSixDigitCode();

                        
                        EmailSend(email, _generatedCode);

                        var responseContent = await response.Content.ReadAsStringAsync();
                        StatusMessageTextBlock.Text = responseContent; // Показываем ответ сервера
                        CodeTextBox.Visibility = Visibility.Visible;
                        VerifyCodeButton.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        StatusMessageTextBlock.Text = "Произошла ошибка при отправке запроса.";
                    }
                }
            }
            catch (Exception ex)
            {
                StatusMessageTextBlock.Text = "Произошла ошибка: " + ex.Message;
            }
        }

        private void EmailSend(string email, string generatedCode)
        {
            string subject = "Код подтверждения";
            string body = $"Ваш код подтверждения: {generatedCode}\n" +
                          "Используйте этот код для подтверждения изменения пароля.\n\n" +
                          "Если вы не запрашивали изменение пароля, пожалуйста, проигнорируйте это письмо.";

            MailMessage message = new MailMessage("oddoneoddonov@gmail.com", email, subject, body);
            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("oddoneoddonov@gmail.com", "siuj xxkp hquy uhcp");

            // Определяем хост и порт SMTP сервера в зависимости от почтового домена
            if (email.Contains("@yandex.ru"))
            {
                client.Host = "smtp.yandex.ru";
                client.Port = 587;
            }
            else if (email.Contains("@mail.ru"))
            {
                client.Host = "smtp.mail.ru";
                client.Port = 587;
            }
            else if (email.Contains("@gmail.com"))
            {
                client.Host = "smtp.gmail.com";
                client.Port = 587;
            }
            client.EnableSsl = true; // Включаем SSL для безопасной передачи
            try
            {
                client.Send(message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось отправить письмо. Ошибка: " + ex.Message);
            }
        }

        private void VerifyCodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (CodeTextBox.Text == _generatedCode)
            {
                NavigationService.Navigate(new ResetPasswordPage());
            }
            else
            {
                StatusMessageTextBlock.Text = "Неправильный код. Попробуйте снова.";
            }
        }

        private void BackToMainHyperlink_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Autho());
        }

        // Метод для генерации шестизначного кода
        private string GenerateSixDigitCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}
