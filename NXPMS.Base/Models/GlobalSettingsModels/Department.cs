using System;
using System.Collections.Generic;
using System.Text;

namespace NXPMS.Base.Models.GlobalSettingsModels
{
    public class Department
    {
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public int DepartmentHeadId { get; set; }
        public string DepartmentHeadName { get; set; }
        public int DepartmentAltHeadId { get; set; }
        public string DepartmentAltHeadName { get; set; }
    }
}
