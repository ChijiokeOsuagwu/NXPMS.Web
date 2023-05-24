using Microsoft.Extensions.Configuration;
using NXPMS.Base.Models.PMSModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NXPMS.Base.Repositories.PMSRepositories
{
    public interface ICompetencyRepository
    {
        IConfiguration _config { get; }
        Task<List<Competency>> GetByAllAsync();
        Task<List<Competency>> GetByIdAsync(int competencyId);
        Task<List<Competency>> GetByCategoryIdAsync(int categoryId);
        Task<List<Competency>> GetByLevelIdAsync(int levelId);
        Task<List<Competency>> GetByCategoryIdAndLevelIdAsync(int categoryId, int levelId);
        Task<bool> AddAsync(Competency competency);
        Task<bool> UpdateAsync(Competency competency);
        Task<bool> DeleteAsync(int competencyId);
    }
}
