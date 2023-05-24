using NXPMS.Base.Enums;
using NXPMS.Base.Models.PMSModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NXPMS.Base.Services
{
    public interface IPerformanceService
    {
        #region Performance Year Service Methods
        Task<bool> AddPerformanceYearAsync(PerformanceYear performanceYear);
        Task<bool> DeletePerformanceYearAsync(int performanceYearId);
        Task<bool> EditPerformanceYearAsync(PerformanceYear performanceYear);
        Task<PerformanceYear> GetPerformanceYearAsync(int PerformanceYearId);
        Task<List<PerformanceYear>> GetPerformanceYearsAsync();
        #endregion

        #region Review Session Service Methods
        Task<List<ReviewSession>> GetReviewSessionsAsync();
        Task<List<ReviewSession>> GetReviewSessionsAsync(int PerformanceYearId);
        Task<ReviewSession> GetReviewSessionAsync(int ReviewSessionId);
        Task<bool> AddReviewSessionAsync(ReviewSession reviewSession);
        Task<bool> EditReviewSessionAsync(ReviewSession reviewSession);
        Task<bool> DeleteReviewSessionAsync(int ReviewSessionId);
        #endregion

        #region Grade Header Service Methods
        Task<List<GradeHeader>> GetGradeHeadersAsync();
        Task<GradeHeader> GetGradeHeaderAsync(int GradeHeaderId);
        Task<bool> AddGradeHeaderAsync(GradeHeader gradeHeader);
        Task<bool> EditGradeHeaderAsync(GradeHeader gradeHeader);
        Task<bool> DeleteGradeHeaderAsync(int gradeHeaderId);
        #endregion

        #region Review Grade Details Service Methods

        Task<List<ReviewGrade>> GetReviewGradesAsync();
        Task<List<ReviewGrade>> GetReviewGradesAsync(int gradeHeaderId);
        Task<ReviewGrade> GetReviewGradeAsync(int ReviewGradeId);
        Task<List<ReviewGrade>> GetPerformanceGradesAsync(int gradeHeaderId);
        Task<List<ReviewGrade>> GetCompetencyGradesAsync(int gradeHeaderId);
        Task<bool> AddReviewGradeAsync(ReviewGrade reviewGrade);
        Task<bool> EditReviewGradeAsync(ReviewGrade reviewGrade);
        Task<bool> DeleteReviewGradeAsync(int reviewGradeId);

        #endregion

        #region Appraisal Grade Service Methods

        Task<List<AppraisalGrade>> GetAppraisalGradesAsync();
        Task<List<AppraisalGrade>> GetAppraisalGradesAsync(int reviewSessionId);
        Task<AppraisalGrade> GetAppraisalGradeAsync(int AppraisalGradeId);
        Task<List<AppraisalGrade>> GetAppraisalPerformanceGradesAsync(int reviewSessionId);
        Task<List<AppraisalGrade>> GetAppraisalCompetencyGradesAsync(int reviewSessionId);
        Task<bool> AddAppraisalGradeAsync(AppraisalGrade appraisalGrade);
        Task<bool> CopyAppraisalGradeAsync(string copiedBy, int reviewSessionId, int gradeTemplateId, ReviewGradeType? gradeType = null);
        Task<bool> EditAppraisalGradeAsync(AppraisalGrade appraisalGrade);
        Task<bool> DeleteAppraisalGradeAsync(int appraisalGradeId);

        #endregion

        #region Session Schedule Service Method
        Task<bool> AddSessionScheduleAsync(SessionSchedule sessionSchedule);
        Task<bool> CancelSessionScheduleAsync(int sessionScheduleId, string cancelledBy);
        Task<bool> DeleteSessionScheduleAsync(int sessionScheduleId);

        Task<List<SessionSchedule>> GetSessionSchdulesAsync(int reviewSessionId);
        Task<SessionSchedule> GetSessionScheduleAsync(int sessionScheduleId);
        #endregion

        #region Review Header Service Method
        Task<ReviewHeader> GetReviewHeaderAsync(int reviewHeaderId);
        Task<ReviewHeader> GetReviewHeaderAsync(int appraiseeId, int reviewSessionId);
        Task<bool> AddReviewHeaderAsync(ReviewHeader reviewHeader);
        Task<bool> UpdatePerformanceGoalAsync(int reviewHeaderId, string performanceGoal);
        Task<bool> UpdateReviewHeaderStageAsync(int reviewHeaderId, int nextStageId);
        #endregion

        #region Review Stage Service Methods
        Task<List<ReviewStage>> GetReviewStagesAsync();
        Task<List<ReviewStage>> GetPreviousReviewStagesAsync(int currentStageId);

        #endregion

        #region Review Metric Service Method
        Task<List<ReviewMetric>> GetReviewMetricsAsync(int reviewHeaderId);
        Task<List<ReviewMetric>> GetKpasAsync(int reviewHeaderId);
        Task<List<ReviewMetric>> GetCompetenciesAsync(int reviewHeaderId);
        Task<ReviewMetric> GetReviewMetricAsync(int reviewMetricId);
        Task<bool> AddReviewMetricAsync(ReviewMetric reviewMetric);
        Task<bool> UpdateReviewMetricAsync(ReviewMetric reviewMetric);
        Task<bool> DeleteReviewMetricAsync(int reviewMetricId);

        #endregion

        #region Competency Service Methods
        Task<List<Competency>> GetFromCompetencyDictionaryAsync();
        Task<Competency> GetFromCompetencyDictionaryByIdAsync(int CompetencyId);
        Task<List<Competency>> GetFromCompetencyDictionaryByCategoryAsync(int CategoryId);
        Task<List<Competency>> GetFromCompetencyDictionaryByLevelAsync(int LevelId);
        Task<List<Competency>> SearchFromCompetencyDictionaryAsync(int CategoryId, int LevelId);
        Task<List<CompetencyCategory>> GetCompetencyCategoriesAsync();
        Task<List<CompetencyLevel>> GetCompetencyLevelsAsync();

        #endregion

        #region Review CDG Service Methods
        Task<List<ReviewCDG>> GetCdgsAsync(int reviewHeaderId);
        Task<ReviewCDG> GetReviewCdgAsync(int reviewCdgId);
        Task<bool> AddReviewCdgAsync(ReviewCDG reviewCdg);
        Task<bool> UpdateReviewCdgAsync(ReviewCDG reviewCdg);
        Task<bool> DeleteReviewCdgAsync(int reviewCdgId);

        #endregion

        #region Review Submission Service Methods
        Task<bool> AddReviewSubmissionAsync(ReviewSubmission reviewSubmission);
        Task<ReviewSubmission> GetReviewSubmissionByIdAsync(int reviewSubmissionId);
        Task<List<ReviewSubmission>> GetReviewSubmissionsByApproverIdAsync(int reviewerId);
        Task<List<ReviewSubmission>> GetReviewSubmissionsByReviewHeaderIdAsync(int reviewHeaderId);
        #endregion

        #region PMS Utility Service Methods
        Task<MoveToNextStageModel> ValidateMoveRequestAsync(int reviewHeaderId);
        Task<List<PmsActivityHistory>> GetPmsActivityHistory(int reviewHeaderId);
        Task<bool> AddPmsActivityHistoryAsync(PmsActivityHistory pmsActivityHistory);

        #endregion

        #region Performance Settings Service Methods
        Task<List<ReviewType>> GetReviewTypesAsync();
        Task<List<ApprovalRole>> GetApprovalRolesAsync();

        #endregion
    }
}