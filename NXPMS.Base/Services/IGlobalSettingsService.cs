using NXPMS.Base.Models.GlobalSettingsModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NXPMS.Base.Services
{
    public interface IGlobalSettingsService
    {
        #region Location Service Methods
        Task<List<Location>> GetLocationsAsync();
        Task<Location> GetLocationByIdAsync(int locationId);
        Task<Location> GetLocationByNameAsync(string locationName);
        Task<bool> AddLocationAsync(Location location);
        Task<bool> UpdateLocationAsync(Location location);
        Task<bool> DeleteLocationAsync(int locationId);

        #endregion

        #region Departments Service Methods
        Task<List<Department>> GetDepartmentsAsync();
        Task<Department> GetDepartmentByCodeAsync(string departmentCode);
        Task<bool> AddDepartmentAsync(Department department);
        Task<bool> UpdateDepartmentAsync(Department department);
        Task<bool> DeleteDepartmentAsync(string departmentCode);

        #endregion

        #region Units Service Methods
       // Task<List<Unit>> GetUnitsAsync();
        Task<List<Unit>> GetUnitsAsync(string departmentCode = null);
        Task<Unit> GetUnitByCodeAsync(string unitCode);
        Task<bool> AddUnitAsync(Unit unit);
        Task<bool> UpdateUnitAsync(Unit unit);
        Task<bool> DeleteUnitAsync(string unitCode);
        #endregion

        #region States Service Methods
        Task<List<Country>> GetCountriesAsync();
        Task<State> GetStateByNameAsync(string StateName);

        #endregion

        #region Country Service Methods
        Task<List<State>> GetStatesAsync();
        #endregion

        #region System Service Methods
        Task<List<SystemApplication>> GetSystemApplicationsAsync();
        Task<List<Industry>> GetIndustriesAsync();
        #endregion
    }
}