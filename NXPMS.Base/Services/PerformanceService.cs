using NXPMS.Base.Enums;
using NXPMS.Base.Models.PMSModels;
using NXPMS.Base.Repositories.EmployeeRecordRepositories;
using NXPMS.Base.Repositories.PMSRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NXPMS.Base.Services
{
    public class PerformanceService : IPerformanceService
    {
        private readonly IEmployeesRepository _employeesRepository;
        private readonly IPerformanceYearRepository _performanceYearRepository;
        private readonly IReviewSessionRepository _reviewSessionRepository;
        private readonly IPerformanceSettingsRepository _performanceSettingsRepository;
        private readonly IReviewGradeRepository _reviewGradeRepository;
        private readonly IGradeHeaderRepository _gradeHeaderRepository;
        private readonly IAppraisalGradeRepository _appraisalGradeRepository;
        private readonly ISessionScheduleRepository _sessionScheduleRepository;
        private readonly IReviewHeaderRepository _reviewHeaderRepository;
        private readonly IReviewStageRepository _reviewStageRepository;
        private readonly IPmsActivityHistoryRepository _pmsActivityHistoryRepository;
        private readonly IReviewMetricRepository _reviewMetricRepository;
        private readonly ICompetencyRepository _competencyRepository;
        private readonly IPmsSystemRepository _pmsSystemRepository;
        private readonly IReviewCDGRepository _reviewCDGRepository;
        private readonly IReviewSubmissionRepository _reviewSubmissionRepository;

        public PerformanceService(IEmployeesRepository employeesRepository, IPerformanceYearRepository performanceYearRepository,
            IReviewSessionRepository reviewSessionRepository, IPerformanceSettingsRepository performanceSettingsRepository,
            IReviewGradeRepository reviewGradeRepository, IGradeHeaderRepository gradeHeaderRepository,
            IAppraisalGradeRepository appraisalGradeRepository, ISessionScheduleRepository sessionScheduleRepository,
            IReviewHeaderRepository reviewHeaderRepository, IReviewStageRepository reviewStageRepository,
            IPmsActivityHistoryRepository pmsActivityHistoryRepository, IReviewMetricRepository reviewMetricRepository,
            ICompetencyRepository competencyRepository, IPmsSystemRepository pmsSystemRepository,
            IReviewCDGRepository reviewCDGRepository, IReviewSubmissionRepository reviewSubmissionRepository)
        {
            _employeesRepository = employeesRepository;
            _performanceYearRepository = performanceYearRepository;
            _reviewSessionRepository = reviewSessionRepository;
            _performanceSettingsRepository = performanceSettingsRepository;
            _reviewGradeRepository = reviewGradeRepository;
            _gradeHeaderRepository = gradeHeaderRepository;
            _appraisalGradeRepository = appraisalGradeRepository;
            _sessionScheduleRepository = sessionScheduleRepository;
            _reviewHeaderRepository = reviewHeaderRepository;
            _reviewStageRepository = reviewStageRepository;
            _pmsActivityHistoryRepository = pmsActivityHistoryRepository;
            _reviewMetricRepository = reviewMetricRepository;
            _competencyRepository = competencyRepository;
            _pmsSystemRepository = pmsSystemRepository;
            _reviewCDGRepository = reviewCDGRepository;
            _reviewSubmissionRepository = reviewSubmissionRepository;
        }


        #region Performance Year Service Methods
        public async Task<List<PerformanceYear>> GetPerformanceYearsAsync()
        {
            List<PerformanceYear> performanceYearList = new List<PerformanceYear>();
            var entities = await _performanceYearRepository.GetAllAsync();
            if (entities != null && entities.Count > 0)
            {
                performanceYearList = entities.ToList();
            }
            return performanceYearList;
        }

        public async Task<PerformanceYear> GetPerformanceYearAsync(int PerformanceYearId)
        {
            PerformanceYear performanceYear = new PerformanceYear();
            var entities = await _performanceYearRepository.GetByIdAsync(PerformanceYearId);
            if (entities != null && entities.Count > 0)
            {
                performanceYear = entities.ToList().FirstOrDefault();
            }
            return performanceYear;
        }

        public async Task<bool> AddPerformanceYearAsync(PerformanceYear performanceYear)
        {
            if (performanceYear == null) { throw new ArgumentNullException(nameof(performanceYear)); }
            var entities = await _performanceYearRepository.GetByNameAsync(performanceYear.Name);
            if (entities != null && entities.Count > 0)
            {
                throw new Exception("A Performance Yaer with the same name already exists in the system.");
            }

            var overlapping_entities = await _performanceYearRepository.GetByOverlappingDatesAsync(performanceYear.StartDate.Value, performanceYear.EndDate.Value);
            if (overlapping_entities != null && overlapping_entities.Count > 0)
            {
                throw new Exception("It appears the Start and End dates overlaps with another Year in the system.");
            }

            return await _performanceYearRepository.AddAsync(performanceYear);
        }

        public async Task<bool> EditPerformanceYearAsync(PerformanceYear performanceYear)
        {
            if (performanceYear == null) { throw new ArgumentNullException(nameof(performanceYear)); }
            var entities = await _performanceYearRepository.GetByNameAsync(performanceYear.Name);
            if (entities != null && entities.Count > 0)
            {
                List<PerformanceYear> performanceYears = entities.ToList();
                foreach (PerformanceYear yr in performanceYears)
                {
                    if (yr.Id != performanceYear.Id)
                    {
                        throw new Exception("A Performance Yaer with the same name already exists in the system.");
                    }
                }
            }

            var overlapping_entities = await _performanceYearRepository.GetByOverlappingDatesAsync(performanceYear.StartDate.Value, performanceYear.EndDate.Value);
            if (overlapping_entities != null && overlapping_entities.Count > 0)
            {
                List<PerformanceYear> performanceYears = entities.ToList();
                foreach (PerformanceYear yr in performanceYears)
                {
                    if (yr.Id != performanceYear.Id)
                    {
                        throw new Exception("It appears the Start and End dates overlaps with another Year in the system.");
                    }
                }
            }
            return await _performanceYearRepository.UpdateAsync(performanceYear);
        }

        public async Task<bool> DeletePerformanceYearAsync(int performanceYearId)
        {
            if (performanceYearId < 1) { throw new ArgumentNullException(nameof(performanceYearId)); }
            return await _performanceYearRepository.DeleteAsync(performanceYearId);
        }
        #endregion

        #region Review Session Read Service Methods
        public async Task<List<ReviewSession>> GetReviewSessionsAsync()
        {
            List<ReviewSession> reviewSessionList = new List<ReviewSession>();
            var entities = await _reviewSessionRepository.GetAllAsync();
            if (entities != null && entities.Count > 0)
            {
                reviewSessionList = entities.ToList();
            }
            return reviewSessionList;
        }

        public async Task<List<ReviewSession>> GetReviewSessionsAsync(int PerformanceYearId)
        {
            List<ReviewSession> reviewSessionList = new List<ReviewSession>();
            var entities = await _reviewSessionRepository.GetByYearIdAsync(PerformanceYearId);
            if (entities != null && entities.Count > 0)
            {
                reviewSessionList = entities.ToList();
            }
            return reviewSessionList;
        }

        public async Task<ReviewSession> GetReviewSessionAsync(int ReviewSessionId)
        {
            ReviewSession reviewSession = new ReviewSession();
            var entities = await _reviewSessionRepository.GetByIdAsync(ReviewSessionId);
            if (entities != null && entities.Count > 0)
            {
                reviewSession = entities.ToList().FirstOrDefault();
            }
            return reviewSession;
        }

        #endregion

        #region Review Session Write Service Methods
        public async Task<bool> AddReviewSessionAsync(ReviewSession reviewSession)
        {
            if (reviewSession == null) { throw new ArgumentNullException(nameof(reviewSession)); }
            var entities = await _reviewSessionRepository.GetByNameAsync(reviewSession.Name);
            if (entities != null && entities.Count > 0)
            {
                throw new Exception("A Review Session with the same name already exists in the system.");
            }

            return await _reviewSessionRepository.AddAsync(reviewSession);
        }

        public async Task<bool> EditReviewSessionAsync(ReviewSession reviewSession)
        {
            if (reviewSession == null) { throw new ArgumentNullException(nameof(reviewSession)); }
            var entities = await _reviewSessionRepository.GetByNameAsync(reviewSession.Name);
            if (entities != null && entities.Count > 0)
            {
                List<ReviewSession> reviewSessions = entities.ToList();
                foreach (ReviewSession r in reviewSessions)
                {
                    if (r.Id != reviewSession.Id)
                    {
                        throw new Exception("A Review Session with the same name already exists in the system.");
                    }
                }
            }
            return await _reviewSessionRepository.UpdateAsync(reviewSession);
        }

        public async Task<bool> DeleteReviewSessionAsync(int ReviewSessionId)
        {
            if (ReviewSessionId < 1) { throw new ArgumentNullException(nameof(ReviewSessionId)); }
            return await _reviewSessionRepository.DeleteAsync(ReviewSessionId);
        }
        #endregion

        #region Grade Header Service Methods
        public async Task<List<GradeHeader>> GetGradeHeadersAsync()
        {
            List<GradeHeader> gradeHeaderList = new List<GradeHeader>();
            var entities = await _gradeHeaderRepository.GetAllAsync();
            if (entities != null && entities.Count > 0)
            {
                gradeHeaderList = entities.ToList();
            }
            return gradeHeaderList;
        }

        public async Task<GradeHeader> GetGradeHeaderAsync(int GradeHeaderId)
        {
            GradeHeader gradeHeader = new GradeHeader();
            var entities = await _gradeHeaderRepository.GetByIdAsync(GradeHeaderId);
            if (entities != null && entities.Count > 0)
            {
                gradeHeader = entities.ToList().FirstOrDefault();
            }
            return gradeHeader;
        }

        public async Task<bool> AddGradeHeaderAsync(GradeHeader gradeHeader)
        {
            if (gradeHeader == null) { throw new ArgumentNullException(nameof(gradeHeader)); }
            var entities = await _gradeHeaderRepository.GetByNameAsync(gradeHeader.GradeHeaderName);
            if (entities != null && entities.Count > 0)
            {
                throw new Exception("A Grading Profile with the same name already exists in the system.");
            }

            return await _gradeHeaderRepository.AddAsync(gradeHeader);
        }

        public async Task<bool> EditGradeHeaderAsync(GradeHeader gradeHeader)
        {
            if (gradeHeader == null) { throw new ArgumentNullException(nameof(gradeHeader)); }
            var entities = await _gradeHeaderRepository.GetByNameAsync(gradeHeader.GradeHeaderName);
            if (entities != null && entities.Count > 0)
            {
                List<GradeHeader> gradeHeaders = entities.ToList();
                foreach (GradeHeader gh in gradeHeaders)
                {
                    if (gh.GradeHeaderId != gradeHeader.GradeHeaderId)
                    {
                        throw new Exception("A Grading Profile with the same name already exists in the system.");
                    }
                }
            }

            return await _gradeHeaderRepository.UpdateAsync(gradeHeader);
        }

        public async Task<bool> DeleteGradeHeaderAsync(int gradeHeaderId)
        {
            if (gradeHeaderId < 1) { throw new ArgumentNullException(nameof(gradeHeaderId)); }
            return await _gradeHeaderRepository.DeleteAsync(gradeHeaderId);
        }
        #endregion

        #region Review Grade Details Write Service Methods
        public async Task<bool> AddReviewGradeAsync(ReviewGrade reviewGrade)
        {
            if (reviewGrade == null) { throw new ArgumentNullException(nameof(reviewGrade)); }
            var entities = await _reviewGradeRepository.GetByNameAsync(reviewGrade.ReviewGradeDescription);
            if (entities != null && entities.Count > 0)
            {
                throw new Exception("A Grade with the same name already exists in the system.");
            }

            return await _reviewGradeRepository.AddAsync(reviewGrade);
        }

        public async Task<bool> EditReviewGradeAsync(ReviewGrade reviewGrade)
        {
            if (reviewGrade == null) { throw new ArgumentNullException(nameof(reviewGrade)); }
            var entities = await _reviewGradeRepository.GetByNameAsync(reviewGrade.ReviewGradeDescription);
            if (entities != null && entities.Count > 0)
            {
                List<ReviewGrade> reviewGrades = entities.ToList();
                foreach (ReviewGrade r in reviewGrades)
                {
                    if (r.ReviewGradeId != reviewGrade.ReviewGradeId)
                    {
                        throw new Exception("A Grade with the same name already exists in the system.");
                    }
                }
            }
            return await _reviewGradeRepository.UpdateAsync(reviewGrade);
        }

        public async Task<bool> DeleteReviewGradeAsync(int reviewGradeId)
        {
            if (reviewGradeId < 1) { throw new ArgumentNullException(nameof(reviewGradeId)); }
            return await _reviewGradeRepository.DeleteAsync(reviewGradeId);
        }
        #endregion

        #region Review Grade Details Read Service Methods
        public async Task<List<ReviewGrade>> GetReviewGradesAsync()
        {
            List<ReviewGrade> reviewGradeList = new List<ReviewGrade>();
            var entities = await _reviewGradeRepository.GetAllAsync();
            if (entities != null && entities.Count > 0)
            {
                reviewGradeList = entities.ToList();
            }
            return reviewGradeList;
        }

        public async Task<List<ReviewGrade>> GetReviewGradesAsync(int gradeHeaderId)
        {
            List<ReviewGrade> reviewGradeList = new List<ReviewGrade>();
            var entities = await _reviewGradeRepository.GetByGradeHeaderIdAsync(gradeHeaderId);
            if (entities != null && entities.Count > 0)
            {
                reviewGradeList = entities.ToList();
            }
            return reviewGradeList;
        }

        public async Task<ReviewGrade> GetReviewGradeAsync(int ReviewGradeId)
        {
            ReviewGrade reviewGrade = new ReviewGrade();
            var entities = await _reviewGradeRepository.GetByIdAsync(ReviewGradeId);
            if (entities != null && entities.Count > 0)
            {
                reviewGrade = entities.ToList().FirstOrDefault();
            }
            return reviewGrade;
        }

        public async Task<List<ReviewGrade>> GetPerformanceGradesAsync(int gradeHeaderId)
        {
            List<ReviewGrade> reviewGradeList = new List<ReviewGrade>();
            var entities = await _reviewGradeRepository.GetByGradeHeaderIdAsync(gradeHeaderId, ReviewGradeType.Performance);
            if (entities != null && entities.Count > 0)
            {
                reviewGradeList = entities.ToList();
            }
            return reviewGradeList;
        }

        public async Task<List<ReviewGrade>> GetCompetencyGradesAsync(int gradeHeaderId)
        {
            List<ReviewGrade> reviewGradeList = new List<ReviewGrade>();
            var entities = await _reviewGradeRepository.GetByGradeHeaderIdAsync(gradeHeaderId, ReviewGradeType.Competency);
            if (entities != null && entities.Count > 0)
            {
                reviewGradeList = entities.ToList();
            }
            return reviewGradeList;
        }
        #endregion

        #region Appraisal Grade Write Service Methods
        public async Task<bool> AddAppraisalGradeAsync(AppraisalGrade appraisalGrade)
        {
            if (appraisalGrade == null) { throw new ArgumentNullException(nameof(appraisalGrade)); }
            var entities = await _appraisalGradeRepository.GetByNameAsync(appraisalGrade.AppraisalGradeDescription);
            if (entities != null && entities.Count > 0)
            {
                throw new Exception("A Grade with the same name already exists in the system.");
            }

            return await _appraisalGradeRepository.AddAsync(appraisalGrade);
        }

        public async Task<bool> CopyAppraisalGradeAsync(string copiedBy, int reviewSessionId, int gradeTemplateId, ReviewGradeType? gradeType = null)
        {
            if (string.IsNullOrWhiteSpace(copiedBy)) { throw new ArgumentNullException(nameof(copiedBy)); }
            if (reviewSessionId < 1) { throw new ArgumentNullException(nameof(reviewSessionId)); }
            if (gradeTemplateId < 1) { throw new ArgumentNullException(nameof(gradeTemplateId)); }

            var entities = await _appraisalGradeRepository.GetByReviewSessionIdAsync(reviewSessionId);
            if (entities != null && entities.Count > 0)
            {
                throw new Exception("Sorry, this Appraisal Session already has Grades set up in the system.");
            }
            if (gradeType == null)
            {
                return await _appraisalGradeRepository.CopyAsync(copiedBy, reviewSessionId, gradeTemplateId);
            }
            else
            {
                return await _appraisalGradeRepository.CopyAsync(copiedBy, reviewSessionId, gradeTemplateId, gradeType.Value);
            }
        }

        public async Task<bool> EditAppraisalGradeAsync(AppraisalGrade appraisalGrade)
        {
            if (appraisalGrade == null) { throw new ArgumentNullException(nameof(appraisalGrade)); }
            var entities = await _appraisalGradeRepository.GetByNameAsync(appraisalGrade.AppraisalGradeDescription);
            if (entities != null && entities.Count > 0)
            {
                List<AppraisalGrade> appraisalGrades = entities.ToList();
                foreach (AppraisalGrade r in appraisalGrades)
                {
                    if (r.AppraisalGradeId != appraisalGrade.AppraisalGradeId)
                    {
                        throw new Exception("A Grade with the same name already exists in the system.");
                    }
                }
            }
            return await _appraisalGradeRepository.UpdateAsync(appraisalGrade);
        }

        public async Task<bool> DeleteAppraisalGradeAsync(int appraisalGradeId)
        {
            if (appraisalGradeId < 1) { throw new ArgumentNullException(nameof(appraisalGradeId)); }
            return await _appraisalGradeRepository.DeleteAsync(appraisalGradeId);
        }
        #endregion

        #region Appraisal Grade Read Service Methods
        public async Task<List<AppraisalGrade>> GetAppraisalGradesAsync()
        {
            List<AppraisalGrade> appraisalGradeList = new List<AppraisalGrade>();
            var entities = await _appraisalGradeRepository.GetAllAsync();
            if (entities != null && entities.Count > 0)
            {
                appraisalGradeList = entities.ToList();
            }
            return appraisalGradeList;
        }

        public async Task<List<AppraisalGrade>> GetAppraisalGradesAsync(int reviewSessionId)
        {
            List<AppraisalGrade> appraisalGradeList = new List<AppraisalGrade>();
            var entities = await _appraisalGradeRepository.GetByReviewSessionIdAsync(reviewSessionId);
            if (entities != null && entities.Count > 0)
            {
                appraisalGradeList = entities.ToList();
            }
            return appraisalGradeList;
        }

        public async Task<AppraisalGrade> GetAppraisalGradeAsync(int AppraisalGradeId)
        {
            AppraisalGrade appraisalGrade = new AppraisalGrade();
            var entities = await _appraisalGradeRepository.GetByIdAsync(AppraisalGradeId);
            if (entities != null && entities.Count > 0)
            {
                appraisalGrade = entities.ToList().FirstOrDefault();
            }
            return appraisalGrade;
        }

        public async Task<List<AppraisalGrade>> GetAppraisalPerformanceGradesAsync(int reviewSessionId)
        {
            List<AppraisalGrade> appraisalGradeList = new List<AppraisalGrade>();
            var entities = await _appraisalGradeRepository.GetByReviewSessionIdAsync(reviewSessionId, ReviewGradeType.Performance);
            if (entities != null && entities.Count > 0)
            {
                appraisalGradeList = entities.ToList();
            }
            return appraisalGradeList;
        }

        public async Task<List<AppraisalGrade>> GetAppraisalCompetencyGradesAsync(int reviewSessionId)
        {
            List<AppraisalGrade> appraisalGradeList = new List<AppraisalGrade>();
            var entities = await _appraisalGradeRepository.GetByReviewSessionIdAsync(reviewSessionId, ReviewGradeType.Competency);
            if (entities != null && entities.Count > 0)
            {
                appraisalGradeList = entities.ToList();
            }
            return appraisalGradeList;
        }
        #endregion

        #region Session Schedule Write Service Methods
        public async Task<bool> AddSessionScheduleAsync(SessionSchedule sessionSchedule)
        {
            if (sessionSchedule == null) { throw new ArgumentNullException(nameof(sessionSchedule)); }

            if (sessionSchedule.ReviewYearId < 1 && sessionSchedule.ReviewSessionId > 0)
            {
                ReviewSession reviewSession = new ReviewSession();
                var entities = await _reviewSessionRepository.GetByIdAsync(sessionSchedule.ReviewSessionId);
                if (entities != null && entities.Count > 0)
                {
                    reviewSession = entities.FirstOrDefault();
                    sessionSchedule.ReviewYearId = reviewSession.ReviewYearId;
                }
            }
            return await _sessionScheduleRepository.AddAsync(sessionSchedule);
        }

        public async Task<bool> CancelSessionScheduleAsync(int sessionScheduleId, string cancelledBy)
        {
            if (string.IsNullOrWhiteSpace(cancelledBy)) { throw new ArgumentNullException(nameof(cancelledBy)); }
            if (sessionScheduleId < 1) { throw new ArgumentNullException(nameof(sessionScheduleId)); }
            return await _sessionScheduleRepository.CancelAsync(sessionScheduleId, cancelledBy);
        }

        public async Task<bool> DeleteSessionScheduleAsync(int sessionScheduleId)
        {
            if (sessionScheduleId < 1) { throw new ArgumentNullException(nameof(sessionScheduleId)); }
            return await _sessionScheduleRepository.DeleteAsync(sessionScheduleId);
        }

        #endregion

        #region Session Schedule Read Service Methods
        public async Task<List<SessionSchedule>> GetSessionSchdulesAsync(int reviewSessionId)
        {
            List<SessionSchedule> sessionScheduleList = new List<SessionSchedule>();
            var entities = await _sessionScheduleRepository.GetByReviewSessionIdAsync(reviewSessionId);
            if (entities != null && entities.Count > 0)
            {
                sessionScheduleList = entities.ToList();
            }
            return sessionScheduleList;
        }
        public async Task<SessionSchedule> GetSessionScheduleAsync(int sessionScheduleId)
        {
            SessionSchedule sessionSchedule = new SessionSchedule();
            var entities = await _sessionScheduleRepository.GetByIdAsync(sessionScheduleId);
            if (entities != null && entities.Count > 0)
            {
                sessionSchedule = entities.ToList().FirstOrDefault();
            }
            return sessionSchedule;
        }

        #endregion

        #region Review Header Service Methods

        //=================== Review Header Read Service Methods ======================================================//
        public async Task<ReviewHeader> GetReviewHeaderAsync(int reviewHeaderId)
        {
            ReviewHeader reviewHeader = new ReviewHeader();
            var entities = await _reviewHeaderRepository.GetByIdAsync(reviewHeaderId);
            if (entities != null && entities.Count > 0)
            {
                reviewHeader = entities.FirstOrDefault();
            }
            return reviewHeader;
        }

        public async Task<ReviewHeader> GetReviewHeaderAsync(int appraiseeId, int reviewSessionId)
        {
            ReviewHeader reviewHeader = new ReviewHeader();
            var entities = await _reviewHeaderRepository.GetByAppraiseeIdAndReviewSessionIdAsync(appraiseeId, reviewSessionId);
            if (entities != null && entities.Count > 0)
            {
                reviewHeader = entities.FirstOrDefault();
            }
            return reviewHeader;
        }

        //================== Review Header Write Service Methods =====================================================//
        public async Task<bool> AddReviewHeaderAsync(ReviewHeader reviewHeader)
        {
            if (reviewHeader == null) { throw new ArgumentNullException(nameof(reviewHeader)); }
            var entities = await _reviewHeaderRepository.GetByAppraiseeIdAndReviewSessionIdAsync(reviewHeader.AppraiseeId, reviewHeader.ReviewSessionId);
            if (entities != null && entities.Count > 0)
            {
                throw new Exception("Error: duplicate entry. There is an already existing appraisal record in the system for this staff and for this Session.");
            }

            return await _reviewHeaderRepository.AddAsync(reviewHeader);
        }
        public async Task<bool> UpdatePerformanceGoalAsync(int reviewHeaderId, string performanceGoal)
        {
            if (reviewHeaderId < 1) { throw new ArgumentNullException(nameof(reviewHeaderId)); }
            return await _reviewHeaderRepository.UpdateGoalAsync(reviewHeaderId, performanceGoal);
        }
        public async Task<bool> UpdateReviewHeaderStageAsync(int reviewHeaderId, int nextStageId)
        {
            if (reviewHeaderId < 1) { throw new ArgumentNullException(nameof(reviewHeaderId)); }
            if (nextStageId < 1) { throw new ArgumentNullException(nameof(nextStageId)); }

            bool IsUpdated = await _reviewHeaderRepository.UpdateStageIdAsync(reviewHeaderId, nextStageId);
            if (IsUpdated)
            {
                PmsActivityHistory activityHistory = new PmsActivityHistory();
                activityHistory.ReviewHeaderId = reviewHeaderId;
                activityHistory.ActivityTime = DateTime.UtcNow;
                switch (nextStageId)
                {
                    case 1:
                        activityHistory.ActivityDescription = $"Commenced the defining of Performance Goal(s).";
                        break;
                    case 2:
                        activityHistory.ActivityDescription = $"Completed the defining of Performance Goal(s). And commenced the Setting up of Key Performance Areas (KPAs).";
                        break;
                    case 3:
                        activityHistory.ActivityDescription = $"Completed the setting up of Key Performance Areas (KPAs). And commenced the Setting up of Competencies.";
                        break;
                    case 4:
                        activityHistory.ActivityDescription = $"Completed the setting up of Competencies. And commenced the defining of Career Development Goal(s).";
                        break;
                    case 5:
                        activityHistory.ActivityDescription = $"Completed the defining of Career Development Goal(s). And commenced submitting for review and approval.";
                        break;
                    default:
                        break;
                }
                await _pmsActivityHistoryRepository.AddAsync(activityHistory);
            }
            return IsUpdated;
        }

        #endregion

        #region Review Stage Read Service Methods
        public async Task<List<ReviewStage>> GetReviewStagesAsync()
        {
            List<ReviewStage> reviewStageList = new List<ReviewStage>();
            var entities = await _reviewStageRepository.GetAllAsync();
            if (entities != null && entities.Count > 0)
            {
                reviewStageList = entities.ToList();
            }
            return reviewStageList;
        }
        public async Task<List<ReviewStage>> GetPreviousReviewStagesAsync(int currentStageId)
        {
            List<ReviewStage> reviewStageList = new List<ReviewStage>();
            var entities = await _reviewStageRepository.GetAllPreviousAsync(currentStageId);
            if (entities != null && entities.Count > 0)
            {
                reviewStageList = entities.ToList();
            }
            return reviewStageList;
        }


        #endregion

        #region Review Metric Service Methods

        //=================== Review Metric Read Service Methods ======================================================//
        public async Task<List<ReviewMetric>> GetReviewMetricsAsync(int reviewHeaderId)
        {
            List<ReviewMetric> reviewMetrics = new List<ReviewMetric>();
            var entities = await _reviewMetricRepository.GetByReviewHeaderIdAsync(reviewHeaderId);
            if (entities != null && entities.Count > 0)
            {
                reviewMetrics = entities;
            }
            return reviewMetrics;
        }

        public async Task<List<ReviewMetric>> GetKpasAsync(int reviewHeaderId)
        {
            List<ReviewMetric> reviewMetrics = new List<ReviewMetric>();
            var entities = await _reviewMetricRepository.GetKpasByReviewHeaderIdAsync(reviewHeaderId);
            if (entities != null && entities.Count > 0)
            {
                reviewMetrics = entities;
            }
            return reviewMetrics;
        }

        public async Task<List<ReviewMetric>> GetCompetenciesAsync(int reviewHeaderId)
        {
            List<ReviewMetric> reviewMetrics = new List<ReviewMetric>();
            var entities = await _reviewMetricRepository.GetCmpsByReviewHeaderIdAsync(reviewHeaderId);
            if (entities != null && entities.Count > 0)
            {
                reviewMetrics = entities;
            }
            return reviewMetrics;
        }
        
        public async Task<ReviewMetric> GetReviewMetricAsync(int reviewMetricId)
        {
            ReviewMetric reviewMetric = new ReviewMetric();
            var entities = await _reviewMetricRepository.GetByIdAsync(reviewMetricId);
            if (entities != null && entities.Count > 0)
            {
                reviewMetric = entities.FirstOrDefault();
            }
            return reviewMetric;
        }

        //================== Review Metric Write Service Methods =====================================================//
        public async Task<bool> AddReviewMetricAsync(ReviewMetric reviewMetric)
        {
            if (reviewMetric == null) { throw new ArgumentNullException(nameof(reviewMetric)); }
            var entities = await _reviewMetricRepository.GetByMetricDescriptionAsync(reviewMetric.ReviewHeaderId, reviewMetric.ReviewMetricDescription);
            if(entities != null && entities.Count > 0)
            {
                throw new Exception("Duplicate entry error. This item has already been added.");
            }
            return await _reviewMetricRepository.AddAsync(reviewMetric);
        }

        public async Task<bool> UpdateReviewMetricAsync(ReviewMetric reviewMetric)
        {
            if (reviewMetric == null) { throw new ArgumentNullException(nameof(reviewMetric)); }
            return await _reviewMetricRepository.UpdateAsync(reviewMetric);
        }

        public async Task<bool> DeleteReviewMetricAsync(int reviewMetricId)
        {
            if (reviewMetricId < 1) { throw new ArgumentNullException(nameof(reviewMetricId)); }
            return await _reviewMetricRepository.DeleteAsync(reviewMetricId);
        }

        #endregion

        #region Competency Service Method
        public async Task<List<Competency>> GetFromCompetencyDictionaryAsync()
        {
            List<Competency> competencies = new List<Competency>();
            var entities = await _competencyRepository.GetByAllAsync();
            if (entities != null && entities.Count > 0)
            {
                competencies = entities;
            }
            return competencies;
        }

        public async Task<List<Competency>> GetFromCompetencyDictionaryByCategoryAsync(int CategoryId)
        {
            List<Competency> competencies = new List<Competency>();
            var entities = await _competencyRepository.GetByCategoryIdAsync(CategoryId);
            if (entities != null && entities.Count > 0)
            {
                competencies = entities;
            }
            return competencies;
        }

        public async Task<List<Competency>> GetFromCompetencyDictionaryByLevelAsync(int LevelId)
        {
            List<Competency> competencies = new List<Competency>();
            var entities = await _competencyRepository.GetByLevelIdAsync(LevelId);
            if (entities != null && entities.Count > 0)
            {
                competencies = entities;
            }
            return competencies;
        }

        public async Task<List<Competency>> SearchFromCompetencyDictionaryAsync(int CategoryId, int LevelId)
        {
            List<Competency> competencies = new List<Competency>();
            if(CategoryId > 0)
            {
                if(LevelId > 0)
                {
                    var category_level_entities = await _competencyRepository.GetByCategoryIdAndLevelIdAsync(CategoryId,LevelId);
                    if (category_level_entities != null && category_level_entities.Count > 0)
                    {
                        competencies = category_level_entities;
                    }
                }
                else
                {
                    var category_entities = await _competencyRepository.GetByCategoryIdAsync(CategoryId);
                    if (category_entities != null && category_entities.Count > 0)
                    {
                        competencies = category_entities;
                    }
                }
            }
            else
            {
                if (LevelId > 0)
                {
                    var level_entities = await _competencyRepository.GetByLevelIdAsync(LevelId);
                    if (level_entities != null && level_entities.Count > 0)
                    {
                        competencies = level_entities;
                    }
                }
                else
                {
                    var entities = await _competencyRepository.GetByAllAsync();
                    if (entities != null && entities.Count > 0)
                    {
                        competencies = entities;
                    }
                }
            }
            return competencies;
        }

        public async Task<Competency> GetFromCompetencyDictionaryByIdAsync(int CompetencyId)
        {
            Competency competency = new Competency();
            var entities = await _competencyRepository.GetByIdAsync(CompetencyId);
            if (entities != null && entities.Count > 0)
            {
                competency = entities.FirstOrDefault();
            }
            return competency;
        }

        public async Task<List<CompetencyCategory>> GetCompetencyCategoriesAsync()
        {
            List<CompetencyCategory> categories = new List<CompetencyCategory>();
            var entities = await _pmsSystemRepository.GetAllCompetencyCategoriesAsync();
            if (entities != null && entities.Count > 0)
            {
                categories = entities;
            }
            return categories;
        }

        public async Task<List<CompetencyLevel>> GetCompetencyLevelsAsync()
        {
            List<CompetencyLevel> levels = new List<CompetencyLevel>();
            var entities = await _pmsSystemRepository.GetAllCompetencyLevelsAsync();
            if (entities != null && entities.Count > 0)
            {
                levels = entities;
            }
            return levels;
        }


        #endregion

        #region Review CDG Service Methods
        public async Task<List<ReviewCDG>> GetCdgsAsync(int reviewHeaderId)
        {
            List<ReviewCDG> reviewCdgs = new List<ReviewCDG>();
            var entities = await _reviewCDGRepository.GetByReviewHeaderIdAsync(reviewHeaderId);
            if (entities != null && entities.Count > 0)
            {
                reviewCdgs = entities;
            }
            return reviewCdgs;
        }

        public async Task<ReviewCDG> GetReviewCdgAsync(int reviewCdgId)
        {
            ReviewCDG reviewCDG = new ReviewCDG();
            var entities = await _reviewCDGRepository.GetByIdAsync(reviewCdgId);
            if (entities != null && entities.Count > 0)
            {
                reviewCDG = entities.FirstOrDefault();
            }
            return reviewCDG;
        }

        public async Task<bool> AddReviewCdgAsync(ReviewCDG reviewCdg)
        {
            if (reviewCdg == null) { throw new ArgumentNullException(nameof(reviewCdg)); }
            return await _reviewCDGRepository.AddAsync(reviewCdg);
        }

        public async Task<bool> UpdateReviewCdgAsync(ReviewCDG reviewCdg)
        {
            if (reviewCdg == null) { throw new ArgumentNullException(nameof(reviewCdg)); }
            return await _reviewCDGRepository.UpdateAsync(reviewCdg);
        }

        public async Task<bool> DeleteReviewCdgAsync(int reviewCdgId)
        {
            if (reviewCdgId < 1) { throw new ArgumentNullException(nameof(reviewCdgId)); }
            return await _reviewCDGRepository.DeleteAsync(reviewCdgId);
        }

        #endregion

        #region Review Submission Service Methods

        public async Task<bool> AddReviewSubmissionAsync(ReviewSubmission reviewSubmission)
        {
            if (reviewSubmission == null) { throw new ArgumentNullException(nameof(reviewSubmission)); }
            return await _reviewSubmissionRepository.AddAsync(reviewSubmission);
        }
        public async Task<ReviewSubmission> GetReviewSubmissionByIdAsync(int reviewSubmissionId)
        {
            ReviewSubmission reviewSubmission = new ReviewSubmission();
            var entities = await _reviewSubmissionRepository.GetByIdAsync(reviewSubmissionId);
            if (entities != null && entities.Count > 0)
            {
                reviewSubmission = entities.FirstOrDefault();
            }
            return reviewSubmission;
        }
        public async Task<List<ReviewSubmission>> GetReviewSubmissionsByApproverIdAsync(int reviewerId)
        {
            List<ReviewSubmission> reviewSubmissions = new List<ReviewSubmission>();
            var entities = await _reviewSubmissionRepository.GetByReviewerIdAsync(reviewerId);
            if (entities != null && entities.Count > 0)
            {
                reviewSubmissions = entities;
            }
            return reviewSubmissions;
        }
        public async Task<List<ReviewSubmission>> GetReviewSubmissionsByReviewHeaderIdAsync(int reviewHeaderId)
        {
            List<ReviewSubmission> reviewSubmissions = new List<ReviewSubmission>();
            var entities = await _reviewSubmissionRepository.GetByReviewHeaderIdAsync(reviewHeaderId);
            if (entities != null && entities.Count > 0)
            {
                reviewSubmissions = entities;
            }
            return reviewSubmissions;
        }

        #endregion

        #region PMS Utility Service Methods
        public async Task<MoveToNextStageModel> ValidateMoveRequestAsync(int reviewHeaderId)
        {
            MoveToNextStageModel model = new MoveToNextStageModel();
            model.ErrorMessages = new List<string>();
            ReviewHeader reviewHeader = new ReviewHeader();
            ReviewSession reviewSession = new ReviewSession();
            //int nextStepId = 
            var entities = await _reviewHeaderRepository.GetByIdAsync(reviewHeaderId);
            if (entities != null && entities.Count > 0)
            {
                reviewHeader = entities.FirstOrDefault();
                model.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                model.AppraiseeID = reviewHeader.AppraiseeId;
                model.CurrentStageID = reviewHeader.ReviewStageId;
                model.ReviewSessionID = reviewHeader.ReviewSessionId;

                switch (model.CurrentStageID)
                {
                    case 1:
                        if (!string.IsNullOrWhiteSpace(reviewHeader.PerformanceGoal))
                        {
                            model.IsQualifiedToMove = true;
                            model.NextStageID = 2;
                        }
                        else
                        {
                            model.ErrorMessages.Add("You have not defined any Performance Goal for this performance period. Please defined at least one Performance Goal before attempting to move to the next step.");
                            model.IsQualifiedToMove = false;
                        }
                        break;
                    case 2:
                        var revision_entities = await _reviewSessionRepository.GetByIdAsync(reviewHeader.ReviewSessionId);
                        reviewSession = revision_entities.ToList().FirstOrDefault();
                        decimal required_Kpa_Weightage = reviewSession.TotalKpaScore;
                        decimal cumulative_kpa_weightage = await _reviewMetricRepository.GetTotalKpaWeightageByReviewHeaderIdAsync(model.ReviewHeaderID);
                        if (required_Kpa_Weightage == cumulative_kpa_weightage)
                        {
                            model.IsQualifiedToMove = true;
                            model.NextStageID = 3;
                        }
                        else if (required_Kpa_Weightage > cumulative_kpa_weightage)
                        {
                            model.ErrorMessages.Add($"The recommended total weightage of all KPAs is {required_Kpa_Weightage}. But yours is {cumulative_kpa_weightage} which falls short of the requirement. You may need to increase the weightages of some of your KPAs to get the total to {required_Kpa_Weightage}.");
                            model.IsQualifiedToMove = false;
                        }
                        else if (required_Kpa_Weightage < cumulative_kpa_weightage)
                        {
                            model.ErrorMessages.Add($"The recommended total weightage of all KPAs is {required_Kpa_Weightage}. But yours is {cumulative_kpa_weightage} which exceeds the requirement. You may need to reduce the weightages of some of your KPAs in order to get the total to {required_Kpa_Weightage}.");
                            model.IsQualifiedToMove = false;
                        }
                        else
                        {
                            model.ErrorMessages.Add($"The recommended total weightage of all KPAs is {required_Kpa_Weightage}. But yours is {cumulative_kpa_weightage} you may need to make the necessary changes to the weightages of some of your KPAs in order to get the total to {required_Kpa_Weightage}.");
                            model.IsQualifiedToMove = false;
                        }
                        break;
                    case 3:
                        var rev_entities = await _reviewSessionRepository.GetByIdAsync(reviewHeader.ReviewSessionId);
                        reviewSession = rev_entities.ToList().FirstOrDefault();
                        int min_cmp_count = reviewSession.MinNoOfCompetencies;
                        int max_cmp_count = reviewSession.MaxNoOfCompetencies;
                        int cmp_count = await _reviewMetricRepository.GetCmpCountByReviewHeaderIdAsync(model.ReviewHeaderID);
                        if (cmp_count >= min_cmp_count && cmp_count <= max_cmp_count)
                        {
                            decimal required_cmp_Weightage = reviewSession.TotalCompetencyScore;
                            decimal cumulative_cmp_weightage = await _reviewMetricRepository.GetTotalCmpWeightageByReviewHeaderIdAsync(model.ReviewHeaderID);
                            if (required_cmp_Weightage == cumulative_cmp_weightage)
                            {
                                model.IsQualifiedToMove = true;
                                model.NextStageID = 4;
                            }
                            else if (required_cmp_Weightage > cumulative_cmp_weightage)
                            {
                                model.ErrorMessages.Add($"The recommended total weightage of all Competencies is {required_cmp_Weightage}. But yours is {cumulative_cmp_weightage} which falls short of the requirement. You may need to ensure all your competencies have the recommended weightages to bring the total to {required_cmp_Weightage}.");
                                model.IsQualifiedToMove = false;
                            }
                            else if (required_cmp_Weightage < cumulative_cmp_weightage)
                            {
                                model.ErrorMessages.Add($"The recommended total weightage of all Competencies is {required_cmp_Weightage}. But yours is {cumulative_cmp_weightage} which exceeds the requirement. You may need to ensure all your competencies have the recommended weightages to bring the total to {required_cmp_Weightage}.");
                                model.IsQualifiedToMove = false;
                            }
                            else
                            {
                                model.ErrorMessages.Add($"The recommended total weightage of all Competencies is {required_cmp_Weightage}. But yours is {cumulative_cmp_weightage}. You may want to ensure all your competencies have the recommended weightages to bring the total to {required_cmp_Weightage}.");
                                model.IsQualifiedToMove = false;
                            }
                        }
                        else if (cmp_count < min_cmp_count)
                        {
                            model.ErrorMessages.Add($"The recommended minimum number of Competencies is {min_cmp_count}. But yours is {cmp_count} which is less than the required number. You may need to add more Competencies to get the total number to at least {min_cmp_count}.");
                            model.IsQualifiedToMove = false;
                        }
                        else if (cmp_count > max_cmp_count)
                        {
                            model.ErrorMessages.Add($"The recommended maximum number of Competencies is {max_cmp_count}. But yours is {cmp_count} which is more than the required number. You may need to delete some Competencies to get the total number to {max_cmp_count} at most.");
                            model.IsQualifiedToMove = false;
                        }
                        else
                        {
                            model.ErrorMessages.Add($"The recommended minimum number of Competencies is {min_cmp_count}, and the recommended maximum number is {max_cmp_count}. Make sure your total number of e Competencies falls within this range.");
                            model.IsQualifiedToMove = false;
                        }
                        break;
                    case 4:
                        List<ReviewCDG> reviewCdgList = new List<ReviewCDG>();
                        var reviewCDG_entities = await _reviewCDGRepository.GetByReviewHeaderIdAsync(reviewHeaderId);
                        if(reviewCDG_entities != null && reviewCDG_entities.Count > 0)
                        {
                            model.IsQualifiedToMove = true;
                            model.NextStageID = 5;
                        }
                        else
                        {
                            model.ErrorMessages.Add("You have not defined any Career Development Goal for this performance period. Please defined at least one Career Development Goal before attempting to move to the next step.");
                            model.IsQualifiedToMove = false;
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                model.ErrorMessages.Add("No appraisal record was found for this appraisal session. Please ensure you have created at least one performance goal before attempting to move to the next step. Thank you. ");
                model.IsQualifiedToMove = false;
            }
            return model;
        }
        public async Task<List<PmsActivityHistory>> GetPmsActivityHistory(int reviewHeaderId)
        {
            List<PmsActivityHistory> activityList = new List<PmsActivityHistory>();
            var entities = await _pmsActivityHistoryRepository.GetByReviewHeaderIdAsync(reviewHeaderId);
            if (entities != null && entities.Count > 0)
            {
                activityList = entities.ToList();
            }
            return activityList;
        }
        public async Task<bool> AddPmsActivityHistoryAsync(PmsActivityHistory pmsActivityHistory)
        {
            if (pmsActivityHistory != null)
            {
                return await _pmsActivityHistoryRepository.AddAsync(pmsActivityHistory);
            }
            return false;
        }

        #endregion

        #region Performance Settings Read Service Methods
        public async Task<List<ReviewType>> GetReviewTypesAsync()
        {
            List<ReviewType> reviewTypeList = new List<ReviewType>();
            var entities = await _performanceSettingsRepository.GetAllReviewTypesAsync();
            if (entities != null && entities.Count > 0)
            {
                reviewTypeList = entities.ToList();
            }
            return reviewTypeList;
        }
        public async Task<List<ApprovalRole>> GetApprovalRolesAsync()
        {
            List<ApprovalRole> approvalRoleList = new List<ApprovalRole>();
            var entities = await _performanceSettingsRepository.GetAllApprovalRolesAsync();
            if (entities != null && entities.Count > 0)
            {
                approvalRoleList = entities.ToList();
            }
            return approvalRoleList;
        }
        #endregion


    }
}
