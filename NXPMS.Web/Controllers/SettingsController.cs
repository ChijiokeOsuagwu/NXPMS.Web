using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NXPMS.Base.Models.GlobalSettingsModels;
using NXPMS.Base.Services;
using NXPMS.Web.Models.SettingsModels;

namespace NXPMS.Web.Controllers
{
    public class SettingsController : Controller
    {
        private readonly IGlobalSettingsService _globalSettingsService;

        public SettingsController(IGlobalSettingsService globalSettingsService)
        {
            _globalSettingsService = globalSettingsService;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Location Controller Action Methods

        [Authorize(Roles = "GBSSTT, XXACC")]
        public async Task<IActionResult> LocationList()
        {
            LocationListViewModel model = new LocationListViewModel();
            var entities = await _globalSettingsService.GetLocationsAsync();
            if (entities != null && entities.Count > 0)
            {
                model.LocationList = entities;
            }
            return View(model);
        }

        [Authorize(Roles = "GBSSTT, XXACC")]
        public async Task<IActionResult> ManageLocation(int id)
        {
            ManageLocationViewModel model = new ManageLocationViewModel();
            try
            {
                if (id > 0)
                {
                    model.IsUpdate = true;
                    model.LocationID = id;
                    Location location = await _globalSettingsService.GetLocationByIdAsync(id);
                    if (location != null && !string.IsNullOrWhiteSpace(location.LocationName))
                    {
                        model.LocationName = location.LocationName;
                        model.LocationState = location.LocationState;
                        model.LocationCountry = location.LocationCountry;
                        model.LocationHeadID = location.LocationHeadId;
                        model.LocationHeadName = location.LocationHeadName;
                        model.LocationAltHeadID = location.LocationAltHeadId;
                        model.LocationAltHeadName = location.LocationAltHeadName;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "GBSSTT, XXACC")]
        public async Task<IActionResult> ManageLocation(ManageLocationViewModel model)
        {
            try
            {
                Location location = new Location();
                if (ModelState.IsValid)
                {
                    location.LocationId = model.LocationID;
                    location.LocationName = model.LocationName;
                    location.LocationState = model.LocationState;
                    location.LocationCountry = model.LocationCountry;
                    location.LocationHeadName = model.LocationHeadName;
                    location.LocationAltHeadName = model.LocationAltHeadName;

                    if (!model.IsUpdate)
                    {
                        bool IsAdded = await _globalSettingsService.AddLocationAsync(location);
                        if (IsAdded)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "New Location added successfully!";
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                        }
                    }
                    else
                    {
                        bool IsUpdated = await _globalSettingsService.UpdateLocationAsync(location);
                        if (IsUpdated)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "Location updated successfully!";
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }


        [Authorize(Roles = "GBSSTT, XXACC")]
        public async Task<IActionResult> LocationDetail(int id)
        {
            ManageLocationViewModel model = new ManageLocationViewModel();
            try
            {
                if (id > 0)
                {
                    model.LocationID = id;
                    Location location = await _globalSettingsService.GetLocationByIdAsync(id);
                    if (location != null && !string.IsNullOrWhiteSpace(location.LocationName))
                    {
                        model.LocationID = location.LocationId;
                        model.LocationName = location.LocationName;
                        model.LocationState = location.LocationState;
                        model.LocationCountry = location.LocationCountry;
                        model.LocationHeadID = location.LocationHeadId;
                        model.LocationHeadName = location.LocationHeadName;
                        model.LocationAltHeadID = location.LocationAltHeadId;
                        model.LocationAltHeadName = location.LocationAltHeadName;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }


        [Authorize(Roles = "GBSSTT, XXACC")]
        public async Task<IActionResult> DeleteLocation(int id)
        {
            ManageLocationViewModel model = new ManageLocationViewModel();
            try
            {
                if (id > 0)
                {
                    model.LocationID = id;
                    Location location = await _globalSettingsService.GetLocationByIdAsync(id);
                    if (location != null && !string.IsNullOrWhiteSpace(location.LocationName))
                    {
                        model.LocationID = location.LocationId;
                        model.LocationName = location.LocationName;
                        model.LocationState = location.LocationState;
                        model.LocationCountry = location.LocationCountry;
                        model.LocationHeadID = location.LocationHeadId;
                        model.LocationHeadName = location.LocationHeadName;
                        model.LocationAltHeadID = location.LocationAltHeadId;
                        model.LocationAltHeadName = location.LocationAltHeadName;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "GBSSTT, XXACC")]
        public async Task<IActionResult> DeleteLocation(ManageLocationViewModel model)
        {
            try
            {
                if (model.LocationID > 0)
                {
                    bool IsDeleted = await _globalSettingsService.DeleteLocationAsync(model.LocationID);
                    if (IsDeleted)
                    {
                        return RedirectToAction("LocationList");
                    }
                    else
                    {
                        model.OperationIsSuccessful = false;
                        model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                    }
                }
                else
                {
                    model.OperationIsSuccessful = false;
                    model.ViewModelErrorMessage = "Sorry, a key parameter required to process your request is missing. Please try again.";
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }


        #endregion

        #region Department Controller Action Methods

        [Authorize(Roles = "GBSSTT, XXACC")]
        public async Task<IActionResult> DepartmentList()
        {
            DepartmentListViewModel model = new DepartmentListViewModel();
            var entities = await _globalSettingsService.GetDepartmentsAsync();
            if (entities != null && entities.Count > 0)
            {
                model.DepartmentList = entities;
            }
            return View(model);
        }

        [Authorize(Roles = "GBSSTT, XXACC")]
        public async Task<IActionResult> ManageDepartment(string cd)
        {
            ManageDepartmentViewModel model = new ManageDepartmentViewModel();
            try
            {
                if (!string.IsNullOrWhiteSpace(cd))
                {
                    model.IsUpdate = true;
                    model.OldCode = cd;
                    model.DepartmentCode = cd;
                    Department department = await _globalSettingsService.GetDepartmentByCodeAsync(cd);
                    if (department != null && !string.IsNullOrWhiteSpace(department.DepartmentName))
                    {
                        model.DepartmentCode = department.DepartmentCode;
                        model.DepartmentName = department.DepartmentName;
                        model.DepartmentHeadID = department.DepartmentHeadId;
                        model.DepartmentHeadName = department.DepartmentHeadName;
                        model.DepartmentAltHeadID = department.DepartmentAltHeadId;
                        model.DepartmentAltHeadName = department.DepartmentAltHeadName;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "GBSSTT, XXACC")]
        public async Task<IActionResult> ManageDepartment(ManageDepartmentViewModel model)
        {
            try
            {
                Department department = new Department();
                if (ModelState.IsValid)
                {
                    department.DepartmentCode = model.DepartmentCode;
                    department.DepartmentName = model.DepartmentName;
                    department.DepartmentHeadName = model.DepartmentHeadName;
                    department.DepartmentAltHeadName = model.DepartmentAltHeadName;

                    if (!model.IsUpdate)
                    {
                        bool IsAdded = await _globalSettingsService.AddDepartmentAsync(department);
                        if (IsAdded)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "New Department added successfully!";
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                        }
                    }
                    else
                    {
                        bool IsUpdated = await _globalSettingsService.UpdateDepartmentAsync(department);
                        if (IsUpdated)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "Department updated successfully!";
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }


        [Authorize(Roles = "GBSSTT, XXACC")]
        public async Task<IActionResult> DepartmentDetail(string cd)
        {
            ManageDepartmentViewModel model = new ManageDepartmentViewModel();
            try
            {
                if (!string.IsNullOrWhiteSpace(cd))
                {
                    model.DepartmentCode = cd;
                    Department department = await _globalSettingsService.GetDepartmentByCodeAsync(cd);
                    if (department != null && !string.IsNullOrWhiteSpace(department.DepartmentName))
                    {
                        model.DepartmentCode = department.DepartmentCode;
                        model.DepartmentName = department.DepartmentName;
                        model.DepartmentHeadID = department.DepartmentHeadId;
                        model.DepartmentHeadName = department.DepartmentHeadName;
                        model.DepartmentAltHeadID = department.DepartmentAltHeadId;
                        model.DepartmentAltHeadName = department.DepartmentAltHeadName;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }


        [Authorize(Roles = "GBSSTT, XXACC")]
        public async Task<IActionResult> DeleteDepartment(string cd)
        {
            ManageDepartmentViewModel model = new ManageDepartmentViewModel();
            try
            {
                if (!string.IsNullOrWhiteSpace(cd))
                {
                    model.DepartmentCode = cd;
                    Department department = await _globalSettingsService.GetDepartmentByCodeAsync(cd);
                    if (department != null && !string.IsNullOrWhiteSpace(department.DepartmentName))
                    {
                        model.DepartmentCode = department.DepartmentCode;
                        model.DepartmentName = department.DepartmentName;
                        model.DepartmentHeadID = department.DepartmentHeadId;
                        model.DepartmentHeadName = department.DepartmentHeadName;
                        model.DepartmentAltHeadID = department.DepartmentAltHeadId;
                        model.DepartmentAltHeadName = department.DepartmentAltHeadName;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "GBSSTT, XXACC")]
        public async Task<IActionResult> DeleteDepartment(ManageDepartmentViewModel model)
        {
            try
            {

                if (!string.IsNullOrWhiteSpace(model.DepartmentCode))
                {
                    bool IsDeleted = await _globalSettingsService.DeleteDepartmentAsync(model.DepartmentCode);
                    if (IsDeleted)
                    {
                        return RedirectToAction("DepartmentList");
                    }
                    else
                    {
                        model.OperationIsSuccessful = false;
                        model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                    }
                }
                else
                {
                    model.OperationIsSuccessful = false;
                    model.ViewModelErrorMessage = "Sorry, a key parameter required to process your request is missing. Please try again.";
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }


        #endregion

        #region Unit Controller Action Methods

        [Authorize(Roles = "GBSSTT, XXACC")]
        public async Task<IActionResult> UnitList(string dc = null)
        {
            UnitListViewModel model = new UnitListViewModel();
            model.dc = dc;
            if (!string.IsNullOrWhiteSpace(dc))
            {
                var entities = await _globalSettingsService.GetUnitsAsync(dc);
                if (entities != null && entities.Count > 0)
                {
                    model.UnitList = entities;
                }
            }
            else
            {
                var entities = await _globalSettingsService.GetUnitsAsync();
                if (entities != null && entities.Count > 0)
                {
                    model.UnitList = entities;
                }
            }

            var dept_entities = await _globalSettingsService.GetDepartmentsAsync();
            if(dept_entities != null && dept_entities.Count > 0)
            {
                ViewBag.DepartmentList = new SelectList(dept_entities, "DepartmentCode", "DepartmentName", dc);
            }
            return View(model);
        }

        [Authorize(Roles = "GBSSTT, XXACC")]
        public async Task<IActionResult> ManageUnit(string cd)
        {
            ManageUnitViewModel model = new ManageUnitViewModel();
            try
            {
                if (!string.IsNullOrWhiteSpace(cd))
                {
                    model.IsUpdate = true;
                    model.OldCode = cd;
                    model.UnitCode = cd;
                    Unit unit = await _globalSettingsService.GetUnitByCodeAsync(cd);
                    if (unit != null && !string.IsNullOrWhiteSpace(unit.UnitName))
                    {
                        model.UnitCode = unit.UnitCode;
                        model.DepartmentCode = unit.DepartmentCode;
                        model.UnitName = unit.UnitName;
                        model.DepartmentName = unit.DepartmentName;
                        model.UnitHeadID = unit.UnitHeadId;
                        model.UnitHeadName = unit.UnitHeadName;
                        model.UnitAltHeadID = unit.UnitAltHeadId;
                        model.UnitAltHeadName = unit.UnitAltHeadName;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            var dept_entities = await _globalSettingsService.GetDepartmentsAsync();
            if (dept_entities != null && dept_entities.Count > 0)
            {
                ViewBag.DepartmentList = new SelectList(dept_entities, "DepartmentCode", "DepartmentName", model.DepartmentCode);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "GBSSTT, XXACC")]
        public async Task<IActionResult> ManageUnit(ManageUnitViewModel model)
        {
            try
            {
                Unit unit = new Unit();
                if (ModelState.IsValid)
                {
                    unit.UnitCode = model.UnitCode;
                    unit.DepartmentCode = model.DepartmentCode;
                    unit.UnitName = model.UnitName;
                    unit.UnitHeadName = model.UnitHeadName;
                    unit.UnitAltHeadName = model.UnitAltHeadName;

                    if (!model.IsUpdate)
                    {
                        bool IsAdded = await _globalSettingsService.AddUnitAsync(unit);
                        if (IsAdded)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "New Unit added successfully!";
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                        }
                    }
                    else
                    {
                        bool IsUpdated = await _globalSettingsService.UpdateUnitAsync(unit);
                        if (IsUpdated)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "Unit updated successfully!";
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            var dept_entities = await _globalSettingsService.GetDepartmentsAsync();
            if (dept_entities != null && dept_entities.Count > 0)
            {
                ViewBag.DepartmentList = new SelectList(dept_entities, "DepartmentCode", "DepartmentName", model.DepartmentCode);
            }
            return View(model);
        }

        [Authorize(Roles = "GBSSTT, XXACC")]
        public async Task<IActionResult> UnitDetail(string cd)
        {
            ManageUnitViewModel model = new ManageUnitViewModel();
            try
            {
                if (!string.IsNullOrWhiteSpace(cd))
                {
                    model.UnitCode = cd;
                    Unit unit = await _globalSettingsService.GetUnitByCodeAsync(cd);
                    if (unit != null && !string.IsNullOrWhiteSpace(unit.UnitName))
                    {
                        model.UnitCode = unit.UnitCode;
                        model.DepartmentCode = unit.DepartmentCode;
                        model.UnitName = unit.UnitName;
                        model.UnitHeadID = unit.UnitHeadId;
                        model.UnitHeadName = unit.UnitHeadName;
                        model.UnitAltHeadID = unit.UnitAltHeadId;
                        model.UnitAltHeadName = unit.UnitAltHeadName;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }


        [Authorize(Roles = "GBSSTT, XXACC")]
        public async Task<IActionResult> DeleteUnit(string cd)
        {
            ManageUnitViewModel model = new ManageUnitViewModel();
            try
            {
                if (!string.IsNullOrWhiteSpace(cd))
                {
                    model.UnitCode = cd;
                    Unit unit = await _globalSettingsService.GetUnitByCodeAsync(cd);
                    if (unit != null && !string.IsNullOrWhiteSpace(unit.UnitName))
                    {
                        model.UnitCode = unit.UnitCode;
                        model.DepartmentCode = unit.DepartmentCode;
                        model.UnitName = unit.UnitName;
                        model.DepartmentName = unit.DepartmentName;
                        model.UnitHeadID = unit.UnitHeadId;
                        model.UnitHeadName = unit.UnitHeadName;
                        model.UnitAltHeadID = unit.UnitAltHeadId;
                        model.UnitAltHeadName = unit.UnitAltHeadName;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "GBSSTT, XXACC")]
        public async Task<IActionResult> DeleteUnit(ManageUnitViewModel model)
        {
            try
            {

                if (!string.IsNullOrWhiteSpace(model.UnitCode))
                {
                    bool IsDeleted = await _globalSettingsService.DeleteUnitAsync(model.UnitCode);
                    if (IsDeleted)
                    {
                        return RedirectToAction("UnitList");
                    }
                    else
                    {
                        model.OperationIsSuccessful = false;
                        model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                    }
                }
                else
                {
                    model.OperationIsSuccessful = false;
                    model.ViewModelErrorMessage = "Sorry, a key parameter required to process your request is missing. Please try again.";
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        #endregion
    }
}