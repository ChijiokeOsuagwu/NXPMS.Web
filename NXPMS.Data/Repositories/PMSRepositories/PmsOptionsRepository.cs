using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace NXPMS.Data.Repositories.PMSRepositories
{
    public class PmsOptionsRepository
    {
        public IConfiguration _config { get; }
        public PmsOptionsRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

    }
}
