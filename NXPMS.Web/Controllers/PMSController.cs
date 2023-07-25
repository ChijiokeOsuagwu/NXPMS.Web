using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NXPMS.Base.Enums;
using NXPMS.Base.Models.EmployeesModels;
using NXPMS.Base.Models.PMSModels;
using NXPMS.Base.Models.SecurityModels;
using NXPMS.Base.Services;
using NXPMS.Web.Models.PMSViewModels;

namespace NXPMS.Web.Controllers
{
    public class PMSController : Controller
    {
        private readonly IPerformanceService _performanceService;
        private readonly IGlobalSettingsService _globalSettingsService;
        private readonly IEmployeeRecordService _employeeRecordService;
        private readonly ISecurityService _securityService;

        public PMSController(IEmployeeRecordService employeeRecordService,
            IGlobalSettingsService globalSettingsService, IPerformanceService performanceService,
            ISecurityService securityService)
        {
            _employeeRecordService = employeeRecordService;
            _globalSettingsService = globalSettingsService;
            _performanceService = performanceService;
            _securityService = securityService;
        }

        [Authorize(Roles = "PMSSTT, XXACC")]
        public IActionResult Settings()
        {
            return View();
        }

        public IActionResult Appraisals()
        {
            return View();
        }

        #region Performance Year Controller Actions

        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> PerformanceYears()
        {
            PerformanceYearsListViewModel model = new PerformanceYearsListViewModel();
            var entities = await _performanceService.GetPerformanceYearsAsync();
            if (entities != null && entities.Count > 0)
            {
                model.PerformanceYearList = entities.ToList();
            }

            if (TempData["Error"] != null)
            {
                model.ViewModelErrorMessage = TempData["Error"].ToString();
            }

            if (TempData["Success"] != null)
            {
                model.ViewModelSuccessMessage = TempData["Success"].ToString();
            }
            return View(model);
        }

        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> ManagePerformanceYear(int id)
        {
            PerformanceYearViewModel model = new PerformanceYearViewModel();
            if (id > 0)
            {
                PerformanceYear performanceYear = await _performanceService.GetPerformanceYearAsync(id);
                if (performanceYear != null && !string.IsNullOrWhiteSpace(performanceYear.Name))
                {
                    model = model.ExtractFromPerformanceYear(performanceYear);
                }
            }
            return View(model);
        }

