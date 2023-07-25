using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using NXPMS.Base.Models.EmployeesModels;
using NXPMS.Base.Models.GlobalSettingsModels;
using NXPMS.Base.Services;
using NXPMS.Web.Models.EmployeesViewModels;

namespace NXPMS.Web.Controllers
{
    public class EmployeesController : Controller
    {
        //private readonly IConfiguration _configuration;
        //private readonly ISecurityService _securityService;
        private readonly IGlobalSettingsService _globalSettingsService;
        private readonly IEmployeeRecordService _employeeRecordService;

        public EmployeesController(IEmployeeRecordService employeeRecordService,
            IGlobalSettingsService globalSettingsService)
        {
            _employeeRecordService = employeeRecordService;
            _globalSettingsService = globalSettingsService;
        }

        #region Employee Read Controller Actions

        [Authorize(Roles = "ERMMNG, XXACC")]
        public IActionResult Records()
        {
            return View();
        }

        [Authorize(Roles = "ERMMNG, XXACC")]
        public async Task<IActionResult> Search(string est)
        {
            EmployeeSearchViewModel model = new EmployeeSearchViewModel();
            if (!string.IsNullOrWhiteSpace(est))
            {
                //EmployeeViewModel employee = new EmployeeViewModel();
                Employee entity = await _employeeRecordService.GetEmployeeByFullNameAsync(est);
                if (entity != null && entity.EmployeeID > 0)
                {
                    EmployeeViewModel employeeModel = new EmployeeViewModel();
                    employeeModel = employeeModel.ExtractFromEmployee(entity);
                    return RedirectToAction("Info", new { id = employeeModel.EmployeeID });
                }
                else
                {
                    var entities = await _employeeRecordService.SearchEmployeesByNameAsync(est);
                    if (entities != null && entities.Count > 0)
                    {
                        List<Employee> employees = new List<Employee>();
                        employees = entities.ToList();
                        model.EmployeesList = employees;
                    }
                }
            }
            return View(model);
        }

        [Authorize(Roles = "ERMMNG, XXACC")]
        public async Task<IActionResult> Listings(int ld = 0, string dc = null, string uc = null)
        {
            EmployeeListingsViewModel model = new EmployeeListingsViewModel();
            List<Location> locations = new List<Location>();
            List<Department> departments = new List<Department>();
            List<Unit> units = new List<Unit>();
            if (ld > 0)
            {
                if (!string.IsNullOrWhiteSpace(uc))
                {
                    var entities = await _employeeRecordService.GetEmployeesByUnitAsync(uc, ld);
                    if (entities != null && entities.Count > 0)
                    {
                        List<Employee> employees = new List<Employee>();
                        employees = entities.ToList();
                        model.EmployeesList = employees;
                    }
                }
                else if (!string.IsNullOrWhiteSpace(dc))
                {
                    var entities = await _employeeRecordService.GetEmployeesByDepartmentAsync(dc, ld);
                    if (entities != null && entities.Count > 0)
                    {
                        List<Employee> employees = new List<Employee>();
                        employees = entities.ToList();
                        model.EmployeesList = employees;
                    }
                }
                else
                {
                    var entities = await _employeeRecordService.GetEmployeesByLocationAsync(ld);
                    if (entities != null && entities.Count > 0)
                    {
                        List<Employee> employees = new List<Employee>();
                        employees = entities.ToList();
                        model.EmployeesList = employees;
                    }
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(uc))
                {
                    var entities = await _employeeRecordService.GetEmployeesByUnitAsync(uc);
                    if (entities != null && entities.Count > 0)
                    {
                        List<Employee> employees = new List<Employee>();
                        employees = entities.ToList();
                        model.EmployeesList = employees;
                    }
                }
                else if (!string.IsNullOrWhiteSpace(dc))
                {
                    var entities = await _employeeRecordService.GetEmployeesByDepartmentAsync(dc);
                    if (entities != null && entities.Count > 0)
                    {
                        List<Employee> employees = new List<Employee>();
                        employees = entities.ToList();
                        model.EmployeesList = employees;
                    }
                }

            }
            locations = await _globalSettingsService.GetLocationsAsync();
            departments = await _globalSettingsService.GetDepartmentsAsync();
            units = await _globalSettingsService.GetUnitsAsync();

            ViewBag.LocationsList = new SelectList(locations, "LocationId", "LocationName", ld);
            ViewBag.DepartmentsList = new SelectList(departments, "DepartmentCode", "DepartmentName", dc);
            ViewBag.UnitsList = new SelectList(units, "UnitCode", "UnitName", uc);

            return View(model);
        }

