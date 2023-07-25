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
        Task<bool> UpdateGoalAsync(int reviewHeaderId, string performanceGoal, int appraiserId);
        Task<bool> UpdateStageIdAsync(int reviewHeaderId, int nextStageId);
        Task<bool> UpdateContractAcceptanceAsync(int reviewHeaderId, bool isAccepted);
        Task<bool> UpdateEvaluationAcceptanceAsync(int reviewHeaderId, bool isAccepted);
        Task<bool> UpdateAppraiseeFlagAsync(int reviewHeaderId, bool isFlagged, string flaggedBy);
        Task<bool> UpdateFeedbackAsync(int reviewHeaderId, string feedbackProblems, string feedbackSolutions);

        Task<bool> UpdateLineManagerRecommendationAsync(int reviewHeaderId, string lineManagerName, string recommendedAction, string remarks);
        Task<bool> UpdateUnitHeadRecommendationAsync(int reviewHeaderId, string unitHeadName, string recommendedAction, string remarks);
        Task<bool> UpdateDepartmentHeadRecommendationAsync(int reviewHeaderId, string deptHeadName, string recommendedAction, string remarks);
        Task<bool> UpdateHrRecommendationAsync(int reviewHeaderId, string hrRepName, string recommendedAction, string remarks);
        Task<bool> UpdateManagementDecisionAsync(int reviewHeaderId, string mgtRepName, string recommendedAction, string remarks);
    }
}
