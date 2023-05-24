using Microsoft.Extensions.Configuration;
using NXPMS.Base.Models.PMSModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NXPMS.Base.Repositories.PMSRepositories
{
    public interface IReviewMetricRepository
    {
        IConfiguration _config { get; }

        Task<bool> AddAsync(ReviewMetric reviewMetric);
        Task<bool> DeleteAsync(int reviewMetricId);
        Task<List<ReviewMetric>> GetByIdAsync(int reviewMetricId);
        Task<List<ReviewMetric>> GetByReviewHeaderIdAsync(int reviewHeaderId);
        Task<List<ReviewMetric>> GetByMetricDescriptionAsync(int reviewHeaderId, string metricDescription);
        Task<List<ReviewMetric>> GetKpasByReviewHeaderIdAsync(int reviewHeaderId);
        Task<List<ReviewMetric>> GetCmpsByReviewHeaderIdAsync(int reviewHeaderId);
        Task<decimal> GetTotalWeightageByReviewHeaderIdAsync(int reviewHeaderId);
        Task<decimal> GetTotalKpaWeightageByReviewHeaderIdAsync(int reviewHeaderId);
        Task<decimal> GetTotalCmpWeightageByReviewHeaderIdAsync(int reviewHeaderId);
        Task<int> GetCmpCountByReviewHeaderIdAsync(int reviewHeaderId);
        Task<bool> UpdateAsync(ReviewMetric reviewMetric);
    }
}