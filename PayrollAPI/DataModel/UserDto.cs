﻿

namespace PayrollAPI.DataModel
{
    public class UserDto
    {
        public int id { get; set; }
        public string? userID { get; set; }
        public string? epf { get; set; }
        public string? empName { get; set; }
        public int? companyCode { get; set; }
        public string? costCenter { get; set; }
        public string? password { get; set; }
        public string? role { get; set; }
        public bool? isAccountLocked { get; set; }
        public string? createdBy { get; set; }
        public DateTime? createdDate { get; set; }
        public string? lastUpdateBy { get; set; }
        public DateTime? lastUpdateDate { get; set; }
    }
}
