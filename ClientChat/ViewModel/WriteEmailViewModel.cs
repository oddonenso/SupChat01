using System;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClientChat.ViewModel
{
    public class WriteEmailViewModel : INotifyPropertyChanged
    {
        private readonly HttpClient _httpClient;
        private string _email;
        private string _statusMessage;

        public WriteEmailViewModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            SendEmailCommand = new RelayCommand(async () => await SendEmail());
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand SendEmailCommand { get; }

        public async Task SendEmail()
        {
            if (string.IsNullOrEmpty(Email))
            {
                StatusMessage = "Введите электронный адрес.";
                return;
            }

            var emailRequest = new PasswordResetRequestModel
            {
                Email = Email
            };

            try
            {
                var response = await _httpClient.PostAsync("api/passwordreset/request",
                    new StringContent(JsonSerializer.Serialize(emailRequest), Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    StatusMessage = "Письмо успешно отправлено.";
                }
                else
                {
                    StatusMessage = "Ошибка при отправке письма.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка при отправке письма: {ex.Message}";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class PasswordResetRequestModel
    {
        public string Email { get; set; }
    }
}
