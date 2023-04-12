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
    public class UserPermissionRepository : IUserPermissionRepository
    {
        public IConfiguration _config { get; }
        public UserPermissionRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region User Permissions Write Action Methods

        public async Task<bool> AddUserPermissionAsync(UserPermission userPermission)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.syssctpms(usr_id, rls_cd, pms_mb, pms_md) ");
            sb.Append("VALUES (@usr_id, @rls_cd, @pms_mb, @pms_md);");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Integer);
                var rls_cd = cmd.Parameters.Add("@rls_cd", NpgsqlDbType.Text);
                var pms_mb = cmd.Parameters.Add("@pms_mb", NpgsqlDbType.Text);
                var pms_md = cmd.Parameters.Add("@pms_md", NpgsqlDbType.TimestampTz);
                cmd.Prepare();
                usr_id.Value = userPermission.UserID;
                rls_cd.Value = userPermission.RoleCode;
                pms_mb.Value = userPermission.LastModifiedBy ?? (object)DBNull.Value;
                pms_md.Value = userPermission.LastModifiedTime ?? (object)DBNull.Value;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteUserPermissionAsync(UserPermission userPermission)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.syssctpms ");
            sb.Append("WHERE (usr_id = @usr_id) AND (rls_cd = @rls_cd);");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Integer);
                var rls_cd = cmd.Parameters.Add("@rls_cd", NpgsqlDbType.Text);
                cmd.Prepare();
                usr_id.Value = userPermission.UserID;
                rls_cd.Value = userPermission.RoleCode;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteUserPermissionAsync(int userPermissionId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.syssctpms ");
            sb.Append("WHERE (pms_id = @pms_id);");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var pms_id = cmd.Parameters.Add("@pms_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                pms_id.Value = userPermissionId;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        #endregion

        #region User Permissions Read Action Methods
        public async Task<IList<ApplicationRole>> GetByAppCodeAsync(string applicationCode)
        {
            if (string.IsNullOrWhiteSpace(applicationCode)) { throw new ArgumentNullException("The required parameter [UserID] is null or has an invalid value."); }

            List<ApplicationRole> applicationRoleList = new List<ApplicationRole>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.rl_nm, r.rl_ds, r.rl_rk, r.rl_cd, r.app_cd, a.app_ds ");
            sb.Append("FROM public.syssctrls r ");
            sb.Append("INNER JOIN public.sysutlaps a ON a.app_cd = r.app_cd ");
            sb.Append("WHERE (r.app_cd = @app_cd) ");
            sb.Append("ORDER BY app_cd, rl_rk;");

            query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var app_cd = cmd.Parameters.Add("@app_cd", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                app_cd.Value = applicationCode;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    applicationRoleList.Add(new ApplicationRole()
                    {
                        Code = reader["rl_cd"] == DBNull.Value ? string.Empty : (reader["rl_cd"]).ToString(),
                        Title = reader["rl_nm"] == DBNull.Value ? string.Empty : reader["rl_nm"].ToString(),
                        Description = reader["rl_ds"] == DBNull.Value ? string.Empty : reader["rl_ds"].ToString(),
                        ApplicationCode = reader["app_cd"] == DBNull.Value ? string.Empty : (reader["app_cd"]).ToString(),
                        Rank = reader["rl_rk"] == DBNull.Value ? 999 : (int)(reader["rl_rk"]),
                        ApplicationDescription = reader["app_ds"] == DBNull.Value ? string.Empty : reader["app_ds"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return applicationRoleList;
        }

        public async Task<IList<ApplicationRole>> GetAllAsync()
        {
            List<ApplicationRole> applicationRoleList = new List<ApplicationRole>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.rl_nm, r.rl_ds, r.rl_rk, r.rl_cd, r.app_cd, a.app_ds ");
            sb.Append("FROM public.syssctrls r ");
            sb.Append("INNER JOIN public.sysutlaps a ON a.app_cd = r.app_cd ");
            sb.Append("ORDER BY app_cd, rl_rk;");

            query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    applicationRoleList.Add(new ApplicationRole()
                    {
                        Code = reader["rl_cd"] == DBNull.Value ? string.Empty : (reader["rl_cd"]).ToString(),
                        Title = reader["rl_nm"] == DBNull.Value ? string.Empty : reader["rl_nm"].ToString(),
                        Description = reader["rl_ds"] == DBNull.Value ? string.Empty : reader["rl_ds"].ToString(),
                        ApplicationCode = reader["app_cd"] == DBNull.Value ? string.Empty : (reader["app_cd"]).ToString(),
                        Rank = reader["rl_rk"] == DBNull.Value ? 999 : (int)(reader["rl_rk"]),
                        ApplicationDescription = reader["app_ds"] == DBNull.Value ? string.Empty : reader["app_ds"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return applicationRoleList;
        }


        public async Task<IList<ApplicationRole>> GetUnallocatedRolesByUserIdAndAppCodeAsync(int userId, string applicationCode)
        {
            if (string.IsNullOrWhiteSpace(applicationCode)) { throw new ArgumentNullException("The required parameter [UserID] is null or has an invalid value."); }

            List<ApplicationRole> applicationRoleList = new List<ApplicationRole>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.rl_nm, r.rl_ds, r.rl_rk, r.rl_cd, r.app_cd, a.app_ds ");
            sb.Append("FROM public.syssctrls r ");
            sb.Append("INNER JOIN public.sysutlaps a ON a.app_cd = r.app_cd ");
            sb.Append("WHERE (r.app_cd = @app_cd) AND r.rl_cd NOT IN  ");
            sb.Append("(SELECT rls_cd FROM public.syssctpms WHERE usr_id = @usr_id) ");
            sb.Append("ORDER BY app_cd, rl_rk;");

            query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var app_cd = cmd.Parameters.Add("@app_cd", NpgsqlDbType.Text);
                var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                app_cd.Value = applicationCode;
                usr_id.Value = userId;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    applicationRoleList.Add(new ApplicationRole()
                    {
                        Code = reader["rl_cd"] == DBNull.Value ? string.Empty : (reader["rl_cd"]).ToString(),
                        Title = reader["rl_nm"] == DBNull.Value ? string.Empty : reader["rl_nm"].ToString(),
                        Description = reader["rl_ds"] == DBNull.Value ? string.Empty : reader["rl_ds"].ToString(),
                        ApplicationCode = reader["app_cd"] == DBNull.Value ? string.Empty : (reader["app_cd"]).ToString(),
                        Rank = reader["rl_rk"] == DBNull.Value ? 999 : (int)(reader["rl_rk"]),
                        ApplicationDescription = reader["app_ds"] == DBNull.Value ? string.Empty : reader["app_ds"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return applicationRoleList;
        }

        public async Task<IList<ApplicationRole>> GetUnallocatedRolesByUserIdAsync(int userId)
        {
            List<ApplicationRole> applicationRoleList = new List<ApplicationRole>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.rl_nm, r.rl_ds, r.rl_rk, r.rl_cd, r.app_cd, a.app_ds ");
            sb.Append("FROM public.syssctrls r ");
            sb.Append("INNER JOIN public.sysutlaps a ON a.app_cd = r.app_cd ");
            sb.Append("WHERE r.rl_cd NOT IN  ");
            sb.Append("(SELECT rls_cd FROM public.syssctpms WHERE usr_id = @usr_id) ");
            sb.Append("ORDER BY app_cd, rl_rk;"); 

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
                    applicationRoleList.Add(new ApplicationRole()
                    {
                        Code = reader["rl_cd"] == DBNull.Value ? string.Empty : (reader["rl_cd"]).ToString(),
                        Title = reader["rl_nm"] == DBNull.Value ? string.Empty : reader["rl_nm"].ToString(),
                        Description = reader["rl_ds"] == DBNull.Value ? string.Empty : reader["rl_ds"].ToString(),
                        ApplicationCode = reader["app_cd"] == DBNull.Value ? string.Empty : (reader["app_cd"]).ToString(),
                        Rank = reader["rl_rk"] == DBNull.Value ? 999 : (int)(reader["rl_rk"]),
                        ApplicationDescription = reader["app_ds"] == DBNull.Value ? string.Empty : reader["app_ds"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return applicationRoleList;
        }






        public async Task<IList<UserPermission>> GetByUserIdAsync(int userId)
        {
            if (userId < 1) { throw new ArgumentNullException("The required parameter [UserID] is null or has an invalid value."); }

            List<UserPermission> userPermissionList = new List<UserPermission>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT p.pms_id, p.usr_id, p.rls_cd, p.pms_mb, p.pms_md, ");
            sb.Append("u.fullname, r.rl_nm, r.app_cd, a.app_ds ");
            sb.Append("FROM public.syssctpms p ");
            sb.Append("LEFT JOIN public.syssctusr u ON u.usr_id = p.usr_id ");
            sb.Append("LEFT JOIN public.syssctrls r ON r.rl_cd = p.rls_cd ");
            sb.Append("LEFT JOIN public.sysutlaps a ON a.app_cd = r.app_cd ");
            sb.Append("WHERE (p.usr_id = @usr_id);");

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
                    userPermissionList.Add(new UserPermission()
                    {
                        PermissionID = reader["pms_id"] == DBNull.Value ? 0 : (int)(reader["pms_id"]),
                        UserID = reader["usr_id"] == DBNull.Value ? 0 : (int)(reader["usr_id"]),
                        RoleCode = reader["rls_cd"] == DBNull.Value ? string.Empty : (reader["rls_cd"]).ToString(),
                        RoleTitle = reader["rl_nm"] == DBNull.Value ? string.Empty : reader["rl_nm"].ToString(),
                        ApplicationCode = reader["app_cd"] == DBNull.Value ? string.Empty : (reader["app_cd"]).ToString(),
                        ApplicationDescription = reader["app_ds"] == DBNull.Value ? string.Empty : reader["app_ds"].ToString(),
                        UserFullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return userPermissionList;
        }

        public async Task<IList<UserPermission>> GetByUserIdAndAppCodeAsync(int userId, string appCode)
        {
            if (userId < 1) { throw new ArgumentNullException("The required parameter [UserID] is null or has an invalid value."); }
            if (string.IsNullOrWhiteSpace(appCode)) { throw new ArgumentNullException("The required parameter [Application Code] is null or has an invalid value."); }

            List<UserPermission> userPermissionList = new List<UserPermission>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT p.pms_id, p.usr_id, p.rls_cd, p.pms_mb, p.pms_md, ");
            sb.Append("u.fullname, r.rl_nm, r.app_cd, a.app_ds ");
            sb.Append("FROM public.syssctpms p ");
            sb.Append("LEFT JOIN public.syssctusr u ON u.usr_id = p.usr_id ");
            sb.Append("LEFT JOIN public.syssctrls r ON r.rl_cd = p.rls_cd ");
            sb.Append("LEFT JOIN public.sysutlaps a ON a.app_cd = r.app_cd ");
            sb.Append("WHERE (p.usr_id = @usr_id) AND (r.app_cd = @app_cd);");

            query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Integer);
                var app_cd = cmd.Parameters.Add("@app_cd", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                usr_id.Value = userId;
                app_cd.Value = appCode;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    userPermissionList.Add(new UserPermission()
                    {
                        PermissionID = reader["pms_id"] == DBNull.Value ? 0 : (int)(reader["pms_id"]),
                        UserID = reader["usr_id"] == DBNull.Value ? 0 : (int)(reader["usr_id"]),
                        RoleCode = reader["rls_cd"] == DBNull.Value ? string.Empty : (reader["rls_cd"]).ToString(),
                        RoleTitle = reader["rl_nm"] == DBNull.Value ? string.Empty : reader["rl_nm"].ToString(),
                        ApplicationCode = reader["app_cd"] == DBNull.Value ? string.Empty : (reader["app_cd"]).ToString(),
                        ApplicationDescription = reader["app_ds"] == DBNull.Value ? string.Empty : reader["app_ds"].ToString(),
                        UserFullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return userPermissionList;
        }
        #endregion
    }
}
