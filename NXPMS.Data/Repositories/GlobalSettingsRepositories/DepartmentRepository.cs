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
    public class DepartmentRepository : IDepartmentRepository
    {
        public IConfiguration _config { get; }
        public DepartmentRepository(IConfiguration configuration)
        {
            _config = configuration;
        }
        #region Departments Action Methods
        public async Task<IList<Department>> GetAllAsync()
        {
            List<Department> departmentList = new List<Department>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT d.dept_cd, d.dept_nm, d.depthd1, d.depthd2, ");
            sb.Append("e.fullname as depthd1_nm, f.fullname as depthd2_nm ");
            sb.Append("FROM public.syscfgdpts d  ");
            sb.Append("LEFT JOIN public.ermempinf e ON e.empid = d.depthd1 ");
            sb.Append("LEFT JOIN public.ermempinf f ON f.empid = d.depthd2 ");
            sb.Append("ORDER BY d.dept_nm;");
            query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    departmentList.Add(new Department()
                    {
                        DepartmentCode = reader["dept_cd"] == DBNull.Value ? string.Empty : (reader["dept_cd"]).ToString(),
                        DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : (reader["dept_nm"]).ToString(),
                        DepartmentHeadId = reader["depthd1"] == DBNull.Value ? 0 : (int)reader["depthd1"],
                        DepartmentHeadName = reader["depthd1_nm"] == DBNull.Value ? string.Empty : reader["depthd1_nm"].ToString(),
                        DepartmentAltHeadId = reader["depthd2"] == DBNull.Value ? 0 : (int)reader["depthd2"],
                        DepartmentAltHeadName = reader["depthd2_nm"] == DBNull.Value ? string.Empty : reader["depthd2_nm"].ToString(),

                    });
                }
            }
            await conn.CloseAsync();
            return departmentList;
        }

        public async Task<IList<Department>> GetByCodeAsync(string departmentCode)
        {
            List<Department> departmentList = new List<Department>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT d.dept_cd, d.dept_nm, d.depthd1, d.depthd2, ");
            sb.Append("e.fullname as depthd1_nm, f.fullname as depthd2_nm ");
            sb.Append("FROM public.syscfgdpts d  ");
            sb.Append("LEFT JOIN public.ermempinf e ON e.empid = d.depthd1 ");
            sb.Append("LEFT JOIN public.ermempinf f ON f.empid = d.depthd2 ");
            sb.Append("WHERE LOWER(d.dept_cd) = LOWER(@dept_cd) ");
            sb.Append("ORDER BY d.dept_nm;");
            query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var dept_cd = cmd.Parameters.Add("@dept_cd", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                dept_cd.Value = departmentCode;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    departmentList.Add(new Department()
                    {
                        DepartmentCode = reader["dept_cd"] == DBNull.Value ? string.Empty : (reader["dept_cd"]).ToString(),
                        DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : (reader["dept_nm"]).ToString(),
                        DepartmentHeadId = reader["depthd1"] == DBNull.Value ? 0 : (int)reader["depthd1"],
                        DepartmentHeadName = reader["depthd1_nm"] == DBNull.Value ? string.Empty : reader["depthd1_nm"].ToString(),
                        DepartmentAltHeadId = reader["depthd2"] == DBNull.Value ? 0 : (int)reader["depthd2"],
                        DepartmentAltHeadName = reader["depthd2_nm"] == DBNull.Value ? string.Empty : reader["depthd2_nm"].ToString(),

                    });
                }
            }
            await conn.CloseAsync();
            return departmentList;
        }

        public async Task<IList<Department>> GetByNameAsync(string departmentName)
        {
            List<Department> departmentList = new List<Department>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT d.dept_cd, d.dept_nm, d.depthd1, d.depthd2, ");
            sb.Append("e.fullname as depthd1_nm, f.fullname as depthd2_nm ");
            sb.Append("FROM public.syscfgdpts d  ");
            sb.Append("LEFT JOIN public.ermempinf e ON e.empid = d.depthd1 ");
            sb.Append("LEFT JOIN public.ermempinf f ON f.empid = d.depthd2 ");
            sb.Append("WHERE LOWER(d.dept_nm) = LOWER(@dept_nm) ");
            sb.Append("ORDER BY d.dept_nm;");
            query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var dept_nm = cmd.Parameters.Add("@dept_nm", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                dept_nm.Value = departmentName;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    departmentList.Add(new Department()
                    {
                        DepartmentCode = reader["dept_cd"] == DBNull.Value ? string.Empty : (reader["dept_cd"]).ToString(),
                        DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : (reader["dept_nm"]).ToString(),
                        DepartmentHeadId = reader["depthd1"] == DBNull.Value ? 0 : (int)reader["depthd1"],
                        DepartmentHeadName = reader["depthd1_nm"] == DBNull.Value ? string.Empty : reader["depthd1_nm"].ToString(),
                        DepartmentAltHeadId = reader["depthd2"] == DBNull.Value ? 0 : (int)reader["depthd2"],
                        DepartmentAltHeadName = reader["depthd2_nm"] == DBNull.Value ? string.Empty : reader["depthd2_nm"].ToString(),

                    });
                }
            }
            await conn.CloseAsync();
            return departmentList;
        }

        public async Task<bool> AddAsync(Department department)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.syscfgdpts(dept_cd, dept_nm, ");
            sb.Append("depthd1, depthd2) VALUES (@dept_cd, @dept_nm, ");
            sb.Append("@depthd1, @depthd2);");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var dept_cd = cmd.Parameters.Add("@dept_cd", NpgsqlDbType.Text);
                    var dept_nm = cmd.Parameters.Add("@dept_nm", NpgsqlDbType.Text);
                    var depthd1 = cmd.Parameters.Add("@depthd1", NpgsqlDbType.Integer);
                    var depthd2 = cmd.Parameters.Add("@depthd2", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    dept_cd.Value = department.DepartmentCode;
                    dept_nm.Value = department.DepartmentName;
                    depthd1.Value = department.DepartmentHeadId ?? (object)DBNull.Value;
                    depthd2.Value = department.DepartmentAltHeadId ?? (object)DBNull.Value;

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

        public async Task<bool> UpdateAsync(Department department)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.syscfgdpts SET dept_nm=@dept_nm, depthd1=@depthd1, ");
            sb.Append("depthd2=@depthd2 WHERE (dept_cd=@dept_cd);");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var dept_cd = cmd.Parameters.Add("@dept_cd", NpgsqlDbType.Text);
                    var dept_nm = cmd.Parameters.Add("@dept_nm", NpgsqlDbType.Text);
                    var depthd1 = cmd.Parameters.Add("@depthd1", NpgsqlDbType.Integer);
                    var depthd2 = cmd.Parameters.Add("@depthd2", NpgsqlDbType.Integer);

                    cmd.Prepare();

                    dept_cd.Value = department.DepartmentCode;
                    dept_nm.Value = department.DepartmentName;
                    depthd1.Value = department.DepartmentHeadId ?? (object)DBNull.Value;
                    depthd2.Value = department.DepartmentAltHeadId ?? (object)DBNull.Value;

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

        public async Task<bool> DeleteAsync(string departmentCode)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));

            string query = "DELETE FROM public.syscfgdpts WHERE LOWER(dept_cd) = LOWER(@dept_cd);";
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var dept_cd = cmd.Parameters.Add("@dept_cd", NpgsqlDbType.Text);
                    cmd.Prepare();
                    dept_cd.Value = departmentCode;

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
