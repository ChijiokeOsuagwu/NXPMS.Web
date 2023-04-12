using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.UtilityModels
{
    public class BaseViewModel
    {
        public string ViewModelErrorMessage { get; set; }
        public string ViewModelWarningMessage { get; set; }
        public string ViewModelSuccessMessage { get; set; }
        public string ViewPageTitle { get; set; }
        public bool OperationIsSuccessful { get; set; }
    }
}
