using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using NXPMS.Base.Models.GlobalSettingsModels;
using NXPMS.Base.Repositories.GlobalSettingsRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NXPMS.Data.Repositories.GlobalSettingsRepositories
{
    public class SystemRepository : ISystemRepository
    {
        public IConfiguration _config { get; }
        public SystemRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<IList<SystemApplication>> GetAllApplicationsAsync()
        {
            List<SystemApplication> applicationsList = new List<SystemApplication>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = "SELECT app_cd, app_ds FROM public.sysutlaps WHERE (app_cd != 'SYS') ORDER BY app_ds;";
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    applicationsList.Add(new SystemApplication()
                    {
                        Code = reader["app_cd"] == DBNull.Value ? string.Empty : (reader["app_cd"]).ToString(),
                        Description = reader["app_ds"] == DBNull.Value ? string.Empty : reader["app_ds"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return applicationsList;
        }
        public async Task<List<Industry>> GetAllIndustriesAsync()
        {
            List<Industry> industryList = new List<Industry>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = "SELECT sys_ind_id, sys_ind_ds FROM public.syscfginds; ";
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    industryList.Add(new Industry()
                    {
                        Id = reader["sys_ind_id"] == DBNull.Value ? 0 : (int)reader["sys_ind_id"],
                        Description = reader["sys_ind_ds"] == DBNull.Value ? string.Empty : reader["sys_ind_ds"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return industryList;
        }

    }
}
