﻿using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using NXPMS.Base.Models.PMSModels;
using NXPMS.Base.Repositories.PMSRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NXPMS.Data.Repositories.PMSRepositories
{
    public class PmsActivityHistoryRepository: IPmsActivityHistoryRepository
    {
        public IConfiguration _config { get; }
        public PmsActivityHistoryRepository(IConfiguration configuration)
        {
            _config = configuration;
        }
        #region PMS Activity History Read Action Methods
        public async Task<IList<PmsActivityHistory>> GetByReviewHeaderIdAsync(int reviewHeaderId)
        {
            List<PmsActivityHistory> activityList = new List<PmsActivityHistory>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT a.pms_hst_id, a.pms_act_ds, a.pms_act_dt, a.rvw_hdr_id ");
            sb.Append("FROM public.pmsloghsts a ");
            sb.Append("WHERE (a.rvw_hdr_id = @rvw_hdr_id); ");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_hdr_id.Value = reviewHeaderId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    activityList.Add(new PmsActivityHistory()
                    {
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ActivityId = reader["pms_hst_id"] == DBNull.Value ? 0 : (int)reader["pms_hst_id"],
                        ActivityDescription = reader["pms_act_ds"] == DBNull.Value ? string.Empty : reader["pms_act_ds"].ToString(),
                        ActivityTime = reader["pms_act_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["pms_act_dt"],
                    });
                }
            }
            await conn.CloseAsync();
            return activityList;
        }
        #endregion

        #region Review Header Write Action Methods
        public async Task<bool> AddAsync(PmsActivityHistory activityHistory)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.pmsloghsts(pms_act_ds, pms_act_dt, ");
            sb.Append("rvw_hdr_id) VALUES (@pms_act_ds, @pms_act_dt, @rvw_hdr_id); ");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                    var pms_act_ds = cmd.Parameters.Add("@pms_act_ds", NpgsqlDbType.Text);
                    var pms_act_dt = cmd.Parameters.Add("@pms_act_dt", NpgsqlDbType.TimestampTz);
                    cmd.Prepare();
                    rvw_hdr_id.Value = activityHistory.ReviewHeaderId;
                    pms_act_ds.Value = activityHistory.ActivityDescription;
                    pms_act_dt.Value = activityHistory.ActivityTime;

                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return rows > 0;
        }

        #endregion
    }
}
