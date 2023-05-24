using Microsoft.Extensions.Configuration;
using NXPMS.Base.Models.PMSModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NXPMS.Base.Repositories.PMSRepositories
{
    public interface IReviewHeaderRepository
    {
        IConfiguration _config { get; }
        Task<IList<ReviewHeader>> GetByIdAsync(int reviewHeaderId);
        Task<IList<ReviewHeader>> GetByAppraiseeIdAndReviewSessionIdAsync(int appraiseeId, int reviewSessionId);
        Task<bool> AddAsync(ReviewHeader reviewHeader);
        Task<bool> UpdateGoalAsync(int reviewHeaderId, string performanceGoal);
        Task<bool> UpdateStageIdAsync(int reviewHeaderId, int nextStageId);
    }
}
