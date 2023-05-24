using NXPMS.Base.Models.PMSModels;
using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.PMSViewModels
{
    public class ReviewSessionsListViewModel:BaseViewModel
    {
        public int? Id { get; set; }
        public List<ReviewSession> ReviewSessionsList { get; set; }
    }
}
