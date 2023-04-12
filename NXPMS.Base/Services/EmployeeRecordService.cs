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
    public class EmployeeRecordService : IEmployeeRecordService
    {
        private readonly IEmployeesRepository _employeesRepository;
        private readonly IEmployeeCategoryRepository _employeeCategoryRepository;
        private readonly IEmployeeTypeRepository _employeeTypeRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IStateRepository _stateRepository;

        public EmployeeRecordService(IEmployeesRepository employeesRepository,
            IEmployeeCategoryRepository employeeCategoryRepository, IEmployeeTypeRepository employeeTypeRepository,
            IUnitRepository unitRepository, IStateRepository stateRepository)
        {
            _employeesRepository = employeesRepository;
            _employeeCategoryRepository = employeeCategoryRepository;
            _employeeTypeRepository = employeeTypeRepository;
            _unitRepository = unitRepository;
            _stateRepository = stateRepository;
        }

        #region Employees Read Service Methods
        public async Task<Employee> GetEmployeeByFullNameAsync(string FullName)
        {
            Employee employee = new Employee();
            var entities = await _employeesRepository.GetByNameAsync(FullName);
            if (entities != null && entities.Count > 0)
            {
                employee = entities.ToList().FirstOrDefault();
            }
            return employee;
        }

        public async Task<Employee> GetEmployeeByIdAsync(int employeeId)
        {
            Employee employee = new Employee();
            var entities = await _employeesRepository.GetByIdAsync(employeeId);
            if (entities != null && entities.Count > 0)
            {
                employee = entities.ToList().FirstOrDefault();
            }
            return employee;
        }

        public async Task<IList<Employee>> SearchEmployeesByNameAsync(string Name)
        {
            List<Employee> employees = new List<Employee>();
            var entities = await _employeesRepository.FindByNameAsync(Name);
            if (entities != null && entities.Count > 0)
            {
                employees = entities.ToList();
            }
            return employees;
        }

        public async Task<IList<Employee>> SearchNonUserEmployeesByNameAsync(string Name)
        {
            List<Employee> employees = new List<Employee>();
            var entities = await _employeesRepository.FindNonUsersByNameAsync(Name);
            if (entities != null && entities.Count > 0)
            {
                employees = entities.ToList();
            }
            return employees;
        }

        public async Task<IList<Employee>> GetAllEmployeesAsync()
        {
            List<Employee> employees = new List<Employee>();
            var entities = await _employeesRepository.GetAllAsync();
            if (entities != null && entities.Count > 0)
            {
                employees = entities.ToList();
            }
            return employees;
        }

        public async Task<IList<Employee>> GetEmployeesByLocationAsync(int LocationID)
        {
            List<Employee> employees = new List<Employee>();
            var entities = await _employeesRepository.GetByLocationIdAsync(LocationID);
            if (entities != null && entities.Count > 0)
            {
                employees = entities.ToList();
            }
            return employees;
        }
        
        public async Task<IList<Employee>> GetEmployeesByDepartmentAsync(string DepartmentCode, int LocationID = 0)
        {
            List<Employee> employees = new List<Employee>();
            if(LocationID > 0)
            {
                var entities = await _employeesRepository.GetByLocationIdAndDepartmentCodeAsync(LocationID, DepartmentCode);
                if (entities != null && entities.Count > 0)
                {
                    employees = entities.ToList();
                }
            }
            else
            {
                var entities = await _employeesRepository.GetByDepartmentCodeAsync(DepartmentCode);
                if (entities != null && entities.Count > 0)
                {
                    employees = entities.ToList();
                }
            }

            return employees;
        }

        public async Task<IList<Employee>> GetEmployeesByUnitAsync(string UnitCode, int LocationID = 0)
        {
            List<Employee> employees = new List<Employee>();
            if(LocationID > 0)
            {
                var entities = await _employeesRepository.GetByLocationIdAndUnitCodeAsync(LocationID, UnitCode);
                if (entities != null && entities.Count > 0)
                {
                    employees = entities.ToList();
                }
            }
            else
            {
                var entities = await _employeesRepository.GetByUnitCodeAsync(UnitCode);
                if (entities != null && entities.Count > 0)
                {
                    employees = entities.ToList();
                }
            }

            return employees;
        }
        #endregion

        #region Employees Write Service Methods

        public async Task<bool> AddEmployeePersonalInfoAsync(Employee employee)
        {
            if(employee == null) { throw new ArgumentNullException(nameof(employee)); }
            var entities = await _employeesRepository.GetByNameAsync(employee.FullName);
            if(entities != null && entities.Count > 0)
            {
                List<Employee> existingEmployees = entities.ToList();
                foreach(Employee emp in existingEmployees)
                {
                    if(emp.BirthDay == employee.BirthDay && emp.BirthMonth == employee.BirthMonth && emp.LocalGovernmentOfOrigin == employee.LocalGovernmentOfOrigin)
                    {
                        throw new Exception("This Employee already exists in the system.");
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(employee.StateOfOrigin))
            {
                var states = await _stateRepository.GetByNameAsync(employee.StateOfOrigin);
                if(states != null && states.Count > 0)
                {
                    State st = states.ToList().FirstOrDefault();
                    if(st != null && !string.IsNullOrWhiteSpace(st.Region))
                    {
                        employee.GeoPoliticalRegion = st.Region;
                    }
                }
            }
            return await _employeesRepository.AddEmployeePersonalInfoOnlyAsync(employee);
        }

        public async Task<bool> UpdateEmployeePersonalInfoAsync(Employee employee)
        {
            if (employee == null) { throw new ArgumentNullException(nameof(employee)); }
            return await _employeesRepository.UpdateEmployeePersonalInfoOnlyAsync(employee);
        }

        public async Task<bool> UpdateEmployeeEmploymentInfoAsync(Employee employee)
        {
            if (employee == null) { throw new ArgumentNullException(nameof(employee)); }

            if (!string.IsNullOrWhiteSpace(employee.UnitCode))
            {
                var entities = await _unitRepository.GetByCodeAsync(employee.UnitCode);
                if (entities != null && entities.Count > 0)
                {
                    Unit unit = entities.ToList().FirstOrDefault();
                    if(unit != null && !string.IsNullOrWhiteSpace(unit.DepartmentCode))
                    {
                        employee.DepartmentCode = unit.DepartmentCode;
                    }
                }
            }

            if (employee.EmployeeTypeID != null && employee.EmployeeTypeID > 0)
            {
                var employeeTypes = await _employeeTypeRepository.GetByIdAsync(employee.EmployeeTypeID.Value);
                if(employeeTypes != null && employeeTypes.Count > 0)
                {
                    EmployeeType employeeType = employeeTypes.ToList().FirstOrDefault();
                if (employeeType != null && employeeType.EmployeeCategoryId > 0)
                {
                    employee.EmployeeCategoryID = employeeType.EmployeeCategoryId;
                }
                }
            }
            return await _employeesRepository.UpdateEmployeeInfoOnlyAsync(employee);
        }

        public async Task<bool> UpdateEmployeeNextOfKinInfoAsync(Employee employee)
        {
            if (employee == null) { throw new ArgumentNullException(nameof(employee)); }
            return await _employeesRepository.UpdateEmployeeNextOfKinInfoOnlyAsync(employee);
        }

        #endregion

        #region Employee Settings Service Methods
        public async Task<IList<EmployeeCategory>> GetAllEmployeeCategoriesAsync()
        {
            List<EmployeeCategory> employeeCategories = new List<EmployeeCategory>();
            var entities = await _employeeCategoryRepository.GetAllAsync();
            if (entities != null && entities.Count > 0)
            {
                employeeCategories = entities.ToList();
            }
            return employeeCategories;
        }

        public async Task<IList<EmployeeType>> GetAllEmployeeTypesAsync()
        {
            List<EmployeeType> employeeTypes = new List<EmployeeType>();
            var entities = await _employeeTypeRepository.GetAllAsync();
            if (entities != null && entities.Count > 0)
            {
                employeeTypes = entities.ToList();
            }
            return employeeTypes;
        }
        #endregion

    }
}