        public async Task<IActionResult> Info(int id)
        {
            EmployeeInfoViewModel model = new EmployeeInfoViewModel();
            EmployeePersonalInfoViewModel personalModel = new EmployeePersonalInfoViewModel();
            EmployeeEmploymentInfoViewModel employmentModel = new EmployeeEmploymentInfoViewModel();
            EmployeeNextOfKinInfoViewModel nextOfKinModel = new EmployeeNextOfKinInfoViewModel();
            Employee employee = new Employee();
            try
            {
                employee = await _employeeRecordService.GetEmployeeByIdAsync(id);

                personalModel = personalModel.ExtractFromEmployee(employee);
                employmentModel = employmentModel.ExtractFromEmployee(employee);
                nextOfKinModel = nextOfKinModel.ExtractFromEmployee(employee);

                model.EmploymentInfo = employmentModel;
                model.PersonalInfo = personalModel;
                model.NextOfKinInfo = nextOfKinModel;
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }
        #endregion

        #region Employees Write Controller Actions

        [Authorize(Roles = "ERMMNG, XXACC")]
        public async Task<IActionResult> AddPersonalInfo(int? id = null)
        {
            EmployeePersonalInfoViewModel model = new EmployeePersonalInfoViewModel();
            if (id != null && id > 0)
            {
                model.EmployeeID = id.Value;
                Employee employee = await _employeeRecordService.GetEmployeeByIdAsync(id.Value);
                model = model.ExtractFromEmployee(employee);
                //model.OperationIsSuccessful = true;
            }
            var locations = await _globalSettingsService.GetLocationsAsync();
            var states = await _globalSettingsService.GetStatesAsync();
            var countries = await _globalSettingsService.GetCountriesAsync();
            var units = await _globalSettingsService.GetUnitsAsync();

            ViewBag.StateList = new SelectList(states, "StateName", "StateName");
            ViewBag.CountryList = new SelectList(countries, "CountryName", "CountryName");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ERMMNG, XXACC")]
        public async Task<IActionResult> AddPersonalInfo(EmployeePersonalInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(model.Title)) { model.Title = model.Title.ToUpper(); }
                    if (!string.IsNullOrWhiteSpace(model.FirstName)) { model.FirstName = model.FirstName.ToUpper(); }
                    if (!string.IsNullOrWhiteSpace(model.Surname)) { model.Surname = model.Surname.ToUpper(); }
                    if (!string.IsNullOrWhiteSpace(model.OtherNames)) { model.OtherNames = model.OtherNames.ToUpper(); }

                    Employee employee = model.ConvertToEmployee();
                    if (!string.IsNullOrWhiteSpace(employee.StateOfOrigin))
                    {
                        State state = await _globalSettingsService.GetStateByNameAsync(employee.StateOfOrigin);
                        if (state != null)
                        {
                            employee.GeoPoliticalRegion = state.Region;
                        }
                    }

                    if (employee.EmployeeID < 1)
                    {
                        employee.CreatedBy = HttpContext.User.Identity.Name;
                        employee.CreatedTime = DateTime.UtcNow;

                        bool EmployeeIsCreated = await _employeeRecordService.AddEmployeePersonalInfoAsync(employee);
                        if (EmployeeIsCreated)
                        {
                            Employee new_employee = await _employeeRecordService.GetEmployeeByFullNameAsync(employee.FullName);
                            if (new_employee != null && new_employee.EmployeeID > 0) { model.EmployeeID = new_employee.EmployeeID; }
                            return RedirectToAction("Info", new { id = model.EmployeeID });
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "An unknown error was encountered. Creating new employee failed. ";
                        }
                    }
                    else
                    {
                        employee.LastModifiedBy = HttpContext.User.Identity.Name;
                        employee.LastModifiedTime = DateTime.UtcNow;

                        bool EmployeeIsUpdated = await _employeeRecordService.UpdateEmployeePersonalInfoAsync(employee);
                        if (EmployeeIsUpdated)
                        {
                            return RedirectToAction("Info", new { id = employee.EmployeeID });
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "An unknown error was encountered. Updating employee failed. ";
                        }
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            var states = await _globalSettingsService.GetStatesAsync();

            ViewBag.StateList = new SelectList(states, "StateName", "StateName");
            return View(model);
        }

        [Authorize(Roles = "ERMMNG, XXACC")]
        public async Task<IActionResult> AddEmploymentInfo(int id)
        {
            EmployeeEmploymentInfoViewModel model = new EmployeeEmploymentInfoViewModel();
            model.EmployeeID = id;
            if (id > 0)
            {
                Employee employee = await _employeeRecordService.GetEmployeeByIdAsync(id);
                if (employee != null && !string.IsNullOrWhiteSpace(employee.FullName))
                {
                    model = model.ExtractFromEmployee(employee);
                }
            }

            var locations = await _globalSettingsService.GetLocationsAsync();
            var units = await _globalSettingsService.GetUnitsAsync();
            var categories = await _employeeRecordService.GetAllEmployeeCategoriesAsync();
            var types = await _employeeRecordService.GetAllEmployeeTypesAsync();

            ViewBag.LocationsList = new SelectList(locations, "LocationId", "LocationName");
            ViewBag.UnitsList = new SelectList(units, "UnitCode", "UnitName");
            ViewBag.CategoriesList = new SelectList(categories, "EmployeeCategoryId", "EmployeeCategoryName");
            ViewBag.TypesList = new SelectList(types, "EmployeeTypeId", "EmployeeTypeName");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ERMMNG, XXACC")]
        public async Task<IActionResult> AddEmploymentInfo(EmployeeEmploymentInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Employee employee = model.ConvertToEmployee();
                    employee.LastModifiedBy = HttpContext.User.Identity.Name;
                    employee.LastModifiedTime = DateTime.UtcNow;

                    bool EmployeeIsUpdated = await _employeeRecordService.UpdateEmployeeEmploymentInfoAsync(employee);
                    if (EmployeeIsUpdated)
                    {
                        return RedirectToAction("Info", new { id = employee.EmployeeID });
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "An unknown error was encountered. Adding Employment Info failed. ";
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            var locations = await _globalSettingsService.GetLocationsAsync();
            var units = await _globalSettingsService.GetUnitsAsync();
            var types = await _employeeRecordService.GetAllEmployeeTypesAsync();

            ViewBag.LocationsList = new SelectList(locations, "LocationId", "LocationName");
            ViewBag.UnitsList = new SelectList(units, "UnitCode", "UnitName");
            ViewBag.TypesList = new SelectList(types, "EmployeeTypeId", "EmployeeTypeName");
            return View(model);
        }

        [Authorize(Roles = "ERMMNG, XXACC")]
        public async Task<IActionResult> AddNextOfKinInfo(int id)
        {
            EmployeeNextOfKinInfoViewModel model = new EmployeeNextOfKinInfoViewModel();
            model.EmployeeID = id;
            if (id > 0)
            {
                Employee employee = await _employeeRecordService.GetEmployeeByIdAsync(id);
                if (employee != null && !string.IsNullOrWhiteSpace(employee.FullName))
                {
                    model = model.ExtractFromEmployee(employee);
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ERMMNG, XXACC")]
        public async Task<IActionResult> AddNextOfKinInfo(EmployeeNextOfKinInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Employee employee = model.ConvertToEmployee();
                    employee.LastModifiedBy = HttpContext.User.Identity.Name;
                    employee.LastModifiedTime = DateTime.UtcNow;

                    bool EmployeeIsUpdated = await _employeeRecordService.UpdateEmployeeNextOfKinInfoAsync(employee);
                    if (EmployeeIsUpdated)
                    {
                        return RedirectToAction("Info", new { id = employee.EmployeeID });
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "An unknown error was encountered. Adding Employment Info failed. ";
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }

        #endregion

        #region Employee Reports Controller Actions

        [Authorize(Roles = "ERMVOR, ERMMNG, XXACC")]
        public async Task<IActionResult> ReportingLines(int id)
        {
            EmployeeReportListViewModel model = new EmployeeReportListViewModel();
            model.ID = id;
            if (id > 0)
            {
                var entities = await _employeeRecordService.GetEmployeeReportsByEmployeeIdAsync(id);
                if (entities != null && entities.Count > 0)
                {
                    model.EmployeeReportList = entities.ToList();
                }
            }
            return View(model);
        }

        [Authorize(Roles = "ERMVOR, ERMMNG, XXACC")]
        public async Task<IActionResult> ManageReportingLine(int id, int? rd = null)
        {
            EmployeeReportLineViewModel model = new EmployeeReportLineViewModel();
            model.EmployeeId = id;
            if (rd != null && rd > 0)
            {
                EmployeeReport employeeReport = new EmployeeReport();
                employeeReport = await _employeeRecordService.GetEmployeeReportByIdAsync(rd.Value);
                model = model.ExtractToModel(employeeReport);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ERMVOR, ERMMNG, XXACC")]
        public async Task<IActionResult> ManageReportingLine(EmployeeReportLineViewModel model)
        {
            EmployeeReport employeeReport = new EmployeeReport();
            if (ModelState.IsValid)
            {
                Employee reportsToEmployee = new Employee();
                employeeReport = model.ConvertToEmployeeReport();
                reportsToEmployee = await _employeeRecordService.GetEmployeeByFullNameAsync(model.ReportsToName);
                if (reportsToEmployee != null && reportsToEmployee.EmployeeID > 0)
                {
                    employeeReport.ReportsToDepartmentCode = reportsToEmployee.DepartmentCode;
                    employeeReport.ReportsToDesignation = reportsToEmployee.CurrentDesignation;
                    employeeReport.ReportsToId = reportsToEmployee.EmployeeID;
                    employeeReport.ReportsToLocationId = reportsToEmployee.LocationID;
                    employeeReport.ReportsToUnitCode = reportsToEmployee.UnitCode;
                    bool IsSuccessful = await _employeeRecordService.AddEmployeeReportAsync(employeeReport);
                    if (IsSuccessful)
                    {
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = "New Reporting Line added successfully!";
                    }
                    else
                    {
                        model.ViewModelSuccessMessage = "An error was encountered. Adding new Reporting Line failed.";
                    }
                }
                else
                {
                    model.ViewModelErrorMessage = "No record was found for the selected Report.";
                }
            }
            return View(model);
        }

        public async Task<IActionResult> ShowReportingLine(int id)
        {
            EmployeeReportLineViewModel model = new EmployeeReportLineViewModel();
            model.EmployeeId = id;
            EmployeeReport employeeReport = new EmployeeReport();
            employeeReport = await _employeeRecordService.GetEmployeeReportByIdAsync(id);
            model = model.ExtractToModel(employeeReport);
            return View(model);
        }

        #endregion


        //======================= Helper Action Methods ======================//
        #region Helper Methods
        public JsonResult GetEmployeeNames(string name)
        {
            List<string> employees = new List<string>();
            if (!string.IsNullOrWhiteSpace(name))
            {
                employees = _employeeRecordService.SearchEmployeesByNameAsync(name).Result.Select(x => x.FullName).ToList();
            }
            return Json(employees);
        }
        #endregion
    }
}