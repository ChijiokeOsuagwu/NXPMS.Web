using System;
using System.Collections.Generic;
using System.Text;

namespace NXPMS.Base.Models.SecurityModels
{
    public class LoginAttempt
    {
       public int AttemptId { get; set; }
        public string Username { get; set; }
        public bool IsSuccessful { get; set; }
        public DateTime? LoginTime { get; set; }
    }
}
