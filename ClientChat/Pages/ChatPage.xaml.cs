using ClientChat.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ClientChat.Pages
{
    public partial class ChatPage : Page
    {
        private readonly HttpClient _httpClient;
        private ObservableCollection<UserModel> _users;
        private ObservableCollection<MessageModel> _messages;
        private UserModel _selectedUser;
        private int _currentUserId;
        private DateTime _lastMessageTimestamp;
        private DispatcherTimer _messagePollingTimer;

        public ChatPage(int userId)
        {
            InitializeComponent();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7267/api/")
            };
            _currentUserId = userId;
            LoadData();
            InitializeMessagePolling();
        }

        private async void LoadData()
        {
            var users = await _httpClient.GetFromJsonAsync<ObservableCollection<UserModel>>($"chat/users/{_currentUserId}");

            // Исключаем текущего пользователя из списка
            _users = new ObservableCollection<UserModel>(users.Where(u => u.UserId != _currentUserId));

            // Преобразуем время последнего сообщения в локальное время клиента
            foreach (var user in _users)
            {
                if (user.LastMessageTimestamp.HasValue)
                {
                    user.LastMessageTimestamp = user.LastMessageTimestamp.Value.ToLocalTime();
                }
            }

            ChatListBox.ItemsSource = _users;
        }

        private async void ChatListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedUser = (UserModel)ChatListBox.SelectedItem;
            if (_selectedUser != null)
            {
                SelectedUserName.Text = _selectedUser.UserName;
                _messages = await _httpClient.GetFromJsonAsync<ObservableCollection<MessageModel>>($"chat/messages/{_currentUserId}/{_selectedUser.UserId}");
                _lastMessageTimestamp = _messages.LastOrDefault()?.Timestamp ?? DateTime.MinValue;

                SelectChatMessage.Visibility = Visibility.Collapsed;
                MessageInputPanel.Visibility = Visibility.Visible;

                DisplayMessages();
            }
            else
            {
                MessageInputPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void DisplayMessages()
        {
            MessagesPanel.Children.Clear();
            foreach (var message in _messages)
            {
                var messageStackPanel = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    HorizontalAlignment = (message.SenderId == _currentUserId) ? HorizontalAlignment.Right : HorizontalAlignment.Left,
                    Margin = new Thickness(5)
                };

                var messageBorder = new Border
                {
                    CornerRadius = new CornerRadius(10),
                    Background = (message.SenderId == _currentUserId) ? System.Windows.Media.Brushes.LightBlue : System.Windows.Media.Brushes.LightGreen,
                    Margin = new Thickness(5)
                };

                var messageTextBlock = new TextBlock
                {
                    Text = message.Text,
                    FontFamily = new System.Windows.Media.FontFamily("Comic Sans MS"),
                    Padding = new Thickness(10)
                };

                var localTime = message.Timestamp.ToLocalTime(); // Преобразуем UTC время в локальное время клиента
                var messageTimeBlock = new TextBlock
                {
                    Text = localTime.ToString("dd/MM/yyyy HH:mm"),
                    FontFamily = new System.Windows.Media.FontFamily("Comic Sans MS"),
                    FontSize = 10,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(5, 0, 5, 0)
                };

                messageBorder.Child = messageTextBlock;
                messageStackPanel.Children.Add(messageBorder);
                messageStackPanel.Children.Add(messageTimeBlock);
                MessagesPanel.Children.Add(messageStackPanel);
            }

            MessageScrollViewer.ScrollToEnd();
        }

        private async void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedUser == null || string.IsNullOrWhiteSpace(MessageTextBox.Text))
            {
                MessageBox.Show("Выберите пользователя и введите сообщение.");
                return;
            }

            var messageModel = new MessageModel
            {
                UserId = _selectedUser.UserId,
                SenderId = _currentUserId,
                Text = MessageTextBox.Text
            };

            var response = await _httpClient.PostAsJsonAsync("chat/send", messageModel);
            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show("Ошибка при отправке сообщения.");
                return;
            }

            var newMessage = await response.Content.ReadFromJsonAsync<MessageModel>();
            _messages.Add(newMessage);
            _lastMessageTimestamp = newMessage.Timestamp;

            DisplayMessages();
            MessageTextBox.Text = string.Empty;
        }

        private void InitializeMessagePolling()
        {
            _messagePollingTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            _messagePollingTimer.Tick += async (sender, e) => await PollForNewMessages();
            _messagePollingTimer.Start();
        }

        private async Task PollForNewMessages()
        {
            if (_selectedUser != null)
            {
                var newMessages = await _httpClient.GetFromJsonAsync<ObservableCollection<MessageModel>>($"chat/messages/{_currentUserId}/{_selectedUser.UserId}?lastTimestamp={_lastMessageTimestamp:O}");
                foreach (var message in newMessages)
                {
                    _messages.Add(message);
                    _lastMessageTimestamp = message.Timestamp;
                }
                if (newMessages.Count > 0)
                {
                    DisplayMessages();
                }
            }
        }
    }
}


public class UserModel
{
    public int UserId { get; set; }
    public string UserName { get; set; }
    public string Avatar { get; set; }
    public string LastMessage { get; set; }
    public DateTime? LastMessageTimestamp { get; set; }
}

