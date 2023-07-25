using Microsoft.Extensions.Configuration;
using NXPMS.Base.Models.PMSModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NXPMS.Base.Repositories.PMSRepositories
{
    public interface IReviewSubmissionRepository
    {
        IConfiguration _config { get; }

        Task<bool> AddAsync(ReviewSubmission reviewSubmission);
        Task<bool> UpdateAsync(int reviewSubmissionId);
        Task<bool> UpdateAsync(int reviewHeaderId, int toEmployeeId, int submissionPurposeId);
        Task<bool> DeleteAsync(int reviewSubmissionId);
        Task<List<ReviewSubmission>> GetByIdAsync(int reviewSubmissionId);
        Task<List<ReviewSubmission>> GetByReviewerIdAsync(int reviewerId);
        Task<List<ReviewSubmission>> GetByReviewerIdAndReviewSessionIdAsync(int reviewerId, int reviewSessionId);
        Task<List<ReviewSubmission>> GetByReviewHeaderIdAsync(int reviewHeaderId);
        Task<List<ReviewSubmission>> GetByReviewHeaderIdAndSubmissionPurposeIdAsync(int reviewHeaderId, int submissionPurposeId);
        Task<List<ReviewSubmission>> GetByReviewHeaderIdAndSubmissionPurposeIdAsync(int reviewHeaderId, int submissionPurposeId, int appraiserId);
    }
}