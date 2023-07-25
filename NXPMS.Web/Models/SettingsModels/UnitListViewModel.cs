using NXPMS.Base.Models.GlobalSettingsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.SettingsModels
{
    public class UnitListViewModel
    {
        public string dc { get; set; }
        public List<Unit> UnitList { get; set; }
    }
}
