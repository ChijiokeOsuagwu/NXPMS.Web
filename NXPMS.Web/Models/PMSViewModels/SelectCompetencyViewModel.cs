﻿using NXPMS.Base.Models.PMSModels;
using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.PMSViewModels
{
    public class SelectCompetencyViewModel:BaseViewModel
    {
        public int ReviewHeaderID { get; set; }
        public int cd { get; set; }
        public int vd { get; set; }
        public int dd { get; set; }
        public List<Competency> CompetenciesList { get; set; }
    }
}
