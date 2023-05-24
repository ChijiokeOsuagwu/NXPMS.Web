using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using NXPMS.Base.Models.EmployeesModels;
using NXPMS.Base.Repositories.EmployeeRecordRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NXPMS.Data.Repositories.EmployeeRecordRepositories
{
    public class EmployeeReportRepository : IEmployeeReportRepository
    {
        public IConfiguration _config { get; }
        public EmployeeReportRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region Employee Reports Read Action Methods
        public async Task<IList<EmployeeReport>> GetByEmployeeIdAsync(int employeeId)
        {
            if (employeeId < 1) { throw new ArgumentNullException("The required parameter [Employee ID] is null or has an invalid value."); }

            List<EmployeeReport> employeeReportsList = new List<EmployeeReport>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.emp_rpt_id, r.emp_id, r.rpt_to_id, ");
            sb.Append("r.rpt_dsg, r.strt_dt, r.ndt_dt, r.is_cur, r.mdb, ");
            sb.Append("r.unit_cd, r.dept_cd, r.loc_id, r.ctb, r.mdt, r.ctt, ");
            sb.Append("e.fullname AS emp_name, f.fullname AS rpt_to_name, ");
            sb.Append("u.unit_nm, d.dept_nm, l.locname ");
            sb.Append("FROM public.ermemprpts r ");
            sb.Append("INNER JOIN public.ermempinf e ON e.empid = r.emp_id ");
            sb.Append("INNER JOIN public.ermempinf f ON f.empid = r.rpt_to_id ");
            sb.Append("LEFT JOIN public.syscfgunts u ON u.unit_cd = r.unit_cd ");
            sb.Append("LEFT JOIN public.syscfgdpts d ON d.dept_cd = r.dept_cd ");
            sb.Append("LEFT JOIN public.syscfglocs l ON l.locid = r.loc_id ");
            sb.Append("WHERE (r.emp_id = @emp_id) ORDER BY r.emp_rpt_id; ");

            query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                emp_id.Value = employeeId;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    employeeReportsList.Add(new EmployeeReport()
                    {
                        EmployeeReportId = reader["emp_rpt_id"] == DBNull.Value ? 0 : (int)reader["emp_rpt_id"],
                        EmployeeId = reader["emp_id"] == DBNull.Value ? 0 : (int)reader["emp_id"],
                        EmployeeName = reader["emp_name"] == DBNull.Value ? string.Empty : (reader["emp_name"]).ToString(),
                        ReportsToId = reader["rpt_to_id"] == DBNull.Value ? 0 : (int)reader["rpt_to_id"],
                        ReportsToName = reader["rpt_to_name"] == DBNull.Value ? string.Empty : (reader["rpt_to_name"]).ToString(),
                        ReportsToDesignation = reader["rpt_dsg"] == DBNull.Value ? string.Empty : (reader["rpt_dsg"]).ToString(),
                        StartDate = reader["strt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["strt_dt"],
                        EndDate = reader["ndt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ndt_dt"],
                        IsCurrent = reader["is_cur"] == DBNull.Value ? false : (bool)reader["is_cur"],
                        ReportsToUnitCode = reader["unit_cd"] == DBNull.Value ? string.Empty : reader["unit_cd"].ToString(),
                        ReportsToUnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                        ReportsToDepartmentCode = reader["dept_cd"] == DBNull.Value ? string.Empty : reader["dept_cd"].ToString(),
                        ReportsToDepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                        ReportsToLocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        ReportsToLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                        CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                        CreatedTime = reader["ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctt"],
                        ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                        ModifiedTime = reader["mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdt"],
                    });
                }
            }
            await conn.CloseAsync();
            return employeeReportsList;
        }

        public async Task<IList<EmployeeReport>> GetByIdAsync(int employeeReportId)
        {
            if (employeeReportId < 1) { throw new ArgumentNullException("The required parameter [EmployeeReportID] is null or has an invalid value."); }

            List<EmployeeReport> employeeReportsList = new List<EmployeeReport>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.emp_rpt_id, r.emp_id, r.rpt_to_id, ");
            sb.Append("r.rpt_dsg, r.strt_dt, r.ndt_dt, r.is_cur, r.mdb, ");
            sb.Append("r.unit_cd, r.dept_cd, r.loc_id, r.ctb, r.mdt, r.ctt, ");
            sb.Append("e.fullname AS emp_name, f.fullname AS rpt_to_name, ");
            sb.Append("u.unit_nm, d.dept_nm, l.locname ");
            sb.Append("FROM public.ermemprpts r ");
            sb.Append("INNER JOIN public.ermempinf e ON e.empid = r.emp_id ");
            sb.Append("INNER JOIN public.ermempinf f ON f.empid = r.rpt_to_id ");
            sb.Append("LEFT JOIN public.syscfgunts u ON u.unit_cd = r.unit_cd ");
            sb.Append("LEFT JOIN public.syscfgdpts d ON d.dept_cd = r.dept_cd ");
            sb.Append("LEFT JOIN public.syscfglocs l ON l.locid = r.loc_id ");
            sb.Append("WHERE (r.emp_rpt_id = @emp_rpt_id); ");

            query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var emp_rpt_id = cmd.Parameters.Add("@emp_rpt_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                emp_rpt_id.Value = employeeReportId;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    employeeReportsList.Add(new EmployeeReport()
                    {
                        EmployeeReportId = reader["emp_rpt_id"] == DBNull.Value ? 0 : (int)reader["emp_rpt_id"],
                        EmployeeId = reader["emp_id"] == DBNull.Value ? 0 : (int)reader["emp_id"],
                        EmployeeName = reader["emp_name"] == DBNull.Value ? string.Empty : (reader["emp_name"]).ToString(),
                        ReportsToId = reader["rpt_to_id"] == DBNull.Value ? 0 : (int)reader["rpt_to_id"],
                        ReportsToName = reader["rpt_to_name"] == DBNull.Value ? string.Empty : (reader["rpt_to_name"]).ToString(),
                        ReportsToDesignation = reader["rpt_dsg"] == DBNull.Value ? string.Empty : (reader["rpt_dsg"]).ToString(),
                        StartDate = reader["strt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["strt_dt"],
                        EndDate = reader["ndt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ndt_dt"],
                        IsCurrent = reader["is_cur"] == DBNull.Value ? false : (bool)reader["is_cur"],
                        ReportsToUnitCode = reader["unit_cd"] == DBNull.Value ? string.Empty : reader["unit_cd"].ToString(),
                        ReportsToUnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                        ReportsToDepartmentCode = reader["dept_cd"] == DBNull.Value ? string.Empty : reader["dept_cd"].ToString(),
                        ReportsToDepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                        ReportsToLocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        ReportsToLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                        CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                        CreatedTime = reader["ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctt"],
                        ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                        ModifiedTime = reader["mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdt"],
                    });
                }
            }
            await conn.CloseAsync();
            return employeeReportsList;
        }
        
        public async Task<IList<EmployeeReport>> GetByEmployeeIdAndReportIdAsync(int employeeId, int reportToId)
        {
            if (employeeId < 1) { throw new ArgumentNullException("The required parameter [EmployeeId] is null or has an invalid value."); }
            if (reportToId < 1) { throw new ArgumentNullException("The required parameter [ReportToId] is null or has an invalid value."); }

            List<EmployeeReport> employeeReportsList = new List<EmployeeReport>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.emp_rpt_id, r.emp_id, r.rpt_to_id, ");
            sb.Append("r.rpt_dsg, r.strt_dt, r.ndt_dt, r.is_cur, r.mdb, ");
            sb.Append("r.unit_cd, r.dept_cd, r.loc_id, r.ctb, r.mdt, r.ctt, ");
            sb.Append("e.fullname AS emp_name, f.fullname AS rpt_to_name, ");
            sb.Append("u.unit_nm, d.dept_nm, l.locname ");
            sb.Append("FROM public.ermemprpts r ");
            sb.Append("INNER JOIN public.ermempinf e ON e.empid = r.emp_id ");
            sb.Append("INNER JOIN public.ermempinf f ON f.empid = r.rpt_to_id ");
            sb.Append("LEFT JOIN public.syscfgunts u ON u.unit_cd = r.unit_cd ");
            sb.Append("LEFT JOIN public.syscfgdpts d ON d.dept_cd = r.dept_cd ");
            sb.Append("LEFT JOIN public.syscfglocs l ON l.locid = r.loc_id ");
            sb.Append("WHERE (r.emp_id = @emp_id) AND (r.rpt_to_id = @rpt_id) ");
            sb.Append("AND (r.is_cur = true) ORDER BY r.emp_rpt_id; ");

            query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Integer);
                var rpt_id = cmd.Parameters.Add("@rpt_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                emp_id.Value = employeeId;
                rpt_id.Value = reportToId;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    employeeReportsList.Add(new EmployeeReport()
                    {
                        EmployeeReportId = reader["emp_rpt_id"] == DBNull.Value ? 0 : (int)reader["emp_rpt_id"],
                        EmployeeId = reader["emp_id"] == DBNull.Value ? 0 : (int)reader["emp_id"],
                        EmployeeName = reader["emp_name"] == DBNull.Value ? string.Empty : (reader["emp_name"]).ToString(),
                        ReportsToId = reader["rpt_to_id"] == DBNull.Value ? 0 : (int)reader["rpt_to_id"],
                        ReportsToName = reader["rpt_to_name"] == DBNull.Value ? string.Empty : (reader["rpt_to_name"]).ToString(),
                        ReportsToDesignation = reader["rpt_dsg"] == DBNull.Value ? string.Empty : (reader["rpt_dsg"]).ToString(),
                        StartDate = reader["strt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["strt_dt"],
                        EndDate = reader["ndt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ndt_dt"],
                        IsCurrent = reader["is_cur"] == DBNull.Value ? false : (bool)reader["is_cur"],
                        ReportsToUnitCode = reader["unit_cd"] == DBNull.Value ? string.Empty : reader["unit_cd"].ToString(),
                        ReportsToUnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                        ReportsToDepartmentCode = reader["dept_cd"] == DBNull.Value ? string.Empty : reader["dept_cd"].ToString(),
                        ReportsToDepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                        ReportsToLocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        ReportsToLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                        CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                        CreatedTime = reader["ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctt"],
                        ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                        ModifiedTime = reader["mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdt"],
                    });
                }
            }
            await conn.CloseAsync();
            return employeeReportsList;
        }

        #endregion

        #region Employee Reports Write Action Methods
        public async Task<bool> AddAsync(EmployeeReport employeeReport)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.ermemprpts(emp_id, rpt_to_id, rpt_dsg, strt_dt, ");
            sb.Append("ndt_dt, is_cur, mdb, ctb, mdt, ctt, unit_cd, dept_cd, loc_id) ");
            sb.Append("VALUES (@emp_id, @rpt_to_id, @rpt_dsg, @strt_dt, @ndt_dt, ");
            sb.Append("@is_cur, @mdb, @ctb, @mdt, @ctt, @unit_cd, @dept_cd, @loc_id); ");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Integer);
                var rpt_to_id = cmd.Parameters.Add("@rpt_to_id", NpgsqlDbType.Integer);
                var rpt_dsg = cmd.Parameters.Add("@rpt_dsg", NpgsqlDbType.Text);
                var strt_dt = cmd.Parameters.Add("@strt_dt", NpgsqlDbType.Date);
                var ndt_dt = cmd.Parameters.Add("@ndt_dt", NpgsqlDbType.Date);
                var is_cur = cmd.Parameters.Add("@is_cur", NpgsqlDbType.Boolean);
                var unit_cd = cmd.Parameters.Add("@unit_cd", NpgsqlDbType.Text);
                var dept_cd = cmd.Parameters.Add("@dept_cd", NpgsqlDbType.Text);
                var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                var mdb = cmd.Parameters.Add("@mdb", NpgsqlDbType.Text);
                var mdt = cmd.Parameters.Add("@mdt", NpgsqlDbType.TimestampTz);
                var ctb = cmd.Parameters.Add("@ctb", NpgsqlDbType.Text);
                var ctt = cmd.Parameters.Add("@ctt", NpgsqlDbType.TimestampTz);
                cmd.Prepare();
                emp_id.Value = employeeReport.EmployeeId;
                rpt_to_id.Value = employeeReport.ReportsToId;
                rpt_dsg.Value = employeeReport.ReportsToDesignation;
                strt_dt.Value = employeeReport.StartDate;
                ndt_dt.Value = employeeReport.EndDate ?? (object)DBNull.Value;
                is_cur.Value = employeeReport.IsCurrent;
                unit_cd.Value = employeeReport.ReportsToUnitCode ?? (object)DBNull.Value;
                dept_cd.Value = employeeReport.ReportsToDepartmentCode ?? (object)DBNull.Value;
                loc_id.Value = employeeReport.ReportsToLocationId ?? (object)DBNull.Value; 
                mdb.Value = employeeReport.ModifiedBy ?? (object)DBNull.Value;
                mdt.Value = employeeReport.ModifiedTime ?? (object)DBNull.Value;
                ctb.Value = employeeReport.CreatedBy ?? (object)DBNull.Value;
                ctt.Value = employeeReport.CreatedTime ?? (object)DBNull.Value;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }
        #endregion
    }
}
