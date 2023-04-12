using Microsoft.Extensions.Configuration;
using NXPMS.Base.Models.EmployeesModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NXPMS.Base.Repositories.EmployeeRecordRepositories
{
    public interface IEmployeeTypeRepository
    {
        IConfiguration _config { get; }

        Task<IList<EmployeeType>> GetAllAsync();
        Task<IList<EmployeeType>> GetByIdAsync(int employeeTypeId);
    }
}