using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using PayrollAPI.Models.HRM;

namespace PayrollAPI.Models.Reservation
{
    public class Bungalow
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int companyCode { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string? bungalowName { get; set; }

        [Column(TypeName = "varchar(500)")]
        public string? description { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string? address { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string? mainImg { get; set; }
        public int noOfRooms { get; set; }
        public int maxBookingPeriod { get; set; }
        public bool isCloded { get; set; }
        public DateTime? reopenDate { get; set; }

        [Column(TypeName = "varchar(15)")]
        public string contactNumber { get; set; }



        [JsonIgnore]
        public ICollection<Bungalow_Reservation> reservations { get; set; } = new HashSet<Bungalow_Reservation>();
        [JsonIgnore]
        public ICollection<BungalowRates>  rates { get; set; } = new HashSet<BungalowRates>();

        // Logs
        [Column(TypeName = "varchar(10)")]
        public string? createdBy { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime createdDate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime createdTime { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string? lastUpdateBy { get; set; }
        public DateTime? lastUpdateDate { get; set; }
        public DateTime? lastUpdateTime { get; set; }
    }
}
