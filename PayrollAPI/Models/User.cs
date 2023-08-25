using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PayrollAPI.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string? userID { get; set; }

        public int epf { get; set; }

        [Column(TypeName = "varchar(60)")]
        public string? empName { get; set; }

        [Column(TypeName = "varchar(6)")]
        public string? costCenter { get; set; }

        [Column(TypeName = "varchar(6)")]
        public string? role { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string? pwdSalt { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string? pwdHash { get; set; }

        [Column(TypeName = "boolean")]
        public bool status { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string? createdBy { get; set; }
        public DateTime createdDate { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string? lastUpdateBy { get; set; }
        public DateTime lastUpdateDate { get; set; }
    }
}
