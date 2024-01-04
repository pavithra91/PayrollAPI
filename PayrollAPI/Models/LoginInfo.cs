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
        public int companyCode { get; set; }
        public int costCenter { get; set; }

        [Column(TypeName = "varchar(300)")]
        public string? tokenID { get; set; }

        [Column(TypeName = "varchar(300)")]
        public string? refreshToken { get; set; }

        public DateTime loginDateTime { get; set; }
        public DateTime loginTime { get; set; }

        [Column(TypeName = "boolean")]
        public bool isActive { get; set; }
    }
}
