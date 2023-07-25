using Microsoft.Extensions.Configuration;
using NXPMS.Base.Models.PMSModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NXPMS.Base.Repositories.PMSRepositories
{
    public interface IReviewResultRepository
    {
        IConfiguration _config { get; }

        Task<IList<ReviewResult>> GetIntitalByThirdPartyAsync(int reviewHeaderId, int appraiserId, int metricTypeId);
        Task<IList<ReviewResult>> GetIntitalByMetricTypeIdAsync(int reviewHeaderId, int appraiserId, int metricTypeId);
        Task<IList<ReviewResult>> GetIntitalByMetricIdAsync(int reviewHeaderId, int appraiserId, int metricId);

        Task<IList<ReviewResult>> GetById(int reviewResultId);
        Task<IList<ReviewResult>> GetByAppraiserIdAndMetricTypeId(int reviewHeaderId, int appraiserId, int reviewMetricTypeId);
        Task<IList<ReviewResult>> GetByAppraiserIdAndMetricId(int reviewHeaderId, int appraiserId, int reviewMetricId);
        Task<IList<ReviewResult>> GetByAppraiserIdAndReviewHeaderId(int reviewHeaderId, int appraiserId);

        Task<List<ReviewScore>> GetScoresByReviewHeaderIdAndAppraiserIdAsync(int reviewHeaderId, int appraiserId);
        Task<bool> AddAsync(ReviewResult reviewResult);
        Task<bool> UpdateAsync(ReviewResult reviewResult);
        Task<bool> AddSummaryAsync(ResultSummary resultSummary);
        Task<bool> UpdateSummaryAsync(ResultSummary resultSummary);
        Task<bool> DeleteSummaryAsync(int reviewHeaderId);

        Task<IList<ResultSummary>> GetSummaryByReviewSessionId(int reviewSessionId);
        Task<IList<ResultSummary>> GetSummaryByReviewSessionIdAndDepartmentCode(int reviewSessionId, string departmentCode);
        Task<IList<ResultSummary>> GetSummaryByReviewSessionIdAndUnitCode(int reviewSessionId, string unitCode);
        Task<IList<ResultSummary>> GetSummaryByReviewSessionIdAndAppraiseeName(int reviewSessionId, string appraiseeName);

        Task<IList<ResultSummary>> GetSummaryByAppraiserIdAndReviewHeaderId(int reviewHeaderId, int appraiserId);
        Task<IList<ResultSummary>> GetSummaryByReportToId(int reportToId, int reviewSessionId);
        Task<IList<ResultSummary>> GetSummaryByReportToIdAndAppraiseeId(int reportToId, int reviewSessionId, int appraiseeId);

        Task<List<int>> GetAppraisersByReviewHeaderId(int reviewHeaderId);

        Task<List<AppraiserDetail>> GetAppraisersDetailsByReviewHeaderId(int reviewHeaderId);

        #region Result Details Action Interfaces
        Task<IList<ResultDetail>> GetPrincipalResultDetailByReviewSessionIdAsync(int reviewSessionId);
        Task<IList<ResultDetail>> GetPrincipalResultDetailByLocationIdAndReviewSessionIdAsync(int reviewSessionId, int locationId);
        Task<IList<ResultDetail>> GetPrincipalResultDetailByDepartmentCodeAndReviewSessionIdAsync(int reviewSessionId, string departmentCode);
        Task<IList<ResultDetail>> GetPrincipalResultDetailByUnitCodeAndReviewSessionIdAsync(int reviewSessionId, string unitCode);
        #endregion
    }
}