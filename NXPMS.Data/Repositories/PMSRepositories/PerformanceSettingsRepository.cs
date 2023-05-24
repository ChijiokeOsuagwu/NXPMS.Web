using Microsoft.Extensions.Configuration;
using Npgsql;
using NXPMS.Base.Models.PMSModels;
using NXPMS.Base.Repositories.PMSRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NXPMS.Data.Repositories.PMSRepositories
{
    public class PerformanceSettingsRepository : IPerformanceSettingsRepository
    {
        public IConfiguration _config { get; }
        public PerformanceSettingsRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<IList<ReviewType>> GetAllReviewTypesAsync()
        {
            List<ReviewType> reviewTypesList = new List<ReviewType>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT rvw_typ_id, rvw_typ_nm FROM public.pmsrvwtyps ");
            sb.Append("ORDER BY rvw_typ_id;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    reviewTypesList.Add(new ReviewType()
                    {
                        ReviewTypeId = reader["rvw_typ_id"] == DBNull.Value ? 0 : (int)(reader["rvw_typ_id"]),
                        ReviewTypeName = reader["rvw_typ_nm"] == DBNull.Value ? string.Empty : (reader["rvw_typ_nm"]).ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return reviewTypesList;
        }

        public async Task<IList<ApprovalRole>> GetAllApprovalRolesAsync()
        {
            List<ApprovalRole> approvalRolesList = new List<ApprovalRole>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT aprv_rl_id, aprv_rl_nm FROM public.pmsaprvrls ");
            sb.Append("ORDER BY aprv_rl_nm;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    approvalRolesList.Add(new ApprovalRole()
                    {
                        ApprovalRoleId = reader["aprv_rl_id"] == DBNull.Value ? 0 : (int)(reader["aprv_rl_id"]),
                        ApprovalRoleName = reader["aprv_rl_nm"] == DBNull.Value ? string.Empty : (reader["aprv_rl_nm"]).ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return approvalRolesList;
        }
    }
}
