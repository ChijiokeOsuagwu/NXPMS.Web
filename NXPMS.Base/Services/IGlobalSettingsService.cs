using NXPMS.Base.Models.GlobalSettingsModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NXPMS.Base.Services
{
    public interface IGlobalSettingsService
    {
        #region Location Service Methods
        Task<List<Location>> GetLocationsAsync();
        #endregion

        #region Departments Service Methods
        Task<List<Department>> GetDepartmentsAsync();
        #endregion

        #region Units Service Methods
        Task<List<Unit>> GetUnitsAsync();
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

        #endregion
    }
}