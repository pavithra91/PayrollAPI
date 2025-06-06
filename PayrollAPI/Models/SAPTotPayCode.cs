﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PayrollAPI.Models
{
    public class SAPTotPayCode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int companyCode { get; set; }
        public int payCode { get; set; }

        [Column(TypeName = "varchar(1)")]
        public string? payType { get; set; }

        [Column(TypeName = "varchar(5)")]
        public string? calCode { get; set; }

        public int period { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal totalAmount { get; set; }

        public int totCount { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string? createdBy { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime createdDate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime createdTime { get; set; }
    }
}
