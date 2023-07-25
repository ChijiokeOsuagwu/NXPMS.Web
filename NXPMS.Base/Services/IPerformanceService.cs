﻿using NXPMS.Base.Enums;
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
        Task<AppraisalGrade> GetAppraisalGradeAsync(int reviewSessionId, ReviewGradeType gradeType, decimal gradeScore);

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
        Task<ReviewSchedule> GetEmployeePerformanceScheduleAsync(int reviewSessionId, int employeeId);
        #endregion

        #region Review Header Service Method
        Task<ReviewHeader> GetReviewHeaderAsync(int reviewHeaderId);
        Task<ReviewHeader> GetReviewHeaderAsync(int appraiseeId, int reviewSessionId);
        Task<bool> AddReviewHeaderAsync(ReviewHeader reviewHeader);
        Task<bool> UpdatePerformanceGoalAsync(int reviewHeaderId, string performanceGoal, int appraiserId);
        Task<bool> UpdateReviewHeaderStageAsync(int reviewHeaderId, int nextStageId);
        Task<bool> UpdateAppraiseeFlagAsync(int reviewHeaderId, bool isFlagged, string flaggedBy);
        Task<bool> UpdateFeedbackAsync(int reviewHeaderId, string feedbackProblems, string feedbackSolutions);
        Task<bool> AddAppraisalRecommendationAsync(ReviewHeaderRecommendation model);
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
        Task<List<ReviewCDG>> GetReviewCdgsAsync(int reviewHeaderId);
        Task<ReviewCDG> GetReviewCdgAsync(int reviewCdgId);
        Task<bool> AddReviewCdgAsync(ReviewCDG reviewCdg);
        Task<bool> UpdateReviewCdgAsync(ReviewCDG reviewCdg);
        Task<bool> DeleteReviewCdgAsync(int reviewCdgId);

        #endregion

        #region Review Submission Service Methods
        Task<bool> AddReviewSubmissionAsync(ReviewSubmission reviewSubmission);
        Task<bool> UpdateReviewSubmissionAsync(int reviewSubmissionId);
        Task<bool> DeleteReviewSubmissionAsync(int reviewSubmissionId);
        Task<ReviewSubmission> GetReviewSubmissionByIdAsync(int reviewSubmissionId);
        Task<List<ReviewSubmission>> GetReviewSubmissionsByApproverIdAsync(int reviewerId, int? reviewSessionId = null);
        Task<List<ReviewSubmission>> GetReviewSubmissionsByReviewHeaderIdAsync(int reviewHeaderId, int? submissionPurposeId = null, int? submittedToEmployeeId = null);
        #endregion

        #region Review Message Service Methods
        Task<List<ReviewMessage>> GetReviewMessagesAsync(int reviewHeaderId);
        Task<ReviewMessage> GetReviewMessageAsync(int reviewMessageId);
        Task<bool> AddReviewMessageAsync(ReviewMessage reviewMessage);
        Task<bool> UpdateReviewMessageAsync(ReviewMessage reviewMessage);
        Task<bool> DeleteReviewMessageAsync(int reviewMessageId);
        #endregion

        #region Approval Roles Service Methods
        Task<List<ApprovalRole>> GetApprovalRolesAsync();
        Task<ApprovalRole> GetApprovalRoleAsync(int ApprovalRoleId);
        Task<bool> AddApprovalRoleAsync(ApprovalRole approvalRole);
        Task<bool> EditApprovalRoleAsync(ApprovalRole approvalRole);
        Task<bool> DeleteApprovalRoleAsync(int approvalRoleId);
        #endregion

        #region Review Approval Service Methods
        Task<bool> ReturnContractToAppraisee(int nextStageId, int reviewSubmissionId, ReviewMessage reviewMessage = null);
        Task<bool> ApproveContractToAppraisee(ReviewApproval reviewApproval, int? reviewSubmissionId);
        Task<bool> AcceptContractByAppraisee(int reviewHeaderId);
        Task<bool> AcceptEvaluationByAppraisee(int reviewHeaderId);
        Task<List<ReviewApproval>> GetReviewApprovalsAsync(int reviewHeaderId);
        #endregion

        #region Review Result Service Methods
        //===================== Review Result Read Service Methods =========================================//
        Task<List<ReviewResult>> GetInitialReviewResultKpasAsync(int reviewHeaderId, int appraiserId);
        Task<List<ReviewResult>> GetInitialReviewResultAsync(int reviewHeaderId, int appraiserId, int reviewMetricId);

        Task<List<ReviewResult>> GetInitialReviewResultCmpsAsync(int reviewHeaderId, int appraiserId);
        Task<List<ReviewResult>> GetReviewResultByAppraiserIdAndReviewMetricIdAsync(int reviewHeaderId, int appraiserId, int reviewMetricId);
        Task<List<ReviewResult>> GetReviewResultByAppraiserIdAndReviewMetricTypeIdAsync(int reviewHeaderId, int appraiserId, int? reviewMetricTypeId = null);

        Task<List<int>> GetAppraisersAsync(int reviewHeaderId);
        Task<List<AppraiserDetail>> GetAppraiserDetailsAsync(int reviewHeaderId);
        Task<ScoreSummary> GetScoreSummaryAsync(int reviewHeaderId, int appraiserId);
        Task<List<ResultSummary>> GetResultSummaryForReportsAsync(int reportToId, int reviewSessionId, int? appraiseeId = null);

        Task<List<ResultSummary>> GetResultSummaryByReviewSessionIdAsync(int reviewSessionId);
        Task<List<ResultSummary>> GetResultSummaryByReviewSessionIdAndDepartmentCodeAsync(int reviewSessionId, string departmentCode);
        Task<List<ResultSummary>> GetResultSummaryByReviewSessionIdAndUnitCodeAsync(int reviewSessionId, string unitCode);
        Task<List<ResultSummary>> GetResultSummaryByReviewSessionIdAndAppraiseeNameAsync(int reviewSessionId, string appraiseeName);


        //===================== Review Result Write Service Methods =========================================//
        Task<bool> AddReviewResultAsync(ReviewResult reviewResult);
        Task<bool> UpdateReviewResultAsync(ReviewResult reviewResult);

        //==================== Result Summary Service Methods ================================================//
        Task<bool> AddResultSummaryAsync(ResultSummary resultSummary);
        Task<bool> UploadResults(int reviewHeaderId);

        //=================== Result Details Service Methods ==================================================//
        Task<List<ResultDetail>> GetPrincipalResultDetailAsync(int reviewSessionId, int? locationId = null, string departmentCode = null, string unitCode = null);

        #endregion

        #region PMS Utility Service Methods
        Task<MoveToNextStageModel> ValidateMoveRequestAsync(int reviewHeaderId, int? appraiserId = null);
        Task<List<PmsActivityHistory>> GetPmsActivityHistory(int reviewHeaderId);
        Task<bool> AddPmsActivityHistoryAsync(PmsActivityHistory pmsActivityHistory);

        #endregion

        #region Performance Settings Service Methods
        Task<List<ReviewType>> GetReviewTypesAsync();
        Task<List<AppraisalRecommendation>> GetAppraisalRecommendationsAsync();
        #endregion
    }
}