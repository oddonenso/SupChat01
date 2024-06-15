namespace ServerChat.Models
{
    public class MessageModell
    {
        public int MessageId { get; set; }
        public int UserId { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
