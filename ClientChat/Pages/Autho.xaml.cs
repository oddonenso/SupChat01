using System;
using System.Net.Http;
using System.Windows.Controls;
using ClientChat.ViewModel;

namespace ClientChat.Pages
{
    public partial class Autho : Page
    {
        private readonly HttpClient _httpClient;

        public Autho()
        {
            InitializeComponent();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7267/api/")
            };
            DataContext = new AuthoViewModel(_httpClient, NavigateToChatPage, NavigateToAdminPage, NavigateToEmailSendPage);
        }

        private void NavigateToEmailSendPage()
        {
            NavigationService.Navigate(new WriteEmail(_httpClient));
        }

        private void NavigateToChatPage(int userId) // Accept user ID as parameter
        {
            NavigationService.Navigate(new ChatPage(userId));
        }

        private void NavigateToAdminPage()
        {
            // Admin page navigation logic
        }
    }
}
