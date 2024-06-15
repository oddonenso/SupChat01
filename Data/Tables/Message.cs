using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Tables
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("MessageID")]
        public int MessageId { get; set; }

        [Column("UserID")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Column("SenderId")]
        public int SenderId { get; set; }

        [ForeignKey("SenderId")]
        public User? Sender { get; set; }

        [Column("Text", TypeName = "varchar(1000)")]
        public string Text { get; set; } = string.Empty;

        [Column("Timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
