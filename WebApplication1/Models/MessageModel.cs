public class MessageModel
{
    public int MessageId { get; set; }
    public int UserId { get; set; }
    public int SenderId { get; set; } // Новый параметр для ID отправителя
    public string Text { get; set; }
    public DateTime Timestamp { get; set; }
}
