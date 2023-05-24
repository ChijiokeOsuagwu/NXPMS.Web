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
        Task<List<ReviewSubmission>> GetByIdAsync(int reviewSubmissionId);
        Task<List<ReviewSubmission>> GetByReviewerIdAsync(int reviewerId);
        Task<List<ReviewSubmission>> GetByReviewHeaderIdAsync(int reviewHeaderId);
    }
}