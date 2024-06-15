using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using ClientChat.Model;

namespace ClientChat.ViewModel
{
    public class ChatViewModel : INotifyPropertyChanged
    {
        private readonly HttpClient _httpClient;
        private string _messageText;

        public ObservableCollection<MessageModel> Messages { get; }
        public ICommand SendMessageCommand { get; }

        public ChatViewModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            Messages = new ObservableCollection<MessageModel>();
            SendMessageCommand = new RelayCommand(async () => await SendMessage());
        }

        public async Task LoadDataAsync()
        {
            await LoadMessages();
            // Здесь можно добавить загрузку других данных, если необходимо
        }

        public string MessageText
        {
            get => _messageText;
            set
            {
                _messageText = value;
                OnPropertyChanged();
            }
        }

        private async Task LoadMessages()
        {
            var response = await _httpClient.GetAsync("chat/messages");
            if (response.IsSuccessStatusCode)
            {
                var messages = await JsonSerializer.DeserializeAsync<IEnumerable<MessageModel>>(await response.Content.ReadAsStreamAsync());
                Messages.Clear();
                foreach (var message in messages)
                {
                    Messages.Add(message);
                }
            }
        }

        private async Task SendMessage()
        {
            if (string.IsNullOrEmpty(MessageText)) return;

            var newMessage = new MessageModel
            {
                UserId = 1, // Установите соответствующий UserId
                Text = MessageText
            };

            var response = await _httpClient.PostAsync("chat/send",
                new StringContent(JsonSerializer.Serialize(newMessage), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var message = await JsonSerializer.DeserializeAsync<MessageModel>(await response.Content.ReadAsStreamAsync());
                Messages.Add(message);
                MessageText = string.Empty;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
