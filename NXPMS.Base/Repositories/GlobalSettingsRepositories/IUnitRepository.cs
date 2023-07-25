using Microsoft.Extensions.Configuration;
using NXPMS.Base.Models.GlobalSettingsModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NXPMS.Base.Repositories.GlobalSettingsRepositories
{
    public interface IUnitRepository
    {
        IConfiguration _config { get; }

        Task<IList<Unit>> GetAllAsync();
        Task<IList<Unit>> GetByCodeAsync(string unitCode);
        Task<IList<Unit>> GetByDepartmentCodeAsync(string departmentCode);
        Task<IList<Unit>> GetByNameAsync(string unitName);
        Task<bool> AddAsync(Unit unit);
        Task<bool> UpdateAsync(Unit unit);
        Task<bool> DeleteAsync(string unitCode);
    }
}