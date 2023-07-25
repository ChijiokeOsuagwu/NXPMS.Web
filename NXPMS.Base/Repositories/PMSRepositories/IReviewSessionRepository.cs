using Microsoft.Extensions.Configuration;
using NXPMS.Base.Models.PMSModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NXPMS.Base.Repositories.PMSRepositories
{
    public interface IReviewSessionRepository
    {
        IConfiguration _config { get; }

        #region Review Session Read Action Methods
        Task<bool> AddAsync(ReviewSession reviewSession);
        Task<bool> DeleteAsync(int reviewSessionId);
        Task<IList<ReviewSession>> GetAllAsync();
        Task<IList<ReviewSession>> GetByIdAsync(int reviewSessionId);
        Task<IList<ReviewSession>> GetByNameAsync(string reviewSessionName);
        Task<IList<ReviewSession>> GetByYearIdAsync(int performanceYearId);
        Task<bool> UpdateAsync(ReviewSession reviewSession);
        #endregion
    }
}