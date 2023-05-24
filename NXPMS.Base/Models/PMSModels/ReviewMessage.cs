using System;
using System.Collections.Generic;
using System.Text;

namespace NXPMS.Base.Models.PMSModels
{
    public class ReviewMessage
    {
        public int ReviewMessageId { get; set; }
        public int ReviewHeaderId { get; set; }
        public int FromEmployeeId { get; set; }
        public string FromEmployeeName { get; set; }
        public int ToEmployeeId { get; set; }
        public string ToEmployeeName { get; set; }
        public string MessageTitle { get; set; }
        public string MessageBody { get; set; }
        public DateTime? MessageTime { get; set; }
        public bool MessageIsRead { get; set; }
        public DateTime? TimeRead { get; set; }
    }
}
