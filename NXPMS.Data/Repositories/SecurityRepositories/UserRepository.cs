using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using NXPMS.Base.Models.SecurityModels;
using NXPMS.Base.Repositories.SecurityRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NXPMS.Data.Repositories.SecurityRepositories
{
    public class UserRepository : IUserRepository
    {
        public IConfiguration _config { get; }
        public UserRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region Application User Read Action Methods
        public async Task<IList<ApplicationUser>> GetByUsernameAsync(string username)
        {
            if (String.IsNullOrEmpty(username)) { throw new ArgumentNullException("The required parameter [Username] is null or has an invalid value."); }

            List<ApplicationUser> applicationUserList = new List<ApplicationUser>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT u.usr_id, u.usr_nm, u.fullname, u.emp_id, u.usr_afc, u.lck_enb, ");
            sb.Append("u.lck_end, u.usr_pwh, u.usr_stp, u.usr_tfe, u.coy_cd, u.usr_cb, u.usr_mb, ");
            sb.Append("u.usr_cd, u.usr_md, u.is_sys, u.is_dlt, u.dlt_by, u.dlt_tm, e.sex, e.loc_id, ");
            sb.Append("e.dept_cd, e.unit_cd, l.locname, d.dept_nm, n.unit_nm FROM public.syssctusr u  ");
            sb.Append("LEFT JOIN ermempinf e ON e.empid = u.emp_id ");
            sb.Append("LEFT JOIN syscfglocs l ON l.locid = e.loc_id ");
            sb.Append("LEFT JOIN syscfgdpts d ON d.dept_cd = e.dept_cd ");
            sb.Append("LEFT JOIN syscfgunts n ON n.unit_cd = e.unit_cd ");
            sb.Append("WHERE LOWER(u.usr_nm) = LOWER(@usr_nm) ");
            sb.Append("AND is_dlt = false;");
            query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var usr_nm = cmd.Parameters.Add("@usr_nm", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                usr_nm.Value = username;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    applicationUserList.Add(new ApplicationUser()
                    {
                        Id = reader["usr_id"] == DBNull.Value ? 0 : (int)(reader["usr_id"]),
                        Username = reader["usr_nm"] == DBNull.Value ? string.Empty : (reader["usr_nm"]).ToString(),
                        AccessFailedCount = reader["usr_afc"] == DBNull.Value ? 0 : (int)reader["usr_afc"],
                        EmployeeId = reader["emp_id"] == DBNull.Value ? 0 : (int)reader["emp_id"],
                        FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                        LockoutEnabled = reader["lck_enb"] == DBNull.Value ? false : (bool)reader["lck_enb"],
                        LockoutEnd = reader["lck_end"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["lck_end"]),
                        PasswordHash = reader["usr_pwh"] == DBNull.Value ? string.Empty : reader["usr_pwh"].ToString(),
                        SecurityStamp = reader["usr_stp"] == DBNull.Value ? string.Empty : reader["usr_stp"].ToString(),
                        TwoFactorEnabled = reader["usr_tfe"] == DBNull.Value ? false : (bool)reader["usr_tfe"],
                        ModifiedBy = reader["usr_mb"] == DBNull.Value ? string.Empty : reader["usr_mb"].ToString(),
                        ModifiedTime = reader["usr_md"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["usr_md"],
                        CreatedTime = reader["usr_cd"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["usr_cd"],
                        CreatedBy = reader["usr_cb"] == DBNull.Value ? string.Empty : reader["usr_cb"].ToString(),
                        Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                        DepartmentCode = reader["dept_cd"] == DBNull.Value ? string.Empty : reader["dept_cd"].ToString(),
                        DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                        UnitCode = reader["unit_cd"] == DBNull.Value ? string.Empty : reader["unit_cd"].ToString(),
                        UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                        LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return applicationUserList;
        }

        public async Task<IList<ApplicationUser>> GetOtherUsersWithSameUsernameAsync(int userId, string username)
        {
            if (String.IsNullOrEmpty(username)) { throw new ArgumentNullException("The required parameter [Username] is null or has an invalid value."); }
            if (userId < 1) { throw new ArgumentNullException("The required parameter [UserID] is null or has an invalid value."); }

            List<ApplicationUser> applicationUserList = new List<ApplicationUser>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT u.usr_id, u.usr_nm, u.fullname, u.emp_id, u.usr_afc, u.lck_enb, ");
            sb.Append("u.lck_end, u.usr_pwh, u.usr_stp, u.usr_tfe, u.coy_cd, u.usr_cb, u.usr_mb, ");
            sb.Append("u.usr_cd, u.usr_md, u.is_sys, u.is_dlt, u.dlt_by, u.dlt_tm, e.sex, e.loc_id, ");
            sb.Append("e.dept_cd, e.unit_cd, l.locname, d.dept_nm, n.unit_nm FROM public.syssctusr u  ");
            sb.Append("LEFT JOIN ermempinf e ON e.empid = u.emp_id ");
            sb.Append("LEFT JOIN syscfglocs l ON l.locid = e.loc_id ");
            sb.Append("LEFT JOIN syscfgdpts d ON d.dept_cd = e.dept_cd ");
            sb.Append("LEFT JOIN syscfgunts n ON n.unit_cd = e.unit_cd ");
            sb.Append("WHERE (LOWER(u.usr_nm) = LOWER(@usr_nm)) ");
            sb.Append("AND (u.usr_id <> @usr_id) AND (u.is_dlt = false); ");
            query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var usr_nm = cmd.Parameters.Add("@usr_nm", NpgsqlDbType.Text);
                var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                usr_nm.Value = username;
                usr_id.Value = userId;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    applicationUserList.Add(new ApplicationUser()
                    {
                        Id = reader["usr_id"] == DBNull.Value ? 0 : (int)(reader["usr_id"]),
                        Username = reader["usr_nm"] == DBNull.Value ? string.Empty : (reader["usr_nm"]).ToString(),
                        AccessFailedCount = reader["usr_afc"] == DBNull.Value ? 0 : (int)reader["usr_afc"],
                        EmployeeId = reader["emp_id"] == DBNull.Value ? 0 : (int)reader["emp_id"],
                        FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                        LockoutEnabled = reader["lck_enb"] == DBNull.Value ? false : (bool)reader["lck_enb"],
                        LockoutEnd = reader["lck_end"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["lck_end"]),
                        PasswordHash = reader["usr_pwh"] == DBNull.Value ? string.Empty : reader["usr_pwh"].ToString(),
                        SecurityStamp = reader["usr_stp"] == DBNull.Value ? string.Empty : reader["usr_stp"].ToString(),
                        TwoFactorEnabled = reader["usr_tfe"] == DBNull.Value ? false : (bool)reader["usr_tfe"],
                        ModifiedBy = reader["usr_mb"] == DBNull.Value ? string.Empty : reader["usr_mb"].ToString(),
                        ModifiedTime = reader["usr_md"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["usr_md"],
                        CreatedTime = reader["usr_cd"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["usr_cd"],
                        CreatedBy = reader["usr_cb"] == DBNull.Value ? string.Empty : reader["usr_cb"].ToString(),
                        Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                        DepartmentCode = reader["dept_cd"] == DBNull.Value ? string.Empty : reader["dept_cd"].ToString(),
                        DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                        UnitCode = reader["unit_cd"] == DBNull.Value ? string.Empty : reader["unit_cd"].ToString(),
                        UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                        LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return applicationUserList;
        }

        public async Task<IList<ApplicationUser>> GetByIdAsync(int userId)
        {
            if (userId < 1) { throw new ArgumentNullException("The required parameter [UserID] is null or has an invalid value."); }

            List<ApplicationUser> applicationUserList = new List<ApplicationUser>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT u.usr_id, u.usr_nm, u.fullname, u.emp_id, u.usr_afc, u.lck_enb, ");
            sb.Append("u.lck_end, u.usr_pwh, u.usr_stp, u.usr_tfe, u.coy_cd, u.usr_cb, u.usr_mb, ");
            sb.Append("u.usr_cd, u.usr_md, u.is_sys, u.is_dlt, u.dlt_by, u.dlt_tm, e.sex, e.loc_id, ");
            sb.Append("e.dept_cd, e.unit_cd, l.locname, d.dept_nm, n.unit_nm FROM public.syssctusr u  ");
            sb.Append("LEFT JOIN ermempinf e ON e.empid = u.emp_id ");
            sb.Append("LEFT JOIN syscfglocs l ON l.locid = e.loc_id ");
            sb.Append("LEFT JOIN syscfgdpts d ON d.dept_cd = e.dept_cd ");
            sb.Append("LEFT JOIN syscfgunts n ON n.unit_cd = e.unit_cd ");
            sb.Append("WHERE (usr_id = @usr_id) ");
            sb.Append("AND (is_dlt = false);");

            query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                usr_id.Value = userId;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    applicationUserList.Add(new ApplicationUser()
                    {
                        Id = reader["usr_id"] == DBNull.Value ? 0 : (int)(reader["usr_id"]),
                        Username = reader["usr_nm"] == DBNull.Value ? string.Empty : (reader["usr_nm"]).ToString(),
                        AccessFailedCount = reader["usr_afc"] == DBNull.Value ? 0 : (int)reader["usr_afc"],
                        EmployeeId = reader["emp_id"] == DBNull.Value ? 0 : (int)reader["emp_id"],
                        FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                        LockoutEnabled = reader["lck_enb"] == DBNull.Value ? false : (bool)reader["lck_enb"],
                        LockoutEnd = reader["lck_end"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["lck_end"]),
                        PasswordHash = reader["usr_pwh"] == DBNull.Value ? string.Empty : reader["usr_pwh"].ToString(),
                        SecurityStamp = reader["usr_stp"] == DBNull.Value ? string.Empty : reader["usr_stp"].ToString(),
                        TwoFactorEnabled = reader["usr_tfe"] == DBNull.Value ? false : (bool)reader["usr_tfe"],
                        ModifiedBy = reader["usr_mb"] == DBNull.Value ? string.Empty : reader["usr_mb"].ToString(),
                        ModifiedTime = reader["usr_md"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["usr_md"],
                        CreatedTime = reader["usr_cd"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["usr_cd"],
                        CreatedBy = reader["usr_cb"] == DBNull.Value ? string.Empty : reader["usr_cb"].ToString(),
                        Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                        DepartmentCode = reader["dept_cd"] == DBNull.Value ? string.Empty : reader["dept_cd"].ToString(),
                        DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                        UnitCode = reader["unit_cd"] == DBNull.Value ? string.Empty : reader["unit_cd"].ToString(),
                        UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                        LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return applicationUserList;
        }

        public async Task<IList<ApplicationUser>> FindByNameAsync(string fullname)
        {
            if (String.IsNullOrEmpty(fullname)) { throw new ArgumentNullException("The required parameter [Full Name] is null or has an invalid value."); }

            List<ApplicationUser> applicationUserList = new List<ApplicationUser>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT u.usr_id, u.usr_nm, u.fullname, u.emp_id, u.usr_afc, u.lck_enb, ");
            sb.Append("u.lck_end, u.usr_pwh, u.usr_stp, u.usr_tfe, u.coy_cd, u.usr_cb, u.usr_mb, ");
            sb.Append("u.usr_cd, u.usr_md, u.is_sys, u.is_dlt, u.dlt_by, u.dlt_tm, e.sex, e.loc_id, ");
            sb.Append("e.dept_cd, e.unit_cd, l.locname, d.dept_nm, n.unit_nm FROM public.syssctusr u  ");
            sb.Append("LEFT JOIN ermempinf e ON e.empid = u.emp_id ");
            sb.Append("LEFT JOIN syscfglocs l ON l.locid = e.loc_id ");
            sb.Append("LEFT JOIN syscfgdpts d ON d.dept_cd = e.dept_cd ");
            sb.Append("LEFT JOIN syscfgunts n ON n.unit_cd = e.unit_cd ");
            sb.Append("WHERE (LOWER(u.fullname) LIKE '%'||LOWER(@f_nm)||'%') ");
            sb.Append("AND (is_dlt = false);");
            query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var f_nm = cmd.Parameters.Add("@f_nm", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                f_nm.Value = fullname;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    applicationUserList.Add(new ApplicationUser()
                    {
                        Id = reader["usr_id"] == DBNull.Value ? 0 : (int)(reader["usr_id"]),
                        Username = reader["usr_nm"] == DBNull.Value ? string.Empty : (reader["usr_nm"]).ToString(),
                        AccessFailedCount = reader["usr_afc"] == DBNull.Value ? 0 : (int)reader["usr_afc"],
                        EmployeeId = reader["emp_id"] == DBNull.Value ? 0 : (int)reader["emp_id"],
                        FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                        LockoutEnabled = reader["lck_enb"] == DBNull.Value ? false : (bool)reader["lck_enb"],
                        LockoutEnd = reader["lck_end"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["lck_end"]),
                        PasswordHash = reader["usr_pwh"] == DBNull.Value ? string.Empty : reader["usr_pwh"].ToString(),
                        SecurityStamp = reader["usr_stp"] == DBNull.Value ? string.Empty : reader["usr_stp"].ToString(),
                        TwoFactorEnabled = reader["usr_tfe"] == DBNull.Value ? false : (bool)reader["usr_tfe"],
                        ModifiedBy = reader["usr_mb"] == DBNull.Value ? string.Empty : reader["usr_mb"].ToString(),
                        ModifiedTime = reader["usr_md"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["usr_md"],
                        CreatedTime = reader["usr_cd"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["usr_cd"],
                        CreatedBy = reader["usr_cb"] == DBNull.Value ? string.Empty : reader["usr_cb"].ToString(),
                        Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                        DepartmentCode = reader["dept_cd"] == DBNull.Value ? string.Empty : reader["dept_cd"].ToString(),
                        DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                        UnitCode = reader["unit_cd"] == DBNull.Value ? string.Empty : reader["unit_cd"].ToString(),
                        UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                        LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return applicationUserList;
        }

        public async Task<IList<ApplicationUser>> GetByNameAsync(string fullname)
        {
            if (String.IsNullOrEmpty(fullname)) { throw new ArgumentNullException("The required parameter [Full Name] is null or has an invalid value."); }

            List<ApplicationUser> applicationUserList = new List<ApplicationUser>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT u.usr_id, u.usr_nm, u.fullname, u.emp_id, u.usr_afc, u.lck_enb, ");
            sb.Append("u.lck_end, u.usr_pwh, u.usr_stp, u.usr_tfe, u.coy_cd, u.usr_cb, u.usr_mb, ");
            sb.Append("u.usr_cd, u.usr_md, u.is_sys, u.is_dlt, u.dlt_by, u.dlt_tm, e.sex, e.loc_id, ");
            sb.Append("e.dept_cd, e.unit_cd, l.locname, d.dept_nm, n.unit_nm FROM public.syssctusr u  ");
            sb.Append("LEFT JOIN ermempinf e ON e.empid = u.emp_id ");
            sb.Append("LEFT JOIN syscfglocs l ON l.locid = e.loc_id ");
            sb.Append("LEFT JOIN syscfgdpts d ON d.dept_cd = e.dept_cd ");
            sb.Append("LEFT JOIN syscfgunts n ON n.unit_cd = e.unit_cd ");
            sb.Append("WHERE (LOWER(u.fullname) = LOWER(@f_nm)) ");
            sb.Append("AND (is_dlt = false);");
            query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var f_nm = cmd.Parameters.Add("@f_nm", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                f_nm.Value = fullname;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    applicationUserList.Add(new ApplicationUser()
                    {
                        Id = reader["usr_id"] == DBNull.Value ? 0 : (int)(reader["usr_id"]),
                        Username = reader["usr_nm"] == DBNull.Value ? string.Empty : (reader["usr_nm"]).ToString(),
                        AccessFailedCount = reader["usr_afc"] == DBNull.Value ? 0 : (int)reader["usr_afc"],
                        EmployeeId = reader["emp_id"] == DBNull.Value ? 0 : (int)reader["emp_id"],
                        FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                        LockoutEnabled = reader["lck_enb"] == DBNull.Value ? false : (bool)reader["lck_enb"],
                        LockoutEnd = reader["lck_end"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["lck_end"]),
                        PasswordHash = reader["usr_pwh"] == DBNull.Value ? string.Empty : reader["usr_pwh"].ToString(),
                        SecurityStamp = reader["usr_stp"] == DBNull.Value ? string.Empty : reader["usr_stp"].ToString(),
                        TwoFactorEnabled = reader["usr_tfe"] == DBNull.Value ? false : (bool)reader["usr_tfe"],
                        ModifiedBy = reader["usr_mb"] == DBNull.Value ? string.Empty : reader["usr_mb"].ToString(),
                        ModifiedTime = reader["usr_md"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["usr_md"],
                        CreatedTime = reader["usr_cd"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["usr_cd"],
                        CreatedBy = reader["usr_cb"] == DBNull.Value ? string.Empty : reader["usr_cb"].ToString(),
                        Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                        DepartmentCode = reader["dept_cd"] == DBNull.Value ? string.Empty : reader["dept_cd"].ToString(),
                        DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                        UnitCode = reader["unit_cd"] == DBNull.Value ? string.Empty : reader["unit_cd"].ToString(),
                        UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                        LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return applicationUserList;
        }

        public async Task<IList<ApplicationUser>> GetByLocationIdAsync(int locationId)
        {
            if (locationId < 1) { throw new ArgumentNullException("The required parameter [LocationID] is null or has an invalid value."); }

            List<ApplicationUser> applicationUserList = new List<ApplicationUser>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT u.usr_id, u.usr_nm, u.fullname, u.emp_id, u.usr_afc, u.lck_enb, ");
            sb.Append("u.lck_end, u.usr_pwh, u.usr_stp, u.usr_tfe, u.coy_cd, u.usr_cb, u.usr_mb, ");
            sb.Append("u.usr_cd, u.usr_md, u.is_sys, u.is_dlt, u.dlt_by, u.dlt_tm, e.sex, e.loc_id, ");
            sb.Append("e.dept_cd, e.unit_cd, l.locname, d.dept_nm, n.unit_nm FROM public.syssctusr u  ");
            sb.Append("LEFT JOIN ermempinf e ON e.empid = u.emp_id ");
            sb.Append("LEFT JOIN syscfglocs l ON l.locid = e.loc_id ");
            sb.Append("LEFT JOIN syscfgdpts d ON d.dept_cd = e.dept_cd ");
            sb.Append("LEFT JOIN syscfgunts n ON n.unit_cd = e.unit_cd ");
            sb.Append("WHERE (e.loc_id = @loc_id) OR (e.loc_id IS NULL) ");
            sb.Append("AND (is_dlt = false);");

            query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                usr_id.Value = locationId;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    applicationUserList.Add(new ApplicationUser()
                    {
                        Id = reader["usr_id"] == DBNull.Value ? 0 : (int)(reader["usr_id"]),
                        Username = reader["usr_nm"] == DBNull.Value ? string.Empty : (reader["usr_nm"]).ToString(),
                        AccessFailedCount = reader["usr_afc"] == DBNull.Value ? 0 : (int)reader["usr_afc"],
                        EmployeeId = reader["emp_id"] == DBNull.Value ? 0 : (int)reader["emp_id"],
                        FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                        LockoutEnabled = reader["lck_enb"] == DBNull.Value ? false : (bool)reader["lck_enb"],
                        LockoutEnd = reader["lck_end"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["lck_end"]),
                        PasswordHash = reader["usr_pwh"] == DBNull.Value ? string.Empty : reader["usr_pwh"].ToString(),
                        SecurityStamp = reader["usr_stp"] == DBNull.Value ? string.Empty : reader["usr_stp"].ToString(),
                        TwoFactorEnabled = reader["usr_tfe"] == DBNull.Value ? false : (bool)reader["usr_tfe"],
                        ModifiedBy = reader["usr_mb"] == DBNull.Value ? string.Empty : reader["usr_mb"].ToString(),
                        ModifiedTime = reader["usr_md"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["usr_md"],
                        CreatedTime = reader["usr_cd"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["usr_cd"],
                        CreatedBy = reader["usr_cb"] == DBNull.Value ? string.Empty : reader["usr_cb"].ToString(),
                        Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                        DepartmentCode = reader["dept_cd"] == DBNull.Value ? string.Empty : reader["dept_cd"].ToString(),
                        DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                        UnitCode = reader["unit_cd"] == DBNull.Value ? string.Empty : reader["unit_cd"].ToString(),
                        UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                        LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return applicationUserList;
        }

        public async Task<IList<ApplicationUser>> GetAllAsync()
        {
            List<ApplicationUser> applicationUserList = new List<ApplicationUser>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT u.usr_id, u.usr_nm, u.fullname, u.emp_id, u.usr_afc, u.lck_enb, ");
            sb.Append("u.lck_end, u.usr_pwh, u.usr_stp, u.usr_tfe, u.coy_cd, u.usr_cb, u.usr_mb, ");
            sb.Append("u.usr_cd, u.usr_md, u.is_sys, u.is_dlt, u.dlt_by, u.dlt_tm, e.sex, e.loc_id, ");
            sb.Append("e.dept_cd, e.unit_cd, l.locname, d.dept_nm, n.unit_nm FROM public.syssctusr u  ");
            sb.Append("LEFT JOIN ermempinf e ON e.empid = u.emp_id ");
            sb.Append("LEFT JOIN syscfglocs l ON l.locid = e.loc_id ");
            sb.Append("LEFT JOIN syscfgdpts d ON d.dept_cd = e.dept_cd ");
            sb.Append("LEFT JOIN syscfgunts n ON n.unit_cd = e.unit_cd ");
            sb.Append("WHERE (is_dlt = false); ");
            query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    applicationUserList.Add(new ApplicationUser()
                    {
                        Id = reader["usr_id"] == DBNull.Value ? 0 : (int)(reader["usr_id"]),
                        Username = reader["usr_nm"] == DBNull.Value ? string.Empty : (reader["usr_nm"]).ToString(),
                        AccessFailedCount = reader["usr_afc"] == DBNull.Value ? 0 : (int)reader["usr_afc"],
                        EmployeeId = reader["emp_id"] == DBNull.Value ? 0 : (int)reader["emp_id"],
                        FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                        LockoutEnabled = reader["lck_enb"] == DBNull.Value ? false : (bool)reader["lck_enb"],
                        LockoutEnd = reader["lck_end"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["lck_end"]),
                        PasswordHash = reader["usr_pwh"] == DBNull.Value ? string.Empty : reader["usr_pwh"].ToString(),
                        SecurityStamp = reader["usr_stp"] == DBNull.Value ? string.Empty : reader["usr_stp"].ToString(),
                        TwoFactorEnabled = reader["usr_tfe"] == DBNull.Value ? false : (bool)reader["usr_tfe"],
                        ModifiedBy = reader["usr_mb"] == DBNull.Value ? string.Empty : reader["usr_mb"].ToString(),
                        ModifiedTime = reader["usr_md"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["usr_md"],
                        CreatedTime = reader["usr_cd"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["usr_cd"],
                        CreatedBy = reader["usr_cb"] == DBNull.Value ? string.Empty : reader["usr_cb"].ToString(),
                        Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                        DepartmentCode = reader["dept_cd"] == DBNull.Value ? string.Empty : reader["dept_cd"].ToString(),
                        DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                        UnitCode = reader["unit_cd"] == DBNull.Value ? string.Empty : reader["unit_cd"].ToString(),
                        UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                        LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return applicationUserList;
        }

        #endregion

        #region Application User Write Action Methods
        public async Task<bool> AddAsync(ApplicationUser applicationUser)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.syssctusr(usr_nm, fullname, emp_id, ");
            sb.Append("usr_pwh, usr_tfe, usr_cb, usr_mb, usr_cd, usr_md) ");
            sb.Append("VALUES (@usr_nm, @fullname, @emp_id,  @usr_pwh,");
            sb.Append(" @usr_tfe, @usr_cb, @usr_mb, @usr_cd, @usr_md);");

            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Integer);
                var usr_nm = cmd.Parameters.Add("@usr_nm", NpgsqlDbType.Text);
                var fullname = cmd.Parameters.Add("@fullname", NpgsqlDbType.Text);
                var usr_pwh = cmd.Parameters.Add("@usr_pwh", NpgsqlDbType.Text);
                var usr_tfe = cmd.Parameters.Add("@usr_tfe", NpgsqlDbType.Boolean);
                var usr_mb = cmd.Parameters.Add("@usr_mb", NpgsqlDbType.Text);
                var usr_md = cmd.Parameters.Add("@usr_md", NpgsqlDbType.TimestampTz);
                var usr_cb = cmd.Parameters.Add("@usr_cb", NpgsqlDbType.Text);
                var usr_cd = cmd.Parameters.Add("@usr_cd", NpgsqlDbType.TimestampTz);
                cmd.Prepare();
                emp_id.Value = applicationUser.EmployeeId;
                usr_nm.Value = applicationUser.Username;
                fullname.Value = applicationUser.FullName;
                usr_pwh.Value = applicationUser.PasswordHash ?? (object)DBNull.Value;
                usr_tfe.Value = applicationUser.TwoFactorEnabled;
                usr_mb.Value = applicationUser.ModifiedBy ?? (object)DBNull.Value;
                usr_md.Value = applicationUser.ModifiedTime ?? (object)DBNull.Value;
                usr_cb.Value = applicationUser.CreatedBy ?? (object)DBNull.Value;
                usr_cd.Value = applicationUser.CreatedTime ?? (object)DBNull.Value;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> UpdateAsync(ApplicationUser applicationUser)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.syssctusr SET usr_nm=@usr_nm, fullname=@fullname, ");
            sb.Append("emp_id=@emp_id, lck_enb=@lck_enb, lck_end=@lck_end, usr_tfe=@usr_tfe,");
            sb.Append("usr_mb=@usr_mb, usr_md=@usr_md WHERE (usr_id=@usr_id);");

            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Integer);
                var usr_nm = cmd.Parameters.Add("@usr_nm", NpgsqlDbType.Text);
                var fullname = cmd.Parameters.Add("@fullname", NpgsqlDbType.Text);
                var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Integer);
                var usr_tfe = cmd.Parameters.Add("@usr_tfe", NpgsqlDbType.Boolean);
                var lck_enb = cmd.Parameters.Add("@lck_enb", NpgsqlDbType.Boolean);
                var lck_end = cmd.Parameters.Add("@lck_end", NpgsqlDbType.TimestampTz);
                var usr_mb = cmd.Parameters.Add("@usr_mb", NpgsqlDbType.Text);
                var usr_md = cmd.Parameters.Add("@usr_md", NpgsqlDbType.Text);

                cmd.Prepare();
                usr_id.Value = applicationUser.Id;
                usr_nm.Value = applicationUser.Username;
                fullname.Value = applicationUser.FullName;
                emp_id.Value = applicationUser.EmployeeId;
                usr_tfe.Value = applicationUser.TwoFactorEnabled;
                lck_enb.Value = applicationUser.LockoutEnabled;
                lck_end.Value = applicationUser.LockoutEnd ?? (object)DBNull.Value;
                usr_mb.Value = applicationUser.ModifiedBy ?? (object)DBNull.Value;
                usr_md.Value = applicationUser.ModifiedTime ?? (object)DBNull.Value;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> UpdateUserPasswordAsync(ApplicationUser applicationUser)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.syssctusr SET usr_pwh=@usr_pwh, usr_md=@usr_md, ");
            sb.Append("usr_mb=@usr_mb WHERE usr_id=@usr_id;");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Integer);
                var usr_pwh = cmd.Parameters.Add("@usr_pwh", NpgsqlDbType.Text);
                var usr_mb = cmd.Parameters.Add("@usr_mb", NpgsqlDbType.Text);
                var usr_md = cmd.Parameters.Add("@usr_md", NpgsqlDbType.TimestampTz);
                cmd.Prepare();
                usr_id.Value = applicationUser.Id;
                usr_pwh.Value = applicationUser.PasswordHash ?? (object)DBNull.Value;
                usr_mb.Value = applicationUser.ModifiedBy ?? (object)DBNull.Value;
                usr_md.Value = applicationUser.ModifiedTime ?? DateTime.UtcNow;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteByIdAsync(int userId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.syssctusr SET is_dlt=true, usr_md=@usr_md, ");
            sb.Append("usr_mb=@usr_mb WHERE usr_id=@usr_id;");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var Id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                Id.Value = userId;
                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }
        #endregion

        #region Logins Action Methods
        public async Task<bool> AddAttemptAsync(LoginAttempt loginAttempt)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.syssctlgns(usr_nm, lgn_ss, lgn_tm) ");
            sb.Append("VALUES (@usr_nm, @lgn_ss, @lgn_tm);");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var usr_nm = cmd.Parameters.Add("@usr_nm", NpgsqlDbType.Text);
                var lgn_ss = cmd.Parameters.Add("@lgn_ss", NpgsqlDbType.Boolean);
                var lgn_tm = cmd.Parameters.Add("@lgn_tm", NpgsqlDbType.TimestampTz);
                cmd.Prepare();
                usr_nm.Value = loginAttempt.Username;
                lgn_ss.Value = loginAttempt.IsSuccessful;
                lgn_tm.Value = loginAttempt.LoginTime ?? DateTime.UtcNow;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> IncreaseFailedLoginCountAsync(string userName)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.syssctusr SET usr_afc = usr_afc + 1");
            sb.Append("WHERE (LOWER(usr_nm) = LOWER(@usr_nm));");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var usr_nm = cmd.Parameters.Add("@usr_nm", NpgsqlDbType.Text);
                cmd.Prepare();
                usr_nm.Value = userName;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> ClearFailedLoginCountAsync(string userName)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.syssctusr SET usr_afc = 0");
            sb.Append("WHERE LOWER(usr_nm) = LOWER(@usr_nm);");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var usr_nm = cmd.Parameters.Add("@usr_nm", NpgsqlDbType.Text);
                cmd.Prepare();
                usr_nm.Value = userName;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<int> GetFailedLoginCountByUsernameAsync(string username)
        {
            if (String.IsNullOrEmpty(username)) { throw new ArgumentNullException("The required parameter [Username] is null or has an invalid value."); }
            int afc_count = 0;
            List<ApplicationUser> applicationUserList = new List<ApplicationUser>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            query = "SELECT usr_afc FROM public.syssctusr WHERE LOWER(usr_nm) = LOWER(@usr_nm); ";
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var usr_nm = cmd.Parameters.Add("@usr_nm", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                usr_nm.Value = username;
                 var count = await cmd.ExecuteScalarAsync();
                afc_count = (int)count;
             }
            await conn.CloseAsync();
            return afc_count;
        }

        public async Task<bool> UpdateLockOutStatusAsync(string userName, bool setToLock, DateTime? endLockDate = null)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.syssctusr SET lck_enb=@lck_enb, lck_end=@lck_end ");
            sb.Append("WHERE (LOWER(usr_nm) = LOWER(@usr_nm));");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var usr_nm = cmd.Parameters.Add("@usr_nm", NpgsqlDbType.Text);
                var lck_enb = cmd.Parameters.Add("@lck_enb", NpgsqlDbType.Boolean);
                var lck_end = cmd.Parameters.Add("@lck_end", NpgsqlDbType.TimestampTz);
                cmd.Prepare();
                usr_nm.Value = userName;
                lck_enb.Value = setToLock;
                lck_end.Value = endLockDate ?? (object)DBNull.Value;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }


        #endregion

    }
}
