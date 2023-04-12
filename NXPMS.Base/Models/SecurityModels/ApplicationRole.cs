using System;
using System.Collections.Generic;
using System.Text;

namespace NXPMS.Base.Models.SecurityModels
{
    public class ApplicationRole
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Rank { get; set; }
        public string ApplicationCode { get; set; }
        public string ApplicationDescription { get; set; }
    }
}
