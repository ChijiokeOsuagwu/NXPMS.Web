using Microsoft.Extensions.DependencyInjection;
using NXPMS.Base.Repositories.EmployeeRecordRepositories;
using NXPMS.Base.Repositories.GlobalSettingsRepositories;
using NXPMS.Base.Repositories.PMSRepositories;
using NXPMS.Base.Repositories.SecurityRepositories;
using NXPMS.Base.Services;
using NXPMS.Data.Repositories.EmployeeRecordRepositories;
using NXPMS.Data.Repositories.GlobalSettingsRepositories;
using NXPMS.Data.Repositories.PMSRepositories;
using NXPMS.Data.Repositories.SecurityRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Extensions
{
    public static class ServicesConfiguration
    {
        public static void ConfigureRepositories(this IServiceCollection services)
        {
            services.AddScoped<ILocationsRepository, LocationsRepository>();
            services.AddScoped<IEmployeesRepository, EmployeesRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IUnitRepository, UnitRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<IStateRepository, StateRepository>();
            services.AddScoped<IEmployeeCategoryRepository, EmployeeCategoryRepository>();
            services.AddScoped<IEmployeeTypeRepository, EmployeeTypeRepository>();
            services.AddScoped<IUserPermissionRepository, UserPermissionRepository>();
            services.AddScoped<ISystemRepository, SystemRepository>();
            services.AddScoped<IEmployeeReportRepository, EmployeeReportRepository>();
            services.AddScoped<IPerformanceYearRepository, PerformanceYearRepository>();
            services.AddScoped<IReviewSessionRepository, ReviewSessionRepository>();
            services.AddScoped<IPerformanceSettingsRepository, PerformanceSettingsRepository>();
            services.AddScoped<IReviewGradeRepository, ReviewGradeRepository>();
            services.AddScoped<IGradeHeaderRepository, GradeHeaderRepository>();
            services.AddScoped<IAppraisalGradeRepository, AppraisalGradeRepository>();
            services.AddScoped<ISessionScheduleRepository, SessionScheduleRepository>();
            services.AddScoped<IReviewHeaderRepository, ReviewHeaderRepository>();
            services.AddScoped<IReviewStageRepository, ReviewStageRepository>();
            services.AddScoped<IPmsActivityHistoryRepository, PmsActivityHistoryRepository>();
            services.AddScoped<IReviewMetricRepository, ReviewMetricRepository>();
            services.AddScoped<ICompetencyRepository, CompetencyRepository>();
            services.AddScoped<IPmsSystemRepository, PmsSystemRepository>();
            services.AddScoped<IReviewCDGRepository, ReviewCDGRepository>();
            services.AddScoped<IReviewSubmissionRepository, ReviewSubmissionRepository>();
            services.AddScoped<IReviewMessageRepository, ReviewMessageRepository>();
            services.AddScoped<IApprovalRoleRepository, ApprovalRoleRepository>();
            services.AddScoped<IReviewApprovalRepository, ReviewApprovalRepository>();
            services.AddScoped<IReviewResultRepository, ReviewResultRepository>();
        }

        public static void ConfigureServiceManagers(this IServiceCollection services)
        {
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<IEmployeeRecordService, EmployeeRecordService>();
            services.AddScoped<IGlobalSettingsService, GlobalSettingsService>();
            services.AddScoped<IPerformanceService, PerformanceService>();
            //services.AddScoped<IBaseModelService, BaseModelService>();
        }
    }
}
