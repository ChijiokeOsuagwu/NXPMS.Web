using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using NXPMS.Web.Extensions;
using static NXPMS.Base.Services.SecurityService;

namespace NXPMS.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("NxpmsConnection");
            services.AddTransient<NpgsqlConnection>(e => new NpgsqlConnection(connectionString));
            services.AddAuthentication(SecurityKeys.NxpmsCookieAuth).AddCookie(SecurityKeys.NxpmsCookieAuth, options =>
            {
                options.Cookie.Name = SecurityKeys.NxpmsCookieAuth;
                options.LoginPath = "/Security/Login";
                options.LogoutPath = "/Security/Logout";
                options.AccessDeniedPath = "/Security/AccessDenied";
            });

            services.AddControllersWithViews();
            services.AddRazorPages();
            services.ConfigureServiceManagers();
            services.ConfigureRepositories();
            services.AddSingleton<DataProtectionEncryptionStrings>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
