using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace PayrollAPI.Models.Reservation
{
    public class ReservationCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string? categoryName { get; set; }


        [JsonIgnore]
        public ICollection<BungalowRates> bungalowRates { get; set; } = new HashSet<BungalowRates>();
        [JsonIgnore]
        public ICollection<Bungalow_Reservation> bungalow_Reservations { get; set; } = new HashSet<Bungalow_Reservation>();


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
