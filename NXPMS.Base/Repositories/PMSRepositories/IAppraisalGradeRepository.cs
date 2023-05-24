using Microsoft.Extensions.Configuration;
using NXPMS.Base.Enums;
using NXPMS.Base.Models.PMSModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NXPMS.Base.Repositories.PMSRepositories
{
    public interface IAppraisalGradeRepository
    {
        IConfiguration _config { get; }

        Task<bool> AddAsync(AppraisalGrade appraisalGrade);
        Task<bool> CopyAsync(string copiedBy, int reviewSessionId, int gradeTemplateId);
        Task<bool> CopyAsync(string copiedBy, int reviewSessionId, int gradeTemplateId, ReviewGradeType gradeType);
        Task<bool> DeleteAsync(int appraisalGradeId);
        Task<IList<AppraisalGrade>> GetAllAsync();
        Task<IList<AppraisalGrade>> GetByIdAsync(int appraisalGradeId);
        Task<IList<AppraisalGrade>> GetByNameAsync(string appraisalGradeName);
        Task<IList<AppraisalGrade>> GetByReviewSessionIdAsync(int reviewSessionId);
        Task<IList<AppraisalGrade>> GetByReviewSessionIdAsync(int reviewSessionId, ReviewGradeType gradeType);
        Task<bool> UpdateAsync(AppraisalGrade appraisalGrade);
    }
}