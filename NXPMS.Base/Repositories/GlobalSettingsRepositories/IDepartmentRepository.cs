using Microsoft.Extensions.Configuration;
using NXPMS.Base.Models.GlobalSettingsModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NXPMS.Base.Repositories.GlobalSettingsRepositories
{
    public interface IDepartmentRepository
    {
        IConfiguration _config { get; }

        Task<IList<Department>> GetAllAsync();
        Task<IList<Department>> GetByCodeAsync(string departmentCode);
        Task<IList<Department>> GetByNameAsync(string departmentName);
        Task<bool> AddAsync(Department department);
        Task<bool> UpdateAsync(Department department);
        Task<bool> DeleteAsync(string departmentCode);
    }
}