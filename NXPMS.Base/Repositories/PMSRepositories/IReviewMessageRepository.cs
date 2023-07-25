using Microsoft.Extensions.Configuration;
using NXPMS.Base.Models.PMSModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NXPMS.Base.Repositories.PMSRepositories
{
    public interface IReviewMessageRepository
    {
        IConfiguration _config { get; }

        Task<bool> AddAsync(ReviewMessage reviewMessage);
        Task<bool> DeleteAsync(int reviewMessageId);
        Task<ReviewMessage> GetByIdAsync(int reviewMessageId);
        Task<List<ReviewMessage>> GetByReviewHeaderIdAsync(int reviewHeaderId);
        Task<bool> UpdateAsync(ReviewMessage reviewMessage);
    }
}