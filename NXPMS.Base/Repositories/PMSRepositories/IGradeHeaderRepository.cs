using Microsoft.Extensions.Configuration;
using NXPMS.Base.Models.PMSModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NXPMS.Base.Repositories.PMSRepositories
{
    public interface IGradeHeaderRepository
    {
        IConfiguration _config { get; }

        Task<bool> AddAsync(GradeHeader gradeHeader);
        Task<bool> DeleteAsync(int gradeHeaderId);
        Task<IList<GradeHeader>> GetAllAsync();
        Task<IList<GradeHeader>> GetByIdAsync(int gradeHeaderId);
        Task<IList<GradeHeader>> GetByNameAsync(string gradeHeaderName);
        Task<bool> UpdateAsync(GradeHeader gradeHeader);
    }
}