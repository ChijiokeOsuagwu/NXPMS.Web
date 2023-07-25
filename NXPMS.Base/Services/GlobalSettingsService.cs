using NXPMS.Base.Models.EmployeesModels;
using NXPMS.Base.Models.GlobalSettingsModels;
using NXPMS.Base.Repositories.EmployeeRecordRepositories;
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
        private readonly IEmployeesRepository _employeesRepository;

        public GlobalSettingsService(ILocationsRepository locationRepository,
            IDepartmentRepository departmentRepository, IUnitRepository unitRepository,
            IStateRepository stateRepository, ICountryRepository countryRepository,
            ISystemRepository systemRepository, IEmployeesRepository employeesRepository)
        {
            _locationRepository = locationRepository;
            _departmentRepository = departmentRepository;
            _unitRepository = unitRepository;
            _stateRepository = stateRepository;
            _countryRepository = countryRepository;
            _employeesRepository = employeesRepository;
            _systemRepository = systemRepository;
        }

        #region Locations Service Methods
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

        public async Task<Location> GetLocationByIdAsync(int locationId)
        {
            Location location = new Location();
            var entities = await _locationRepository.GetByIdAsync(locationId);
            if (entities != null && entities.Count > 0)
            {
                location = entities.FirstOrDefault();
            }
            return location;
        }

        public async Task<Location> GetLocationByNameAsync(string locationName)
        {
            Location location = new Location();
            var entities = await _locationRepository.GetByNameAsync(locationName);
            if (entities != null && entities.Count > 0)
            {
                location = entities.FirstOrDefault();
            }
            return location;
        }

        public async Task<bool> AddLocationAsync(Location location)
        {
            if (location == null) { throw new Exception("Location Entity cannot be null."); }
            var same_code_entities = await _locationRepository.GetByNameAsync(location.LocationName);
            if (same_code_entities != null && same_code_entities.Count > 0)
            {
                throw new Exception("Duplicate Entry Error. There is an existing Location with the same name in the system.");
            }

           if (!string.IsNullOrWhiteSpace(location.LocationHeadName))
            {
                var hod_entity = await _employeesRepository.GetByNameAsync(location.LocationHeadName);
                if (hod_entity != null && hod_entity.Count > 0)
                {
                    Employee hod_emp = hod_entity.FirstOrDefault();
                    location.LocationHeadId = hod_emp.EmployeeID;
                }
                else
                {
                    location.LocationHeadId = null;
                }
            }
            else
            {
                location.LocationHeadId = null;
            }

            if (!string.IsNullOrWhiteSpace(location.LocationAltHeadName))
            {
                var alt_hod_entity = await _employeesRepository.GetByNameAsync(location.LocationAltHeadName);
                if (alt_hod_entity != null && alt_hod_entity.Count > 0)
                {
                    Employee alt_hod_emp = alt_hod_entity.FirstOrDefault();
                    location.LocationAltHeadId = alt_hod_emp.EmployeeID;
                }
                else
                {
                    location.LocationAltHeadId = null;
                }
            }
            else
            {
                location.LocationAltHeadId = null;
            }
            return await _locationRepository.AddAsync(location);
        }

        public async Task<bool> UpdateLocationAsync(Location location)
        {
            if (location == null) { throw new Exception("Invalid Argument Exception: Location entity cannot be null."); }
            var original_entity_list = await _locationRepository.GetByNameAsync(location.LocationName);
            if (original_entity_list == null || original_entity_list.Count < 1)
            {
                throw new Exception("No Location was found with this Name.");
            }

            if (!string.IsNullOrWhiteSpace(location.LocationHeadName))
            {
                var hod_entity = await _employeesRepository.GetByNameAsync(location.LocationHeadName);
                if (hod_entity != null && hod_entity.Count > 0)
                {
                    Employee hod_emp = hod_entity.FirstOrDefault();
                    location.LocationHeadId = hod_emp.EmployeeID;
                }
                else
                {
                    location.LocationHeadId = null;
                }
            }
            else
            {
                location.LocationHeadId = null;
            }

            if (!string.IsNullOrWhiteSpace(location.LocationAltHeadName))
            {
                var alt_hod_entity = await _employeesRepository.GetByNameAsync(location.LocationAltHeadName);
                if (alt_hod_entity != null && alt_hod_entity.Count > 0)
                {
                    Employee alt_hod_emp = alt_hod_entity.FirstOrDefault();
                    location.LocationAltHeadId = alt_hod_emp.EmployeeID;
                }
                else
                {
                    location.LocationAltHeadId = null;
                }
            }
            else
            {
                location.LocationAltHeadId = null;
            }

            return await _locationRepository.UpdateAsync(location);
        }

        public async Task<bool> DeleteLocationAsync(int locationId)
        {
            if (locationId < 1) { throw new ArgumentNullException(); }
            return await _locationRepository.DeleteAsync(locationId);
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

        public async Task<Department> GetDepartmentByCodeAsync(string departmentCode)
        {
            Department department = new Department();
            var entities = await _departmentRepository.GetByCodeAsync(departmentCode);
            if (entities != null && entities.Count > 0)
            {
                department = entities.FirstOrDefault();
            }
            return department;
        }

        public async Task<Department> GetDepartmentByNameeAsync(string departmentName)
        {
            Department department = new Department();
            var entities = await _departmentRepository.GetByNameAsync(departmentName);
            if (entities != null && entities.Count > 0)
            {
                department = entities.FirstOrDefault();
            }
            return department;
        }
        
        public async Task<bool> AddDepartmentAsync(Department department)
        {
            if (department == null) { throw new Exception("Department Entity cannot be null."); }
            var same_code_entities = await _departmentRepository.GetByCodeAsync(department.DepartmentCode);
            if (same_code_entities != null && same_code_entities.Count > 0)
            {
                throw new Exception("Duplicate Entry Error. There is an existing Department with the same Code in the system.");
            }

            var same_name_entities = await _departmentRepository.GetByNameAsync(department.DepartmentName);
            if (same_name_entities != null && same_name_entities.Count > 0)
            {
                throw new Exception("Duplicate Entry Error. There is an existing Department with the same Name in the system.");
            }

            if (!string.IsNullOrWhiteSpace(department.DepartmentHeadName))
            {
                var hod_entity = await _employeesRepository.GetByNameAsync(department.DepartmentHeadName);
                if (hod_entity != null && hod_entity.Count > 0)
                {
                    Employee hod_emp = hod_entity.FirstOrDefault();
                    department.DepartmentHeadId = hod_emp.EmployeeID;
                }
                else
                {
                    department.DepartmentHeadId = null;
                }
            }
            else
            {
                department.DepartmentHeadId = null;
            }

            if (!string.IsNullOrWhiteSpace(department.DepartmentAltHeadName))
            {
                var alt_hod_entity = await _employeesRepository.GetByNameAsync(department.DepartmentAltHeadName);
                if (alt_hod_entity != null && alt_hod_entity.Count > 0)
                {
                    Employee alt_hod_emp = alt_hod_entity.FirstOrDefault();
                    department.DepartmentAltHeadId = alt_hod_emp.EmployeeID;
                }
                else
                {
                    department.DepartmentAltHeadId = null;
                }
            }
            else
            {
                department.DepartmentAltHeadId = null;
            }
            return await _departmentRepository.AddAsync(department);
        }

        public async Task<bool> UpdateDepartmentAsync(Department department)
        {
            if (department == null) { throw new Exception("Invalid Argument Exception: Department entity cannot be null."); }
            var original_entity_list = await _departmentRepository.GetByCodeAsync(department.DepartmentCode);
            if (original_entity_list == null || original_entity_list.Count < 1)
            {
                throw new Exception("No Department was found with this Code.");
            }

            if (!string.IsNullOrWhiteSpace(department.DepartmentHeadName))
            {
                var hod_entity = await _employeesRepository.GetByNameAsync(department.DepartmentHeadName);
                if (hod_entity != null && hod_entity.Count > 0)
                {
                    Employee hod_emp = hod_entity.FirstOrDefault();
                    department.DepartmentHeadId = hod_emp.EmployeeID;
                }
                else
                {
                    department.DepartmentHeadId = null;
                }
            }
            else
            {
                department.DepartmentHeadId = null;
            }

            if (!string.IsNullOrWhiteSpace(department.DepartmentAltHeadName))
            {
                var alt_hod_entity = await _employeesRepository.GetByNameAsync(department.DepartmentAltHeadName);
                if (alt_hod_entity != null && alt_hod_entity.Count > 0)
                {
                    Employee alt_hod_emp = alt_hod_entity.FirstOrDefault();
                    department.DepartmentAltHeadId = alt_hod_emp.EmployeeID;
                }
                else
                {
                    department.DepartmentAltHeadId = null;
                }
            }
            else
            {
                department.DepartmentAltHeadId = null;
            }

            return await _departmentRepository.UpdateAsync(department);
        }

        public async Task<bool> DeleteDepartmentAsync(string departmentCode)
        {
            if (string.IsNullOrWhiteSpace(departmentCode)) { throw new ArgumentNullException(); }
            return await _departmentRepository.DeleteAsync(departmentCode);
        }

        #endregion

        #region Units Service Methods

        public async Task<List<Unit>> GetUnitsAsync(string departmentCode = null)
        {
            List<Unit> units = new List<Unit>();
            if (string.IsNullOrWhiteSpace(departmentCode))
            {
                var entities = await _unitRepository.GetAllAsync();
                if (entities != null && entities.Count > 0)
                {
                    units = entities.ToList();
                }
            }
            else
            {
                var entities = await _unitRepository.GetByDepartmentCodeAsync(departmentCode);
                if (entities != null && entities.Count > 0)
                {
                    units = entities.ToList();
                }
            }
            return units;
        }

        public async Task<Unit> GetUnitByCodeAsync(string unitCode)
        {
            Unit unit = new Unit();
            var entities = await _unitRepository.GetByCodeAsync(unitCode);
            if (entities != null && entities.Count > 0)
            {
                unit = entities.FirstOrDefault();
            }
            return unit;
        }

        public async Task<bool> AddUnitAsync(Unit unit)
        {
            if (unit == null) { throw new Exception("Unit Entity cannot be null."); }
            var same_code_entities = await _unitRepository.GetByCodeAsync(unit.UnitCode);
            if (same_code_entities != null && same_code_entities.Count > 0)
            {
                throw new Exception("Duplicate Entry Error. There is an existing Unit with the same Code in the system.");
            }

            var same_name_entities = await _unitRepository.GetByNameAsync(unit.UnitName);
            if (same_name_entities != null && same_name_entities.Count > 0)
            {
                throw new Exception("Duplicate Entry Error. There is an existing Unit with the same Name in the system.");
            }

            if (!string.IsNullOrWhiteSpace(unit.UnitHeadName))
            {
                var hod_entity = await _employeesRepository.GetByNameAsync(unit.UnitHeadName);
                if (hod_entity != null && hod_entity.Count > 0)
                {
                    Employee hod_emp = hod_entity.FirstOrDefault();
                    unit.UnitHeadId = hod_emp.EmployeeID;
                }
                else
                {
                    unit.UnitHeadId = null;
                }
            }
            else
            {
                unit.UnitHeadId = null;
            }

            if (!string.IsNullOrWhiteSpace(unit.UnitAltHeadName))
            {
                var alt_hod_entity = await _employeesRepository.GetByNameAsync(unit.UnitAltHeadName);
                if (alt_hod_entity != null && alt_hod_entity.Count > 0)
                {
                    Employee alt_hod_emp = alt_hod_entity.FirstOrDefault();
                    unit.UnitAltHeadId = alt_hod_emp.EmployeeID;
                }
                else
                {
                    unit.UnitAltHeadId = null;
                }
            }
            else
            {
                unit.UnitAltHeadId = null;
            }
            return await _unitRepository.AddAsync(unit);
        }

        public async Task<bool> UpdateUnitAsync(Unit unit)
        {
            if (unit == null) { throw new Exception("Invalid Argument Exception: Unit entity cannot be null."); }
            var original_entity_list = await _unitRepository.GetByCodeAsync(unit.UnitCode);
            if (original_entity_list == null || original_entity_list.Count < 1)
            {
                throw new Exception("No Department was found with this Code.");
            }

            if (!string.IsNullOrWhiteSpace(unit.UnitHeadName))
            {
                var hod_entity = await _employeesRepository.GetByNameAsync(unit.UnitHeadName);
                if (hod_entity != null && hod_entity.Count > 0)
                {
                    Employee hod_emp = hod_entity.FirstOrDefault();
                    unit.UnitHeadId = hod_emp.EmployeeID;
                }
                else
                {
                    unit.UnitHeadId = null;
                }
            }
            else
            {
                unit.UnitHeadId = null;
            }

            if (!string.IsNullOrWhiteSpace(unit.UnitAltHeadName))
            {
                var alt_hod_entity = await _employeesRepository.GetByNameAsync(unit.UnitAltHeadName);
                if (alt_hod_entity != null && alt_hod_entity.Count > 0)
                {
                    Employee alt_hod_emp = alt_hod_entity.FirstOrDefault();
                    unit.UnitAltHeadId = alt_hod_emp.EmployeeID;
                }
                else
                {
                    unit.UnitAltHeadId = null;
                }
            }
            else
            {
                unit.UnitAltHeadId = null;
            }

            return await _unitRepository.UpdateAsync(unit);
        }

        public async Task<bool> DeleteUnitAsync(string unitCode)
        {
            if (string.IsNullOrWhiteSpace(unitCode)) { throw new ArgumentNullException(); }
            return await _unitRepository.DeleteAsync(unitCode);
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

        public async Task<List<Industry>> GetIndustriesAsync()
        {
            List<Industry> industries = new List<Industry>();
            var entities = await _systemRepository.GetAllIndustriesAsync();
            if (entities != null && entities.Count > 0)
            {
                industries = entities.ToList();
            }
            return industries;
        }

        #endregion

    }
}
