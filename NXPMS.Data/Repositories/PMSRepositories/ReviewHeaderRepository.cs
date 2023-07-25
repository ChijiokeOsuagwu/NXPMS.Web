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
    public class ReviewHeaderRepository: IReviewHeaderRepository
    {
        public IConfiguration _config { get; }
        public ReviewHeaderRepository(IConfiguration configuration)
        {
            _config = configuration;
        }
        #region Review Header Read Action Methods
        public async Task<IList<ReviewHeader>> GetByIdAsync(int reviewHeaderId)
        {
            List<ReviewHeader> reviewHeadersList = new List<ReviewHeader>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT h.rvw_hdr_id, h.rvw_sxn_id, h.rvw_emp_id, h.rvw_stg_id, ");
            sb.Append("h.rvw_yr_id, h.pry_apr_id, h.fbk_probs, h.fbk_solns, h.lm_rec, ");
            sb.Append("h.unit_cd, h.dept_cd, h.loc_id, h.lm_rmk, h.uh_rmk, h.uh_rec, ");
            sb.Append("h.dh_rmk, h.dh_rec, h.hr_rmk, h.hr_rec, h.mgt_rmk, h.mgt_dec, ");
            sb.Append("h.con_acpt, h.dt_con_acpt, h.eva_acpt, h.dt_eva_acpt, h.is_flg, ");
            sb.Append("h.flg_by, h.flg_dt, h.rvw_gls, h.lm_nm, h.uh_nm, h.dh_nm, h.hr_nm, ");
            sb.Append("h.mgt_nm, s.rvw_sxn_nm, e.fullname as appraisee_name, ");
            sb.Append("g.rvw_stg_nm, g.stg_xtn_ds, y.pms_yr_nm, p.fullname as appraiser_name, ");
            sb.Append("e.current_designation AS appraisee_designation, ");
            sb.Append("p.current_designation AS appraiser_designation, ");
            sb.Append("u.unit_nm, d.dept_nm, l.locname FROM public.pmsrvwhdrs h ");
            sb.Append("INNER JOIN public.pmsrvwsxns s ON s.rvw_sxn_id = h.rvw_sxn_id ");
            sb.Append("INNER JOIN public.ermempinf e ON e.empid = h.rvw_emp_id ");
            sb.Append("INNER JOIN public.pmssttstgs g ON g.rvw_stg_id = h.rvw_stg_id ");
            sb.Append("INNER JOIN public.pmssttyrs y ON y.pms_yr_id = h.rvw_yr_id ");
            sb.Append("INNER JOIN public.syscfgunts u ON u.unit_cd = h.unit_cd ");
            sb.Append("INNER JOIN public.syscfgdpts d ON d.dept_cd = h.dept_cd ");
            sb.Append("INNER JOIN public.syscfglocs l ON l.locid = h.loc_id ");
            sb.Append("LEFT JOIN public.ermempinf p ON p.empid = h.pry_apr_id ");
            sb.Append("WHERE (h.rvw_hdr_id = @rvw_hdr_id); ");
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
                    reviewHeadersList.Add(new ReviewHeader()
                    {
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        AppraiseeId = reader["rvw_emp_id"] == DBNull.Value ? 0 : (int)reader["rvw_emp_id"],
                        ReviewStageId = reader["rvw_stg_id"] == DBNull.Value ? 0 : (int)reader["rvw_stg_id"],
                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        PrimaryAppraiserId = reader["pry_apr_id"] == DBNull.Value ? 0 : (int)reader["pry_apr_id"],
                        FeedbackProblems = reader["fbk_probs"] == DBNull.Value ? string.Empty : reader["fbk_probs"].ToString(),
                        FeedbackSolutions = reader["fbk_solns"] == DBNull.Value ? string.Empty : reader["fbk_solns"].ToString(),
                        LineManagerRecommendation = reader["lm_rec"] == DBNull.Value ? string.Empty : reader["lm_rec"].ToString(),
                        LineManagerName = reader["lm_nm"] == DBNull.Value ? string.Empty : reader["lm_nm"].ToString(),
                        UnitCode = reader["unit_cd"] == DBNull.Value ? string.Empty : reader["unit_cd"].ToString(),
                        DepartmentCode = reader["dept_cd"] == DBNull.Value ? string.Empty : reader["dept_cd"].ToString(),
                        LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        LineManagerComments = reader["lm_rmk"] == DBNull.Value ? string.Empty : reader["lm_rmk"].ToString(),
                        UnitHeadComments = reader["uh_rmk"] == DBNull.Value ? string.Empty : reader["uh_rmk"].ToString(),
                        UnitHeadName = reader["uh_nm"] == DBNull.Value ? string.Empty : reader["uh_nm"].ToString(),
                        UnitHeadRecommendation = reader["uh_rec"] == DBNull.Value ? string.Empty : reader["uh_rec"].ToString(),
                        DepartmentHeadComments = reader["dh_rmk"] == DBNull.Value ? string.Empty : reader["dh_rmk"].ToString(),
                        DepartmentHeadName = reader["dh_nm"] == DBNull.Value ? string.Empty : reader["dh_nm"].ToString(),
                        DepartmentHeadRecommendation = reader["dh_rec"] == DBNull.Value ? string.Empty : reader["dh_rec"].ToString(),
                        HrComments = reader["hr_rmk"] == DBNull.Value ? string.Empty : reader["hr_rmk"].ToString(),
                        HrName = reader["hr_nm"] == DBNull.Value ? string.Empty : reader["hr_nm"].ToString(),
                        HrRecommendation = reader["hr_rec"] == DBNull.Value ? string.Empty : reader["hr_rec"].ToString(),
                        ManagementComments = reader["mgt_rmk"] == DBNull.Value ? string.Empty : reader["mgt_rmk"].ToString(),
                        ManagementName = reader["mgt_nm"] == DBNull.Value ? string.Empty : reader["mgt_nm"].ToString(),
                        ManagementDecision = reader["mgt_dec"] == DBNull.Value ? string.Empty : reader["mgt_dec"].ToString(),
                        ContractIsAccepted = reader["con_acpt"] == DBNull.Value ? false : (bool)reader["con_acpt"],
                        TimeContractAccepted = reader["dt_con_acpt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_con_acpt"],
                        EvaluationIsAccepted = reader["eva_acpt"] == DBNull.Value ? false : (bool)reader["eva_acpt"],
                        TimeEvaluationAccepted = reader["dt_eva_acpt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_eva_acpt"],
                        IsFlagged = reader["is_flg"] == DBNull.Value ? false : (bool)reader["is_flg"],
                        FlaggedBy = reader["flg_by"] == DBNull.Value ? string.Empty : reader["flg_by"].ToString(),
                        FlaggedTime = reader["flg_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["flg_dt"],
                        PerformanceGoal = reader["rvw_gls"] == DBNull.Value ? string.Empty : reader["rvw_gls"].ToString(),
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),
                        AppraiseeName = reader["appraisee_name"] == DBNull.Value ? string.Empty : reader["appraisee_name"].ToString(),
                        AppraiseeDesignation = reader["appraisee_designation"] == DBNull.Value ? string.Empty : reader["appraisee_designation"].ToString(),
                        ReviewStageDescription = reader["rvw_stg_nm"] == DBNull.Value ? string.Empty : reader["rvw_stg_nm"].ToString(),
                        ReviewStageActionDescription = reader["stg_xtn_ds"] == DBNull.Value ? string.Empty : reader["stg_xtn_ds"].ToString(),
                        ReviewYearName = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : reader["pms_yr_nm"].ToString(),
                        PrimaryAppraiserName = reader["appraiser_name"] == DBNull.Value ? string.Empty : reader["appraiser_name"].ToString(),
                        PrimaryAppraiserDesignation = reader["appraiser_designation"] == DBNull.Value ? string.Empty : reader["appraiser_designation"].ToString(),
                        UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                        DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                        LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return reviewHeadersList;
        }

        public async Task<IList<ReviewHeader>> GetByAppraiseeIdAndReviewSessionIdAsync(int appraiseeId, int reviewSessionId)
        {
            List<ReviewHeader> reviewHeadersList = new List<ReviewHeader>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT h.rvw_hdr_id, h.rvw_sxn_id, h.rvw_emp_id, h.rvw_stg_id, ");
            sb.Append("h.rvw_yr_id, h.pry_apr_id, h.fbk_probs, h.fbk_solns, h.lm_rec, ");
            sb.Append("h.unit_cd, h.dept_cd, h.loc_id, h.lm_rmk, h.uh_rmk, h.uh_rec, ");
            sb.Append("h.dh_rmk, h.dh_rec, h.hr_rmk, h.hr_rec, h.mgt_rmk, h.mgt_dec, ");
            sb.Append("h.con_acpt, h.dt_con_acpt, h.eva_acpt, h.dt_eva_acpt, h.is_flg, ");
            sb.Append("h.flg_by, h.flg_dt, h.rvw_gls, h.lm_nm, h.uh_nm, h.dh_nm, h.hr_nm, ");
            sb.Append("h.mgt_nm, s.rvw_sxn_nm, e.fullname as appraisee_name, ");
            sb.Append("g.rvw_stg_nm, g.stg_xtn_ds, y.pms_yr_nm, p.fullname as appraiser_name, ");
            sb.Append("e.current_designation AS appraisee_designation, ");
            sb.Append("p.current_designation AS appraiser_designation, ");
            sb.Append("u.unit_nm, d.dept_nm, l.locname FROM public.pmsrvwhdrs h ");
            sb.Append("INNER JOIN public.pmsrvwsxns s ON s.rvw_sxn_id = h.rvw_sxn_id ");
            sb.Append("INNER JOIN public.ermempinf e ON e.empid = h.rvw_emp_id ");
            sb.Append("INNER JOIN public.pmssttstgs g ON g.rvw_stg_id = h.rvw_stg_id ");
            sb.Append("INNER JOIN public.pmssttyrs y ON y.pms_yr_id = h.rvw_yr_id ");
            sb.Append("INNER JOIN public.syscfgunts u ON u.unit_cd = h.unit_cd ");
            sb.Append("INNER JOIN public.syscfgdpts d ON d.dept_cd = h.dept_cd ");
            sb.Append("INNER JOIN public.syscfglocs l ON l.locid = h.loc_id ");
            sb.Append("LEFT JOIN public.ermempinf p ON p.empid = h.pry_apr_id ");
            sb.Append("WHERE (h.rvw_sxn_id = @rvw_sxn_id) ");
            sb.Append("AND (h.rvw_emp_id = @rvw_emp_id); ");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                var rvw_emp_id = cmd.Parameters.Add("@rvw_emp_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_sxn_id.Value = reviewSessionId;
                rvw_emp_id.Value = appraiseeId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    reviewHeadersList.Add(new ReviewHeader()
                    {
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        AppraiseeId = reader["rvw_emp_id"] == DBNull.Value ? 0 : (int)reader["rvw_emp_id"],
                        ReviewStageId = reader["rvw_stg_id"] == DBNull.Value ? 0 : (int)reader["rvw_stg_id"],
                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        PrimaryAppraiserId = reader["pry_apr_id"] == DBNull.Value ? 0 : (int)reader["pry_apr_id"],
                        FeedbackProblems = reader["fbk_probs"] == DBNull.Value ? string.Empty : reader["fbk_probs"].ToString(),
                        FeedbackSolutions = reader["fbk_solns"] == DBNull.Value ? string.Empty : reader["fbk_solns"].ToString(),
                        LineManagerRecommendation = reader["lm_rec"] == DBNull.Value ? string.Empty : reader["lm_rec"].ToString(),
                        LineManagerName = reader["lm_nm"] == DBNull.Value ? string.Empty : reader["lm_nm"].ToString(),
                        UnitCode = reader["unit_cd"] == DBNull.Value ? string.Empty : reader["unit_cd"].ToString(),
                        DepartmentCode = reader["dept_cd"] == DBNull.Value ? string.Empty : reader["dept_cd"].ToString(),
                        LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        LineManagerComments = reader["lm_rmk"] == DBNull.Value ? string.Empty : reader["lm_rmk"].ToString(),
                        UnitHeadComments = reader["uh_rmk"] == DBNull.Value ? string.Empty : reader["uh_rmk"].ToString(),
                        UnitHeadName = reader["uh_nm"] == DBNull.Value ? string.Empty : reader["uh_nm"].ToString(),
                        UnitHeadRecommendation = reader["uh_rec"] == DBNull.Value ? string.Empty : reader["uh_rec"].ToString(),
                        DepartmentHeadComments = reader["dh_rmk"] == DBNull.Value ? string.Empty : reader["dh_rmk"].ToString(),
                        DepartmentHeadName = reader["dh_nm"] == DBNull.Value ? string.Empty : reader["dh_nm"].ToString(),
                        DepartmentHeadRecommendation = reader["dh_rec"] == DBNull.Value ? string.Empty : reader["dh_rec"].ToString(),
                        HrComments = reader["hr_rmk"] == DBNull.Value ? string.Empty : reader["hr_rmk"].ToString(),
                        HrName = reader["hr_nm"] == DBNull.Value ? string.Empty : reader["hr_nm"].ToString(),
                        HrRecommendation = reader["hr_rec"] == DBNull.Value ? string.Empty : reader["hr_rec"].ToString(),
                        ManagementComments = reader["mgt_rmk"] == DBNull.Value ? string.Empty : reader["mgt_rmk"].ToString(),
                        ManagementName = reader["mgt_nm"] == DBNull.Value ? string.Empty : reader["mgt_nm"].ToString(),
                        ManagementDecision = reader["mgt_dec"] == DBNull.Value ? string.Empty : reader["mgt_dec"].ToString(),
                        ContractIsAccepted = reader["con_acpt"] == DBNull.Value ? false : (bool)reader["con_acpt"],
                        TimeContractAccepted = reader["dt_con_acpt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_con_acpt"],
                        EvaluationIsAccepted = reader["eva_acpt"] == DBNull.Value ? false : (bool)reader["eva_acpt"],
                        TimeEvaluationAccepted = reader["dt_eva_acpt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_eva_acpt"],
                        IsFlagged = reader["is_flg"] == DBNull.Value ? false : (bool)reader["is_flg"],
                        FlaggedBy = reader["flg_by"] == DBNull.Value ? string.Empty : reader["flg_by"].ToString(),
                        FlaggedTime = reader["flg_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["flg_dt"],
                        PerformanceGoal = reader["rvw_gls"] == DBNull.Value ? string.Empty : reader["rvw_gls"].ToString(),
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),
                        AppraiseeName = reader["appraisee_name"] == DBNull.Value ? string.Empty : reader["appraisee_name"].ToString(),
                        AppraiseeDesignation = reader["appraisee_designation"] == DBNull.Value ? string.Empty : reader["appraisee_designation"].ToString(),
                        ReviewStageDescription = reader["rvw_stg_nm"] == DBNull.Value ? string.Empty : reader["rvw_stg_nm"].ToString(),
                        ReviewStageActionDescription = reader["stg_xtn_ds"] == DBNull.Value ? string.Empty : reader["stg_xtn_ds"].ToString(),
                        ReviewYearName = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : reader["pms_yr_nm"].ToString(),
                        PrimaryAppraiserName = reader["appraiser_name"] == DBNull.Value ? string.Empty : reader["appraiser_name"].ToString(),
                        PrimaryAppraiserDesignation = reader["appraiser_designation"] == DBNull.Value ? string.Empty : reader["appraiser_designation"].ToString(),
                        UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                        DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                        LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return reviewHeadersList;
        }
        #endregion

        #region Review Header Write Action Methods
        public async Task<bool> AddAsync(ReviewHeader reviewHeader)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.pmsrvwhdrs(rvw_sxn_id, rvw_emp_id, ");
            sb.Append("rvw_stg_id, rvw_yr_id, pry_apr_id, fbk_probs, fbk_solns, ");
            sb.Append("unit_cd, dept_cd, loc_id, lm_rmk, lm_rec, uh_rmk, uh_rec, ");
            sb.Append("dh_rmk, dh_rec, hr_rmk, hr_rec, mgt_rmk, mgt_dec, con_acpt, ");
            sb.Append("dt_con_acpt, eva_acpt, dt_eva_acpt, is_flg, flg_by, flg_dt, ");
            sb.Append("rvw_gls) VALUES (@rvw_sxn_id, @rvw_emp_id, @rvw_stg_id, ");
            sb.Append("@rvw_yr_id, @pry_apr_id, @fbk_probs, @fbk_solns, @unit_cd, ");
            sb.Append("@dept_cd, @loc_id, @lm_rmk, @lm_rec, @uh_rmk, @uh_rec, @dh_rmk, ");
            sb.Append("@dh_rec, @hr_rmk, @hr_rec, @mgt_rmk, @mgt_dec, @con_acpt, ");
            sb.Append("@dt_con_acpt, @eva_acpt, @dt_eva_acpt, ");
            sb.Append("@is_flg, @flg_by, @flg_dt, @rvw_gls); ");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                    var rvw_emp_id = cmd.Parameters.Add("@rvw_emp_id", NpgsqlDbType.Integer);
                    var rvw_stg_id = cmd.Parameters.Add("@rvw_stg_id", NpgsqlDbType.Integer);
                    var rvw_yr_id = cmd.Parameters.Add("@rvw_yr_id", NpgsqlDbType.Integer);
                    var pry_apr_id = cmd.Parameters.Add("@pry_apr_id", NpgsqlDbType.Integer);
                    var fbk_probs = cmd.Parameters.Add("@fbk_probs", NpgsqlDbType.Text);
                    var fbk_solns = cmd.Parameters.Add("@fbk_solns", NpgsqlDbType.Text);
                    var unit_cd = cmd.Parameters.Add("@unit_cd", NpgsqlDbType.Text);
                    var dept_cd = cmd.Parameters.Add("@dept_cd", NpgsqlDbType.Text);
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    var lm_rmk = cmd.Parameters.Add("@lm_rmk", NpgsqlDbType.Text);
                    var lm_rec = cmd.Parameters.Add("@lm_rec", NpgsqlDbType.Integer);
                    var uh_rmk = cmd.Parameters.Add("@uh_rmk", NpgsqlDbType.Text);
                    var uh_rec = cmd.Parameters.Add("@uh_rec", NpgsqlDbType.Integer);
                    var dh_rmk = cmd.Parameters.Add("@dh_rmk", NpgsqlDbType.Text);
                    var dh_rec = cmd.Parameters.Add("@dh_rec", NpgsqlDbType.Integer);
                    var hr_rmk = cmd.Parameters.Add("@hr_rmk", NpgsqlDbType.Text);
                    var hr_rec = cmd.Parameters.Add("@hr_rec", NpgsqlDbType.Integer);
                    var mgt_rmk = cmd.Parameters.Add("@mgt_rmk", NpgsqlDbType.Text);
                    var mgt_dec = cmd.Parameters.Add("@mgt_dec", NpgsqlDbType.Integer);
                    var con_acpt = cmd.Parameters.Add("@con_acpt", NpgsqlDbType.Boolean);
                    var dt_con_acpt = cmd.Parameters.Add("@dt_con_acpt", NpgsqlDbType.TimestampTz);
                    var eva_acpt = cmd.Parameters.Add("@eva_acpt", NpgsqlDbType.Boolean);
                    var dt_eva_acpt = cmd.Parameters.Add("@dt_eva_acpt", NpgsqlDbType.TimestampTz);
                    var is_flg = cmd.Parameters.Add("@is_flg", NpgsqlDbType.Boolean);
                    var flg_by = cmd.Parameters.Add("@flg_by", NpgsqlDbType.Text);
                    var flg_dt = cmd.Parameters.Add("@flg_dt", NpgsqlDbType.TimestampTz);
                    var rvw_gls = cmd.Parameters.Add("@rvw_gls", NpgsqlDbType.Text);
                    cmd.Prepare();
                    rvw_sxn_id.Value = reviewHeader.ReviewSessionId;
                    rvw_emp_id.Value = reviewHeader.AppraiseeId;
                    rvw_stg_id.Value = reviewHeader.ReviewStageId;
                    rvw_yr_id.Value = reviewHeader.ReviewYearId;
                    pry_apr_id.Value = reviewHeader.PrimaryAppraiserId ?? (object)DBNull.Value;
                    fbk_probs.Value = reviewHeader.FeedbackProblems ?? (object)DBNull.Value;
                    fbk_solns.Value = reviewHeader.FeedbackSolutions ?? (object)DBNull.Value; 
                    unit_cd.Value = reviewHeader.UnitCode ?? (object)DBNull.Value;
                    dept_cd.Value = reviewHeader.DepartmentCode ?? (object)DBNull.Value;
                    loc_id.Value = reviewHeader.LocationId ?? (object)DBNull.Value;
                    lm_rmk.Value = reviewHeader.LineManagerComments ?? (object)DBNull.Value;
                    lm_rec.Value = reviewHeader.LineManagerRecommendation ?? (object)DBNull.Value;
                    uh_rmk.Value = reviewHeader.UnitHeadComments ?? (object)DBNull.Value;
                    uh_rec.Value = reviewHeader.UnitHeadRecommendation ?? (object)DBNull.Value;
                    dh_rmk.Value = reviewHeader.DepartmentHeadComments ?? (object)DBNull.Value;
                    dh_rec.Value = reviewHeader.DepartmentHeadRecommendation ?? (object)DBNull.Value;
                    hr_rmk.Value = reviewHeader.HrComments ?? (object)DBNull.Value;
                    hr_rec.Value = reviewHeader.HrRecommendation ?? (object)DBNull.Value;
                    mgt_rmk.Value = reviewHeader.ManagementComments ?? (object)DBNull.Value;
                    mgt_dec.Value = reviewHeader.ManagementDecision ?? (object)DBNull.Value;
                    con_acpt.Value = reviewHeader.ContractIsAccepted ?? (object)DBNull.Value;
                    dt_con_acpt.Value = reviewHeader.TimeContractAccepted ?? (object)DBNull.Value;
                    eva_acpt.Value = reviewHeader.EvaluationIsAccepted ?? (object)DBNull.Value;
                    dt_eva_acpt.Value = reviewHeader.TimeEvaluationAccepted ?? (object)DBNull.Value;
                    is_flg.Value = reviewHeader.IsFlagged;
                    flg_by.Value = reviewHeader.FlaggedBy ?? (object)DBNull.Value;
                    flg_dt.Value = reviewHeader.FlaggedTime ?? (object)DBNull.Value;
                    rvw_gls.Value = reviewHeader.PerformanceGoal ?? (object)DBNull.Value;

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

        public async Task<bool> UpdateGoalAsync(int reviewHeaderId, string performanceGoal, int appraiserId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.pmsrvwhdrs SET rvw_gls=@rvw_gls, ");
            sb.Append("pry_apr_id=@pry_apr_id ");
            sb.Append("WHERE (rvw_hdr_id=@rvw_hdr_id);");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                    var rvw_gls = cmd.Parameters.Add("@rvw_gls", NpgsqlDbType.Text);
                    var pry_apr_id = cmd.Parameters.Add("@pry_apr_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    rvw_hdr_id.Value = reviewHeaderId;
                    rvw_gls.Value = performanceGoal ?? (object)DBNull.Value;
                    pry_apr_id.Value = appraiserId;

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

        public async Task<bool> UpdateStageIdAsync(int reviewHeaderId, int nextStageId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.pmsrvwhdrs SET rvw_stg_id=@rvw_stg_id ");
            sb.Append("WHERE (rvw_hdr_id=@rvw_hdr_id);");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                    var rvw_stg_id = cmd.Parameters.Add("@rvw_stg_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    rvw_hdr_id.Value = reviewHeaderId;
                    rvw_stg_id.Value = nextStageId;

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

        public async Task<bool> UpdateContractAcceptanceAsync(int reviewHeaderId, bool isAccepted)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.pmsrvwhdrs SET con_acpt=@con_acpt, ");
            sb.Append("dt_con_acpt=@dt_con_acpt ");
            sb.Append("WHERE (rvw_hdr_id=@rvw_hdr_id);");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                    var con_acpt = cmd.Parameters.Add("@con_acpt", NpgsqlDbType.Boolean);
                    var dt_con_acpt = cmd.Parameters.Add("@dt_con_acpt", NpgsqlDbType.TimestampTz);
                    cmd.Prepare();
                    rvw_hdr_id.Value = reviewHeaderId;
                    con_acpt.Value = isAccepted;
                    dt_con_acpt.Value = DateTime.UtcNow;

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

        public async Task<bool> UpdateEvaluationAcceptanceAsync(int reviewHeaderId, bool isAccepted)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.pmsrvwhdrs SET eva_acpt=@eva_acpt, ");
            sb.Append("dt_eva_acpt=@dt_eva_acpt ");
            sb.Append("WHERE (rvw_hdr_id=@rvw_hdr_id);");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                    var eva_acpt = cmd.Parameters.Add("@eva_acpt", NpgsqlDbType.Boolean);
                    var dt_eva_acpt = cmd.Parameters.Add("@dt_eva_acpt", NpgsqlDbType.TimestampTz);
                    cmd.Prepare();
                    rvw_hdr_id.Value = reviewHeaderId;
                    eva_acpt.Value = isAccepted;
                    dt_eva_acpt.Value = DateTime.UtcNow;

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

        public async Task<bool> UpdateAppraiseeFlagAsync(int reviewHeaderId, bool isFlagged, string flaggedBy)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.pmsrvwhdrs SET is_flg=@is_flg, ");
            sb.Append("flg_by=@flg_by, flg_dt=@flg_dt ");
            sb.Append("WHERE (rvw_hdr_id=@rvw_hdr_id);");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                    var is_flg = cmd.Parameters.Add("@is_flg", NpgsqlDbType.Boolean);
                    var flg_by = cmd.Parameters.Add("@flg_by", NpgsqlDbType.Text);
                    var flg_dt = cmd.Parameters.Add("@flg_dt", NpgsqlDbType.TimestampTz);
                    cmd.Prepare();
                    rvw_hdr_id.Value = reviewHeaderId;
                    is_flg.Value = isFlagged;
                    flg_by.Value = flaggedBy;
                    flg_dt.Value = DateTime.UtcNow;

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

        public async Task<bool> UpdateFeedbackAsync(int reviewHeaderId, string feedbackProblems, string feedbackSolutions)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.pmsrvwhdrs SET fbk_probs=@fbk_probs, ");
            sb.Append("fbk_solns=@fbk_solns ");
            sb.Append("WHERE (rvw_hdr_id=@rvw_hdr_id);");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                    var fbk_probs = cmd.Parameters.Add("@fbk_probs", NpgsqlDbType.Text);
                    var fbk_solns = cmd.Parameters.Add("@fbk_solns", NpgsqlDbType.Text);
                    cmd.Prepare();
                    rvw_hdr_id.Value = reviewHeaderId;
                    fbk_probs.Value = feedbackProblems;
                    fbk_solns.Value = feedbackSolutions;

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

        public async Task<bool> UpdateLineManagerRecommendationAsync(int reviewHeaderId, string lineManagerName, string recommendedAction, string remarks)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.pmsrvwhdrs SET lm_rmk=@lm_rmk, lm_rec=@lm_rec, ");
            sb.Append("lm_nm=@lm_nm  WHERE (rvw_hdr_id=@rvw_hdr_id);");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                    var lm_nm = cmd.Parameters.Add("@lm_nm", NpgsqlDbType.Text);
                    var lm_rec = cmd.Parameters.Add("@lm_rec", NpgsqlDbType.Text);
                    var lm_rmk = cmd.Parameters.Add("@lm_rmk", NpgsqlDbType.Text);
                    cmd.Prepare();
                    rvw_hdr_id.Value = reviewHeaderId;
                    lm_nm.Value = lineManagerName;
                    lm_rec.Value = recommendedAction;
                    lm_rmk.Value = remarks;

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

        public async Task<bool> UpdateUnitHeadRecommendationAsync(int reviewHeaderId, string unitHeadName, string recommendedAction, string remarks)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.pmsrvwhdrs SET uh_rmk=@uh_rmk, uh_rec=@uh_rec, ");
            sb.Append("uh_nm=@uh_nm  WHERE (rvw_hdr_id=@rvw_hdr_id);");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                    var uh_nm = cmd.Parameters.Add("@uh_nm", NpgsqlDbType.Text);
                    var uh_rec = cmd.Parameters.Add("@uh_rec", NpgsqlDbType.Text);
                    var uh_rmk = cmd.Parameters.Add("@uh_rmk", NpgsqlDbType.Text);
                    cmd.Prepare();
                    rvw_hdr_id.Value = reviewHeaderId;
                    uh_nm.Value = unitHeadName;
                    uh_rec.Value = recommendedAction;
                    uh_rmk.Value = remarks;

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

        public async Task<bool> UpdateDepartmentHeadRecommendationAsync(int reviewHeaderId, string deptHeadName, string recommendedAction, string remarks)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.pmsrvwhdrs SET dh_rmk=@dh_rmk, dh_rec=@dh_rec, ");
            sb.Append("dh_nm=@dh_nm  WHERE (rvw_hdr_id=@rvw_hdr_id);");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                    var dh_nm = cmd.Parameters.Add("@dh_nm", NpgsqlDbType.Text);
                    var dh_rec = cmd.Parameters.Add("@dh_rec", NpgsqlDbType.Text);
                    var dh_rmk = cmd.Parameters.Add("@dh_rmk", NpgsqlDbType.Text);
                    cmd.Prepare();
                    rvw_hdr_id.Value = reviewHeaderId;
                    dh_nm.Value = deptHeadName;
                    dh_rec.Value = recommendedAction;
                    dh_rmk.Value = remarks;

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

        public async Task<bool> UpdateHrRecommendationAsync(int reviewHeaderId, string hrRepName, string recommendedAction, string remarks)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.pmsrvwhdrs SET hr_rmk=@hr_rmk, hr_rec=@hr_rec, ");
            sb.Append("hr_nm=@hr_nm  WHERE (rvw_hdr_id=@rvw_hdr_id);");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                    var hr_nm = cmd.Parameters.Add("@hr_nm", NpgsqlDbType.Text);
                    var hr_rec = cmd.Parameters.Add("@hr_rec", NpgsqlDbType.Text);
                    var hr_rmk = cmd.Parameters.Add("@hr_rmk", NpgsqlDbType.Text);
                    cmd.Prepare();
                    rvw_hdr_id.Value = reviewHeaderId;
                    hr_nm.Value = hrRepName;
                    hr_rec.Value = recommendedAction;
                    hr_rmk.Value = remarks;

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

        public async Task<bool> UpdateManagementDecisionAsync(int reviewHeaderId, string mgtRepName, string recommendedAction, string remarks)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.pmsrvwhdrs SET mgt_rmk=@mgt_rmk, mgt_dec=@mgt_dec, ");
            sb.Append("mgt_nm=@mgt_nm  WHERE (rvw_hdr_id=@rvw_hdr_id);");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                    var mgt_nm = cmd.Parameters.Add("@mgt_nm", NpgsqlDbType.Text);
                    var mgt_dec = cmd.Parameters.Add("@mgt_dec", NpgsqlDbType.Text);
                    var mgt_rmk = cmd.Parameters.Add("@mgt_rmk", NpgsqlDbType.Text);
                    cmd.Prepare();
                    rvw_hdr_id.Value = reviewHeaderId;
                    mgt_nm.Value = mgtRepName;
                    mgt_dec.Value = recommendedAction;
                    mgt_rmk.Value = remarks;

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
