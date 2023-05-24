using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public IActionResult Settings()
        {
            return View();
        }

        public IActionResult Appraisals()
        {
            return View();
        }

        #region Performance Year Controller Actions
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

        #region Review Session Controller Actions
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

        public async Task<IActionResult> MyAppraisalSteps(int id)
        {
            MyAppraisalStepsViewModel model = new MyAppraisalStepsViewModel();
            ReviewSession reviewSession = new ReviewSession();
            ApplicationUser user = new ApplicationUser();
            int userId = 0;
            var userStringId = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
            if (!string.IsNullOrWhiteSpace(userStringId)) { userId = Convert.ToInt32(userStringId); }
            user = await _securityService.GetUserByIdAsync(userId);
            if (!string.IsNullOrWhiteSpace(user.FullName))
            {
                model.AppraiseeName = user.FullName;
                model.AppraiseeId = user.EmployeeId;
            }

            if (id > 0)
            {
                model.ReviewSessionId = id;
                reviewSession = await _performanceService.GetReviewSessionAsync(id);
                if (!string.IsNullOrWhiteSpace(reviewSession.Name)) { model.ReviewSessionName = reviewSession.Name; }
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
        #endregion

        #region Performance Goals Controller Actions
        public IActionResult CreatePerformanceGoals(int id, int ad)
        {
            CreatePerformanceGoalsViewModel model = new CreatePerformanceGoalsViewModel();
            model.AppraiseeID = ad;
            model.ReviewSessionID = id;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePerformanceGoals(CreatePerformanceGoalsViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ReviewHeader reviewHeader = new ReviewHeader();
                    reviewHeader.ReviewSessionId = model.ReviewSessionID;
                    reviewHeader.ReviewHeaderId = model.ReviewHeaderID ?? 0;
                    reviewHeader.AppraiseeId = model.AppraiseeID;
                    reviewHeader.PerformanceGoal = model.PerformanceGoals;
                    reviewHeader.ReviewStageId = 1;

                    ApplicationUser applicationUser = new ApplicationUser();
                    applicationUser = await _securityService.GetUserByIdAsync(model.AppraiseeID);
                    if (applicationUser != null)
                    {
                        reviewHeader.UnitCode = applicationUser.UnitCode;
                        reviewHeader.DepartmentCode = applicationUser.DepartmentCode;
                        reviewHeader.LocationId = applicationUser.LocationId;
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
            return View(model);
        }

        public async Task<IActionResult> ManagePerformanceGoal(int id)
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
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManagePerformanceGoal(CreatePerformanceGoalsViewModel model)
        {
            try
            {
                if (model.ReviewHeaderID > 0)
                {
                    bool isUpdated = await _performanceService.UpdatePerformanceGoalAsync(model.ReviewHeaderID.Value, model.PerformanceGoals);
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
            return View(model);
        }

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
        public async Task<IActionResult> MoveToNextStep(int id)
        {
            MoveToNextStageModel model = new MoveToNextStageModel();
            if (id > 0)
            {
                model = await _performanceService.ValidateMoveRequestAsync(id);
            }
            else
            {
                return RedirectToAction("MyAppraisalSessions");
            }
            return View(model);
        }

        [HttpPost]
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
        public async Task<IActionResult> MoveToPreviousStep(MoveToNextStageModel model)
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
        #endregion

        #region Appraisal KPAs Controller Actions
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
                        model.AppraiserDesignation = reviewMetric.AppraiserDesignation;
                        model.AppraiserRole = reviewMetric.AppraiserRole;
                        model.MetricAppraiserId = reviewMetric.MetricAppraiserId;
                        model.MetricAppraiserName = reviewMetric.MetricAppraiserName;
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

            var entities = await _employeeRecordService.GetEmployeeReportsByEmployeeIdAsync(model.AppraiseeId);
            ViewBag.AppraiserRolesList = new SelectList(entities, "ReportsToId", "ReportsToName", model.MetricAppraiserId);
            return View(model);
        }

        [HttpPost]
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
                        model.AppraiserDesignation = reviewMetric.AppraiserDesignation;
                        model.AppraiserRole = reviewMetric.AppraiserRole;
                        model.MetricAppraiserId = reviewMetric.MetricAppraiserId;
                        model.MetricAppraiserName = reviewMetric.MetricAppraiserName;
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

                var entities = await _performanceService.GetCdgsAsync(id);
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
    }
}