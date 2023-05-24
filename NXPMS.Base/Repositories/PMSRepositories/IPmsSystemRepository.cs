using Microsoft.Extensions.Configuration;
using NXPMS.Base.Models.PMSModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NXPMS.Base.Repositories.PMSRepositories
{
    public interface IPmsSystemRepository
    {
        IConfiguration _config { get; }

        Task<List<CompetencyCategory>> GetAllCompetencyCategoriesAsync();
        Task<List<CompetencyLevel>> GetAllCompetencyLevelsAsync();
    }
}