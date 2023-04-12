using NXPMS.Base.Models.GlobalSettingsModels;
using NXPMS.Base.Repositories.GlobalSettingsRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NXPMS.Base.Services
{
    public class GlobalSettingsService : IGlobalSettingsService
    {
        private readonly ILocationsRepository _locationRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IStateRepository _stateRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly ISystemRepository _systemRepository;

        public GlobalSettingsService(ILocationsRepository locationRepository,
            IDepartmentRepository departmentRepository, IUnitRepository unitRepository,
            IStateRepository stateRepository, ICountryRepository countryRepository,
            ISystemRepository systemRepository)
        {
            _locationRepository = locationRepository;
            _departmentRepository = departmentRepository;
            _unitRepository = unitRepository;
            _stateRepository = stateRepository;
            _countryRepository = countryRepository;
            _systemRepository = systemRepository;
        }

        #region Locations Action Methods
        public async Task<List<Location>> GetLocationsAsync()
        {
            List<Location> locations = new List<Location>();
            var entities = await _locationRepository.GetAllAsync();
            if (entities != null && entities.Count > 0)
            {
                locations = entities.ToList();
            }
            return locations;
        }

        #endregion

        #region Departments Service Methods
        public async Task<List<Department>> GetDepartmentsAsync()
        {
            List<Department> departments = new List<Department>();
            var entities = await _departmentRepository.GetAllAsync();
            if (entities != null && entities.Count > 0)
            {
                departments = entities.ToList();
            }
            return departments;
        }
        #endregion

        #region Units Service Methods
        public async Task<List<Unit>> GetUnitsAsync()
        {
            List<Unit> units = new List<Unit>();
            var entities = await _unitRepository.GetAllAsync();
            if (entities != null && entities.Count > 0)
            {
                units = entities.ToList();
            }
            return units;
        }
        #endregion

        #region States Action Methods
        public async Task<List<State>> GetStatesAsync()
        {
            List<State> states = new List<State>();
            var entities = await _stateRepository.GetAllAsync();
            if (entities != null && entities.Count > 0)
            {
                states = entities.ToList();
            }
            return states;
        }

        public async Task<State> GetStateByNameAsync(string StateName)
        {
            State state = new State();
            var entities = await _stateRepository.GetByNameAsync(StateName);
            if (entities != null && entities.Count > 0)
            {
                state = entities.FirstOrDefault();
            }
            return state;
        }

        #endregion

        #region Countries Action Methods
        public async Task<List<Country>> GetCountriesAsync()
        {
            List<Country> countries = new List<Country>();
            var entities = await _countryRepository.GetAllAsync();
            if (entities != null && entities.Count > 0)
            {
                countries = entities.ToList();
            }
            return countries;
        }

        #endregion

        #region System  Service Methods
        public async Task<List<SystemApplication>> GetSystemApplicationsAsync()
        {
            List<SystemApplication> applications = new List<SystemApplication>();
            var entities = await _systemRepository.GetAllApplicationsAsync();
            if (entities != null && entities.Count > 0)
            {
                applications = entities.ToList();
            }
            return applications;
        }

        #endregion

    }
}
