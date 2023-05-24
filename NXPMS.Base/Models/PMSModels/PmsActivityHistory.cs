using System;
using System.Collections.Generic;
using System.Text;

namespace NXPMS.Base.Models.PMSModels
{
    public class PmsActivityHistory
    {
        public int ActivityId { get; set; }
        public string ActivityDescription { get; set; }
        public DateTime? ActivityTime { get; set; }
        public int ReviewHeaderId { get; set; }
    }
}
