using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PayrollAPI.Models
{
    [Keyless]
    public class OTHours_View
    {
        public int companyCode { get; set; }
        public string? costCenter { get; set; }
        public int period { get; set; }
        public float othours { get; set; }
    }
}
