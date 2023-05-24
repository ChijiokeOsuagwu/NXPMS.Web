using Microsoft.Extensions.Configuration;
using NXPMS.Base.Models.PMSModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NXPMS.Base.Repositories.PMSRepositories
{
    public interface IPerformanceSettingsRepository
    {
        IConfiguration _config { get; }

        Task<IList<ReviewType>> GetAllReviewTypesAsync();

        Task<IList<ApprovalRole>> GetAllApprovalRolesAsync();
    }
}