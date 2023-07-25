using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.PMSViewModels
{
    public class ApprovalRoleViewModel:BaseViewModel
    {
        public int ApprovalRoleID { get; set; }
        [Required]
        [Display(Name="Role Name")]
        public string ApprovalRoleName { get; set; }
        [Required]
        [Display(Name= "Must Approve Performance Contract")]
        public bool MustApproveContract { get; set; }
        [Required]
        [Display(Name = "Must Approve Evaluation Result")]
        public bool MustApproveEvaluation { get; set; }
    }
}
