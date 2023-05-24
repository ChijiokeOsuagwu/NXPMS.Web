using Microsoft.Extensions.Configuration;
using NXPMS.Base.Models.PMSModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NXPMS.Base.Repositories.PMSRepositories
{
    public interface IPerformanceYearRepository
    {
        IConfiguration _config { get; }

        Task<bool> AddAsync(PerformanceYear performanceYear);
        Task<bool> UpdateAsync(PerformanceYear performanceYear);
        Task<bool> DeleteAsync(int performanceYearId);

        Task<IList<PerformanceYear>> GetAllAsync();
        Task<IList<PerformanceYear>> GetByIdAsync(int performanceYearId);
        Task<IList<PerformanceYear>> GetByNameAsync(string performanceYearName);
        Task<IList<PerformanceYear>> GetByOverlappingDatesAsync(DateTime startDate, DateTime endDate);
      
    }
}