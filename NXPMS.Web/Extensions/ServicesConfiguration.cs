using Microsoft.Extensions.DependencyInjection;
using NXPMS.Base.Repositories.EmployeeRecordRepositories;
using NXPMS.Base.Repositories.GlobalSettingsRepositories;
using NXPMS.Base.Repositories.SecurityRepositories;
using NXPMS.Base.Services;
using NXPMS.Data.Repositories.EmployeeRecordRepositories;
using NXPMS.Data.Repositories.GlobalSettingsRepositories;
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
        }

        public static void ConfigureServiceManagers(this IServiceCollection services)
        {
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<IEmployeeRecordService, EmployeeRecordService>();
            services.AddScoped<IGlobalSettingsService, GlobalSettingsService>();
            //services.AddScoped<IBaseModelService, BaseModelService>();
        }
    }
}
