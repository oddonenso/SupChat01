using ClientChat.Model;
using System;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClientChat.ViewModel
{
    public class AuthoViewModel : INotifyPropertyChanged
    {
        private readonly HttpClient _httpClient;
        private string _username;
        private string _password;
        private string _statusMessage;
        private readonly Action<int> _navigateToChatPage; // Accept user ID as parameter
        private readonly Action _navigateToAdminPage;
        private readonly Action _navigateToEmailSendPage;

        public AuthoViewModel(HttpClient httpClient, Action<int> navigateToChatPage, Action navigateToAdminPage, Action navigateToEmailSendPage)
        {
            _httpClient = httpClient;
            _navigateToChatPage = navigateToChatPage;
            _navigateToAdminPage = navigateToAdminPage;
            _navigateToEmailSendPage = navigateToEmailSendPage;
            LoginCommand = new RelayCommand(async () => await Login());
            ForgotPasswordCommand = new RelayCommand(ForgotPassword);
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
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

        public ICommand LoginCommand { get; }
        public ICommand ForgotPasswordCommand { get; }

        private async Task Login()
        {
            var loginDetails = new UserLoginModel
            {
                Login = Username,
                Password = Password
            };

            try
            {
                var response = await _httpClient.PostAsync("auth/login",
                    new StringContent(JsonSerializer.Serialize(loginDetails), Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    var user = await response.Content.ReadFromJsonAsync<UserResponseModel>();

                    StatusMessage = "Вход выполнен успешно.";

                    if (user.Role == "Admin")
                    {
                        _navigateToAdminPage?.Invoke();
                    }
                    else
                    {
                        _navigateToChatPage?.Invoke(user.UserId); // Pass user ID to chat page navigation
                    }
                }
                else
                {
                    StatusMessage = "Неверные учетные данные.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
            }
        }

        private void ForgotPassword()
        {
            _navigateToEmailSendPage?.Invoke();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class UserResponseModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
    }
}
