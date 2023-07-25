using Microsoft.Extensions.Configuration;
using NXPMS.Base.Models.PMSModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NXPMS.Base.Repositories.PMSRepositories
{
    public interface IReviewApprovalRepository
    {
        IConfiguration _config { get; }

        Task<bool> AddAsync(ReviewApproval reviewApproval);
        Task<bool> DeleteAsync(int reviewApprovalId);
        Task<bool> DeleteAsync(ReviewApproval reviewApproval);

        Task<IList<ReviewApproval>> GetAllAsync();
        Task<IList<ReviewApproval>> GetByIdAsync(int reviewApprovalId);
        Task<IList<ReviewApproval>> GetByReviewHeaderIdAsync(int reviewHeaderId);
        Task<IList<ReviewApproval>> GetByReviewHeaderIdAsync(int reviewHeaderId, int approvalTypeId);
    }
}