namespace ServerChat.Models
{
    public class UserModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Avatar { get; set; }
        public string LastMessageText { get; set; }
        public DateTime? LastMessageTimestamp { get; set; }
        public string LastMessage { get; set; } // Combined field
    }
}
