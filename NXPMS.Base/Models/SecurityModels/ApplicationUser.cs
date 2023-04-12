using System;
using System.Collections.Generic;
using System.Text;

namespace NXPMS.Base.Models.SecurityModels
{
    public class ApplicationUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public int EmployeeId { get; set; }
        public int AccessFailedCount { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedTime { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public string Sex { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
    }
}
