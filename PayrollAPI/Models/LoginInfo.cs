using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PayrollAPI.Models
{
    public class LoginInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string? userID { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string? tokenID { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string? refreshToken { get; set; }

        public DateTime loginDateTime { get; set; }

        [Column(TypeName = "boolean")]
        public bool isActive { get; set; }
    }
}
