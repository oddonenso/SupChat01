namespace ServerChat.Models
{
    public class PasswordResetModel
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
