﻿using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using NXPMS.Base.Enums;
using NXPMS.Base.Models.PMSModels;
using NXPMS.Base.Repositories.PMSRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NXPMS.Data.Repositories.PMSRepositories
{
    public class ReviewSessionRepository : IReviewSessionRepository
    {
        public IConfiguration _config { get; }
        public ReviewSessionRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region Review Session Read Action Methods
        public async Task<IList<ReviewSession>> GetAllAsync()
        {
            List<ReviewSession> reviewSessionsList = new List<ReviewSession>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.rvw_sxn_id, s.rvw_sxn_nm, s.rvw_yr_id, s.rvw_typ_id, ");
            sb.Append("s.rvw_str_dt, s.rvw_ndt_dt, s.min_cmp_no, s.max_cmp_no, ");
            sb.Append("s.ttl_cmp_scr, s.ttl_kpa_scr, s.ttl_cmb_scr, s.rvw_ctt, ");
            sb.Append("s.rvw_ctb, s.rvw_mdt, s.rvw_mdb, y.pms_yr_nm, ");
            sb.Append("t.rvw_typ_nm, s.is_ctv FROM public.pmsrvwsxns s ");
            sb.Append("INNER JOIN public.pmssttyrs y ON y.pms_yr_id = s.rvw_yr_id ");
            sb.Append("INNER JOIN public.pmsrvwtyps t ON t.rvw_typ_id = s.rvw_typ_id ");
            sb.Append("ORDER BY s.rvw_ndt_dt DESC;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    reviewSessionsList.Add(new ReviewSession()
                    {
                        Id = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)(reader["rvw_sxn_id"]),
                        Name = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : (reader["rvw_sxn_nm"]).ToString(),
                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)(reader["rvw_yr_id"]),
                        ReviewYearName = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : (reader["pms_yr_nm"]).ToString(),
                        ReviewTypeId = reader["rvw_typ_id"] == DBNull.Value ? 0 : (int)(reader["rvw_typ_id"]),
                        ReviewTypeName = reader["rvw_typ_nm"] == DBNull.Value ? string.Empty : (reader["rvw_typ_nm"]).ToString(),
                        StartDate = reader["rvw_str_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rvw_str_dt"],
                        EndDate = reader["rvw_ndt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rvw_ndt_dt"],
                        MinNoOfCompetencies = reader["min_cmp_no"] == DBNull.Value ? 0 : (int)(reader["min_cmp_no"]),
                        MaxNoOfCompetencies = reader["max_cmp_no"] == DBNull.Value ? 0 : (int)(reader["max_cmp_no"]),
                        TotalCompetencyScore = reader["ttl_cmp_scr"] == DBNull.Value ? 0.00M : (decimal)(reader["ttl_cmp_scr"]),
                        TotalKpaScore = reader["ttl_kpa_scr"] == DBNull.Value ? 0.00M : (decimal)(reader["ttl_kpa_scr"]),
                        TotalCombinedScore = reader["ttl_cmb_scr"] == DBNull.Value ? 0.00M : (decimal)(reader["ttl_cmb_scr"]),
                        IsActive = reader["is_ctv"] == DBNull.Value ? false : (bool)reader["is_ctv"],
                        LastModifiedBy = reader["rvw_mdb"] == DBNull.Value ? string.Empty : (reader["rvw_mdb"]).ToString(),
                        LastModifiedTime = reader["rvw_mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rvw_mdt"],
                        CreatedBy = reader["rvw_ctb"] == DBNull.Value ? string.Empty : (reader["rvw_ctb"]).ToString(),
                        CreatedTime = reader["rvw_ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rvw_ctt"],
                    });
                }
            }
            await conn.CloseAsync();
            return reviewSessionsList;
        }

        public async Task<IList<ReviewSession>> GetByIdAsync(int reviewSessionId)
        {
            List<ReviewSession> reviewSessionsList = new List<ReviewSession>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.rvw_sxn_id, s.rvw_sxn_nm, s.rvw_yr_id, s.rvw_typ_id, ");
            sb.Append("s.rvw_str_dt, s.rvw_ndt_dt, s.min_cmp_no, s.max_cmp_no, ");
            sb.Append("s.ttl_cmp_scr, s.ttl_kpa_scr, s.ttl_cmb_scr, s.rvw_ctt, ");
            sb.Append("s.rvw_ctb, s.rvw_mdt, s.rvw_mdb, y.pms_yr_nm, t.rvw_typ_nm, ");
            sb.Append("s.is_ctv FROM public.pmsrvwsxns s ");
            sb.Append("INNER JOIN public.pmssttyrs y ON y.pms_yr_id = s.rvw_yr_id ");
            sb.Append("INNER JOIN public.pmsrvwtyps t ON t.rvw_typ_id = s.rvw_typ_id ");
            sb.Append("WHERE (s.rvw_sxn_id = @rvw_sxn_id) ORDER BY s.rvw_ndt_dt DESC;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_sxn_id.Value = reviewSessionId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    reviewSessionsList.Add(new ReviewSession()
                    {
                        Id = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)(reader["rvw_sxn_id"]),
                        Name = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : (reader["rvw_sxn_nm"]).ToString(),
                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)(reader["rvw_yr_id"]),
                        ReviewYearName = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : (reader["pms_yr_nm"]).ToString(),
                        ReviewTypeId = reader["rvw_typ_id"] == DBNull.Value ? 0 : (int)(reader["rvw_typ_id"]),
                        ReviewTypeName = reader["rvw_typ_nm"] == DBNull.Value ? string.Empty : (reader["rvw_typ_nm"]).ToString(),
                        StartDate = reader["rvw_str_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rvw_str_dt"],
                        EndDate = reader["rvw_ndt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rvw_ndt_dt"],
                        MinNoOfCompetencies = reader["min_cmp_no"] == DBNull.Value ? 0 : (int)(reader["min_cmp_no"]),
                        MaxNoOfCompetencies = reader["max_cmp_no"] == DBNull.Value ? 0 : (int)(reader["max_cmp_no"]),
                        TotalCompetencyScore = reader["ttl_cmp_scr"] == DBNull.Value ? 0.00M : (decimal)(reader["ttl_cmp_scr"]),
                        TotalKpaScore = reader["ttl_kpa_scr"] == DBNull.Value ? 0.00M : (decimal)(reader["ttl_kpa_scr"]),
                        TotalCombinedScore = reader["ttl_cmb_scr"] == DBNull.Value ? 0.00M : (decimal)(reader["ttl_cmb_scr"]),
                        IsActive = reader["is_ctv"] == DBNull.Value ? false : (bool)reader["is_ctv"],
                        LastModifiedBy = reader["rvw_mdb"] == DBNull.Value ? string.Empty : (reader["rvw_mdb"]).ToString(),
                        LastModifiedTime = reader["rvw_mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rvw_mdt"],
                        CreatedBy = reader["rvw_ctb"] == DBNull.Value ? string.Empty : (reader["rvw_ctb"]).ToString(),
                        CreatedTime = reader["rvw_ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rvw_ctt"],
                    });
                }
            }
            await conn.CloseAsync();
            return reviewSessionsList;
        }

        public async Task<IList<ReviewSession>> GetByYearIdAsync(int performanceYearId)
        {
            List<ReviewSession> reviewSessionsList = new List<ReviewSession>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.rvw_sxn_id, s.rvw_sxn_nm, s.rvw_yr_id, s.rvw_typ_id, ");
            sb.Append("s.rvw_str_dt, s.rvw_ndt_dt, s.min_cmp_no, s.max_cmp_no, ");
            sb.Append("s.ttl_cmp_scr, s.ttl_kpa_scr, s.ttl_cmb_scr, s.rvw_ctt, ");
            sb.Append("s.rvw_ctb, s.rvw_mdt, s.rvw_mdb, y.pms_yr_nm, t.rvw_typ_nm, ");
            sb.Append("s.is_ctv FROM public.pmsrvwsxns s ");
            sb.Append("INNER JOIN public.pmssttyrs y ON y.pms_yr_id = s.rvw_yr_id ");
            sb.Append("INNER JOIN public.pmsrvwtyps t ON t.rvw_typ_id = s.rvw_typ_id ");
            sb.Append("WHERE (s.rvw_yr_id = @rvw_yr_id) ORDER BY s.rvw_ndt_dt DESC; ");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_yr_id = cmd.Parameters.Add("@rvw_yr_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_yr_id.Value = performanceYearId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    reviewSessionsList.Add(new ReviewSession()
                    {
                        Id = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)(reader["rvw_sxn_id"]),
                        Name = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : (reader["rvw_sxn_nm"]).ToString(),
                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)(reader["rvw_yr_id"]),
                        ReviewYearName = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : (reader["pms_yr_nm"]).ToString(),
                        ReviewTypeId = reader["rvw_typ_id"] == DBNull.Value ? 0 : (int)(reader["rvw_typ_id"]),
                        ReviewTypeName = reader["rvw_typ_nm"] == DBNull.Value ? string.Empty : (reader["rvw_typ_nm"]).ToString(),
                        StartDate = reader["rvw_str_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rvw_str_dt"],
                        EndDate = reader["rvw_ndt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rvw_ndt_dt"],
                        MinNoOfCompetencies = reader["min_cmp_no"] == DBNull.Value ? 0 : (int)(reader["min_cmp_no"]),
                        MaxNoOfCompetencies = reader["max_cmp_no"] == DBNull.Value ? 0 : (int)(reader["max_cmp_no"]),
                        TotalCompetencyScore = reader["ttl_cmp_scr"] == DBNull.Value ? 0.00M : (decimal)(reader["ttl_cmp_scr"]),
                        TotalKpaScore = reader["ttl_kpa_scr"] == DBNull.Value ? 0.00M : (decimal)(reader["ttl_kpa_scr"]),
                        TotalCombinedScore = reader["ttl_cmb_scr"] == DBNull.Value ? 0.00M : (decimal)(reader["ttl_cmb_scr"]),
                        IsActive = reader["is_ctv"] == DBNull.Value ? false : (bool)reader["is_ctv"],
                        LastModifiedBy = reader["rvw_mdb"] == DBNull.Value ? string.Empty : (reader["rvw_mdb"]).ToString(),
                        LastModifiedTime = reader["rvw_mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rvw_mdt"],
                        CreatedBy = reader["rvw_ctb"] == DBNull.Value ? string.Empty : (reader["rvw_ctb"]).ToString(),
                        CreatedTime = reader["rvw_ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rvw_ctt"],
                    });
                }
            }
            await conn.CloseAsync();
            return reviewSessionsList;
        }

        public async Task<IList<ReviewSession>> GetByNameAsync(string reviewSessionName)
        {
            List<ReviewSession> reviewSessionsList = new List<ReviewSession>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.rvw_sxn_id, s.rvw_sxn_nm, s.rvw_yr_id, s.rvw_typ_id, ");
            sb.Append("s.rvw_str_dt, s.rvw_ndt_dt, s.min_cmp_no, s.max_cmp_no, ");
            sb.Append("s.ttl_cmp_scr, s.ttl_kpa_scr, s.ttl_cmb_scr, s.rvw_ctt, ");
            sb.Append("s.rvw_ctb, s.rvw_mdt, s.rvw_mdb, y.pms_yr_nm, t.rvw_typ_nm, ");
            sb.Append("s.is_ctv FROM public.pmsrvwsxns s ");
            sb.Append("INNER JOIN public.pmssttyrs y ON y.pms_yr_id = s.rvw_yr_id ");
            sb.Append("INNER JOIN public.pmsrvwtyps t ON t.rvw_typ_id = s.rvw_typ_id ");
            sb.Append("WHERE (LOWER(s.rvw_sxn_nm) = LOWER(@rvw_sxn_nm))");
            sb.Append(" ORDER BY s.rvw_ndt_dt DESC; ");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_sxn_nm = cmd.Parameters.Add("@rvw_sxn_nm", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                rvw_sxn_nm.Value = reviewSessionName;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    reviewSessionsList.Add(new ReviewSession()
                    {
                        Id = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)(reader["rvw_sxn_id"]),
                        Name = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : (reader["rvw_sxn_nm"]).ToString(),
                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)(reader["rvw_yr_id"]),
                        ReviewYearName = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : (reader["pms_yr_nm"]).ToString(),
                        ReviewTypeId = reader["rvw_typ_id"] == DBNull.Value ? 0 : (int)(reader["rvw_typ_id"]),
                        ReviewTypeName = reader["rvw_typ_nm"] == DBNull.Value ? string.Empty : (reader["rvw_typ_nm"]).ToString(),
                        StartDate = reader["rvw_str_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rvw_str_dt"],
                        EndDate = reader["rvw_ndt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rvw_ndt_dt"],
                        MinNoOfCompetencies = reader["min_cmp_no"] == DBNull.Value ? 0 : (int)(reader["min_cmp_no"]),
                        MaxNoOfCompetencies = reader["max_cmp_no"] == DBNull.Value ? 0 : (int)(reader["max_cmp_no"]),
                        TotalCompetencyScore = reader["ttl_cmp_scr"] == DBNull.Value ? 0.00M : (decimal)(reader["ttl_cmp_scr"]),
                        TotalKpaScore = reader["ttl_kpa_scr"] == DBNull.Value ? 0.00M : (decimal)(reader["ttl_kpa_scr"]),
                        TotalCombinedScore = reader["ttl_cmb_scr"] == DBNull.Value ? 0.00M : (decimal)(reader["ttl_cmb_scr"]),
                        IsActive = reader["is_ctv"] == DBNull.Value ? false : (bool)reader["is_ctv"],
                        LastModifiedBy = reader["rvw_mdb"] == DBNull.Value ? string.Empty : (reader["rvw_mdb"]).ToString(),
                        LastModifiedTime = reader["rvw_mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rvw_mdt"],
                        CreatedBy = reader["rvw_ctb"] == DBNull.Value ? string.Empty : (reader["rvw_ctb"]).ToString(),
                        CreatedTime = reader["rvw_ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rvw_ctt"],
                    });
                }
            }
            await conn.CloseAsync();
            return reviewSessionsList;
        }
        #endregion

        #region Review Session Write Action Methods
        public async Task<bool> AddAsync(ReviewSession reviewSession)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.pmsrvwsxns(rvw_sxn_nm, rvw_yr_id, ");
            sb.Append("rvw_typ_id, rvw_str_dt, rvw_ndt_dt, min_cmp_no, ");
            sb.Append("max_cmp_no, ttl_cmp_scr, ttl_kpa_scr, ttl_cmb_scr, ");
            sb.Append("rvw_ctt, rvw_ctb, rvw_mdt, rvw_mdb, is_ctv) ");
            sb.Append("VALUES (@rvw_sxn_nm, @rvw_yr_id, @rvw_typ_id, ");
            sb.Append("@rvw_str_dt, @rvw_ndt_dt, @min_cmp_no, @max_cmp_no, ");
            sb.Append("@ttl_cmp_scr, @ttl_kpa_scr, @ttl_cmb_scr, @rvw_ctt, ");
            sb.Append("@rvw_ctb, @rvw_mdt, @rvw_mdb, @is_ctv);");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var rvw_sxn_nm = cmd.Parameters.Add("@rvw_sxn_nm", NpgsqlDbType.Text);
                    var rvw_yr_id = cmd.Parameters.Add("@rvw_yr_id", NpgsqlDbType.Integer);
                    var rvw_typ_id = cmd.Parameters.Add("@rvw_typ_id", NpgsqlDbType.Integer);
                    var rvw_str_dt = cmd.Parameters.Add("@rvw_str_dt", NpgsqlDbType.Date);
                    var rvw_ndt_dt = cmd.Parameters.Add("@rvw_ndt_dt", NpgsqlDbType.Date);
                    var min_cmp_no = cmd.Parameters.Add("@min_cmp_no", NpgsqlDbType.Integer);
                    var max_cmp_no = cmd.Parameters.Add("@max_cmp_no", NpgsqlDbType.Integer);
                    var ttl_cmp_scr = cmd.Parameters.Add("@ttl_cmp_scr", NpgsqlDbType.Numeric);
                    var ttl_kpa_scr = cmd.Parameters.Add("@ttl_kpa_scr", NpgsqlDbType.Numeric);
                    var ttl_cmb_scr = cmd.Parameters.Add("@ttl_cmb_scr", NpgsqlDbType.Numeric);
                    var rvw_ctb = cmd.Parameters.Add("@rvw_ctb", NpgsqlDbType.Text);
                    var rvw_ctt = cmd.Parameters.Add("@rvw_ctt", NpgsqlDbType.TimestampTz);
                    var rvw_mdb = cmd.Parameters.Add("@rvw_mdb", NpgsqlDbType.Text);
                    var rvw_mdt = cmd.Parameters.Add("@rvw_mdt", NpgsqlDbType.TimestampTz);
                    var is_ctv = cmd.Parameters.Add("@is_ctv", NpgsqlDbType.Boolean);
                    cmd.Prepare();
                    rvw_sxn_nm.Value = reviewSession.Name;
                    rvw_yr_id.Value = reviewSession.ReviewYearId;
                    rvw_typ_id.Value = reviewSession.ReviewTypeId;
                    rvw_str_dt.Value = reviewSession.StartDate;
                    rvw_ndt_dt.Value = reviewSession.EndDate;
                    min_cmp_no.Value = reviewSession.MinNoOfCompetencies;
                    max_cmp_no.Value = reviewSession.MaxNoOfCompetencies;
                    ttl_cmp_scr.Value = reviewSession.TotalCompetencyScore;
                    ttl_kpa_scr.Value = reviewSession.TotalKpaScore;
                    ttl_cmb_scr.Value = reviewSession.TotalCombinedScore;
                    is_ctv.Value = reviewSession.IsActive;
                    rvw_ctb.Value = reviewSession.CreatedBy ?? (object)DBNull.Value;
                    rvw_ctt.Value = reviewSession.CreatedTime ?? (object)DBNull.Value;
                    rvw_mdb.Value = reviewSession.CreatedBy ?? (object)DBNull.Value;
                    rvw_mdt.Value = reviewSession.CreatedTime ?? (object)DBNull.Value;

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

        public async Task<bool> UpdateAsync(ReviewSession reviewSession)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.pmsrvwsxns SET  rvw_sxn_nm=@rvw_sxn_nm, ");
            sb.Append("rvw_yr_id=@rvw_yr_id, rvw_typ_id=@rvw_typ_id, ");
            sb.Append("rvw_str_dt=@rvw_str_dt, rvw_ndt_dt=@rvw_ndt_dt, ");
            sb.Append("min_cmp_no=@min_cmp_no, max_cmp_no=@max_cmp_no, ");
            sb.Append("ttl_cmp_scr=@ttl_cmp_scr, ttl_kpa_scr=@ttl_kpa_scr, ");
            sb.Append("ttl_cmb_scr=@ttl_cmb_scr, rvw_mdt=@rvw_mdt,  ");
            sb.Append("rvw_mdb=@rvw_mdb, is_ctv=@is_ctv ");
            sb.Append("WHERE (rvw_sxn_id = @rvw_sxn_id);");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var rvw_sxn_nm = cmd.Parameters.Add("@rvw_sxn_nm", NpgsqlDbType.Text);
                    var rvw_yr_id = cmd.Parameters.Add("@rvw_yr_id", NpgsqlDbType.Integer);
                    var rvw_typ_id = cmd.Parameters.Add("@rvw_typ_id", NpgsqlDbType.Integer);
                    var rvw_str_dt = cmd.Parameters.Add("@rvw_str_dt", NpgsqlDbType.Date);
                    var rvw_ndt_dt = cmd.Parameters.Add("@rvw_ndt_dt", NpgsqlDbType.Date);
                    var min_cmp_no = cmd.Parameters.Add("@min_cmp_no", NpgsqlDbType.Integer);
                    var max_cmp_no = cmd.Parameters.Add("@max_cmp_no", NpgsqlDbType.Integer);
                    var ttl_cmp_scr = cmd.Parameters.Add("@ttl_cmp_scr", NpgsqlDbType.Numeric);
                    var ttl_kpa_scr = cmd.Parameters.Add("@ttl_kpa_scr", NpgsqlDbType.Numeric);
                    var ttl_cmb_scr = cmd.Parameters.Add("@ttl_cmb_scr", NpgsqlDbType.Numeric);
                    var is_ctv = cmd.Parameters.Add("@is_ctv", NpgsqlDbType.Boolean);
                    var rvw_ctb = cmd.Parameters.Add("@rvw_ctb", NpgsqlDbType.Text);
                    var rvw_ctt = cmd.Parameters.Add("@rvw_ctt", NpgsqlDbType.TimestampTz);
                    var rvw_mdb = cmd.Parameters.Add("@rvw_mdb", NpgsqlDbType.Text);
                    var rvw_mdt = cmd.Parameters.Add("@rvw_mdt", NpgsqlDbType.TimestampTz);
                    var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);

                    cmd.Prepare();

                    rvw_sxn_nm.Value = reviewSession.Name;
                    rvw_yr_id.Value = reviewSession.ReviewYearId;
                    rvw_typ_id.Value = reviewSession.ReviewTypeId;
                    rvw_str_dt.Value = reviewSession.StartDate;
                    rvw_ndt_dt.Value = reviewSession.EndDate;
                    min_cmp_no.Value = reviewSession.MinNoOfCompetencies;
                    max_cmp_no.Value = reviewSession.MaxNoOfCompetencies;
                    ttl_cmp_scr.Value = reviewSession.TotalCompetencyScore;
                    ttl_kpa_scr.Value = reviewSession.TotalKpaScore;
                    ttl_cmb_scr.Value = reviewSession.TotalCombinedScore;
                    is_ctv.Value = reviewSession.IsActive;
                    rvw_ctb.Value = reviewSession.CreatedBy ?? (object)DBNull.Value;
                    rvw_ctt.Value = reviewSession.CreatedTime ?? (object)DBNull.Value;
                    rvw_mdb.Value = reviewSession.CreatedBy ?? (object)DBNull.Value;
                    rvw_mdt.Value = reviewSession.CreatedTime ?? (object)DBNull.Value;
                    rvw_sxn_id.Value = reviewSession.Id;

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

        public async Task<bool> DeleteAsync(int reviewSessionId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));

            string query = "DELETE FROM public.pmsrvwsxns WHERE (rvw_sxn_id=@rvw_sxn_id);";
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    rvw_sxn_id.Value = reviewSessionId;

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
