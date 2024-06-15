namespace ClientChat.Model
{
    public class MessageModel
    {
        public int MessageId { get; set; }
        public int UserId { get; set; }
        public int SenderId { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
        public string FormattedTimestamp => Timestamp.ToString("dd/MM/yyyy HH:mm");
    }
}
