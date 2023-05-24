using Microsoft.Extensions.Configuration;
using NXPMS.Base.Models.EmployeesModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NXPMS.Base.Repositories.EmployeeRecordRepositories
{
    public interface IEmployeesRepository
    {
        IConfiguration _config { get; }

        #region Employees Read Action Methods

        Task<IList<Employee>> GetByNameAsync(string fullname);
        Task<IList<Employee>> GetByIdAsync(int employeeId);
        Task<EmployeeCardinal> GetEmployeeCardinalsByIdAsync(int employeeId);
        Task<IList<Employee>> FindByNameAsync(string fullname);
        Task<IList<Employee>> FindNonUsersByNameAsync(string fullname);
        Task<IList<Employee>> GetAllAsync();
        Task<IList<Employee>> GetByDepartmentCodeAsync(string departmentCode);
        Task<IList<Employee>> GetByLocationIdAndDepartmentCodeAsync(int locationId, string departmentCode);
        Task<IList<Employee>> GetByLocationIdAndUnitCodeAsync(int locationId, string unitCode);
        Task<IList<Employee>> GetByLocationIdAsync(int locationId = 0);
        Task<IList<Employee>> GetByUnitCodeAsync(string unitCode);
        #endregion

        #region Employees Write Action Methods
        Task<bool> AddEmployeePersonalInfoOnlyAsync(Employee employee);
        Task<bool> UpdateEmployeePersonalInfoOnlyAsync(Employee employee);
        Task<bool> UpdateEmployeeInfoOnlyAsync(Employee employee);
        Task<bool> UpdateEmployeeNextOfKinInfoOnlyAsync(Employee employee);
        #endregion
    }
}