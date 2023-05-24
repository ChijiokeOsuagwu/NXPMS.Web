using Microsoft.Extensions.Configuration;
using NXPMS.Base.Models.PMSModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NXPMS.Base.Repositories.PMSRepositories
{
    public interface IPmsActivityHistoryRepository
    {
        IConfiguration _config { get; }
        Task<IList<PmsActivityHistory>> GetByReviewHeaderIdAsync(int reviewHeaderId);
        Task<bool> AddAsync(PmsActivityHistory activityHistory);
    }
}
