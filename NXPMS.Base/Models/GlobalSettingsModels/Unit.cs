using System;
using System.Collections.Generic;
using System.Text;

namespace NXPMS.Base.Models.GlobalSettingsModels
{
    public class Unit
    {
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public int UnitHeadId { get; set; }
        public string UnitHeadName { get; set; }
        public int UnitAltHeadId { get; set; }
        public string UnitAltHeadName { get; set; }

    }
}
