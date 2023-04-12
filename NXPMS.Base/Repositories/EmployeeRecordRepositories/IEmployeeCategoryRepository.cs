using Microsoft.Extensions.Configuration;
using NXPMS.Base.Models.EmployeesModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NXPMS.Base.Repositories.EmployeeRecordRepositories
{
    public interface IEmployeeCategoryRepository
    {
        IConfiguration _config { get; }

        Task<IList<EmployeeCategory>> GetAllAsync();
    }
}