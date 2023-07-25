using Microsoft.Extensions.Configuration;
using NXPMS.Base.Models.EmployeesModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NXPMS.Base.Repositories.EmployeeRecordRepositories
{
    public interface IEmployeeReportRepository
    {
        IConfiguration _config { get; }

        Task<bool> AddAsync(EmployeeReport employeeReport);
        Task<IList<EmployeeReport>> GetByEmployeeIdAsync(int employeeId);
        Task<IList<EmployeeReport>> GetByReportsToIdAsync(int reportsToId);
        Task<IList<EmployeeReport>> GetByIdAsync(int employeeReportId);
        Task<IList<EmployeeReport>> GetByEmployeeIdAndReportIdAsync(int employeeId, int reportToId);
    }
}