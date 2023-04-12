using Microsoft.Extensions.Configuration;
using Npgsql;
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
            sb.Append("LEFT JOIN public.ermempinf f ON f.empid = d.depthd2; ");
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

    }
}
