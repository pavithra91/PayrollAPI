using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PayrollAPI.Models.HRM
{
    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int epf { get; set; }
        public string? description { get; set; }
        public bool markAsRead { get; set; } = false;
        public int type { get; set; }

        [Column(TypeName = "varchar(6)")]
        public string? reference { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string? target { get; set; }
        
        [Column(TypeName = "varchar(10)")]
        public string? status { get; set; }
        public DateTime createdDate { get; set; }
    }
}
