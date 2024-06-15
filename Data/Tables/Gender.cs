using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Tables
{
    public class Gender
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("GenderID")]
        public int GenderID { get; set; }

        [Required]
        [Column("GenderName")]
        [StringLength(20)]
        public string GenderName { get; set; } = string.Empty;
    }
}