        [Authorize(Roles = "PMSSTT, XXACC")]
        [HttpPost]
        public async Task<IActionResult> ManagePerformanceYear(PerformanceYearViewModel model)
        {
            try
            {
                PerformanceYear performanceYear = new PerformanceYear();
                if (ModelState.IsValid)
                {
                    performanceYear = model.ConvertToPerformanceYear();
                    performanceYear.CreatedBy = HttpContext.User.Identity.Name;
                    performanceYear.CreatedTime = DateTime.UtcNow;
                    if (performanceYear.Id < 1)
                    {
                        bool IsAdded = await _performanceService.AddPerformanceYearAsync(performanceYear);
                        if (IsAdded)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "New Performance Year was added successfully!";
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                        }
                    }
                    else
                    {
                        performanceYear.LastModifiedBy = HttpContext.User.Identity.Name;
                        performanceYear.LastModifiedTime = DateTime.UtcNow;
                        bool IsUpdated = await _performanceService.EditPerformanceYearAsync(performanceYear);
                        if (IsUpdated)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "Performance Year was updated successfully!";
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

        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> DeletePerformanceYear(int yd)
        {

            try
            {
                if (yd > 0)
                {
                    bool IsDeleted = await _performanceService.DeletePerformanceYearAsync(yd);
                    if (IsDeleted)
                    {
                        TempData["Success"] = "Records deleted successfully!";
                    }
                    else
                    {
                        TempData["Error"] = "Sorry, an error was encountered. Delete operation failed.";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("PerformanceYears");
        }
        #endregion

        #region Approver Roles Controller Actions

        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> ApproverRoles()
        {
            ApprovalRolesListViewModel model = new ApprovalRolesListViewModel();
            var entities = await _performanceService.GetApprovalRolesAsync();
            if (entities != null && entities.Count > 0)
            {
                model.ApprovalRoleList = entities.ToList();
            }

            return View(model);
        }

        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> ManageApproverRole(int id)
        {
            ApprovalRoleViewModel model = new ApprovalRoleViewModel();
            if (id > 0)
            {
                ApprovalRole approvalRole = await _performanceService.GetApprovalRoleAsync(id);
                if (approvalRole != null && !string.IsNullOrWhiteSpace(approvalRole.ApprovalRoleName))
                {
                    model.ApprovalRoleID = approvalRole.ApprovalRoleId;
                    model.ApprovalRoleName = approvalRole.ApprovalRoleName;
                    model.MustApproveContract = approvalRole.MustApproveContract;
                    model.MustApproveEvaluation = approvalRole.MustApproveEvaluation;
                }
            }
            return View(model);
        }

        [Authorize(Roles = "PMSSTT, XXACC")]
        [HttpPost]
        public async Task<IActionResult> ManageApproverRole(ApprovalRoleViewModel model)
        {
            try
            {
                ApprovalRole approvalRole = new ApprovalRole();
                if (ModelState.IsValid)
                {
                    approvalRole.ApprovalRoleId = model.ApprovalRoleID;
                    approvalRole.ApprovalRoleName = model.ApprovalRoleName;
                    approvalRole.MustApproveContract = model.MustApproveContract;
                    approvalRole.MustApproveEvaluation = model.MustApproveEvaluation;

                    if (approvalRole.ApprovalRoleId < 1)
                    {
                        bool IsAdded = await _performanceService.AddApprovalRoleAsync(approvalRole);
                        if (IsAdded)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "New Approver Role was added successfully!";
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                        }
                    }
                    else
                    {
                        bool IsUpdated = await _performanceService.EditApprovalRoleAsync(approvalRole);
                        if (IsUpdated)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "Approver Role was updated successfully!";
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

        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> DeleteApproverRole(int id)
        {
            ApprovalRoleViewModel model = new ApprovalRoleViewModel();
            if (id > 0)
            {
                ApprovalRole approvalRole = await _performanceService.GetApprovalRoleAsync(id);
                if (approvalRole != null && !string.IsNullOrWhiteSpace(approvalRole.ApprovalRoleName))
                {
                    model.ApprovalRoleID = approvalRole.ApprovalRoleId;
                    model.ApprovalRoleName = approvalRole.ApprovalRoleName;
                    model.MustApproveContract = approvalRole.MustApproveContract;
                    model.MustApproveEvaluation = approvalRole.MustApproveEvaluation;
                }
            }
            return View(model);
        }

        [Authorize(Roles = "PMSSTT, XXACC")]
        [HttpPost]
        public async Task<IActionResult> DeleteApproverRole(ApprovalRoleViewModel model)
        {
            try
            {
                if (model.ApprovalRoleID > 0)
                {
                    bool IsDeleted = await _performanceService.DeleteApprovalRoleAsync(model.ApprovalRoleID);
                    if (IsDeleted)
                    {
                        return RedirectToAction("ApproverRoles");
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "Sorry, an error was encountered. Delete operation failed.";
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }
        #endregion

        #region Review Session Controller Actions

        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> ReviewSessions(int? id)
        {
            ReviewSessionsListViewModel model = new ReviewSessionsListViewModel();
            if (id != null && id.Value > 0)
            {
                var entities = await _performanceService.GetReviewSessionsAsync(id.Value);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewSessionsList = entities.ToList();
                }
            }
            else
            {
                var entities = await _performanceService.GetReviewSessionsAsync();
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewSessionsList = entities.ToList();
                }
            }

            List<PerformanceYear> pyears = await _performanceService.GetPerformanceYearsAsync();
            if (pyears != null && pyears.Count > 0)
            {
                ViewBag.PerformanceYearsList = new SelectList(pyears, "Id", "Name", id);
            }

            return View(model);
        }

        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> ManageReviewSession(int id)
        {
            ManageReviewSessionViewModel model = new ManageReviewSessionViewModel();
            if (id > 0)
            {
                ReviewSession reviewSession = await _performanceService.GetReviewSessionAsync(id);
                if (reviewSession != null && !string.IsNullOrWhiteSpace(reviewSession.Name))
                {
                    model = model.ExtractViewModel(reviewSession);
                    if (reviewSession.IsActive) { model.IsActive = 1; } else { model.IsActive = 0; }
                }
            }

            List<PerformanceYear> pyears = await _performanceService.GetPerformanceYearsAsync();
            if (pyears != null && pyears.Count > 0)
            {
                ViewBag.PerformanceYearsList = new SelectList(pyears, "Id", "Name");
            }

            List<ReviewType> types = await _performanceService.GetReviewTypesAsync();
            if (types != null && types.Count > 0)
            {
                ViewBag.ReviewTypesList = new SelectList(types, "ReviewTypeId", "ReviewTypeName");
            }

            return View(model);
        }

        [Authorize(Roles = "PMSSTT, XXACC")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageReviewSession(ManageReviewSessionViewModel model)
        {
            try
            {
                ReviewSession reviewSession = new ReviewSession();
                if (ModelState.IsValid)
                {
                    reviewSession = model.ConvertToReviewSession();
                    reviewSession.CreatedBy = HttpContext.User.Identity.Name;
                    reviewSession.CreatedTime = DateTime.UtcNow;
                    if (model.IsActive == 0) { reviewSession.IsActive = false; } else { reviewSession.IsActive = true; }
                    if (reviewSession.Id < 1)
                    {
                        bool IsAdded = await _performanceService.AddReviewSessionAsync(reviewSession);
                        if (IsAdded)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "New Review Session was added successfully!";
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                        }
                    }
                    else
                    {
                        reviewSession.LastModifiedBy = HttpContext.User.Identity.Name;
                        reviewSession.LastModifiedTime = DateTime.UtcNow;
                        bool IsUpdated = await _performanceService.EditReviewSessionAsync(reviewSession);
                        if (IsUpdated)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "Appraisal Session was updated successfully!";
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

        public async Task<IActionResult> ShowReviewSession(int id)
        {
            ManageReviewSessionViewModel model = new ManageReviewSessionViewModel();
            if (id > 0)
            {
                ReviewSession reviewSession = await _performanceService.GetReviewSessionAsync(id);
                if (reviewSession != null && !string.IsNullOrWhiteSpace(reviewSession.Name))
                {
                    model = model.ExtractViewModel(reviewSession);
                    model.StartDateFormatted = reviewSession.StartDate.Value.ToString("yyyy-MM-dd");
                    model.EndDateFormatted = reviewSession.EndDate.Value.ToString("yyyy-MM-dd");
                    if (reviewSession.IsActive) { model.IsActive = 1; } else { model.IsActive = 0; }
                }
                else
                {
                    model.ViewModelErrorMessage = "Sorry, the record you selected was not found.";
                }
                return View(model);
            }
            return View(model);
        }

        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> DeleteReviewSession(int id)
        {
            ManageReviewSessionViewModel model = new ManageReviewSessionViewModel();
            if (id > 0)
            {
                ReviewSession reviewSession = await _performanceService.GetReviewSessionAsync(id);
                if (reviewSession != null && !string.IsNullOrWhiteSpace(reviewSession.Name))
                {
                    model = model.ExtractViewModel(reviewSession);
                    model.StartDateFormatted = reviewSession.StartDate.Value.ToString("yyyy-MM-dd");
                    model.EndDateFormatted = reviewSession.EndDate.Value.ToString("yyyy-MM-dd");
                }
                else
                {
                    model.ViewModelErrorMessage = "Sorry, the record you selected was not found.";
                }
                return View(model);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> DeleteReviewSession(ManageReviewSessionViewModel model)
        {
            try
            {
                if (model.Id > 0)
                {
                    bool IsDeleted = await _performanceService.DeleteReviewSessionAsync(model.Id.Value);
                    if (IsDeleted)
                    {
                        return RedirectToAction("ReviewSessions");
                    }
                    else
                    {
                        model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                    }
                }
                else
                {
                    model.ViewModelSuccessMessage = "The record was not deleted. Because some parameter had invalid values. Please try again.";
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        public async Task<IActionResult> ReviewSessionInfo(int id)
        {
            ManageReviewSessionViewModel model = new ManageReviewSessionViewModel();
            if (id > 0)
            {
                ReviewSession reviewSession = await _performanceService.GetReviewSessionAsync(id);
                if (reviewSession != null && !string.IsNullOrWhiteSpace(reviewSession.Name))
                {
                    model = model.ExtractViewModel(reviewSession);
                    model.StartDateFormatted = reviewSession.StartDate.Value.ToString("yyyy-MM-dd");
                    model.EndDateFormatted = reviewSession.EndDate.Value.ToString("yyyy-MM-dd");
                    if (reviewSession.IsActive) { model.IsActive = 1; } else { model.IsActive = 0; }
                }
                else
                {
                    model.ViewModelErrorMessage = "Sorry, the record you selected was not found.";
                }
                return View(model);
            }
            return View(model);
        }

        #endregion

        #region Review Grades Controller Actions

        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> Grading()
        {
            GradingListViewModel model = new GradingListViewModel();
            var entities = await _performanceService.GetGradeHeadersAsync();
            if (entities != null && entities.Count > 0)
            {
                model.GradeHeaderList = entities.ToList();
            }
            return View(model);
        }

        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> ManageGradeProfile(int id)
        {
            ManageGradeProfileViewModel model = new ManageGradeProfileViewModel();
            if (id > 0)
            {
                GradeHeader gradeHeader = await _performanceService.GetGradeHeaderAsync(id);
                if (gradeHeader != null && !string.IsNullOrWhiteSpace(gradeHeader.GradeHeaderName))
                {
                    model.GradeHeaderId = gradeHeader.GradeHeaderId;
                    model.GradeHeaderName = gradeHeader.GradeHeaderName;
                    model.GradeHeaderDescription = gradeHeader.GradeHeaderDescription;
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> ManageGradeProfile(ManageGradeProfileViewModel model)
        {
            try
            {
                GradeHeader gradeHeader = new GradeHeader();
                if (ModelState.IsValid)
                {
                    gradeHeader.GradeHeaderId = model.GradeHeaderId ?? 0;
                    gradeHeader.GradeHeaderName = model.GradeHeaderName;
                    gradeHeader.GradeHeaderDescription = model.GradeHeaderDescription;

                    if (gradeHeader.GradeHeaderId < 1)
                    {
                        bool IsAdded = await _performanceService.AddGradeHeaderAsync(gradeHeader);
                        if (IsAdded)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "New Grading Profile was added successfully!";
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                        }
                    }
                    else
                    {
                        bool IsUpdated = await _performanceService.EditGradeHeaderAsync(gradeHeader);
                        if (IsUpdated)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "Grading Profile was updated successfully!";
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

        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> DeleteGradeProfile(int id)
        {
            ManageGradeProfileViewModel model = new ManageGradeProfileViewModel();
            if (id > 0)
            {
                GradeHeader gradeHeader = await _performanceService.GetGradeHeaderAsync(id);
                if (gradeHeader != null && !string.IsNullOrWhiteSpace(gradeHeader.GradeHeaderName))
                {
                    model.GradeHeaderId = gradeHeader.GradeHeaderId;
                    model.GradeHeaderName = gradeHeader.GradeHeaderName;
                    model.GradeHeaderDescription = gradeHeader.GradeHeaderDescription;
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> DeleteGradeProfile(ManageGradeProfileViewModel model)
        {
            try
            {
                GradeHeader gradeHeader = new GradeHeader();
                if (ModelState.IsValid)
                {
                    gradeHeader.GradeHeaderId = model.GradeHeaderId ?? 0;
                    gradeHeader.GradeHeaderName = model.GradeHeaderName;
                    gradeHeader.GradeHeaderDescription = model.GradeHeaderDescription;

                    if (gradeHeader.GradeHeaderId > 0)
                    {
                        bool IsDeleted = await _performanceService.DeleteGradeHeaderAsync(gradeHeader.GradeHeaderId);
                        if (IsDeleted)
                        {
                            return RedirectToAction("Grading");
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                        }
                    }
                    else
                    {
                        model.ViewModelSuccessMessage = "Sorry, a key parameter is missing. The operation could not be completed.";
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }


        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> PerformanceGradeTemplate(int id)
        {
            ReviewGradeListViewModel model = new ReviewGradeListViewModel();
            if (id > 0)
            {
                model.TemplateId = id;
                var entities = await _performanceService.GetPerformanceGradesAsync(id);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewGradeList = entities.ToList();
                }
            }

            return View(model);
        }


        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> ManagePerformanceGrade(int td, int? id)
        {
            ManageReviewGradeViewModel model = new ManageReviewGradeViewModel();
            model.GradeHeaderId = td;
            if (id > 0)
            {
                ReviewGrade reviewGrade = await _performanceService.GetReviewGradeAsync(id.Value);
                if (reviewGrade != null && !string.IsNullOrWhiteSpace(reviewGrade.ReviewGradeDescription))
                {
                    model.GradeHeaderId = reviewGrade.GradeHeaderId;
                    model.GradeHeaderName = reviewGrade.GradeHeaderName;
                    model.ReviewGradeDescription = reviewGrade.ReviewGradeDescription;
                    model.GradeRank = reviewGrade.GradeRank;
                    model.GradeType = reviewGrade.GradeType;
                    model.LowerBandScore = reviewGrade.LowerBandScore;
                    model.ReviewGradeId = reviewGrade.ReviewGradeId;
                    model.UpperBandScore = reviewGrade.UpperBandScore;
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> ManagePerformanceGrade(ManageReviewGradeViewModel model)
        {
            try
            {
                ReviewGrade reviewGrade = new ReviewGrade();
                if (ModelState.IsValid)
                {
                    reviewGrade = model.ConvertToPerformanceGrade();
                    reviewGrade.CreatedBy = HttpContext.User.Identity.Name;
                    reviewGrade.CreatedTime = DateTime.UtcNow;
                    if (reviewGrade.ReviewGradeId < 1)
                    {
                        bool IsAdded = await _performanceService.AddReviewGradeAsync(reviewGrade);
                        if (IsAdded)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "New Performance Grade was added successfully!";
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                        }
                    }
                    else
                    {
                        reviewGrade.LastModifiedBy = HttpContext.User.Identity.Name;
                        reviewGrade.LastModifiedTime = DateTime.UtcNow;
                        bool IsUpdated = await _performanceService.EditReviewGradeAsync(reviewGrade);
                        if (IsUpdated)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "Performance Grade was updated successfully!";
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

        public async Task<IActionResult> PerformanceGradeInfo(int id)
        {
            ManageReviewGradeViewModel model = new ManageReviewGradeViewModel();
            if (id > 0)
            {
                ReviewGrade reviewGrade = await _performanceService.GetReviewGradeAsync(id);
                if (reviewGrade != null && !string.IsNullOrWhiteSpace(reviewGrade.ReviewGradeDescription))
                {
                    model.GradeHeaderId = reviewGrade.GradeHeaderId;
                    model.GradeHeaderName = reviewGrade.GradeHeaderName;
                    model.ReviewGradeDescription = reviewGrade.ReviewGradeDescription;
                    model.GradeRank = reviewGrade.GradeRank;
                    model.GradeType = reviewGrade.GradeType;
                    model.LowerBandScore = reviewGrade.LowerBandScore;
                    model.ReviewGradeId = reviewGrade.ReviewGradeId;
                    model.UpperBandScore = reviewGrade.UpperBandScore;
                }
            }
            return View(model);
        }

        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> DeletePerformanceGrade(int id)
        {
            ManageReviewGradeViewModel model = new ManageReviewGradeViewModel();
            if (id > 0)
            {
                ReviewGrade reviewGrade = await _performanceService.GetReviewGradeAsync(id);
                if (reviewGrade != null && !string.IsNullOrWhiteSpace(reviewGrade.ReviewGradeDescription))
                {
                    model.GradeHeaderId = reviewGrade.GradeHeaderId;
                    model.GradeHeaderName = reviewGrade.GradeHeaderName;
                    model.ReviewGradeDescription = reviewGrade.ReviewGradeDescription;
                    model.GradeRank = reviewGrade.GradeRank;
                    model.GradeRankDescription = reviewGrade.GradeRankDescription;
                    model.GradeType = reviewGrade.GradeType;
                    model.LowerBandScore = reviewGrade.LowerBandScore;
                    model.ReviewGradeId = reviewGrade.ReviewGradeId;
                    model.UpperBandScore = reviewGrade.UpperBandScore;
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> DeletePerformanceGrade(ManageReviewGradeViewModel model)
        {

            try
            {
                if (model != null && model.ReviewGradeId > 0)
                {
                    bool IsDeleted = await _performanceService.DeleteReviewGradeAsync(model.ReviewGradeId);
                    if (IsDeleted)
                    {
                        model.ViewModelSuccessMessage = "Records deleted successfully!";
                        return RedirectToAction("PerformanceGradeTemplate", new { id = model.GradeHeaderId });
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "Sorry, an error was encountered. Delete operation failed.";
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        //============================== Competency Grades =======================================//    

        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> CompetencyGradeTemplate(int id)
        {
            ReviewGradeListViewModel model = new ReviewGradeListViewModel();
            if (id > 0)
            {
                model.TemplateId = id;
                var entities = await _performanceService.GetCompetencyGradesAsync(id);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewGradeList = entities.ToList();
                }
            }

            return View(model);
        }

        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> ManageCompetencyGrade(int td, int? id)
        {
            ManageReviewGradeViewModel model = new ManageReviewGradeViewModel();
            model.GradeHeaderId = td;
            if (id > 0)
            {
                ReviewGrade reviewGrade = await _performanceService.GetReviewGradeAsync(id.Value);
                if (reviewGrade != null && !string.IsNullOrWhiteSpace(reviewGrade.ReviewGradeDescription))
                {
                    model.GradeHeaderId = reviewGrade.GradeHeaderId;
                    model.GradeHeaderName = reviewGrade.GradeHeaderName;
                    model.ReviewGradeDescription = reviewGrade.ReviewGradeDescription;
                    model.GradeRank = reviewGrade.GradeRank;
                    model.GradeType = reviewGrade.GradeType;
                    model.LowerBandScore = reviewGrade.LowerBandScore;
                    model.ReviewGradeId = reviewGrade.ReviewGradeId;
                    model.UpperBandScore = reviewGrade.UpperBandScore;
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> ManageCompetencyGrade(ManageReviewGradeViewModel model)
        {
            try
            {
                ReviewGrade reviewGrade = new ReviewGrade();
                if (ModelState.IsValid)
                {
                    reviewGrade = model.ConvertToCompetencyGrade();
                    reviewGrade.CreatedBy = HttpContext.User.Identity.Name;
                    reviewGrade.CreatedTime = DateTime.UtcNow;
                    if (reviewGrade.ReviewGradeId < 1)
                    {
                        bool IsAdded = await _performanceService.AddReviewGradeAsync(reviewGrade);
                        if (IsAdded)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "New Competency Grade was added successfully!";
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                        }
                    }
                    else
                    {
                        reviewGrade.LastModifiedBy = HttpContext.User.Identity.Name;
                        reviewGrade.LastModifiedTime = DateTime.UtcNow;
                        bool IsUpdated = await _performanceService.EditReviewGradeAsync(reviewGrade);
                        if (IsUpdated)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "Competency Grade was updated successfully!";
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

        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> CompetencyGradeInfo(int id)
        {
            ManageReviewGradeViewModel model = new ManageReviewGradeViewModel();
            if (id > 0)
            {
                ReviewGrade reviewGrade = await _performanceService.GetReviewGradeAsync(id);
                if (reviewGrade != null && !string.IsNullOrWhiteSpace(reviewGrade.ReviewGradeDescription))
                {
                    model.GradeHeaderId = reviewGrade.GradeHeaderId;
                    model.GradeHeaderName = reviewGrade.GradeHeaderName;
                    model.ReviewGradeDescription = reviewGrade.ReviewGradeDescription;
                    model.GradeRank = reviewGrade.GradeRank;
                    model.GradeType = reviewGrade.GradeType;
                    model.LowerBandScore = reviewGrade.LowerBandScore;
                    model.ReviewGradeId = reviewGrade.ReviewGradeId;
                    model.UpperBandScore = reviewGrade.UpperBandScore;
                }
            }
            return View(model);
        }

        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> DeleteCompetencyGrade(int id)
        {
            ManageReviewGradeViewModel model = new ManageReviewGradeViewModel();
            if (id > 0)
            {
                ReviewGrade reviewGrade = await _performanceService.GetReviewGradeAsync(id);
                if (reviewGrade != null && !string.IsNullOrWhiteSpace(reviewGrade.ReviewGradeDescription))
                {
                    model.GradeHeaderId = reviewGrade.GradeHeaderId;
                    model.GradeHeaderName = reviewGrade.GradeHeaderName;
                    model.ReviewGradeDescription = reviewGrade.ReviewGradeDescription;
                    model.GradeRank = reviewGrade.GradeRank;
                    model.GradeRankDescription = reviewGrade.GradeRankDescription;
                    model.GradeType = reviewGrade.GradeType;
                    model.LowerBandScore = reviewGrade.LowerBandScore;
                    model.ReviewGradeId = reviewGrade.ReviewGradeId;
                    model.UpperBandScore = reviewGrade.UpperBandScore;
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> DeleteCompetencyGrade(ManageReviewGradeViewModel model)
        {

            try
            {
                if (model != null && model.ReviewGradeId > 0)
                {
                    bool IsDeleted = await _performanceService.DeleteReviewGradeAsync(model.ReviewGradeId);
                    if (IsDeleted)
                    {
                        model.ViewModelSuccessMessage = "Records deleted successfully!";
                        return RedirectToAction("CompetencyGradeTemplate", new { id = model.GradeHeaderId });
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "Sorry, an error was encountered. Delete operation failed.";
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        #endregion

        #region Appraisal Grades Controller Actions

        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> AppraisalGradeSettings(int id)
        {
            AppraisalGradeSettingsListViewModel model = new AppraisalGradeSettingsListViewModel();
            if (id > 0)
            {
                model.ReviewSessionID = id;
                var p_entities = await _performanceService.GetAppraisalPerformanceGradesAsync(id);
                if (p_entities != null && p_entities.Count > 0)
                {
                    model.AppraisalPerformanceGradeList = p_entities.ToList();
                }

                var c_entities = await _performanceService.GetAppraisalCompetencyGradesAsync(id);
                if (c_entities != null && c_entities.Count > 0)
                {
                    model.AppraisalCompetencyGradeList = c_entities.ToList();
                }
            }
            return View(model);
        }

        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> CopyAppraisalGrades(int id)
        {
            CopyAppraisalGradesViewModel model = new CopyAppraisalGradesViewModel();
            model.ReviewSessionId = id;
            List<GradeHeader> gradeHeaders = new List<GradeHeader>();
            gradeHeaders = await _performanceService.GetGradeHeadersAsync();
            if (gradeHeaders != null && gradeHeaders.Count > 0)
            {
                ViewBag.GradeProfileList = new SelectList(gradeHeaders, "GradeHeaderId", "GradeHeaderName");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> CopyAppraisalGrades(CopyAppraisalGradesViewModel model)
        {
            List<GradeHeader> gradeHeaders = new List<GradeHeader>();
            if (ModelState.IsValid)
            {
                try
                {
                    string CopiedBy = HttpContext.User.Identity.Name;
                    bool IsCopied = await _performanceService.CopyAppraisalGradeAsync(CopiedBy, model.ReviewSessionId, model.GradeProfileId, null);
                    if (IsCopied)
                    {
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = "Appraisal Grades were copied successfully!";
                    }

                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }

            gradeHeaders = await _performanceService.GetGradeHeadersAsync();
            if (gradeHeaders != null && gradeHeaders.Count > 0)
            {
                ViewBag.GradeProfileList = new SelectList(gradeHeaders, "GradeHeaderId", "GradeHeaderName");
            }
            return View(model);
        }


        public async Task<IActionResult> AppraisalGradesInfo(int id)
        {
            AppraisalGradeSettingsListViewModel model = new AppraisalGradeSettingsListViewModel();
            if (id > 0)
            {
                model.ReviewSessionID = id;
                var p_entities = await _performanceService.GetAppraisalPerformanceGradesAsync(id);
                if (p_entities != null && p_entities.Count > 0)
                {
                    model.AppraisalPerformanceGradeList = p_entities.ToList();
                }

                var c_entities = await _performanceService.GetAppraisalCompetencyGradesAsync(id);
                if (c_entities != null && c_entities.Count > 0)
                {
                    model.AppraisalCompetencyGradeList = c_entities.ToList();
                }
            }
            return View(model);
        }

        #endregion

        #region Appraisal Performance Grade Controller Actions

        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> ManageAppraisalPerformanceGrade(int id, int? rd = null)
        {
            ManageAppraisalGradeViewModel model = new ManageAppraisalGradeViewModel();
            if (rd != null && rd.Value > 0) { model.ReviewSessionId = rd.Value; }

            if (id > 0)
            {
                AppraisalGrade appraisalGrade = await _performanceService.GetAppraisalGradeAsync(id);
                if (appraisalGrade != null && !string.IsNullOrWhiteSpace(appraisalGrade.AppraisalGradeDescription))
                {
                    model.ReviewSessionId = appraisalGrade.ReviewSessionId;
                    model.ReviewSessionName = appraisalGrade.ReviewSessionName;
                    model.AppraisalGradeDescription = appraisalGrade.AppraisalGradeDescription;
                    model.GradeRank = appraisalGrade.GradeRank;
                    model.GradeType = appraisalGrade.GradeType;
                    model.LowerBandScore = appraisalGrade.LowerBandScore;
                    model.AppraisalGradeId = appraisalGrade.AppraisalGradeId;
                    model.UpperBandScore = appraisalGrade.UpperBandScore;
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> ManageAppraisalPerformanceGrade(ManageAppraisalGradeViewModel model)
        {
            try
            {
                AppraisalGrade appraisalGrade = new AppraisalGrade();
                if (ModelState.IsValid)
                {
                    appraisalGrade = model.ConvertToPerformanceGrade();
                    appraisalGrade.CreatedBy = HttpContext.User.Identity.Name;
                    appraisalGrade.CreatedTime = DateTime.UtcNow;
                    if (appraisalGrade.AppraisalGradeId < 1)
                    {
                        bool IsAdded = await _performanceService.AddAppraisalGradeAsync(appraisalGrade);
                        if (IsAdded)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "New Performance Grade was added successfully!";
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                        }
                    }
                    else
                    {
                        appraisalGrade.LastModifiedBy = HttpContext.User.Identity.Name;
                        appraisalGrade.LastModifiedTime = DateTime.UtcNow;
                        bool IsUpdated = await _performanceService.EditAppraisalGradeAsync(appraisalGrade);
                        if (IsUpdated)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "Performance Grade was updated successfully!";
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

        public async Task<IActionResult> AppraisalPerformanceGradeInfo(int id)
        {
            ManageAppraisalGradeViewModel model = new ManageAppraisalGradeViewModel();
            if (id > 0)
            {
                AppraisalGrade appraisalGrade = await _performanceService.GetAppraisalGradeAsync(id);
                if (appraisalGrade != null && !string.IsNullOrWhiteSpace(appraisalGrade.AppraisalGradeDescription))
                {
                    model.ReviewSessionId = appraisalGrade.ReviewSessionId;
                    model.ReviewSessionName = appraisalGrade.ReviewSessionName;
                    model.AppraisalGradeDescription = appraisalGrade.AppraisalGradeDescription;
                    model.GradeRank = appraisalGrade.GradeRank;
                    model.GradeType = appraisalGrade.GradeType;
                    model.LowerBandScore = appraisalGrade.LowerBandScore;
                    model.AppraisalGradeId = appraisalGrade.AppraisalGradeId;
                    model.UpperBandScore = appraisalGrade.UpperBandScore;
                }
            }
            return View(model);
        }


        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> DeleteAppraisalPerformanceGrade(int id)
        {
            ManageAppraisalGradeViewModel model = new ManageAppraisalGradeViewModel();
            if (id > 0)
            {
                AppraisalGrade appraisalGrade = await _performanceService.GetAppraisalGradeAsync(id);
                if (appraisalGrade != null && !string.IsNullOrWhiteSpace(appraisalGrade.AppraisalGradeDescription))
                {
                    model.ReviewSessionId = appraisalGrade.ReviewSessionId;
                    model.ReviewSessionName = appraisalGrade.ReviewSessionName;
                    model.AppraisalGradeDescription = appraisalGrade.AppraisalGradeDescription;
                    model.GradeRank = appraisalGrade.GradeRank;
                    model.GradeRankDescription = appraisalGrade.GradeRankDescription;
                    model.GradeType = appraisalGrade.GradeType;
                    model.LowerBandScore = appraisalGrade.LowerBandScore;
                    model.AppraisalGradeId = appraisalGrade.AppraisalGradeId;
                    model.UpperBandScore = appraisalGrade.UpperBandScore;
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> DeleteAppraisalPerformanceGrade(ManageAppraisalGradeViewModel model)
        {

            try
            {
                if (model != null && model.AppraisalGradeId > 0)
                {
                    bool IsDeleted = await _performanceService.DeleteAppraisalGradeAsync(model.AppraisalGradeId);
                    if (IsDeleted)
                    {
                        model.ViewModelSuccessMessage = "Records deleted successfully!";
                        return RedirectToAction("AppraisalGradeSettings", new { id = model.ReviewSessionId });
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "Sorry, an error was encountered. Delete operation failed.";
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        #endregion

        #region Appraisal Competency Grade Controller Actions

        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> ManageAppraisalCompetencyGrade(int id, int? rd = null)
        {
            ManageAppraisalGradeViewModel model = new ManageAppraisalGradeViewModel();
            if (rd != null && rd.Value > 0) { model.ReviewSessionId = rd.Value; }
            if (id > 0)
            {
                AppraisalGrade appraisalGrade = await _performanceService.GetAppraisalGradeAsync(id);
                if (appraisalGrade != null && !string.IsNullOrWhiteSpace(appraisalGrade.AppraisalGradeDescription))
                {
                    model.ReviewSessionId = appraisalGrade.ReviewSessionId;
                    model.ReviewSessionName = appraisalGrade.ReviewSessionName;
                    model.AppraisalGradeDescription = appraisalGrade.AppraisalGradeDescription;
                    model.GradeRank = appraisalGrade.GradeRank;
                    model.GradeType = appraisalGrade.GradeType;
                    model.LowerBandScore = appraisalGrade.LowerBandScore;
                    model.AppraisalGradeId = appraisalGrade.AppraisalGradeId;
                    model.UpperBandScore = appraisalGrade.UpperBandScore;
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> ManageAppraisalCompetencyGrade(ManageAppraisalGradeViewModel model)
        {
            try
            {
                AppraisalGrade appraisalGrade = new AppraisalGrade();
                if (ModelState.IsValid)
                {
                    appraisalGrade = model.ConvertToCompetencyGrade();
                    appraisalGrade.CreatedBy = HttpContext.User.Identity.Name;
                    appraisalGrade.CreatedTime = DateTime.UtcNow;
                    if (appraisalGrade.AppraisalGradeId < 1)
                    {
                        bool IsAdded = await _performanceService.AddAppraisalGradeAsync(appraisalGrade);
                        if (IsAdded)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "New Competency Grade was added successfully!";
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                        }
                    }
                    else
                    {
                        appraisalGrade.LastModifiedBy = HttpContext.User.Identity.Name;
                        appraisalGrade.LastModifiedTime = DateTime.UtcNow;
                        bool IsUpdated = await _performanceService.EditAppraisalGradeAsync(appraisalGrade);
                        if (IsUpdated)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "Competency Grade was updated successfully!";
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

        public async Task<IActionResult> AppraisalCompetencyGradeInfo(int id)
        {
            ManageAppraisalGradeViewModel model = new ManageAppraisalGradeViewModel();
            if (id > 0)
            {
                AppraisalGrade appraisalGrade = await _performanceService.GetAppraisalGradeAsync(id);
                if (appraisalGrade != null && !string.IsNullOrWhiteSpace(appraisalGrade.AppraisalGradeDescription))
                {
                    model.ReviewSessionId = appraisalGrade.ReviewSessionId;
                    model.ReviewSessionName = appraisalGrade.ReviewSessionName;
                    model.AppraisalGradeDescription = appraisalGrade.AppraisalGradeDescription;
                    model.GradeRank = appraisalGrade.GradeRank;
                    model.GradeType = appraisalGrade.GradeType;
                    model.LowerBandScore = appraisalGrade.LowerBandScore;
                    model.AppraisalGradeId = appraisalGrade.AppraisalGradeId;
                    model.UpperBandScore = appraisalGrade.UpperBandScore;
                }
            }
            return View(model);
        }

        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> DeleteAppraisalCompetencyGrade(int id)
        {
            ManageAppraisalGradeViewModel model = new ManageAppraisalGradeViewModel();
            if (id > 0)
            {
                AppraisalGrade appraisalGrade = await _performanceService.GetAppraisalGradeAsync(id);
                if (appraisalGrade != null && !string.IsNullOrWhiteSpace(appraisalGrade.AppraisalGradeDescription))
                {
                    model.ReviewSessionId = appraisalGrade.ReviewSessionId;
                    model.ReviewSessionName = appraisalGrade.ReviewSessionName;
                    model.AppraisalGradeDescription = appraisalGrade.AppraisalGradeDescription;
                    model.GradeRank = appraisalGrade.GradeRank;
                    model.GradeRankDescription = appraisalGrade.GradeRankDescription;
                    model.GradeType = appraisalGrade.GradeType;
                    model.LowerBandScore = appraisalGrade.LowerBandScore;
                    model.AppraisalGradeId = appraisalGrade.AppraisalGradeId;
                    model.UpperBandScore = appraisalGrade.UpperBandScore;
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> DeleteAppraisalCompetencyGrade(ManageAppraisalGradeViewModel model)
        {
            try
            {
                if (model != null && model.AppraisalGradeId > 0)
                {
                    bool IsDeleted = await _performanceService.DeleteAppraisalGradeAsync(model.AppraisalGradeId);
                    if (IsDeleted)
                    {
                        model.ViewModelSuccessMessage = "Records deleted successfully!";
                        return RedirectToAction("AppraisalGradeSettings", new { id = model.ReviewSessionId });
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "Sorry, an error was encountered. Delete operation failed.";
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        #endregion

        #region Appraisal Schedules Controller Actions

        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> AppraisalSchedules(int id)
        {
            AppraisalSchedulesListViewModel model = new AppraisalSchedulesListViewModel();
            ReviewSession reviewSession = new ReviewSession();
            model.ReviewSessionId = id;
            reviewSession = await _performanceService.GetReviewSessionAsync(id);
            if (reviewSession != null && !string.IsNullOrWhiteSpace(reviewSession.Name))
            {
                model.ReviewSessionName = reviewSession.Name;
                model.ReviewSessionId = reviewSession.Id;

                var entities = await _performanceService.GetSessionSchdulesAsync(model.ReviewSessionId);
                if (entities != null && entities.Count > 0)
                {
                    model.SessionScheduleList = entities;
                }
            }

            var locations = await _globalSettingsService.GetLocationsAsync();
            var depts = await _globalSettingsService.GetDepartmentsAsync();
            var units = await _globalSettingsService.GetUnitsAsync();

            ViewBag.LocationList = new SelectList(locations, "LocationId", "LocationName");
            ViewBag.DepartmentList = new SelectList(depts, "DepartmentCode", "DepartmentName");
            ViewBag.UnitList = new SelectList(units, "UnitCode", "UnitName");

            return View(model);
        }

        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> AppraisalSessionSchedules(int id)
        {
            AppraisalSchedulesListViewModel model = new AppraisalSchedulesListViewModel();
            ReviewSession reviewSession = new ReviewSession();
            model.ReviewSessionId = id;
            reviewSession = await _performanceService.GetReviewSessionAsync(id);
            if (reviewSession != null && !string.IsNullOrWhiteSpace(reviewSession.Name))
            {
                model.ReviewSessionName = reviewSession.Name;
                model.ReviewSessionId = reviewSession.Id;

                var entities = await _performanceService.GetSessionSchdulesAsync(model.ReviewSessionId);
                if (entities != null && entities.Count > 0)
                {
                    model.SessionScheduleList = entities;
                }
            }

            var locations = await _globalSettingsService.GetLocationsAsync();
            var depts = await _globalSettingsService.GetDepartmentsAsync();
            var units = await _globalSettingsService.GetUnitsAsync();

            ViewBag.LocationList = new SelectList(locations, "LocationId", "LocationName");
            ViewBag.DepartmentList = new SelectList(depts, "DepartmentCode", "DepartmentName");
            ViewBag.UnitList = new SelectList(units, "UnitCode", "UnitName");

            return View(model);
        }

        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> NewAppraisalSchedule(int id, int tp)
        {
            ManageAppraisalScheduleViewModel model = new ManageAppraisalScheduleViewModel();
            model.ReviewSessionId = id;
            model.ScheduleTypeId = tp;

            var locations = await _globalSettingsService.GetLocationsAsync();
            var depts = await _globalSettingsService.GetDepartmentsAsync();
            var units = await _globalSettingsService.GetUnitsAsync();

            ViewBag.LocationList = new SelectList(locations, "LocationId", "LocationName");
            ViewBag.DepartmentList = new SelectList(depts, "DepartmentCode", "DepartmentName");
            ViewBag.UnitList = new SelectList(units, "UnitCode", "UnitName");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> NewAppraisalSchedule(ManageAppraisalScheduleViewModel model)
        {
            SessionSchedule sessionSchedule = new SessionSchedule();
            sessionSchedule = model.ConvertToSessionSchedule();
            bool IsAdded = await _performanceService.AddSessionScheduleAsync(sessionSchedule);
            if (IsAdded) { return RedirectToAction("AppraisalSchedules", new { id = model.ReviewSessionId }); }
            else { model.ViewModelErrorMessage = "Sorry an error was encountered. New Schedule could not be created."; }
            return View(model);
        }


        public async Task<IActionResult> AppraisalScheduleInfo(int id)
        {
            ManageAppraisalScheduleViewModel model = new ManageAppraisalScheduleViewModel();
            if (id > 0)
            {
                SessionSchedule sessionSchedule = new SessionSchedule();
                sessionSchedule = await _performanceService.GetSessionScheduleAsync(id);
                if (sessionSchedule != null)
                {
                    model.ReviewSessionId = sessionSchedule.ReviewSessionId;
                    model.ReviewSessionName = sessionSchedule.ReviewSessionName;
                    model.ActivityTypeId = (int)sessionSchedule.ActivityType;
                    model.ActivityTypeDescription = sessionSchedule.SessionActivityTypeDescription;
                    model.ReviewSessionId = sessionSchedule.ReviewSessionId;
                    model.ReviewSessionName = sessionSchedule.ReviewSessionName;
                    model.ReviewYearId = sessionSchedule.ReviewYearId;
                    model.ReviewYearName = sessionSchedule.ReviewYearName;
                    model.ScheduleDepartmentCode = sessionSchedule.ScheduleDepartmentCode;
                    model.ScheduleDepartmentName = sessionSchedule.ScheduleDepartmentName;
                    model.ScheduleEmployeeId = sessionSchedule.ScheduleEmployeeId;
                    model.ScheduleEmployeeName = sessionSchedule.ScheduleEmployeeName;
                    model.ScheduleEndTime = sessionSchedule.ScheduleEndTime;
                    model.ScheduleLocationId = sessionSchedule.ScheduleLocationId;
                    model.ScheduleLocationName = sessionSchedule.ScheduleLocationName;
                    model.ScheduleStartTime = sessionSchedule.ScheduleStartTime;
                    model.ScheduleTypeDescription = sessionSchedule.ScheduleTypeDescription;
                    model.ScheduleTypeId = (int)sessionSchedule.ScheduleType;
                    model.ScheduleUnitCode = sessionSchedule.ScheduleUnitCode;
                    model.ScheduleUnitName = sessionSchedule.ScheduleUnitName;
                    model.SessionScheduleId = sessionSchedule.SessionScheduleId;
                }
            }
            return View(model);
        }

        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> CancelAppraisalSchedule(int id)
        {
            ManageAppraisalScheduleViewModel model = new ManageAppraisalScheduleViewModel();
            if (id > 0)
            {
                SessionSchedule sessionSchedule = new SessionSchedule();
                sessionSchedule = await _performanceService.GetSessionScheduleAsync(id);
                if (sessionSchedule != null)
                {
                    model.ReviewSessionId = sessionSchedule.ReviewSessionId;
                    model.ReviewSessionName = sessionSchedule.ReviewSessionName;
                    model.ActivityTypeId = (int)sessionSchedule.ActivityType;
                    model.ActivityTypeDescription = sessionSchedule.SessionActivityTypeDescription;
                    model.ReviewSessionId = sessionSchedule.ReviewSessionId;
                    model.ReviewSessionName = sessionSchedule.ReviewSessionName;
                    model.ReviewYearId = sessionSchedule.ReviewYearId;
                    model.ReviewYearName = sessionSchedule.ReviewYearName;
                    model.ScheduleDepartmentCode = sessionSchedule.ScheduleDepartmentCode;
                    model.ScheduleDepartmentName = sessionSchedule.ScheduleDepartmentName;
                    model.ScheduleEmployeeId = sessionSchedule.ScheduleEmployeeId;
                    model.ScheduleEmployeeName = sessionSchedule.ScheduleEmployeeName;
                    model.ScheduleEndTime = sessionSchedule.ScheduleEndTime;
                    model.ScheduleLocationId = sessionSchedule.ScheduleLocationId;
                    model.ScheduleLocationName = sessionSchedule.ScheduleLocationName;
                    model.ScheduleStartTime = sessionSchedule.ScheduleStartTime;
                    model.ScheduleTypeDescription = sessionSchedule.ScheduleTypeDescription;
                    model.ScheduleTypeId = (int)sessionSchedule.ScheduleType;
                    model.ScheduleUnitCode = sessionSchedule.ScheduleUnitCode;
                    model.ScheduleUnitName = sessionSchedule.ScheduleUnitName;
                    model.SessionScheduleId = sessionSchedule.SessionScheduleId;
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSSTT, XXACC")]
        public async Task<IActionResult> CancelAppraisalSchedule(ManageAppraisalScheduleViewModel model)
        {
            if (model != null && model.SessionScheduleId > 0)
            {
                string CancelledBy = HttpContext.User.Identity.Name;
                bool IsAdded = await _performanceService.CancelSessionScheduleAsync(model.SessionScheduleId, CancelledBy);
                if (IsAdded)
                {
                    model.ViewModelSuccessMessage = "Appraisal Schedule was cancelled successfully!";
                    model.OperationIsSuccessful = true;
                }
                else { model.ViewModelErrorMessage = "Sorry an error was encountered. Appraisal Schedule could not be cancelled."; }

            }

            SessionSchedule sessionSchedule = new SessionSchedule();
            sessionSchedule = await _performanceService.GetSessionScheduleAsync(model.SessionScheduleId);
            if (sessionSchedule != null)
            {
                model.ReviewSessionId = sessionSchedule.ReviewSessionId;
                model.ReviewSessionName = sessionSchedule.ReviewSessionName;
                model.ActivityTypeId = (int)sessionSchedule.ActivityType;
                model.ActivityTypeDescription = sessionSchedule.SessionActivityTypeDescription;
                model.ReviewSessionId = sessionSchedule.ReviewSessionId;
                model.ReviewSessionName = sessionSchedule.ReviewSessionName;
                model.ReviewYearId = sessionSchedule.ReviewYearId;
                model.ReviewYearName = sessionSchedule.ReviewYearName;
                model.ScheduleDepartmentCode = sessionSchedule.ScheduleDepartmentCode;
                model.ScheduleDepartmentName = sessionSchedule.ScheduleDepartmentName;
                model.ScheduleEmployeeId = sessionSchedule.ScheduleEmployeeId;
                model.ScheduleEmployeeName = sessionSchedule.ScheduleEmployeeName;
                model.ScheduleEndTime = sessionSchedule.ScheduleEndTime;
                model.ScheduleLocationId = sessionSchedule.ScheduleLocationId;
                model.ScheduleLocationName = sessionSchedule.ScheduleLocationName;
                model.ScheduleStartTime = sessionSchedule.ScheduleStartTime;
                model.ScheduleTypeDescription = sessionSchedule.ScheduleTypeDescription;
                model.ScheduleTypeId = (int)sessionSchedule.ScheduleType;
                model.ScheduleUnitCode = sessionSchedule.ScheduleUnitCode;
                model.ScheduleUnitName = sessionSchedule.ScheduleUnitName;
                model.SessionScheduleId = sessionSchedule.SessionScheduleId;
            }
            return View(model);
        }

        #endregion

        #region My Appraisal Sessions Controller Actions

        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> MyAppraisalSessions(int? id)
        {
            ReviewSessionsListViewModel model = new ReviewSessionsListViewModel();
            if (id != null && id.Value > 0)
            {
                var entities = await _performanceService.GetReviewSessionsAsync(id.Value);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewSessionsList = entities.ToList();
                }
            }
            else
            {
                var entities = await _performanceService.GetReviewSessionsAsync();
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewSessionsList = entities.ToList();
                }
            }

            List<PerformanceYear> pyears = await _performanceService.GetPerformanceYearsAsync();
            if (pyears != null && pyears.Count > 0)
            {
                ViewBag.PerformanceYearsList = new SelectList(pyears, "Id", "Name", id);
            }
            ViewBag.AppraiseeName = HttpContext.User.Identity.Name;

            return View(model);
        }

        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> MyAppraisalSteps(int id)
        {
            MyAppraisalStepsViewModel model = new MyAppraisalStepsViewModel();
            ReviewSession reviewSession = new ReviewSession();

            if (id > 0)
            {
                model.ReviewSessionId = id;
                reviewSession = await _performanceService.GetReviewSessionAsync(id);
                if (!string.IsNullOrWhiteSpace(reviewSession.Name))
                {
                    model.ReviewSessionName = reviewSession.Name;
                    model.IsActive = reviewSession.IsActive;
                    if (!model.IsActive)
                    {
                        model.ViewModelWarningMessage = $"Note: The selected Appraisal Session is closed. All action buttons have been disabled. ";
                    }
                }
            }

            ApplicationUser user = new ApplicationUser();
            int userId = 0;
            var userStringId = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
            if (!string.IsNullOrWhiteSpace(userStringId)) { userId = Convert.ToInt32(userStringId); }
            user = await _securityService.GetUserByIdAsync(userId);
            if (user != null && !string.IsNullOrWhiteSpace(user.FullName))
            {
                model.AppraiseeName = user.FullName;
                model.AppraiseeId = user.EmployeeId;
                if (user.EmployeeId < 1)
                {
                    model.ViewModelErrorMessage = "Sorry, no employee record was found for the selected user. ";
                    return View(model);
                }
                else
                {
                    ReviewSchedule reviewSchedule = new ReviewSchedule();
                    reviewSchedule = await _performanceService.GetEmployeePerformanceScheduleAsync(id, model.AppraiseeId);
                    if ((reviewSchedule == null) || (reviewSchedule.AllActivitiesScheduled == false && reviewSchedule.ContractDefinitionScheduled == false && reviewSchedule.PerformanceEvaluationScheduled == false))
                    {
                        model.ViewModelWarningMessage = $"Note: You are not scheduled for any activity on this Appraisal Session. All action buttons have been disabled. ";
                    }

                    model.AllActivitiesScheduled = reviewSchedule.AllActivitiesScheduled;
                    model.ContractDefinitionScheduled = reviewSchedule.ContractDefinitionScheduled;
                    model.PerformanceEvaluationScheduled = reviewSchedule.PerformanceEvaluationScheduled;
                }
            }


            if (id > 0 && model.AppraiseeId > 0)
            {
                ReviewHeader reviewHeader = new ReviewHeader();
                reviewHeader = await _performanceService.GetReviewHeaderAsync(model.AppraiseeId, id);
                if (reviewHeader != null)
                {
                    model.CurrentReviewStageId = reviewHeader.ReviewStageId;
                    model.ReviewHeaderId = reviewHeader.ReviewHeaderId;
                }
                else
                {
                    model.CurrentReviewStageId = 0;
                }
            }
            var stagesEntities = await _performanceService.GetReviewStagesAsync();
            if (stagesEntities != null && stagesEntities.Count > 0)
            {
                model.AppraisalStageList = stagesEntities;
            }
            return View(model);
        }

        [Authorize(Roles = "PMSAPR, PMSAPV, XXACC")]
        public async Task<IActionResult> AppraisalActivities(int id, string sp)
        {
            AppraisalActivitiesViewModel model = new AppraisalActivitiesViewModel();
            model.SourcePage = sp;
            if (id > 0)
            {
                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.AppraiseeName = reviewHeader.AppraiseeName;
                    model.AppraiseeID = reviewHeader.AppraiseeId;

                    ApplicationUser user = new ApplicationUser();
                    int userId = 0;
                    var userStringId = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
                    if (!string.IsNullOrWhiteSpace(userStringId)) { userId = Convert.ToInt32(userStringId); }
                    user = await _securityService.GetUserByIdAsync(userId);
                    if (!string.IsNullOrWhiteSpace(user.FullName))
                    {
                        model.LoggedInEmployeeID = user.EmployeeId;
                    }
                }

                var entities = await _performanceService.GetPmsActivityHistory(id);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewActivityList = entities.ToList();
                }
            }
            return View(model);
        }

        #endregion

        #region Performance Goals Controller Actions

        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> CreatePerformanceGoals(int id, int ad)
        {
            CreatePerformanceGoalsViewModel model = new CreatePerformanceGoalsViewModel();
            List<EmployeeReport> reports = new List<EmployeeReport>();
            model.AppraiseeID = ad;
            model.ReviewSessionID = id;
            var reporting_entities = await _employeeRecordService.GetEmployeeReportsByEmployeeIdAsync(ad);
            if (reporting_entities != null && reporting_entities.Count > 0)
            {
                reports = reporting_entities;
            }
            ViewBag.ReportList = new SelectList(reports, "ReportsToId", "ReportsToName");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> CreatePerformanceGoals(CreatePerformanceGoalsViewModel model)
        {
            List<EmployeeReport> reports = new List<EmployeeReport>();
            if (ModelState.IsValid)
            {
                try
                {
                    ReviewHeader reviewHeader = new ReviewHeader();
                    reviewHeader.ReviewSessionId = model.ReviewSessionID;
                    reviewHeader.ReviewHeaderId = model.ReviewHeaderID ?? 0;
                    reviewHeader.AppraiseeId = model.AppraiseeID;
                    reviewHeader.PerformanceGoal = model.PerformanceGoals;
                    reviewHeader.PrimaryAppraiserId = model.AppraiserID;
                    reviewHeader.ReviewStageId = 1;

                    Employee employee = await _employeeRecordService.GetEmployeeByIdAsync(model.AppraiseeID);
                    if (employee != null)
                    {
                        if (string.IsNullOrWhiteSpace(employee.UnitCode))
                        {
                            reviewHeader.UnitCode = null;
                        }
                        else { reviewHeader.UnitCode = employee.UnitCode; }

                        if (string.IsNullOrWhiteSpace(employee.DepartmentCode))
                        {
                            reviewHeader.DepartmentCode = null;
                        }
                        else { reviewHeader.DepartmentCode = employee.DepartmentCode; }

                        if (employee.LocationID < 1) { reviewHeader.LocationId = null; }
                        else { reviewHeader.LocationId = employee.LocationID; }
                    }

                    ReviewSession reviewSession = new ReviewSession();
                    reviewSession = await _performanceService.GetReviewSessionAsync(model.ReviewSessionID);
                    if (reviewSession != null)
                    {
                        reviewHeader.ReviewYearId = reviewSession.ReviewYearId;
                    }

                    bool isAdded = await _performanceService.AddReviewHeaderAsync(reviewHeader);
                    if (isAdded)
                    {
                        PmsActivityHistory activityHistory = new PmsActivityHistory();
                        var entity = await _performanceService.GetReviewHeaderAsync(model.AppraiseeID, model.ReviewSessionID);
                        if (entity != null)
                        {
                            activityHistory.ReviewHeaderId = entity.ReviewHeaderId;
                            activityHistory.ActivityTime = DateTime.UtcNow;
                            activityHistory.ActivityDescription = $"Started the Appraisal Process and added performance goal.";
                            await _performanceService.AddPmsActivityHistoryAsync(activityHistory);
                        }
                        model.ViewModelSuccessMessage = "Performance Goal added successfully!";
                        model.OperationIsSuccessful = true;
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            var reporting_entities = await _employeeRecordService.GetEmployeeReportsByEmployeeIdAsync(model.AppraiseeID);
            if (reporting_entities != null && reporting_entities.Count > 0)
            {
                reports = reporting_entities;
            }
            ViewBag.ReportList = new SelectList(reports, "ReportsToId", "ReportsToName");
            return View(model);
        }

        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> ManagePerformanceGoal(int id)
        {
            CreatePerformanceGoalsViewModel model = new CreatePerformanceGoalsViewModel();
            List<EmployeeReport> reports = new List<EmployeeReport>();
            ReviewHeader reviewHeader = new ReviewHeader();

            try
            {
                reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                    model.PerformanceGoals = reviewHeader.PerformanceGoal;
                    model.AppraiserID = reviewHeader.PrimaryAppraiserId.Value;
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            var reporting_entities = await _employeeRecordService.GetEmployeeReportsByEmployeeIdAsync(reviewHeader.AppraiseeId);
            if (reporting_entities != null && reporting_entities.Count > 0)
            {
                reports = reporting_entities;
            }
            ViewBag.ReportList = new SelectList(reports, "ReportsToId", "ReportsToName", model.AppraiserID);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> ManagePerformanceGoal(CreatePerformanceGoalsViewModel model)
        {
            List<EmployeeReport> reports = new List<EmployeeReport>();
            try
            {
                if (model.ReviewHeaderID > 0)
                {
                    bool isUpdated = await _performanceService.UpdatePerformanceGoalAsync(model.ReviewHeaderID.Value, model.PerformanceGoals, model.AppraiserID);
                    if (isUpdated)
                    {
                        PmsActivityHistory activityHistory = new PmsActivityHistory();
                        var entity = await _performanceService.GetReviewHeaderAsync(model.AppraiseeID, model.ReviewSessionID);
                        if (entity != null)
                        {
                            activityHistory.ReviewHeaderId = entity.ReviewHeaderId;
                            activityHistory.ActivityTime = DateTime.UtcNow;
                            activityHistory.ActivityDescription = $"Updated performance goal(s) for this appraisal session.";
                            await _performanceService.AddPmsActivityHistoryAsync(activityHistory);
                        }
                        model.ViewModelSuccessMessage = "Performance Goal updated successfully!";
                        model.OperationIsSuccessful = true;
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "An error was encountered. The attempted update failed.";
                    }
                }
                else
                {
                    model.ViewModelErrorMessage = "Missing Parameter Error. The operation failed because a key parameter is missing.";
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            var reporting_entities = await _employeeRecordService.GetEmployeeReportsByEmployeeIdAsync(model.AppraiseeID);
            if (reporting_entities != null && reporting_entities.Count > 0)
            {
                reports = reporting_entities;
            }
            ViewBag.ReportList = new SelectList(reports, "ReportsToId", "ReportsToName");

            return View(model);
        }

        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> ShowPerformanceGoal(int id)
        {
            CreatePerformanceGoalsViewModel model = new CreatePerformanceGoalsViewModel();
            ReviewHeader reviewHeader = new ReviewHeader();
            try
            {
                reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                    model.PerformanceGoals = reviewHeader.PerformanceGoal;
                    model.AppraiserName = reviewHeader.PrimaryAppraiserName;
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }
        #endregion

        #region MoveToNextStep Controller Actions

        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> MoveToNextStep(int id, int? ad = null)
        {
            MoveToNextStageModel model = new MoveToNextStageModel();
            if (id > 0)
            {
                model = await _performanceService.ValidateMoveRequestAsync(id, ad);
            }
            else
            {
                return RedirectToAction("MyAppraisalSessions");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> MoveToNextStep(MoveToNextStageModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.ReviewHeaderID > 0 && model.NextStageID > 1)
                {
                    await _performanceService.UpdateReviewHeaderStageAsync(model.ReviewHeaderID, model.NextStageID);
                }
            }
            else
            {
                return RedirectToAction("MyAppraisalSessions");
            }
            return RedirectToAction("MyAppraisalSteps", new { id = model.ReviewSessionID });
        }

        [Authorize(Roles = "PMSAPR, PMSAPV, XXACC")]
        public async Task<IActionResult> MoveToPreviousStep(int id)
        {
            MoveToPreviousStepModel model = new MoveToPreviousStepModel();
            ReviewHeader reviewHeader = new ReviewHeader();
            List<ReviewStage> ReviewStages = new List<ReviewStage>();
            if (id > 0)
            {
                reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                    model.CurrentStageDescription = reviewHeader.ReviewStageDescription;
                    model.CurrentStageID = reviewHeader.ReviewStageId;
                    model.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;

                    var entities = await _performanceService.GetPreviousReviewStagesAsync(reviewHeader.ReviewStageId);
                    if (entities != null && entities.Count > 0)
                    {
                        ReviewStages = entities;
                    }
                }
                else { model.ViewModelErrorMessage = "The selected record could not be retrieved at this time. Please try again."; }
            }
            else
            {
                return RedirectToAction("MyAppraisalSessions");
            }
            ViewBag.ReviewStageList = new SelectList(ReviewStages, "ReviewStageId", "ReviewStageName");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSAPR, PMSAPV, XXACC")]
        public async Task<IActionResult> MoveToPreviousStep(MoveToPreviousStepModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.ReviewHeaderID > 0 && model.NewStageID > 1)
                {
                    await _performanceService.UpdateReviewHeaderStageAsync(model.ReviewHeaderID, model.NewStageID);
                }
            }
            else
            {
                return RedirectToAction("MyAppraisalSessions");
            }
            return RedirectToAction("MyAppraisalSteps", new { id = model.ReviewSessionID });
        }
        #endregion

        #region Appraisal KPAs Controller Actions

        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> AppraisalKpas(int id)
        {
            AppraisalKpaViewModel model = new AppraisalKpaViewModel();
            if (id > 0)
            {
                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.ReviewStageID = reviewHeader.ReviewStageId;
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                }

                var entities = await _performanceService.GetKpasAsync(id);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewMetricList = entities.ToList();
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be r";
            }

            return View(model);
        }

        public async Task<IActionResult> ShowAppraisalKpas(int id)
        {
            AppraisalKpaViewModel model = new AppraisalKpaViewModel();
            if (id > 0)
            {
                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.ReviewStageID = reviewHeader.ReviewStageId;
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                }

                var entities = await _performanceService.GetKpasAsync(id);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewMetricList = entities.ToList();
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be r";
            }
            return View(model);
        }

        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> ManageAppraisalKpa(int id, int? md = null)
        {
            ManageAppraisalKpaViewModel model = new ManageAppraisalKpaViewModel();
            ReviewHeader reviewHeader = new ReviewHeader();
            try
            {
                model.ReviewHeaderId = id;
                if (id > 0)
                {
                    reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                    if (reviewHeader != null)
                    {
                        model.ReviewSessionId = reviewHeader.ReviewSessionId;
                        model.ReviewYearId = reviewHeader.ReviewYearId;
                        model.AppraiseeId = reviewHeader.AppraiseeId;
                    }
                }

                if (md != null && md.Value > 0)
                {
                    ReviewMetric reviewMetric = await _performanceService.GetReviewMetricAsync(md.Value);
                    if (reviewMetric != null)
                    {
                        model.ReviewMetricId = reviewMetric.ReviewMetricId;
                        model.ReviewMetricDescription = reviewMetric.ReviewMetricDescription;
                        model.ReviewMetricKpi = reviewMetric.ReviewMetricKpi;
                        model.ReviewMetricTarget = reviewMetric.ReviewMetricTarget;
                        model.ReviewMetricWeightage = reviewMetric.ReviewMetricWeightage;
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
        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> ManageAppraisalKpa(ManageAppraisalKpaViewModel model)
        {
            if (ModelState.IsValid)
            {
                ReviewMetric reviewMetric = new ReviewMetric();
                reviewMetric = model.ConvertToReviewMetric();
                try
                {
                    if (reviewMetric.ReviewMetricId > 0)
                    {
                        bool isUpdated = await _performanceService.UpdateReviewMetricAsync(reviewMetric);
                        if (isUpdated)
                        {
                            model.ViewModelSuccessMessage = "KPA updated successfully!";
                            model.OperationIsSuccessful = true;
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "An error was encountered. The attempted update failed.";
                        }
                    }
                    else
                    {
                        bool isAdded = await _performanceService.AddReviewMetricAsync(reviewMetric);
                        if (isAdded)
                        {
                            model.ViewModelSuccessMessage = "KPA added successfully!";
                            model.OperationIsSuccessful = true;
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "An error was encountered. The attempted update failed.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }

        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> DeleteAppraisalKpa(int id)
        {
            ManageAppraisalKpaViewModel model = new ManageAppraisalKpaViewModel();
            ReviewMetric reviewMetric = new ReviewMetric();
            try
            {
                if (id > 0)
                {
                    reviewMetric = await _performanceService.GetReviewMetricAsync(id);
                    if (reviewMetric != null)
                    {
                        model.ReviewMetricId = reviewMetric.ReviewMetricId;
                        model.ReviewMetricDescription = reviewMetric.ReviewMetricDescription;
                        model.ReviewMetricKpi = reviewMetric.ReviewMetricKpi;
                        model.ReviewMetricTarget = reviewMetric.ReviewMetricTarget;
                        model.ReviewMetricWeightage = reviewMetric.ReviewMetricWeightage;
                        model.ReviewSessionId = reviewMetric.ReviewSessionId;
                        model.ReviewYearId = reviewMetric.ReviewYearId;
                        model.AppraiseeId = reviewMetric.AppraiseeId;
                        model.ReviewHeaderId = reviewMetric.ReviewHeaderId;
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
        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> DeleteAppraisalKpa(ManageAppraisalKpaViewModel model)
        {
            if (ModelState.IsValid)
            {
                ReviewMetric reviewMetric = new ReviewMetric();
                reviewMetric = model.ConvertToReviewMetric();
                try
                {
                    if (reviewMetric.ReviewMetricId > 0)
                    {
                        bool isDeleted = await _performanceService.DeleteReviewMetricAsync(reviewMetric.ReviewMetricId);
                        if (isDeleted)
                        {
                            return RedirectToAction("AppraisalKpas", new { id = model.ReviewHeaderId });
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "An error was encountered. The attempted delete operation failed.";
                        }
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

        #region Appraisal Competencies Controller Actions

        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> AppraisalCompetencies(int id)
        {
            AppraisalCompetenciesViewModel model = new AppraisalCompetenciesViewModel();
            if (id > 0)
            {
                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.ReviewStageID = reviewHeader.ReviewStageId;
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                }

                var entities = await _performanceService.GetCompetenciesAsync(id);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewMetricList = entities.ToList();
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be r";
            }

            return View(model);
        }

        public async Task<IActionResult> ShowAppraisalCompetencies(int id)
        {
            AppraisalCompetenciesViewModel model = new AppraisalCompetenciesViewModel();
            if (id > 0)
            {
                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.ReviewStageID = reviewHeader.ReviewStageId;
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                }

                var entities = await _performanceService.GetCompetenciesAsync(id);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewMetricList = entities.ToList();
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be r";
            }

            return View(model);
        }

        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> SelectCompetencies(int id, int vd, int cd)
        {
            SelectCompetencyViewModel model = new SelectCompetencyViewModel();
            if (id > 0)
            {
                model.ReviewHeaderID = id;
                var entities = await _performanceService.SearchFromCompetencyDictionaryAsync(cd, vd);
                if (entities != null && entities.Count > 0)
                {
                    model.CompetenciesList = entities.ToList();
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be r";
            }
            var levelEntities = await _performanceService.GetCompetencyLevelsAsync();
            var categoryEntities = await _performanceService.GetCompetencyCategoriesAsync();

            ViewBag.LevelList = new SelectList(levelEntities, "Id", "Description", vd);
            ViewBag.CategoryList = new SelectList(categoryEntities, "Id", "Description", cd);
            return View(model);
        }

        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> AddCompetency(int id, int rd)
        {
            AddCompetencyViewModel model = new AddCompetencyViewModel();
            ReviewMetric reviewMetric = new ReviewMetric();
            ReviewHeader reviewHeader = new ReviewHeader();
            if (id < 1)
            {
                model.ViewModelErrorMessage = "Sorry, a key parameter is missing thus the selected Competency could not be retrieved. Please try again.";
                model.ReviewHeaderId = rd;
            }
            else
            {
                Competency competency = new Competency();
                competency = await _performanceService.GetFromCompetencyDictionaryByIdAsync(id);
                if (competency != null && !string.IsNullOrWhiteSpace(competency.Description))
                {
                    model.CompetencyId = competency.Id;
                    model.ReviewMetricDescription = competency.Description;
                    if (rd < 1)
                    {
                        model.ViewModelErrorMessage = "Sorry, a key parameter is missing thus the selected Competency could not be retrieved. Please try again.";
                    }
                    else
                    {
                        reviewHeader = await _performanceService.GetReviewHeaderAsync(rd);
                        if (reviewHeader != null)
                        {
                            model.AppraiseeId = reviewHeader.AppraiseeId;
                            model.AppraiseeName = reviewHeader.AppraiseeName;
                            model.ReviewHeaderId = reviewHeader.ReviewHeaderId;
                            model.ReviewSessionId = reviewHeader.ReviewSessionId;
                            model.ReviewYearId = reviewHeader.ReviewYearId;
                        }
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> AddCompetency(AddCompetencyViewModel model)
        {
            if (ModelState.IsValid)
            {
                ReviewMetric reviewMetric = new ReviewMetric();
                reviewMetric = model.ConvertToReviewMetric();
                try
                {
                    if (reviewMetric.ReviewMetricId > 0)
                    {
                        bool isUpdated = await _performanceService.UpdateReviewMetricAsync(reviewMetric);
                        if (isUpdated)
                        {
                            model.ViewModelSuccessMessage = "Competency updated successfully!";
                            model.OperationIsSuccessful = true;
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "An error was encountered. The attempted update failed.";
                        }
                    }
                    else
                    {
                        bool isAdded = await _performanceService.AddReviewMetricAsync(reviewMetric);
                        if (isAdded)
                        {
                            model.ViewModelSuccessMessage = "Competency added successfully!";
                            model.OperationIsSuccessful = true;
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "An error was encountered. The attempted operation failed.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }

        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> ManageAppraisalCompetency(int id)
        {
            AddCompetencyViewModel model = new AddCompetencyViewModel();
            ReviewMetric reviewMetric = new ReviewMetric();
            ReviewHeader reviewHeader = new ReviewHeader();
            if (id < 1)
            {
                model.ViewModelErrorMessage = "Sorry, a key parameter is missing thus the selected Competency could not be retrieved. Please try again.";
            }
            else
            {
                reviewMetric = await _performanceService.GetReviewMetricAsync(id);
                if (reviewMetric != null)
                {
                    model.AppraiseeId = reviewMetric.AppraiseeId;
                    model.AppraiseeName = reviewMetric.AppraiseeName;
                    model.ReviewHeaderId = reviewMetric.ReviewHeaderId;
                    model.ReviewSessionId = reviewMetric.ReviewSessionId;
                    model.ReviewYearId = reviewMetric.ReviewYearId;
                    model.ReviewMetricDescription = reviewMetric.ReviewMetricDescription;
                    model.ReviewMetricId = reviewMetric.ReviewMetricId;
                    model.ReviewMetricWeightage = reviewMetric.ReviewMetricWeightage;
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> ManageAppraisalCompetency(AddCompetencyViewModel model)
        {
            if (ModelState.IsValid)
            {
                ReviewMetric reviewMetric = new ReviewMetric();
                reviewMetric = model.ConvertToReviewMetric();
                try
                {
                    if (reviewMetric.ReviewMetricId > 0)
                    {
                        bool isUpdated = await _performanceService.UpdateReviewMetricAsync(reviewMetric);
                        if (isUpdated)
                        {
                            model.ViewModelSuccessMessage = "Competency updated successfully!";
                            model.OperationIsSuccessful = true;
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "An error was encountered. The attempted update failed.";
                        }
                    }
                    else
                    {
                        bool isAdded = await _performanceService.AddReviewMetricAsync(reviewMetric);
                        if (isAdded)
                        {
                            model.ViewModelSuccessMessage = "Competency added successfully!";
                            model.OperationIsSuccessful = true;
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "An error was encountered. The attempted operation failed.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }

        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> DeleteAppraisalCompetency(int id)
        {
            AddCompetencyViewModel model = new AddCompetencyViewModel();
            ReviewMetric reviewMetric = new ReviewMetric();
            ReviewHeader reviewHeader = new ReviewHeader();
            if (id < 1)
            {
                model.ViewModelErrorMessage = "Sorry, a key parameter is missing thus the selected Competency could not be retrieved. Please try again.";
            }
            else
            {
                reviewMetric = await _performanceService.GetReviewMetricAsync(id);
                if (reviewMetric != null)
                {
                    model.AppraiseeId = reviewMetric.AppraiseeId;
                    model.AppraiseeName = reviewMetric.AppraiseeName;
                    model.ReviewHeaderId = reviewMetric.ReviewHeaderId;
                    model.ReviewSessionId = reviewMetric.ReviewSessionId;
                    model.ReviewSessionDescription = reviewMetric.ReviewSessionDescription;
                    model.ReviewYearId = reviewMetric.ReviewYearId;
                    model.ReviewYearName = reviewMetric.ReviewYearName;
                    model.ReviewMetricDescription = reviewMetric.ReviewMetricDescription;
                    model.ReviewMetricId = reviewMetric.ReviewMetricId;
                    model.ReviewMetricWeightage = reviewMetric.ReviewMetricWeightage;
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> DeleteAppraisalCompetency(AddCompetencyViewModel model)
        {
            if (ModelState.IsValid)
            {
                ReviewMetric reviewMetric = new ReviewMetric();
                reviewMetric = model.ConvertToReviewMetric();
                try
                {
                    if (reviewMetric.ReviewMetricId > 0)
                    {
                        bool isDeleted = await _performanceService.DeleteReviewMetricAsync(reviewMetric.ReviewMetricId);
                        if (isDeleted)
                        {
                            return RedirectToAction("AppraisalCompetencies", new { id = model.ReviewHeaderId });
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "An error was encountered. The attempted delete operation failed.";
                        }
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "A key parameter is missing. The delete operation failed. Please try again.";
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

        #region Appraisal Career Development Goals Controller Actions

        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> AppraisalCdgs(int id)
        {
            AppraisalCdgViewModel model = new AppraisalCdgViewModel();
            if (id > 0)
            {
                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.ReviewStageID = reviewHeader.ReviewStageId;
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                }

                var entities = await _performanceService.GetReviewCdgsAsync(id);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewCdgList = entities.ToList();
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be retrieved.";
            }
            return View(model);
        }

        public async Task<IActionResult> ShowAppraisalCdgs(int id)
        {
            AppraisalCdgViewModel model = new AppraisalCdgViewModel();
            if (id > 0)
            {
                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.ReviewStageID = reviewHeader.ReviewStageId;
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                }

                var entities = await _performanceService.GetReviewCdgsAsync(id);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewCdgList = entities.ToList();
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be retrieved.";
            }
            return View(model);
        }

        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> ManageAppraisalCdg(int id, int? cd = null)
        {
            ManageAppraisalCdgViewModel model = new ManageAppraisalCdgViewModel();
            ReviewHeader reviewHeader = new ReviewHeader();
            try
            {
                model.ReviewHeaderId = id;
                if (id > 0)
                {
                    reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                    if (reviewHeader != null)
                    {
                        model.ReviewSessionId = reviewHeader.ReviewSessionId;
                        model.ReviewYearId = reviewHeader.ReviewYearId;
                        model.AppraiseeId = reviewHeader.AppraiseeId;
                    }
                }

                if (cd != null && cd.Value > 0)
                {
                    ReviewCDG reviewCDG = await _performanceService.GetReviewCdgAsync(cd.Value);
                    if (reviewCDG != null)
                    {
                        model.ReviewCdgId = reviewCDG.ReviewCdgId;
                        model.ReviewCdgDescription = reviewCDG.ReviewCdgDescription;
                        model.AppraiseeId = reviewCDG.AppraiseeId;
                        model.ReviewCdgObjective = reviewCDG.ReviewCdgObjective;
                        model.ReviewCdgActionPlan = reviewCDG.ReviewCdgActionPlan;
                        model.ReviewHeaderId = reviewCDG.ReviewHeaderId;
                        model.ReviewSessionId = reviewCDG.ReviewSessionId;
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
        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> ManageAppraisalCdg(ManageAppraisalCdgViewModel model)
        {
            if (ModelState.IsValid)
            {
                ReviewCDG reviewCDG = new ReviewCDG();
                reviewCDG = model.ConvertToReviewCdg();
                try
                {
                    if (reviewCDG.ReviewCdgId > 0)
                    {
                        bool isUpdated = await _performanceService.UpdateReviewCdgAsync(reviewCDG);
                        if (isUpdated)
                        {
                            model.ViewModelSuccessMessage = "CDG updated successfully!";
                            model.OperationIsSuccessful = true;
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "An error was encountered. The attempted update failed.";
                        }
                    }
                    else
                    {
                        bool isAdded = await _performanceService.AddReviewCdgAsync(reviewCDG);
                        if (isAdded)
                        {
                            model.ViewModelSuccessMessage = "CDG added successfully!";
                            model.OperationIsSuccessful = true;
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "An error was encountered. The attempted update failed.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }

        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> DeleteAppraisalCdg(int id)
        {
            ManageAppraisalCdgViewModel model = new ManageAppraisalCdgViewModel();
            ReviewCDG reviewCDG = new ReviewCDG();
            try
            {
                if (id > 0)
                {
                    reviewCDG = await _performanceService.GetReviewCdgAsync(id);
                    if (reviewCDG != null)
                    {
                        model.ReviewCdgId = reviewCDG.ReviewCdgId;
                        model.ReviewCdgDescription = reviewCDG.ReviewCdgDescription;
                        model.AppraiseeId = reviewCDG.AppraiseeId;
                        model.AppraiseeName = reviewCDG.AppraiseeName;
                        model.ReviewCdgObjective = reviewCDG.ReviewCdgObjective;
                        model.ReviewCdgActionPlan = reviewCDG.ReviewCdgActionPlan;
                        model.ReviewSessionId = reviewCDG.ReviewSessionId;
                        model.ReviewSessionDescription = reviewCDG.ReviewSessionName;
                        model.ReviewYearId = reviewCDG.ReviewYearId;
                        model.AppraiseeId = reviewCDG.AppraiseeId;
                        model.ReviewHeaderId = reviewCDG.ReviewHeaderId;
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
        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> DeleteAppraisalCdg(ManageAppraisalCdgViewModel model)
        {
            if (ModelState.IsValid)
            {
                ReviewCDG reviewCDG = new ReviewCDG();
                reviewCDG = model.ConvertToReviewCdg();
                try
                {
                    if (reviewCDG.ReviewCdgId > 0)
                    {
                        bool isDeleted = await _performanceService.DeleteReviewCdgAsync(reviewCDG.ReviewCdgId);
                        if (isDeleted)
                        {
                            return RedirectToAction("AppraisalCdgs", new { id = model.ReviewHeaderId });
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "An error was encountered. The attempted delete operation failed.";
                        }
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

        #region Appraisal Submissions Controller Actions
        public async Task<IActionResult> AppraisalSubmissionHistory(int id)
        {
            AppraisalSubmissionHistoryViewModel model = new AppraisalSubmissionHistoryViewModel();
            if (id > 0)
            {
                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.ReviewStageID = reviewHeader.ReviewStageId;
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                }

                var entities = await _performanceService.GetReviewSubmissionsByReviewHeaderIdAsync(id);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewSubmissionList = entities.ToList();
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be retrieved.";
            }

            return View(model);
        }

        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> SubmitAppraisal(int id)
        {
            SubmitAppraisalViewModel model = new SubmitAppraisalViewModel();
            ReviewHeader reviewHeader = new ReviewHeader();
            try
            {
                model.ReviewHeaderID = id;
                if (id > 0)
                {
                    reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                    if (reviewHeader != null)
                    {
                        model.ReviewSessionID = reviewHeader.ReviewSessionId;
                        model.AppraiseeID = reviewHeader.AppraiseeId;
                        model.FromEmployeeID = reviewHeader.AppraiseeId;
                        model.ToEmployeeID = reviewHeader.PrimaryAppraiserId.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            var role_entities = await _performanceService.GetApprovalRolesAsync();
            ViewBag.ApproverRolesList = new SelectList(role_entities, "ApprovalRoleId", "ApprovalRoleName", model.ToEmployeeID);

            var entities = await _employeeRecordService.GetEmployeeReportsByEmployeeIdAsync(model.AppraiseeID);
            ViewBag.ReportingLinesList = new SelectList(entities, "ReportsToId", "ReportsToName", model.ToEmployeeID);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> SubmitAppraisal(SubmitAppraisalViewModel model)
        {
            if (ModelState.IsValid)
            {
                ReviewSubmission reviewSubmission = new ReviewSubmission();
                reviewSubmission = model.ConvertToReviewSubmission();
                reviewSubmission.TimeSubmitted = DateTime.UtcNow;
                try
                {
                    bool isAdded = await _performanceService.AddReviewSubmissionAsync(reviewSubmission);
                    if (isAdded)
                    {
                        model.ViewModelSuccessMessage = "Appraisal submitted successfully!";
                        model.OperationIsSuccessful = true;

                        Employee sender = new Employee();
                        sender = await _employeeRecordService.GetEmployeeByIdAsync(model.FromEmployeeID);
                        Employee approver = new Employee();
                        approver = await _employeeRecordService.GetEmployeeByIdAsync(model.ToEmployeeID);
                        PmsActivityHistory history = new PmsActivityHistory();
                        history.ActivityDescription = $"Appraisal was submitted to {approver.FullName} by {sender.FullName} on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()}";
                        history.ActivityTime = DateTime.UtcNow;
                        history.ReviewHeaderId = model.ReviewHeaderID;
                        await _performanceService.AddPmsActivityHistoryAsync(history);
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "An error was encountered. The attempted submission failed.";
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }

        public async Task<IActionResult> AppraisalSubmissionDetails(int id)
        {
            SubmitAppraisalViewModel model = new SubmitAppraisalViewModel();
            ReviewSubmission reviewSubmission = new ReviewSubmission();
            try
            {
                model.ReviewSubmissionID = id;
                if (id > 0)
                {
                    reviewSubmission = await _performanceService.GetReviewSubmissionByIdAsync(id);
                    if (reviewSubmission != null)
                    {
                        model.ReviewSessionID = reviewSubmission.ReviewSessionId;
                        model.FromEmployeeID = reviewSubmission.FromEmployeeId;
                        model.FromEmployeeName = reviewSubmission.FromEmployeeName;
                        model.ReviewHeaderID = reviewSubmission.ReviewHeaderId;
                        model.ReviewSubmissionID = reviewSubmission.ReviewSubmissionId;
                        model.SubmissionMessage = reviewSubmission.SubmissionMessage;
                        model.SubmissionPurposeDescription = reviewSubmission.SubmissionPurposeDescription;
                        model.SubmissionPurposeID = reviewSubmission.SubmissionPurposeId;
                        model.TimeSubmitted = reviewSubmission.TimeSubmitted;
                        model.ToEmployeeID = reviewSubmission.ToEmployeeId;
                        model.ToEmployeeName = reviewSubmission.ToEmployeeName;
                        model.ToEmployeeRoleID = reviewSubmission.ToEmployeeRoleId;
                        model.ToEmployeeRoleName = reviewSubmission.ToEmployeeRoleName;

                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            return View(model);
        }

        #endregion

        #region Appraisals Submitted To Me Controller Actions

        [Authorize(Roles = "PMSAPV, XXACC")]
        public async Task<IActionResult> AppraisalsSubmittedtoMe(int? id)
        {
            AppraisalsSubmittedtoMeViewModel model = new AppraisalsSubmittedtoMeViewModel();
            model.id = id;

            ApplicationUser user = new ApplicationUser();
            int userId = 0;
            var userStringId = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
            if (!string.IsNullOrWhiteSpace(userStringId)) { userId = Convert.ToInt32(userStringId); }
            user = await _securityService.GetUserByIdAsync(userId);
            if (!string.IsNullOrWhiteSpace(user.FullName))
            {
                model.AppraiserName = user.FullName;
                model.AppraiserID = user.EmployeeId;
            }

            if (model.id != null && model.id.Value > 0)
            {
                model.id = id.Value;
                var entities = await _performanceService.GetReviewSubmissionsByApproverIdAsync(model.AppraiserID, model.id.Value);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewSubmissionList = entities.ToList();
                }
            }
            else
            {
                var entities = await _performanceService.GetReviewSubmissionsByApproverIdAsync(model.AppraiserID, null);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewSubmissionList = entities.ToList();
                }
            }

            List<ReviewSession> reviewSessions = await _performanceService.GetReviewSessionsAsync();
            if (reviewSessions != null && reviewSessions.Count > 0)
            {
                ViewBag.ReviewSessionList = new SelectList(reviewSessions, "Id", "Name", id);
            }

            return View(model);
        }

        public async Task<IActionResult> AppraisalInfo(int id, int sd)
        {
            AppraisalInfoViewModel model = new AppraisalInfoViewModel();
            ReviewHeader reviewHeader = new ReviewHeader();
            List<ReviewMetric> reviewKpas = new List<ReviewMetric>();
            List<ReviewMetric> reviewCmps = new List<ReviewMetric>();
            List<ReviewCDG> reviewCDGs = new List<ReviewCDG>();

            if (sd > 0) { model.ReviewSubmissionID = sd; }

            if (id > 0)
            {
                reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null) { model.AppraisalReviewHeader = reviewHeader; }
                reviewKpas = await _performanceService.GetKpasAsync(id);
                if (reviewKpas != null && reviewKpas.Count > 0) { model.KpaList = reviewKpas; }
                reviewCmps = await _performanceService.GetCompetenciesAsync(id);
                if (reviewCmps != null && reviewCmps.Count > 0) { model.CompetencyList = reviewCmps; }
                reviewCDGs = await _performanceService.GetReviewCdgsAsync(id);
                if (reviewCDGs != null && reviewCDGs.Count > 0) { model.CdgList = reviewCDGs; }
            }

            return View(model);
        }

        [Authorize(Roles = "PMSAPV, XXACC")]
        public async Task<IActionResult> ReturnContract(int id, int sd)
        {
            ReturnContractViewModel model = new ReturnContractViewModel();
            ReviewMessage reviewMessage = new ReviewMessage();
            try
            {
                if (id > 0) { model.ReviewHeaderID = id; }
                if (sd > 0) { model.ReviewSubmissionID = sd; }

                ApplicationUser user = new ApplicationUser();
                int userId = 0;
                var userStringId = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
                if (!string.IsNullOrWhiteSpace(userStringId)) { userId = Convert.ToInt32(userStringId); }
                user = await _securityService.GetUserByIdAsync(userId);
                if (!string.IsNullOrWhiteSpace(user.FullName))
                {
                    model.LoggedInEmployeeID = user.EmployeeId;
                    model.FromEmployeeName = user.FullName;
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
        [Authorize(Roles = "PMSAPV, XXACC")]
        public async Task<IActionResult> ReturnContract(ReturnContractViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ReviewMessage reviewMessage = new ReviewMessage();
                    if (!string.IsNullOrWhiteSpace(model.MessageBody))
                    {
                        reviewMessage = model.ConvertToReviewMessage();
                        reviewMessage.MessageTime = DateTime.UtcNow;
                        reviewMessage.FromEmployeeId = model.LoggedInEmployeeID;
                    }
                    else { reviewMessage = null; }

                    bool isReturned = await _performanceService.ReturnContractToAppraisee(1, model.ReviewSubmissionID, reviewMessage);
                    if (isReturned)
                    {
                        model.ViewModelSuccessMessage = "Returned to Appraisee successfully!";
                        model.OperationIsSuccessful = true;

                        PmsActivityHistory history = new PmsActivityHistory();
                        history.ActivityDescription = $"Performance Contract was not approved by {model.FromEmployeeName}. The appraisal was returned for corrections on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()} GMT";
                        history.ActivityTime = DateTime.UtcNow;
                        history.ReviewHeaderId = model.ReviewHeaderID;
                        await _performanceService.AddPmsActivityHistoryAsync(history);
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "An error was encountered. The attempted operation failed.";
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }

        [Authorize(Roles = "PMSAPV, XXACC")]
        public async Task<IActionResult> ApproveContract(int id, int sd)
        {
            ApproveContractViewModel model = new ApproveContractViewModel();
            model.SubmissionID = sd;
            model.ApprovalTypeID = (int)ReviewApprovalType.ApprovePerformanceContract;

            ReviewHeader reviewHeader = new ReviewHeader();
            try
            {
                if (id > 0)
                {
                    model.ReviewHeaderID = id;
                    reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                    if (reviewHeader != null)
                    {
                        model.AppraiseeID = reviewHeader.AppraiseeId;
                        model.AppraiseeName = reviewHeader.AppraiseeName;
                    }
                }

                ApplicationUser user = new ApplicationUser();
                int userId = 0;
                var userStringId = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
                if (!string.IsNullOrWhiteSpace(userStringId)) { userId = Convert.ToInt32(userStringId); }
                user = await _securityService.GetUserByIdAsync(userId);
                if (!string.IsNullOrWhiteSpace(user.FullName))
                {
                    model.ApproverID = user.EmployeeId;
                    model.ApproverName = user.FullName;
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            List<ApprovalRole> roles = new List<ApprovalRole>();
            var role_entities = await _performanceService.GetApprovalRolesAsync();
            if (role_entities != null) { roles = role_entities; }
            ViewBag.RoleList = new SelectList(roles, "ApprovalRoleId", "ApprovalRoleName");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSAPV, XXACC")]
        public async Task<IActionResult> ApproveContract(ApproveContractViewModel model)
        {
            if (ModelState.IsValid)
            {
                ReviewApproval reviewApproval = model.ConvertToReviewApproval();
                reviewApproval.IsApproved = true;
                reviewApproval.ApprovedTime = DateTime.UtcNow;
                reviewApproval.ApprovalTypeId = (int)ReviewApprovalType.ApprovePerformanceContract;
                try
                {
                    ApprovalRole approvalRole = new ApprovalRole();
                    approvalRole = await _performanceService.GetApprovalRoleAsync(reviewApproval.ApproverRoleId);
                    if (approvalRole != null) { model.ApproverRoleDescription = approvalRole.ApprovalRoleName; }

                    bool isApproved = await _performanceService.ApproveContractToAppraisee(reviewApproval, model.SubmissionID);
                    if (isApproved)
                    {
                        model.ViewModelSuccessMessage = "Appraisal Approved successfully!";
                        model.OperationIsSuccessful = true;

                        PmsActivityHistory history = new PmsActivityHistory();
                        history.ActivityDescription = $"Performance Contract was approved by {model.ApproverName} as {model.ApproverRoleDescription} on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()} GMT";
                        history.ActivityTime = DateTime.UtcNow;
                        history.ReviewHeaderId = model.ReviewHeaderID;
                        await _performanceService.AddPmsActivityHistoryAsync(history);
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "An error was encountered. The attempted operation failed.";
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }

        [Authorize(Roles = "PMSAPV, XXACC")]
        public async Task<IActionResult> ApproveEvaluation(int id, int sd)
        {
            ApproveContractViewModel model = new ApproveContractViewModel();
            model.SubmissionID = sd;
            model.ApprovalTypeID = (int)ReviewApprovalType.ApproveEvaluationResult;

            ReviewHeader reviewHeader = new ReviewHeader();
            try
            {
                if (id > 0)
                {
                    model.ReviewHeaderID = id;
                    reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                    if (reviewHeader != null)
                    {
                        model.AppraiseeID = reviewHeader.AppraiseeId;
                        model.AppraiseeName = reviewHeader.AppraiseeName;
                    }
                }

                ApplicationUser user = new ApplicationUser();
                int userId = 0;
                var userStringId = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
                if (!string.IsNullOrWhiteSpace(userStringId)) { userId = Convert.ToInt32(userStringId); }
                user = await _securityService.GetUserByIdAsync(userId);
                if (!string.IsNullOrWhiteSpace(user.FullName))
                {
                    model.ApproverID = user.EmployeeId;
                    model.ApproverName = user.FullName;
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            List<ApprovalRole> roles = new List<ApprovalRole>();
            var role_entities = await _performanceService.GetApprovalRolesAsync();
            if (role_entities != null) { roles = role_entities; }
            ViewBag.RoleList = new SelectList(roles, "ApprovalRoleId", "ApprovalRoleName");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSAPV, XXACC")]
        public async Task<IActionResult> ApproveEvaluation(ApproveContractViewModel model)
        {
            if (ModelState.IsValid)
            {
                ReviewApproval reviewApproval = model.ConvertToReviewApproval();
                reviewApproval.IsApproved = true;
                reviewApproval.ApprovedTime = DateTime.UtcNow;
                reviewApproval.ApprovalTypeId = (int)ReviewApprovalType.ApproveEvaluationResult;
                reviewApproval.SubmissionPurposeId = (int)ReviewSubmissionPurpose.ResultApproval;
                try
                {
                    ApprovalRole approvalRole = new ApprovalRole();
                    approvalRole = await _performanceService.GetApprovalRoleAsync(reviewApproval.ApproverRoleId);
                    if (approvalRole != null) { model.ApproverRoleDescription = approvalRole.ApprovalRoleName; }

                    bool isApproved = await _performanceService.ApproveContractToAppraisee(reviewApproval, null);
                    if (isApproved)
                    {
                        model.ViewModelSuccessMessage = "Evaluation approved successfully!";
                        model.OperationIsSuccessful = true;

                        PmsActivityHistory history = new PmsActivityHistory();
                        history.ActivityDescription = $"Evaluation result was approved by {model.ApproverName} as {model.ApproverRoleDescription} on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()} GMT";
                        history.ActivityTime = DateTime.UtcNow;
                        history.ReviewHeaderId = model.ReviewHeaderID;
                        await _performanceService.AddPmsActivityHistoryAsync(history);
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "An error was encountered. The attempted operation failed.";
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

        #region Appraisee Accept Contract and Views

        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> AcceptContract(int id)
        {
            AcceptContractViewModel model = new AcceptContractViewModel();
            ReviewHeader reviewHeader = new ReviewHeader();
            try
            {
                if (id > 0)
                {
                    model.ReviewHeaderID = id;
                    reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                    if (reviewHeader != null)
                    {
                        model.ReviewSessionID = reviewHeader.ReviewSessionId;
                        model.ReviewSessionName = reviewHeader.ReviewSessionName;
                        model.AppraiseeID = reviewHeader.AppraiseeId;
                        model.AppraiseeName = reviewHeader.AppraiseeName;
                        model.ReviewYearName = reviewHeader.ReviewYearName;
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
        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> AcceptContract(AcceptContractViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool isAccepted = await _performanceService.AcceptContractByAppraisee(model.ReviewHeaderID);
                    if (isAccepted)
                    {
                        model.ViewModelSuccessMessage = "Performance Contract Agreement Signed Off successfully!";
                        model.OperationIsSuccessful = true;

                        PmsActivityHistory history = new PmsActivityHistory();
                        history.ActivityDescription = $"Performance Contract was agreed to and signed off by {model.AppraiseeName} on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()} GMT";
                        history.ActivityTime = DateTime.UtcNow;
                        history.ReviewHeaderId = model.ReviewHeaderID;
                        await _performanceService.AddPmsActivityHistoryAsync(history);
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "An error was encountered. The attempted operation failed.";
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }

        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> AcceptEvaluation(int id)
        {
            AcceptContractViewModel model = new AcceptContractViewModel();
            ReviewHeader reviewHeader = new ReviewHeader();
            try
            {
                if (id > 0)
                {
                    model.ReviewHeaderID = id;
                    reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                    if (reviewHeader != null)
                    {
                        model.ReviewSessionID = reviewHeader.ReviewSessionId;
                        model.ReviewSessionName = reviewHeader.ReviewSessionName;
                        model.AppraiseeID = reviewHeader.AppraiseeId;
                        model.AppraiseeName = reviewHeader.AppraiseeName;
                        model.ReviewYearName = reviewHeader.ReviewYearName;
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
        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> AcceptEvaluation(AcceptContractViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool resultUploaded = await _performanceService.UploadResults(model.ReviewHeaderID);
                    if (!resultUploaded)
                    {
                        model.ViewModelErrorMessage = "An error was encountered while trying to process your evaluation result. The attempted operation could not be completed.";
                    }
                    else
                    {
                        if (model.IsNotAccepted)
                        {
                            await _performanceService.UpdateAppraiseeFlagAsync(model.ReviewHeaderID, true, model.AppraiseeName);
                        }

                        bool isSignedOff = await _performanceService.AcceptEvaluationByAppraisee(model.ReviewHeaderID);
                        if (isSignedOff)
                        {
                            model.ViewModelSuccessMessage = "Performance Evaluation Result Signed Off successfully!";
                            model.OperationIsSuccessful = true;

                            PmsActivityHistory history = new PmsActivityHistory();
                            history.ActivityDescription = $"Performance Evaluation Result was signed off by {model.AppraiseeName} on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()} GMT";
                            history.ActivityTime = DateTime.UtcNow;
                            history.ReviewHeaderId = model.ReviewHeaderID;
                            await _performanceService.AddPmsActivityHistoryAsync(history);
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "An error was encountered. The attempted operation failed.";
                        }
                    }


                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }

        public async Task<IActionResult> ShowAppraisalApprovals(int id)
        {
            AppraisalApprovalViewModel model = new AppraisalApprovalViewModel();
            if (id > 0)
            {
                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.ReviewStageID = reviewHeader.ReviewStageId;
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                }

                var entities = await _performanceService.GetReviewApprovalsAsync(id);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewApprovalList = entities.ToList();
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be retrieved.";
            }
            return View(model);
        }

        public async Task<IActionResult> ShowAppraiseeAgreement(int id)
        {
            AcceptContractViewModel model = new AcceptContractViewModel();
            try
            {
                if (id > 0)
                {
                    ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                    if (reviewHeader != null)
                    {
                        model.ReviewSessionID = reviewHeader.ReviewSessionId;
                        model.ReviewSessionName = reviewHeader.ReviewSessionName;
                        model.AppraiseeID = reviewHeader.AppraiseeId;
                        model.AppraiseeName = reviewHeader.AppraiseeName;
                        model.ReviewYearName = reviewHeader.ReviewYearName;
                        model.SignedOffTimeFormatted = $"{reviewHeader.TimeContractAccepted.Value.ToLongDateString()} {reviewHeader.TimeContractAccepted.Value.ToLongTimeString()} GMT";
                    }
                }
                else
                {
                    model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be retrieved.";
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        public async Task<IActionResult> ShowContractInfo(int id)
        {
            AppraisalInfoViewModel model = new AppraisalInfoViewModel();
            ReviewHeader reviewHeader = new ReviewHeader();
            List<ReviewMetric> reviewKpas = new List<ReviewMetric>();
            List<ReviewMetric> reviewCmps = new List<ReviewMetric>();
            List<ReviewCDG> reviewCDGs = new List<ReviewCDG>();

            if (id > 0)
            {
                reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null) { model.AppraisalReviewHeader = reviewHeader; }
                reviewKpas = await _performanceService.GetKpasAsync(id);
                if (reviewKpas != null && reviewKpas.Count > 0) { model.KpaList = reviewKpas; }
                reviewCmps = await _performanceService.GetCompetenciesAsync(id);
                if (reviewCmps != null && reviewCmps.Count > 0) { model.CompetencyList = reviewCmps; }
                reviewCDGs = await _performanceService.GetReviewCdgsAsync(id);
                if (reviewCDGs != null && reviewCDGs.Count > 0) { model.CdgList = reviewCDGs; }
            }

            return View(model);
        }

        public async Task<IActionResult> ShowEvaluationSignOff(int id)
        {
            AcceptContractViewModel model = new AcceptContractViewModel();
            try
            {
                if (id > 0)
                {
                    ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                    if (reviewHeader != null)
                    {
                        model.IsNotAccepted = reviewHeader.IsFlagged;
                        model.ReviewSessionID = reviewHeader.ReviewSessionId;
                        model.ReviewSessionName = reviewHeader.ReviewSessionName;
                        model.AppraiseeID = reviewHeader.AppraiseeId;
                        model.AppraiseeName = reviewHeader.AppraiseeName;
                        model.ReviewYearName = reviewHeader.ReviewYearName;
                        model.SignedOffTimeFormatted = $"{reviewHeader.TimeEvaluationAccepted.Value.ToLongDateString()} {reviewHeader.TimeEvaluationAccepted.Value.ToLongTimeString()} GMT";
                    }
                }
                else
                {
                    model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be retrieved.";
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        #endregion

        #region Review Message Controller Actions
        public async Task<IActionResult> AppraisalNotes(int id, string sp)
        {
            AppraisalNotesViewModel model = new AppraisalNotesViewModel();
            model.SourcePage = sp;
            if (id > 0)
            {
                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.AppraiseeName = reviewHeader.AppraiseeName;
                    model.AppraiseeID = reviewHeader.AppraiseeId;

                    ApplicationUser user = new ApplicationUser();
                    int userId = 0;
                    var userStringId = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
                    if (!string.IsNullOrWhiteSpace(userStringId)) { userId = Convert.ToInt32(userStringId); }
                    user = await _securityService.GetUserByIdAsync(userId);
                    if (!string.IsNullOrWhiteSpace(user.FullName))
                    {
                        model.LoggedInEmployeeID = user.EmployeeId;
                    }
                }

                var entities = await _performanceService.GetReviewMessagesAsync(id);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewMessageList = entities.ToList();
                }
            }
            return View(model);
        }

        [Authorize(Roles = "PMSAPR, PMSAPV, XXACC")]
        public IActionResult AddAppraisalNote(int id, int sd, string sp)
        {
            AddAppraisalNoteViewModel model = new AddAppraisalNoteViewModel();
            ReviewMessage reviewMessage = new ReviewMessage();
            try
            {
                if (id > 0) { model.ReviewHeaderID = id; }
                if (sd > 0) { model.FromEmployeeID = sd; }
                if (!string.IsNullOrWhiteSpace(sp)) { model.SourcePage = sp; }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSAPR, PMSAPV, XXACC")]
        public async Task<IActionResult> AddAppraisalNote(AddAppraisalNoteViewModel model)
        {
            if (ModelState.IsValid)
            {
                ReviewMessage reviewMessage = new ReviewMessage();
                reviewMessage = model.ConvertToReviewMessage();
                reviewMessage.MessageTime = DateTime.UtcNow;
                try
                {
                    bool isAdded = await _performanceService.AddReviewMessageAsync(reviewMessage);
                    if (isAdded)
                    {
                        model.ViewModelSuccessMessage = "Note saved successfully!";
                        model.OperationIsSuccessful = true;
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "An error was encountered. The attempted operation failed.";
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

        #region Self Evaluation Controller Actions

        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> KpaEvaluationList(int id, int ad)
        {
            EvaluationListViewModel model = new EvaluationListViewModel();
            if (id > 0 && ad > 0)
            {
                model.AppraiserID = ad;
                model.ReviewHeaderID = id;
                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                }

                var entities = await _performanceService.GetInitialReviewResultKpasAsync(id, ad);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewResultList = entities.ToList();
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be r";
            }

            return View(model);
        }

        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> CmpEvaluationList(int id, int ad)
        {
            EvaluationListViewModel model = new EvaluationListViewModel();
            if (id > 0 && ad > 0)
            {
                model.ReviewHeaderID = id;
                model.AppraiserID = ad;

                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                }

                var entities = await _performanceService.GetInitialReviewResultCmpsAsync(id, ad);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewResultList = entities.ToList();
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be r";
            }

            return View(model);
        }

        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> SelfEvaluateKpa(int id, int ad, int md)
        {
            SelfEvaluateKpaViewModel model = new SelfEvaluateKpaViewModel();
            if (id > 0 && ad > 0 && md > 0)
            {
                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader == null)
                {
                    model.ViewModelErrorMessage = "Sorry, no record was found for the selected KPA.";
                }
                else
                {
                    model.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                    model.ReviewYearID = reviewHeader.ReviewYearId;
                    model.AppraiserTypeID = (int)AppraiserType.SelfAppraiser;
                    model.AppraiserID = ad;
                    model.ReviewMetricID = md;

                    var entity_list = await _performanceService.GetInitialReviewResultAsync(id, ad, md);
                    if (entity_list == null || entity_list.Count < 1)
                    {
                        model.ViewModelErrorMessage = "Sorry, no record was found for the selected KPA.";
                    }
                    else
                    {
                        ReviewResult reviewResult = new ReviewResult();
                        reviewResult = entity_list.FirstOrDefault();
                        model.ReviewMetricDescription = reviewResult.ReviewMetricDescription;
                        model.ReviewMetricTypeID = reviewResult.ReviewMetricTypeId;
                        model.ReviewMetricID = reviewResult.ReviewMetricId;
                        model.ReviewMetricMeasurement = reviewResult.ReviewMetricMeasurement;
                        model.ReviewMetricTarget = reviewResult.ReviewMetricTarget;
                        model.ActualAchievement = reviewResult.ActualAchievement;
                        model.AppraiserComment = reviewResult.AppraiserComment;
                        model.AppraiserScore = reviewResult.AppraiserScore;
                        model.ReviewResultID = reviewResult.ReviewResultId;
                        model.ReviewMetricWeightage = reviewResult.ReviewMetricWeightage;
                    }
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be r";
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> SelfEvaluateKpa(SelfEvaluateKpaViewModel model)
        {
            if (ModelState.IsValid)
            {
                ReviewResult reviewResult = new ReviewResult();
                try
                {
                    reviewResult.ActualAchievement = model.ActualAchievement;
                    reviewResult.AppraiseeId = model.AppraiseeID;
                    reviewResult.AppraiserComment = model.AppraiserComment;
                    reviewResult.AppraiserId = model.AppraiserID;
                    reviewResult.AppraiserRoleId = null;
                    reviewResult.AppraiserScore = model.AppraiserScore;
                    reviewResult.AppraiserTypeId = (int)AppraiserType.SelfAppraiser;
                    reviewResult.ReviewHeaderId = model.ReviewHeaderID;
                    reviewResult.ReviewMetricId = model.ReviewMetricID;
                    reviewResult.ReviewMetricTypeId = model.ReviewMetricTypeID;
                    reviewResult.ReviewSessionId = model.ReviewSessionID;
                    reviewResult.ReviewYearId = model.ReviewYearID;
                    reviewResult.ScoreTime = DateTime.UtcNow;
                    reviewResult.ReviewResultId = model.ReviewResultID;
                    reviewResult.AppraiserScoreDescription = $"{(Convert.ToInt32(model.AppraiserScore)).ToString("D")}%";
                    reviewResult.AppraiserScoreValue = (model.AppraiserScore * model.ReviewMetricWeightage) / 100;

                    if (reviewResult.ReviewResultId > 0)
                    {
                        bool IsSuccessful = await _performanceService.UpdateReviewResultAsync(reviewResult);
                        if (IsSuccessful) { return RedirectToAction("KpaEvaluationList", new { id = model.ReviewHeaderID, ad = model.AppraiserID }); }
                        else { model.ViewModelErrorMessage = "An error was encountered. Attempted operation could not be completed."; }
                    }

                    bool IsAdded = await _performanceService.AddReviewResultAsync(reviewResult);
                    if (IsAdded) { return RedirectToAction("KpaEvaluationList", new { id = model.ReviewHeaderID, ad = model.AppraiserID }); }
                    else { model.ViewModelErrorMessage = "An error was encountered. Attempted operation could not be completed."; }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }

        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> SelfEvaluateCmp(int id, int ad, int md)
        {
            SelfEvaluateKpaViewModel model = new SelfEvaluateKpaViewModel();
            if (id > 0 && ad > 0 && md > 0)
            {
                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader == null)
                {
                    model.ViewModelErrorMessage = "Sorry, no record was found for the selected KPA.";
                }
                else
                {
                    model.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                    model.ReviewYearID = reviewHeader.ReviewYearId;
                    model.AppraiserTypeID = (int)AppraiserType.SelfAppraiser;
                    model.AppraiserID = ad;
                    model.ReviewMetricID = md;

                    var entity_list = await _performanceService.GetInitialReviewResultAsync(id, ad, md);
                    if (entity_list == null || entity_list.Count < 1)
                    {
                        model.ViewModelErrorMessage = "Sorry, no record was found for the selected KPA.";
                    }
                    else
                    {
                        ReviewResult reviewResult = new ReviewResult();
                        reviewResult = entity_list.FirstOrDefault();
                        model.ReviewMetricDescription = reviewResult.ReviewMetricDescription;
                        model.ReviewMetricTypeID = reviewResult.ReviewMetricTypeId;
                        model.ReviewMetricID = reviewResult.ReviewMetricId;
                        model.ReviewMetricMeasurement = reviewResult.ReviewMetricMeasurement;
                        model.ReviewMetricTarget = reviewResult.ReviewMetricTarget;
                        model.ActualAchievement = reviewResult.ActualAchievement;
                        model.AppraiserComment = reviewResult.AppraiserComment;
                        model.AppraiserScore = reviewResult.AppraiserScore;
                        model.ReviewResultID = reviewResult.ReviewResultId;
                        model.AppraiserScore = reviewResult.AppraiserScore;
                        model.AppraiserScoreDescription = reviewResult.AppraiserScoreDescription;
                    }
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be r";
            }

            var grade_entities = await _performanceService.GetAppraisalCompetencyGradesAsync(model.ReviewSessionID);
            if (grade_entities != null)
            {
                ViewBag.GradeList = new SelectList(grade_entities, "AppraisalGradeId", "AppraisalGradeDescription");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> SelfEvaluateCmp(SelfEvaluateKpaViewModel model)
        {
            if (ModelState.IsValid)
            {
                ReviewResult reviewResult = new ReviewResult();
                AppraisalGrade appraisalGrade = new AppraisalGrade();

                try
                {
                    if (model.ScoreGradeID > 0)
                    {
                        appraisalGrade = await _performanceService.GetAppraisalGradeAsync(model.ScoreGradeID);
                        if (appraisalGrade != null)
                        {
                            reviewResult.AppraiserScore = appraisalGrade.UpperBandScore;
                            reviewResult.AppraiserScoreDescription = $"{appraisalGrade.AppraisalGradeDescription} ({appraisalGrade.UpperBandScore})";
                            reviewResult.AppraiserScoreValue = appraisalGrade.UpperBandScore;
                        }
                    }
                    reviewResult.ActualAchievement = model.ActualAchievement;
                    reviewResult.AppraiseeId = model.AppraiseeID;
                    reviewResult.AppraiserComment = model.AppraiserComment;
                    reviewResult.AppraiserId = model.AppraiserID;
                    reviewResult.AppraiserRoleId = null;
                    reviewResult.AppraiserScore = model.AppraiserScore;
                    reviewResult.AppraiserTypeId = (int)AppraiserType.SelfAppraiser;
                    reviewResult.ReviewHeaderId = model.ReviewHeaderID;
                    reviewResult.ReviewMetricId = model.ReviewMetricID;
                    reviewResult.ReviewMetricTypeId = model.ReviewMetricTypeID;
                    reviewResult.ReviewSessionId = model.ReviewSessionID;
                    reviewResult.ReviewYearId = model.ReviewYearID;
                    reviewResult.ScoreTime = DateTime.UtcNow;
                    reviewResult.ReviewResultId = model.ReviewResultID;

                    if (reviewResult.ReviewResultId > 0)
                    {
                        bool IsSuccessful = await _performanceService.UpdateReviewResultAsync(reviewResult);
                        if (IsSuccessful) { return RedirectToAction("CmpEvaluationList", new { id = model.ReviewHeaderID, ad = model.AppraiserID }); }
                        else { model.ViewModelErrorMessage = "An error was encountered. Attempted operation could not be completed."; }
                    }

                    bool IsAdded = await _performanceService.AddReviewResultAsync(reviewResult);
                    if (IsAdded) { return RedirectToAction("CmpEvaluationList", new { id = model.ReviewHeaderID, ad = model.AppraiserID }); }
                    else { model.ViewModelErrorMessage = "An error was encountered. Attempted operation could not be completed."; }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }

        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> EvaluationResult(int id, int ad)
        {
            EvaluationResultViewModel model = new EvaluationResultViewModel();
            model.AppraiserID = ad;
            model.ReviewHeaderID = id;
            if (id > 0 && ad > 0)
            {
                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.ReviewSessionName = reviewHeader.ReviewSessionName;
                    model.ReviewYearID = reviewHeader.ReviewYearId;
                    model.ReviewYearName = reviewHeader.ReviewYearName;
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                    model.AppraiseeName = reviewHeader.AppraiseeName;

                    ReviewSession reviewSession = await _performanceService.GetReviewSessionAsync(model.ReviewSessionID);
                    if (reviewSession != null)
                    {
                        model.QualitativeScoreObtainable = reviewSession.TotalCompetencyScore;
                        model.QuantitativeScoreObtainable = reviewSession.TotalKpaScore;
                        model.TotalScoreObtainable = reviewSession.TotalCombinedScore;
                    }
                }

                var result_entities = await _performanceService.GetReviewResultByAppraiserIdAndReviewMetricTypeIdAsync(model.ReviewHeaderID, model.AppraiserID, null);
                if (result_entities != null && result_entities.Count > 0)
                {
                    ReviewResult reviewResult = result_entities.FirstOrDefault();
                    model.AppraiserName = reviewResult.AppraiserName;
                    model.AppraiserRoleID = reviewResult.AppraiserRoleId ?? 0;
                    model.AppraiserRoleName = reviewResult.AppraiserRoleName;
                    model.AppraiserTypeDescription = reviewResult.AppraiserTypeDescription;
                    model.AppraisalTime = $"{reviewResult.ScoreTime.Value.ToLongDateString()} {reviewResult.ScoreTime.Value.ToLongTimeString()} GMT";
                }

                ScoreSummary scoreSummary = await _performanceService.GetScoreSummaryAsync(id, ad);
                if (scoreSummary != null)
                {
                    model.QualitativeScoreObtained = scoreSummary.QualitativeScore;
                    model.QuantitativeScoreObtained = scoreSummary.QuantitativeScore;
                    model.TotalScoreObtained = scoreSummary.TotalPerformanceScore;
                }

                AppraisalGrade appraisalGrade = await _performanceService.GetAppraisalGradeAsync(model.ReviewSessionID, ReviewGradeType.Performance, model.TotalScoreObtained);
                if (appraisalGrade != null)
                {
                    model.PerformanceRating = appraisalGrade.AppraisalGradeDescription;
                    model.PerformanceRank = appraisalGrade.GradeRankDescription;
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be r";
            }
            return View(model);
        }

        #endregion

        #region Final Evaluation Controller Actions

        [Authorize(Roles = "PMSAPV, XXACC")]
        public async Task<IActionResult> FinalEvaluationKpaList(int id, int ad)
        {
            EvaluationListViewModel model = new EvaluationListViewModel();
            if (id > 0 && ad > 0)
            {
                model.AppraiserID = ad;
                model.ReviewHeaderID = id;
                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                }

                var entities = await _performanceService.GetInitialReviewResultKpasAsync(id, ad);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewResultList = entities.ToList();
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be r";
            }

            return View(model);
        }

        [Authorize(Roles = "PMSAPV, XXACC")]
        public async Task<IActionResult> FinalEvaluationCmpList(int id, int ad)
        {
            EvaluationListViewModel model = new EvaluationListViewModel();
            if (id > 0 && ad > 0)
            {
                model.ReviewHeaderID = id;
                model.AppraiserID = ad;

                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                }

                var entities = await _performanceService.GetInitialReviewResultCmpsAsync(id, ad);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewResultList = entities.ToList();
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be r";
            }

            return View(model);
        }

        [Authorize(Roles = "PMSAPV, XXACC")]
        public async Task<IActionResult> FinalEvaluateKpa(int id, int ad, int md)
        {
            SelfEvaluateKpaViewModel model = new SelfEvaluateKpaViewModel();
            if (id > 0 && ad > 0 && md > 0)
            {
                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader == null)
                {
                    model.ViewModelErrorMessage = "Sorry, no record was found for the selected KPA.";
                }
                else
                {
                    if (reviewHeader.PrimaryAppraiserId != null && reviewHeader.PrimaryAppraiserId > 0)
                    {
                        if (reviewHeader.PrimaryAppraiserId.Value == ad)
                        { model.AppraiserTypeID = (int)AppraiserType.PrincipalAppraiser; }
                        else { model.AppraiserTypeID = (int)AppraiserType.ThirdPartyAppraiser; }
                    }

                    model.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                    model.ReviewYearID = reviewHeader.ReviewYearId;
                    model.AppraiserID = ad;
                    model.ReviewMetricID = md;

                    var entity_list = await _performanceService.GetInitialReviewResultAsync(id, ad, md);
                    if (entity_list == null || entity_list.Count < 1)
                    {
                        model.ViewModelErrorMessage = "Sorry, no record was found for the selected KPA.";
                    }
                    else
                    {
                        ReviewResult reviewResult = new ReviewResult();
                        reviewResult = entity_list.FirstOrDefault();
                        model.ReviewMetricDescription = reviewResult.ReviewMetricDescription;
                        model.ReviewMetricTypeID = reviewResult.ReviewMetricTypeId;
                        model.ReviewMetricID = reviewResult.ReviewMetricId;
                        model.ReviewMetricMeasurement = reviewResult.ReviewMetricMeasurement;
                        model.ReviewMetricTarget = reviewResult.ReviewMetricTarget;
                        model.ActualAchievement = reviewResult.ActualAchievement;
                        model.AppraiserComment = reviewResult.AppraiserComment;
                        model.AppraiserScore = reviewResult.AppraiserScore;
                        model.ReviewResultID = reviewResult.ReviewResultId;
                        model.ReviewMetricWeightage = reviewResult.ReviewMetricWeightage;

                        model.AppraiseeScore = reviewResult.AppraiseeScore;
                        model.AppraiseeAchievement = reviewResult.AppraiseeAchievement;
                    }
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be r";
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSAPV, XXACC")]
        public async Task<IActionResult> FinalEvaluateKpa(SelfEvaluateKpaViewModel model)
        {
            if (ModelState.IsValid)
            {

                List<ReviewSubmission> reviewSubmissions = new List<ReviewSubmission>();
                ReviewSubmission reviewSubmission = new ReviewSubmission();
                ReviewResult reviewResult = new ReviewResult();
                try
                {
                    reviewSubmissions = await _performanceService.GetReviewSubmissionsByReviewHeaderIdAsync(model.ReviewHeaderID, (int)ReviewSubmissionPurpose.FinalEvaluation, model.AppraiserID);
                    if (reviewSubmissions != null && reviewSubmissions.Count > 0)
                    {
                        reviewSubmission = reviewSubmissions.FirstOrDefault();
                        reviewResult.AppraiserRoleId = reviewSubmission.ToEmployeeRoleId;
                        reviewResult.AppraiserRoleName = reviewSubmission.ToEmployeeRoleName;
                    }

                    reviewResult.ActualAchievement = model.ActualAchievement;
                    reviewResult.AppraiseeId = model.AppraiseeID;
                    reviewResult.AppraiserComment = model.AppraiserComment;
                    reviewResult.AppraiserId = model.AppraiserID;
                    //reviewResult.AppraiserRoleId = null;
                    reviewResult.AppraiserScore = model.AppraiserScore;
                    reviewResult.AppraiserTypeId = model.AppraiserTypeID;
                    reviewResult.ReviewHeaderId = model.ReviewHeaderID;
                    reviewResult.ReviewMetricId = model.ReviewMetricID;
                    reviewResult.ReviewMetricTypeId = model.ReviewMetricTypeID;
                    reviewResult.ReviewSessionId = model.ReviewSessionID;
                    reviewResult.ReviewYearId = model.ReviewYearID;
                    reviewResult.ScoreTime = DateTime.UtcNow;
                    reviewResult.ReviewResultId = model.ReviewResultID;
                    reviewResult.AppraiserScoreDescription = $"{(Convert.ToInt32(model.AppraiserScore)).ToString("D")}%";
                    reviewResult.AppraiserScoreValue = (model.AppraiserScore * model.ReviewMetricWeightage) / 100;

                    switch (reviewResult.AppraiserTypeId)
                    {
                        case 0:
                            reviewResult.AppraiserTypeDescription = "Self Appraiser";
                            break;
                        case 1:
                            reviewResult.AppraiserTypeDescription = "Principal Appraiser";
                            break;
                        case 2:
                            reviewResult.AppraiserTypeDescription = "Third Party Appraiser";
                            break;
                        default:
                            break;
                    }


                    if (reviewResult.ReviewResultId > 0)
                    {
                        bool IsSuccessful = await _performanceService.UpdateReviewResultAsync(reviewResult);
                        if (IsSuccessful) { return RedirectToAction("FinalEvaluationKpaList", new { id = model.ReviewHeaderID, ad = model.AppraiserID }); }
                        else { model.ViewModelErrorMessage = "An error was encountered. Attempted operation could not be completed."; }
                    }

                    bool IsAdded = await _performanceService.AddReviewResultAsync(reviewResult);
                    if (IsAdded) { return RedirectToAction("FinalEvaluationKpaList", new { id = model.ReviewHeaderID, ad = model.AppraiserID }); }
                    else { model.ViewModelErrorMessage = "An error was encountered. Attempted operation could not be completed."; }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }

        [Authorize(Roles = "PMSAPV, XXACC")]
        public async Task<IActionResult> FinalEvaluateCmp(int id, int ad, int md)
        {
            SelfEvaluateKpaViewModel model = new SelfEvaluateKpaViewModel();
            if (id > 0 && ad > 0 && md > 0)
            {
                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader == null)
                {
                    model.ViewModelErrorMessage = "Sorry, no record was found for the selected KPA.";
                }
                else
                {
                    if (reviewHeader.PrimaryAppraiserId != null && reviewHeader.PrimaryAppraiserId > 0)
                    {
                        if (reviewHeader.PrimaryAppraiserId.Value == ad)
                        { model.AppraiserTypeID = (int)AppraiserType.PrincipalAppraiser; }
                        else { model.AppraiserTypeID = (int)AppraiserType.ThirdPartyAppraiser; }
                    }

                    model.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                    model.ReviewYearID = reviewHeader.ReviewYearId;
                    model.AppraiserTypeID = (int)AppraiserType.SelfAppraiser;
                    model.AppraiserID = ad;
                    model.ReviewMetricID = md;

                    var entity_list = await _performanceService.GetInitialReviewResultAsync(id, ad, md);
                    if (entity_list == null || entity_list.Count < 1)
                    {
                        model.ViewModelErrorMessage = "Sorry, no record was found for the selected KPA.";
                    }
                    else
                    {
                        ReviewResult reviewResult = new ReviewResult();
                        reviewResult = entity_list.FirstOrDefault();
                        model.ReviewMetricDescription = reviewResult.ReviewMetricDescription;
                        model.ReviewMetricTypeID = reviewResult.ReviewMetricTypeId;
                        model.ReviewMetricID = reviewResult.ReviewMetricId;
                        model.ReviewMetricMeasurement = reviewResult.ReviewMetricMeasurement;
                        model.ReviewMetricTarget = reviewResult.ReviewMetricTarget;
                        model.ActualAchievement = reviewResult.ActualAchievement;
                        model.AppraiserComment = reviewResult.AppraiserComment;
                        model.AppraiserScore = reviewResult.AppraiserScore;
                        model.ReviewResultID = reviewResult.ReviewResultId;
                        model.AppraiserScore = reviewResult.AppraiserScore;
                        model.AppraiserScoreDescription = reviewResult.AppraiserScoreDescription;
                        model.AppraiseeAchievement = reviewResult.AppraiseeAchievement;
                        model.AppraiseeScore = reviewResult.AppraiseeScore;
                    }
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be r";
            }

            var grade_entities = await _performanceService.GetAppraisalCompetencyGradesAsync(model.ReviewSessionID);
            if (grade_entities != null)
            {
                ViewBag.GradeList = new SelectList(grade_entities, "AppraisalGradeId", "AppraisalGradeDescription");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSAPV, XXACC")]
        public async Task<IActionResult> FinalEvaluateCmp(SelfEvaluateKpaViewModel model)
        {
            if (ModelState.IsValid)
            {
                ReviewResult reviewResult = new ReviewResult();
                List<ReviewSubmission> reviewSubmissions = new List<ReviewSubmission>();
                ReviewSubmission reviewSubmission = new ReviewSubmission();
                AppraisalGrade appraisalGrade = new AppraisalGrade();

                try
                {
                    if (model.ScoreGradeID > 0)
                    {
                        appraisalGrade = await _performanceService.GetAppraisalGradeAsync(model.ScoreGradeID);
                        if (appraisalGrade != null)
                        {
                            reviewResult.AppraiserScore = appraisalGrade.UpperBandScore;
                            reviewResult.AppraiserScoreDescription = $"{appraisalGrade.AppraisalGradeDescription} ({appraisalGrade.UpperBandScore})";
                            reviewResult.AppraiserScoreValue = appraisalGrade.UpperBandScore;
                        }
                    }

                    reviewSubmissions = await _performanceService.GetReviewSubmissionsByReviewHeaderIdAsync(model.ReviewHeaderID, (int)ReviewSubmissionPurpose.FinalEvaluation, model.AppraiserID);
                    if (reviewSubmissions != null && reviewSubmissions.Count > 0)
                    {
                        reviewSubmission = reviewSubmissions.FirstOrDefault();
                        reviewResult.AppraiserRoleId = reviewSubmission.ToEmployeeRoleId;
                        reviewResult.AppraiserRoleName = reviewSubmission.ToEmployeeRoleName;
                    }
                    reviewResult.ActualAchievement = model.ActualAchievement;
                    reviewResult.AppraiseeId = model.AppraiseeID;
                    reviewResult.AppraiserComment = model.AppraiserComment;
                    reviewResult.AppraiserId = model.AppraiserID;
                    reviewResult.AppraiserScore = model.AppraiserScore;
                    //reviewResult.AppraiserTypeId = (int)AppraiserType.SelfAppraiser;
                    reviewResult.ReviewHeaderId = model.ReviewHeaderID;
                    reviewResult.ReviewMetricId = model.ReviewMetricID;
                    reviewResult.ReviewMetricTypeId = model.ReviewMetricTypeID;
                    reviewResult.ReviewSessionId = model.ReviewSessionID;
                    reviewResult.ReviewYearId = model.ReviewYearID;
                    reviewResult.ScoreTime = DateTime.UtcNow;
                    reviewResult.ReviewResultId = model.ReviewResultID;

                    switch (reviewResult.AppraiserTypeId)
                    {
                        case 0:
                            reviewResult.AppraiserTypeDescription = "Self Appraiser";
                            break;
                        case 1:
                            reviewResult.AppraiserTypeDescription = "Principal Appraiser";
                            break;
                        case 2:
                            reviewResult.AppraiserTypeDescription = "Third Party Appraiser";
                            break;
                        default:
                            break;
                    }



                    if (reviewResult.ReviewResultId > 0)
                    {
                        bool IsSuccessful = await _performanceService.UpdateReviewResultAsync(reviewResult);
                        if (IsSuccessful) { return RedirectToAction("FinalEvaluationCmpList", new { id = model.ReviewHeaderID, ad = model.AppraiserID }); }
                        else { model.ViewModelErrorMessage = "An error was encountered. Attempted operation could not be completed."; }
                    }

                    bool IsAdded = await _performanceService.AddReviewResultAsync(reviewResult);
                    if (IsAdded) { return RedirectToAction("FinalEvaluationCmpList", new { id = model.ReviewHeaderID, ad = model.AppraiserID }); }
                    else { model.ViewModelErrorMessage = "An error was encountered. Attempted operation could not be completed."; }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }

        [Authorize(Roles = "PMSAPV, XXACC")]
        public async Task<IActionResult> FinalEvaluationResult(int id, int ad)
        {
            EvaluationResultViewModel model = new EvaluationResultViewModel();
            model.AppraiserID = ad;
            model.ReviewHeaderID = id;
            if (id > 0 && ad > 0)
            {
                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.ReviewSessionName = reviewHeader.ReviewSessionName;
                    model.ReviewYearID = reviewHeader.ReviewYearId;
                    model.ReviewYearName = reviewHeader.ReviewYearName;
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                    model.AppraiseeName = reviewHeader.AppraiseeName;

                    ReviewSession reviewSession = await _performanceService.GetReviewSessionAsync(model.ReviewSessionID);
                    if (reviewSession != null)
                    {
                        model.QualitativeScoreObtainable = reviewSession.TotalCompetencyScore;
                        model.QuantitativeScoreObtainable = reviewSession.TotalKpaScore;
                        model.TotalScoreObtainable = reviewSession.TotalCombinedScore;
                    }
                }

                var result_entities = await _performanceService.GetReviewResultByAppraiserIdAndReviewMetricTypeIdAsync(model.ReviewHeaderID, model.AppraiserID, null);
                if (result_entities != null && result_entities.Count > 0)
                {
                    ReviewResult reviewResult = result_entities.FirstOrDefault();
                    model.AppraiserName = reviewResult.AppraiserName;
                    model.AppraiserRoleID = reviewResult.AppraiserRoleId ?? 0;
                    model.AppraiserRoleName = reviewResult.AppraiserRoleName;
                    model.AppraiserTypeDescription = reviewResult.AppraiserTypeDescription;
                    model.AppraisalTime = $"{reviewResult.ScoreTime.Value.ToLongDateString()} {reviewResult.ScoreTime.Value.ToLongTimeString()} GMT";
                }

                ScoreSummary scoreSummary = await _performanceService.GetScoreSummaryAsync(id, ad);
                if (scoreSummary != null)
                {
                    model.QualitativeScoreObtained = scoreSummary.QualitativeScore;
                    model.QuantitativeScoreObtained = scoreSummary.QuantitativeScore;
                    model.TotalScoreObtained = scoreSummary.TotalPerformanceScore;
                }

                AppraisalGrade appraisalGrade = await _performanceService.GetAppraisalGradeAsync(model.ReviewSessionID, ReviewGradeType.Performance, model.TotalScoreObtained);
                if (appraisalGrade != null)
                {
                    model.PerformanceRating = appraisalGrade.AppraisalGradeDescription;
                    model.PerformanceRank = appraisalGrade.GradeRankDescription;
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be r";
            }
            return View(model);
        }

        #endregion

        #region Evaluation Results

        [Authorize(Roles = "PMSAPV, PMSAPR, XXACC")]
        public async Task<IActionResult> ShowEvaluations(int id)
        {
            ShowEvaluationsViewModel model = new ShowEvaluationsViewModel();
            ReviewSubmission reviewSubmission = new ReviewSubmission();
            try
            {
                if (id > 0)
                {
                    model.ReviewHeaderID = id;
                    var entities = await _performanceService.GetReviewSubmissionsByReviewHeaderIdAsync(id, (int)ReviewSubmissionPurpose.FinalEvaluation);
                    if (entities != null && entities.Count > 0)
                    {
                        model.Submissions = entities;
                        reviewSubmission = entities.FirstOrDefault();
                    }
                }

            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            return View(model);
        }

        [Authorize(Roles = "PMSAPV, PMSAPR, XXACC")]
        public async Task<IActionResult> ShowResultSummary(int id, int ad)
        {
            EvaluationResultViewModel model = new EvaluationResultViewModel();
            model.AppraiserID = ad;
            model.ReviewHeaderID = id;
            if (id > 0 && ad > 0)
            {
                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.ReviewSessionName = reviewHeader.ReviewSessionName;
                    model.ReviewYearID = reviewHeader.ReviewYearId;
                    model.ReviewYearName = reviewHeader.ReviewYearName;
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                    model.AppraiseeName = reviewHeader.AppraiseeName;

                    ReviewSession reviewSession = await _performanceService.GetReviewSessionAsync(model.ReviewSessionID);
                    if (reviewSession != null)
                    {
                        model.QualitativeScoreObtainable = reviewSession.TotalCompetencyScore;
                        model.QuantitativeScoreObtainable = reviewSession.TotalKpaScore;
                        model.TotalScoreObtainable = reviewSession.TotalCombinedScore;
                    }
                }

                var result_entities = await _performanceService.GetReviewResultByAppraiserIdAndReviewMetricTypeIdAsync(model.ReviewHeaderID, model.AppraiserID, null);
                if (result_entities != null && result_entities.Count > 0)
                {
                    ReviewResult reviewResult = result_entities.FirstOrDefault();
                    model.AppraiserName = reviewResult.AppraiserName;
                    model.AppraiserRoleID = reviewResult.AppraiserRoleId ?? 0;
                    model.AppraiserRoleName = reviewResult.AppraiserRoleName;
                    model.AppraiserTypeDescription = reviewResult.AppraiserTypeDescription;
                    model.AppraisalTime = $"{reviewResult.ScoreTime.Value.ToLongDateString()} {reviewResult.ScoreTime.Value.ToLongTimeString()} GMT";
                }

                ScoreSummary scoreSummary = await _performanceService.GetScoreSummaryAsync(id, ad);
                if (scoreSummary != null)
                {
                    model.QualitativeScoreObtained = scoreSummary.QualitativeScore;
                    model.QuantitativeScoreObtained = scoreSummary.QuantitativeScore;
                    model.TotalScoreObtained = scoreSummary.TotalPerformanceScore;
                }

                AppraisalGrade appraisalGrade = await _performanceService.GetAppraisalGradeAsync(model.ReviewSessionID, ReviewGradeType.Performance, model.TotalScoreObtained);
                if (appraisalGrade != null)
                {
                    model.PerformanceRating = appraisalGrade.AppraisalGradeDescription;
                    model.PerformanceRank = appraisalGrade.GradeRankDescription;
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be r";
            }
            return View(model);
        }

        [Authorize(Roles = "PMSAPV, PMSAPR, XXACC")]
        public async Task<IActionResult> ShowFullResult(int id, int ad)
        {
            ShowFullResultViewModel model = new ShowFullResultViewModel();
            model.EvaluationSummaryResult = new EvaluationResultViewModel();
            model.KpaFullResult = new EvaluationListViewModel();
            model.CmpFullResult = new EvaluationListViewModel();

            model.EvaluationSummaryResult.AppraiserID = ad;
            model.EvaluationSummaryResult.ReviewHeaderID = id;
            if (id > 0 && ad > 0)
            {
                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.EvaluationSummaryResult.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                    model.EvaluationSummaryResult.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.EvaluationSummaryResult.ReviewSessionName = reviewHeader.ReviewSessionName;
                    model.EvaluationSummaryResult.ReviewYearID = reviewHeader.ReviewYearId;
                    model.EvaluationSummaryResult.ReviewYearName = reviewHeader.ReviewYearName;
                    model.EvaluationSummaryResult.AppraiseeID = reviewHeader.AppraiseeId;
                    model.EvaluationSummaryResult.AppraiseeName = reviewHeader.AppraiseeName;

                    ReviewSession reviewSession = await _performanceService.GetReviewSessionAsync(model.EvaluationSummaryResult.ReviewSessionID);
                    if (reviewSession != null)
                    {
                        model.EvaluationSummaryResult.QualitativeScoreObtainable = reviewSession.TotalCompetencyScore;
                        model.EvaluationSummaryResult.QuantitativeScoreObtainable = reviewSession.TotalKpaScore;
                        model.EvaluationSummaryResult.TotalScoreObtainable = reviewSession.TotalCombinedScore;
                    }
                }

                var result_entities = await _performanceService.GetReviewResultByAppraiserIdAndReviewMetricTypeIdAsync(model.EvaluationSummaryResult.ReviewHeaderID, model.EvaluationSummaryResult.AppraiserID, null);
                if (result_entities != null && result_entities.Count > 0)
                {
                    ReviewResult reviewResult = result_entities.FirstOrDefault();
                    model.EvaluationSummaryResult.AppraiserName = reviewResult.AppraiserName;
                    model.EvaluationSummaryResult.AppraiserRoleID = reviewResult.AppraiserRoleId ?? 0;
                    model.EvaluationSummaryResult.AppraiserRoleName = reviewResult.AppraiserRoleName;
                    model.EvaluationSummaryResult.AppraiserTypeDescription = reviewResult.AppraiserTypeDescription;
                    model.EvaluationSummaryResult.AppraisalTime = $"{reviewResult.ScoreTime.Value.ToLongDateString()} {reviewResult.ScoreTime.Value.ToLongTimeString()} GMT";
                }

                ScoreSummary scoreSummary = await _performanceService.GetScoreSummaryAsync(id, ad);
                if (scoreSummary != null)
                {
                    model.EvaluationSummaryResult.QualitativeScoreObtained = scoreSummary.QualitativeScore;
                    model.EvaluationSummaryResult.QuantitativeScoreObtained = scoreSummary.QuantitativeScore;
                    model.EvaluationSummaryResult.TotalScoreObtained = scoreSummary.TotalPerformanceScore;
                }

                AppraisalGrade appraisalGrade = await _performanceService.GetAppraisalGradeAsync(model.EvaluationSummaryResult.ReviewSessionID, ReviewGradeType.Performance, model.EvaluationSummaryResult.TotalScoreObtained);
                if (appraisalGrade != null)
                {
                    model.EvaluationSummaryResult.PerformanceRating = appraisalGrade.AppraisalGradeDescription;
                    model.EvaluationSummaryResult.PerformanceRank = appraisalGrade.GradeRankDescription;
                }

                // Get KPA Results
                var kpa_entities = await _performanceService.GetInitialReviewResultKpasAsync(id, ad);
                if (kpa_entities != null && kpa_entities.Count > 0)
                {
                    model.KpaFullResult.ReviewResultList = kpa_entities.ToList();
                }

                // Get Competency Results
                var cmp_entities = await _performanceService.GetInitialReviewResultCmpsAsync(id, ad);
                if (cmp_entities != null && cmp_entities.Count > 0)
                {
                    model.CmpFullResult.ReviewResultList = cmp_entities.ToList();
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be r";
            }
            return View(model);
        }

        public async Task<IActionResult> ShowSelectedResult(int id, int ad)
        {
            ShowSelectedResultViewModel model = new ShowSelectedResultViewModel();
            model.EvaluationSummaryResult = new EvaluationResultViewModel();
            model.KpaFullResult = new EvaluationListViewModel();
            model.CmpFullResult = new EvaluationListViewModel();
            model.ReviewHeaderInfo = new ReviewHeader();
            model.ReviewCDGs = new List<ReviewCDG>();
            model.id = id;
            model.ad = ad;
            if (ad > 0)
            {
                model.EvaluationSummaryResult.AppraiserID = ad;
                model.EvaluationSummaryResult.ReviewHeaderID = id;
                if (id > 0 && ad > 0)
                {
                    ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                    if (reviewHeader != null)
                    {
                        model.ReviewHeaderInfo = reviewHeader;
                        model.EvaluationSummaryResult.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                        model.EvaluationSummaryResult.ReviewSessionID = reviewHeader.ReviewSessionId;
                        model.EvaluationSummaryResult.ReviewSessionName = reviewHeader.ReviewSessionName;
                        model.EvaluationSummaryResult.ReviewYearID = reviewHeader.ReviewYearId;
                        model.EvaluationSummaryResult.ReviewYearName = reviewHeader.ReviewYearName;
                        model.EvaluationSummaryResult.AppraiseeID = reviewHeader.AppraiseeId;
                        model.EvaluationSummaryResult.AppraiseeName = reviewHeader.AppraiseeName;

                        ReviewSession reviewSession = await _performanceService.GetReviewSessionAsync(model.EvaluationSummaryResult.ReviewSessionID);
                        if (reviewSession != null)
                        {
                            model.EvaluationSummaryResult.QualitativeScoreObtainable = reviewSession.TotalCompetencyScore;
                            model.EvaluationSummaryResult.QuantitativeScoreObtainable = reviewSession.TotalKpaScore;
                            model.EvaluationSummaryResult.TotalScoreObtainable = reviewSession.TotalCombinedScore;
                        }

                        List<ReviewCDG> reviewCDGs = await _performanceService.GetReviewCdgsAsync(id);
                        if (reviewCDGs != null)
                        {
                            model.ReviewCDGs = reviewCDGs;
                        }
                    }

                    var result_entities = await _performanceService.GetReviewResultByAppraiserIdAndReviewMetricTypeIdAsync(model.EvaluationSummaryResult.ReviewHeaderID, model.EvaluationSummaryResult.AppraiserID, null);
                    if (result_entities != null && result_entities.Count > 0)
                    {
                        ReviewResult reviewResult = result_entities.FirstOrDefault();
                        model.EvaluationSummaryResult.AppraiserName = reviewResult.AppraiserName;
                        model.EvaluationSummaryResult.AppraiserRoleID = reviewResult.AppraiserRoleId ?? 0;
                        model.EvaluationSummaryResult.AppraiserRoleName = reviewResult.AppraiserRoleName;
                        model.EvaluationSummaryResult.AppraiserTypeDescription = reviewResult.AppraiserTypeDescription;
                        model.EvaluationSummaryResult.AppraisalTime = $"{reviewResult.ScoreTime.Value.ToLongDateString()} {reviewResult.ScoreTime.Value.ToLongTimeString()} GMT";
                    }

                    ScoreSummary scoreSummary = await _performanceService.GetScoreSummaryAsync(id, ad);
                    if (scoreSummary != null)
                    {
                        model.EvaluationSummaryResult.QualitativeScoreObtained = scoreSummary.QualitativeScore;
                        model.EvaluationSummaryResult.QuantitativeScoreObtained = scoreSummary.QuantitativeScore;
                        model.EvaluationSummaryResult.TotalScoreObtained = scoreSummary.TotalPerformanceScore;
                    }

                    AppraisalGrade appraisalGrade = await _performanceService.GetAppraisalGradeAsync(model.EvaluationSummaryResult.ReviewSessionID, ReviewGradeType.Performance, model.EvaluationSummaryResult.TotalScoreObtained);
                    if (appraisalGrade != null)
                    {
                        model.EvaluationSummaryResult.PerformanceRating = appraisalGrade.AppraisalGradeDescription;
                        model.EvaluationSummaryResult.PerformanceRank = appraisalGrade.GradeRankDescription;
                    }

                    // Get KPA Results
                    var kpa_entities = await _performanceService.GetInitialReviewResultKpasAsync(id, ad);
                    if (kpa_entities != null && kpa_entities.Count > 0)
                    {
                        model.KpaFullResult.ReviewResultList = kpa_entities.ToList();
                    }

                    // Get Competency Results
                    var cmp_entities = await _performanceService.GetInitialReviewResultCmpsAsync(id, ad);
                    if (cmp_entities != null && cmp_entities.Count > 0)
                    {
                        model.CmpFullResult.ReviewResultList = cmp_entities.ToList();
                    }
                }
                else
                {
                    model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be r";
                }
            }

            var appraisers = await _performanceService.GetAppraiserDetailsAsync(id);
            if (appraisers != null)
            {
                ViewBag.AppraisersList = new SelectList(appraisers, "AppraiserId", "AppraiserFullDescription", ad);
            }
            return View(model);
        }


        #endregion

        #region Evaluation Feedback
        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> ManageFeedback(int id, int sd)
        {
            ManageFeedbackViewModel model = new ManageFeedbackViewModel();
            ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
            if (reviewHeader != null)
            {
                model.ProblemDescription = reviewHeader.FeedbackProblems;
                model.SolutionDescription = reviewHeader.FeedbackSolutions;
            }
            model.ReviewHeaderID = id;
            model.ReviewSessionID = sd;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSAPR, XXACC")]
        public async Task<IActionResult> ManageFeedback(ManageFeedbackViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int ReviewSessionId = model.ReviewSessionID;
                    int ReviewHeaderId = model.ReviewHeaderID;
                    string FeedbackProblems = model.ProblemDescription;
                    string FeedbackSolutions = model.SolutionDescription;

                    bool isAdded = await _performanceService.UpdateFeedbackAsync(ReviewHeaderId, FeedbackProblems, FeedbackSolutions);
                    if (isAdded)
                    {
                        PmsActivityHistory activityHistory = new PmsActivityHistory();
                        activityHistory.ReviewHeaderId = ReviewHeaderId;
                        activityHistory.ActivityTime = DateTime.UtcNow;
                        activityHistory.ActivityDescription = $"Added Feedback to the Performance Evaluation record.";
                        await _performanceService.AddPmsActivityHistoryAsync(activityHistory);

                        model.ViewModelSuccessMessage = "Feedback added successfully!";
                        model.OperationIsSuccessful = true;
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

        #region Direct Reports
        [Authorize(Roles = "PMSAPV, XXACC")]
        public async Task<IActionResult> DirectReportEvaluations(int id, int sd)
        {
            DirectReportEvaluationsViewModel model = new DirectReportEvaluationsViewModel();
            model.id = id;
            model.sd = sd;
            model.ReportsResultSummaryList = new List<ResultSummary>();
            int ReportsToID = 0;
            try
            {
                ApplicationUser user = new ApplicationUser();
                int userId = 0;
                var userStringId = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
                if (!string.IsNullOrWhiteSpace(userStringId)) { userId = Convert.ToInt32(userStringId); }
                user = await _securityService.GetUserByIdAsync(userId);
                if (user != null && !string.IsNullOrWhiteSpace(user.FullName))
                {
                    ReportsToID = user.EmployeeId;
                }

                if (ReportsToID < 1)
                {
                    model.ViewModelErrorMessage = "Sorry, no employee record was found for this user.";
                }
                else
                {
                    if (id > 0)
                    {
                        if (sd > 0)
                        {
                            var entities = await _performanceService.GetResultSummaryForReportsAsync(ReportsToID, id, sd);
                            if (entities != null && entities.Count > 0)
                            {
                                model.ReportsResultSummaryList = entities;
                            }
                        }
                        else
                        {
                            var entities = await _performanceService.GetResultSummaryForReportsAsync(ReportsToID, id);
                            if (entities != null && entities.Count > 0)
                            {
                                model.ReportsResultSummaryList = entities;
                            }
                        }
                    }

                    var reports_entities = await _employeeRecordService.GetEmployeeReportsByReportsToIdAsync(ReportsToID);
                    if (reports_entities != null && reports_entities.Count > 0)
                    {
                        ViewBag.ReportsList = new SelectList(reports_entities, "EmployeeId", "EmployeeName", sd);
                    }
                }

                var sessions_entities = await _performanceService.GetReviewSessionsAsync();
                if (sessions_entities != null && sessions_entities.Count > 0)
                {
                    ViewBag.SessionsList = new SelectList(sessions_entities, "Id", "Name", id);
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            return View(model);
        }

        [Authorize(Roles = "PMSAPV, XXACC")]
        public async Task<IActionResult> AddRecommendation(int id)
        {
            AddRecommendationViewModel model = new AddRecommendationViewModel();
            try
            {
                model.ReviewHeaderID = id;
                ApplicationUser user = new ApplicationUser();
                int userId = 0;
                var userStringId = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
                if (!string.IsNullOrWhiteSpace(userStringId)) { userId = Convert.ToInt32(userStringId); }
                user = await _securityService.GetUserByIdAsync(userId);
                if (!string.IsNullOrWhiteSpace(user.FullName))
                {
                    model.RecommenderID = user.EmployeeId;
                }
                model.RecommenderName = HttpContext.User.Identity.Name;

                List<AppraisalRecommendation> entities = await _performanceService.GetAppraisalRecommendationsAsync();
                if (entities != null && entities.Count > 0)
                {
                    ViewBag.RecommendedActionList = new SelectList(entities, "Description", "Description");
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
        [Authorize(Roles = "PMSAPV, XXACC")]
        public async Task<IActionResult> AddRecommendation(AddRecommendationViewModel model)
        {
            if (ModelState.IsValid)
            {
                ReviewHeaderRecommendation recommendationModel = new ReviewHeaderRecommendation();
                recommendationModel.RecommendationRemarks = model.Remarks;
                recommendationModel.RecommendationType = model.RecommenderRole;
                recommendationModel.RecommendedAction = model.RecommendedAction;
                recommendationModel.ReviewHeaderId = model.ReviewHeaderID;
                recommendationModel.RecommendedByName = model.RecommenderName;
                try
                {
                    bool isAdded = await _performanceService.AddAppraisalRecommendationAsync(recommendationModel);
                    if (isAdded)
                    {
                        PmsActivityHistory activityHistory = new PmsActivityHistory();
                        activityHistory.ReviewHeaderId = model.ReviewHeaderID;
                        activityHistory.ActivityTime = DateTime.UtcNow;
                        activityHistory.ActivityDescription = $"Added {model.RecommenderRoleDescription} recommendation to the Performance Evaluation record.";
                        await _performanceService.AddPmsActivityHistoryAsync(activityHistory);

                        model.ViewModelSuccessMessage = "Recommendation added successfully!";
                        model.OperationIsSuccessful = true;
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }

            List<AppraisalRecommendation> entities = await _performanceService.GetAppraisalRecommendationsAsync();
            if (entities != null && entities.Count > 0)
            {
                ViewBag.RecommendedActionList = new SelectList(entities, "Description", "Description", model.RecommendedAction);
            }
            return View(model);
        }

        #endregion

        #region Enquiry and Reports

        [Authorize(Roles = "PMSNQR, XXACC")]
        public async Task<IActionResult> AppraisalEnquiry(int id, string dc, string uc, string nm)
        {
            PmsEnquiryViewModel model = new PmsEnquiryViewModel();
            model.ResultSummaryList = new List<ResultSummary>();
            model.id = id;
            model.dc = dc;
            model.uc = uc;
            model.nm = nm;

            try
            {
                if (id > 0)
                {
                    if (!string.IsNullOrWhiteSpace(nm))
                    {
                        var entities = await _performanceService.GetResultSummaryByReviewSessionIdAndAppraiseeNameAsync(id, nm);
                        if (entities != null && entities.Count > 0)
                        {
                            model.ResultSummaryList = entities;
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(uc))
                    {
                        var entities = await _performanceService.GetResultSummaryByReviewSessionIdAndUnitCodeAsync(id, uc);
                        if (entities != null && entities.Count > 0)
                        {
                            model.ResultSummaryList = entities;
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(dc))
                    {
                        var entities = await _performanceService.GetResultSummaryByReviewSessionIdAndDepartmentCodeAsync(id, dc);
                        if (entities != null && entities.Count > 0)
                        {
                            model.ResultSummaryList = entities;
                        }
                    }
                    else
                    {
                        var entities = await _performanceService.GetResultSummaryByReviewSessionIdAsync(id);
                        if (entities != null && entities.Count > 0)
                        {
                            model.ResultSummaryList = entities;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            var sessions_entities = await _performanceService.GetReviewSessionsAsync();
            if (sessions_entities != null && sessions_entities.Count > 0)
            {
                ViewBag.SessionsList = new SelectList(sessions_entities, "Id", "Name", id);
            }

            var dept_entities = await _globalSettingsService.GetDepartmentsAsync();
            if (dept_entities != null && dept_entities.Count > 0)
            {
                ViewBag.DepartmentList = new SelectList(dept_entities, "DepartmentCode", "DepartmentName", dc);
            }

            var unit_entities = await _globalSettingsService.GetUnitsAsync();
            if (unit_entities != null && unit_entities.Count > 0)
            {
                ViewBag.UnitList = new SelectList(unit_entities, "UnitCode", "UnitName", uc);
            }

            return View(model);
        }

        [Authorize(Roles = "PMSNQR, XXACC")]
        public async Task<IActionResult> ResultReport(int id, int? lc = null, string dc = null, string uc = null)
        {
            ResultReportViewModel model = new ResultReportViewModel();
            model.ResultDetailList = new List<ResultDetail>();
            model.id = id;
            model.lc = lc;
            model.dc = dc;
            model.uc = uc;

            model.ResultDetailList = new List<ResultDetail>();
            try
            {
                if (id > 0)
                {
                    var entities = await _performanceService.GetPrincipalResultDetailAsync(id, lc, dc, uc);
                    if (entities != null && entities.Count > 0)
                    {
                        model.ResultDetailList = entities;
                        ResultDetail resultDetail = new ResultDetail();
                        resultDetail = entities.FirstOrDefault();
                        model.ReviewSessionDescription = resultDetail.ReviewSessionName;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            var sessions_entities = await _performanceService.GetReviewSessionsAsync();
            if (sessions_entities != null && sessions_entities.Count > 0)
            {
                ViewBag.SessionsList = new SelectList(sessions_entities, "Id", "Name", id);
            }

            var loc_entities = await _globalSettingsService.GetLocationsAsync();
            if (loc_entities != null && loc_entities.Count > 0)
            {
                ViewBag.LocationList = new SelectList(loc_entities, "LocationId", "LocationName", lc);
            }

            var dept_entities = await _globalSettingsService.GetDepartmentsAsync();
            if (dept_entities != null && dept_entities.Count > 0)
            {
                ViewBag.DepartmentList = new SelectList(dept_entities, "DepartmentCode", "DepartmentName", dc);
            }

            var unit_entities = await _globalSettingsService.GetUnitsAsync();
            if (unit_entities != null && unit_entities.Count > 0)
            {
                ViewBag.UnitList = new SelectList(unit_entities, "UnitCode", "UnitName", uc);
            }

            if (TempData["ErrorMessage"] != null)
            {
                model.ViewModelErrorMessage = TempData["ErrorMessage"].ToString();
            }

            return View(model);
        }

        public async Task<FileResult> DownloadResultReport(int id, int? lc = null, string dc = null, string uc = null)
        {
            List<ResultDetail> ResultDetailList = new List<ResultDetail>();
            string ReviewSessionDescription = string.Empty;
            string fileName = string.Empty;
            try
            {
                if (id > 0)
                {
                    var entities = await _performanceService.GetPrincipalResultDetailAsync(id, lc, dc, uc);
                    if (entities != null && entities.Count > 0)
                    {
                        ResultDetailList = entities;
                        ResultDetail resultDetail = new ResultDetail();
                        resultDetail = entities.FirstOrDefault();
                        ReviewSessionDescription = resultDetail.ReviewSessionName;
                        fileName = $"Appraisal Report {DateTime.UtcNow.ToString("yyyyMMddHHmmssfff")}.xlsx";
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
                //TempData["ErrorMessage"] = ex.Message;
                //return RedirectToAction("ResultReport", new { id, lc, dc, uc });
            }
            return GenerateResultReportExcel(fileName, ResultDetailList);
        }

        #endregion

        #region Helper Methods
        public string DeleteSubmission(int sd)
        {
            if (sd < 1) { return "parameter"; }
            //string actionBy = HttpContext.User.Identity.Name;
            try
            {
                if (_performanceService.DeleteReviewSubmissionAsync(sd).Result)
                {
                    return "deleted";
                }
                else
                {
                    return "failed";
                }
            }
            catch
            {
                return "failed";
            }
        }

        public string MarkSubmissionAsDone(int sd)
        {
            if (sd < 1) { return "parameter"; }
            //string actionBy = HttpContext.User.Identity.Name;
            try
            {
                if (_performanceService.UpdateReviewSubmissionAsync(sd).Result)
                {
                    return "marked";
                }
                else
                {
                    return "failed";
                }
            }
            catch
            {
                return "failed";
            }
        }

        private FileResult GenerateResultReportExcel(string fileName, IEnumerable<ResultDetail> results)
        {
            DataTable dataTable = new DataTable("results");
            dataTable.Columns.AddRange(new DataColumn[]
            {
              new DataColumn("Appraisee No"),
              new DataColumn("Appraisee Name"),
              new DataColumn("Appraisee Designation"),
              new DataColumn("Appraisee Unit"),
              new DataColumn("Appraisee Department"),
              new DataColumn("Appraisee Location"),
              new DataColumn("Feedback Problems"),
              new DataColumn("Feedback Solutions"),
              new DataColumn("Appraisee Disagrees"),
              new DataColumn("Appraiser Name"),
              new DataColumn("Appraiser Designation"),
              new DataColumn("Appraiser Role"),
              new DataColumn("Appraiser Type"),
              new DataColumn("Kpa Score"),
              new DataColumn("Competency Score"),
              new DataColumn("Total Score"),
              new DataColumn("Rating"),
              new DataColumn("Line Manager Recommendation"),
              new DataColumn("Line Manager Comments"),
              new DataColumn("Line Manager Name"),
              new DataColumn("Unit Head Recommendation"),
              new DataColumn("Unit Head Comments"),
              new DataColumn("Unit Head Name"),
              new DataColumn("Department Head Recommendation"),
              new DataColumn("Department Head Comments"),
              new DataColumn("Department Head Name"),
              new DataColumn("HR Recommendation"),
              new DataColumn("HR Comments"),
              new DataColumn("Management Decision"),
              new DataColumn("ManagementComments"),
            });

            foreach (var result in results)
            {
                dataTable.Rows.Add(
                    result.EmployeeNo,
                    result.AppraiseeName,
                    result.AppraiseeDesignation,
                    result.UnitName,
                    result.DepartmentName,
                    result.LocationName,
                    result.FeedbackProblems,
                    result.FeedbackSolutions,
                    result.IsFlagged,
                    result.AppraiserName,
                    result.AppraiserDesignation,
                    result.AppraiserRoleDescription,
                    result.AppraiserTypeDescription,
                    result.KpaScoreObtained,
                    result.CompetencyScoreObtained,
                    result.CombinedScoreObtained,
                    result.PerformanceRating,
                    result.LineManagerRecommendation,
                    result.LineManagerComments,
                    result.LineManagerName,
                    result.UnitHeadRecommendation,
                    result.UnitHeadComments,
                    result.UnitHeadName,
                    result.DepartmentHeadRecommendation,
                    result.DepartmentHeadComments,
                    result.DepartmentHeadName,
                    result.HrRecommendation,
                    result.HrComments,
                    result.ManagementDecision,
                    result.ManagementComments
                    );
            }

            using (XLWorkbook workbook = new XLWorkbook())
            {
                workbook.Worksheets.Add(dataTable);
                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }
        
        #endregion
    }
}