using Microsoft.Extensions.Configuration;
using NXPMS.Base.Enums;
using NXPMS.Base.Models.PMSModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NXPMS.Base.Repositories.PMSRepositories
{
    public interface IReviewGradeRepository
    {
        IConfiguration _config { get; }

        Task<bool> AddAsync(ReviewGrade reviewGrade);
        Task<bool> DeleteAsync(int reviewGradeId);
        Task<IList<ReviewGrade>> GetAllAsync();
        Task<IList<ReviewGrade>> GetByIdAsync(int reviewGradeId);
        Task<IList<ReviewGrade>> GetByNameAsync(string reviewGradeName);
        Task<IList<ReviewGrade>> GetByGradeHeaderIdAsync(int gradeHeaderId);
        Task<IList<ReviewGrade>> GetByGradeHeaderIdAsync(int gradeHeaderId, ReviewGradeType gradeType);
        Task<bool> UpdateAsync(ReviewGrade reviewGrade);
    }
}