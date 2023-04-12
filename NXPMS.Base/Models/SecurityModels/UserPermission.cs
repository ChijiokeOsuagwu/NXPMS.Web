using System;
using System.Collections.Generic;
using System.Text;

namespace NXPMS.Base.Models.SecurityModels
{
    public class UserPermission
    {
        public int PermissionID { get; set; }
        public int UserID { get; set; }
        public string RoleCode { get; set; }
        public string UserFullName { get; set; }
        public string RoleTitle { get; set; }
        public string ApplicationCode { get; set; }
        public string ApplicationDescription { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedTime { get; set; }
    }
}
