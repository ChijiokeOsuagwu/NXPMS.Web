using NXPMS.Base.Models.EmployeesModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NXPMS.Base.Services
{
    public interface IEmployeeRecordService
    {
        #region Employee Read Service Methods
        Task<IList<Employee>> GetAllEmployeesAsync();
        Task<Employee> GetEmployeeByFullNameAsync(string FullName);
        Task<Employee> GetEmployeeByIdAsync(int employeeId);
        Task<IList<Employee>> SearchEmployeesByNameAsync(string Name);
        Task<IList<Employee>> SearchNonUserEmployeesByNameAsync(string Name);
        Task<IList<Employee>> GetEmployeesByLocationAsync(int LocationID);
        Task<IList<Employee>> GetEmployeesByDepartmentAsync(string DepartmentCode, int LocationID = 0);
        Task<IList<Employee>> GetEmployeesByUnitAsync(string UnitCode, int LocationID = 0);
        #endregion

        #region Employee Write Service Actions
        Task<bool> AddEmployeePersonalInfoAsync(Employee employee);
        Task<bool> UpdateEmployeePersonalInfoAsync(Employee employee);
        Task<bool> UpdateEmployeeEmploymentInfoAsync(Employee employee);
        Task<bool> UpdateEmployeeNextOfKinInfoAsync(Employee employee);
        #endregion

        #region Employee Reports Service Methods
        Task<List<EmployeeReport>> GetEmployeeReportsByEmployeeIdAsync(int employeeId);
        Task<List<EmployeeReport>> GetEmployeeReportsByReportsToIdAsync(int reportToId);
        Task<EmployeeReport> GetEmployeeReportByIdAsync(int employeeReportId);
        Task<bool> AddEmployeeReportAsync(EmployeeReport employeeReport);
        #endregion

        #region Employee Settings Service Methods
        Task<IList<EmployeeCategory>> GetAllEmployeeCategoriesAsync();
        Task<IList<EmployeeType>> GetAllEmployeeTypesAsync();

        #endregion
    }
}