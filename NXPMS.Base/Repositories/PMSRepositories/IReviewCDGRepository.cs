using Microsoft.Extensions.Configuration;
using NXPMS.Base.Models.PMSModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NXPMS.Base.Repositories.PMSRepositories
{
    public interface IReviewCDGRepository
    {
        IConfiguration _config { get; }

        Task<bool> AddAsync(ReviewCDG reviewCDG);
        Task<bool> DeleteAsync(int reviewCdgId);
        Task<List<ReviewCDG>> GetByIdAsync(int reviewCdgId);
        Task<List<ReviewCDG>> GetByReviewHeaderIdAsync(int reviewHeaderId);
        Task<bool> UpdateAsync(ReviewCDG reviewCdg);
    }
}