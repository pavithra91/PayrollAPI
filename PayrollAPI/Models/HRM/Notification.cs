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
        public string? target { get; set; }
        public string? status { get; set; }
        public DateTime createdDate { get; set; }
    }
}
