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
    public class EmployeeTypeRepository : IEmployeeTypeRepository
    {
        public IConfiguration _config { get; }
        public EmployeeTypeRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<IList<EmployeeType>> GetAllAsync()
        {
            List<EmployeeType> employeeTypesList = new List<EmployeeType>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT t.emp_typ_id, t.emp_typ_nm, t.emp_ctg_id, c.emp_ctg_nm  ");
            sb.Append("FROM public.ermsttemptyp t ");
            sb.Append("LEFT JOIN public.ermsttempctg c ON c.emp_ctg_id = t.emp_ctg_id ");
            sb.Append("ORDER BY emp_typ_nm; ");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    employeeTypesList.Add(new EmployeeType()
                    {
                        EmployeeTypeId = reader["emp_typ_id"] == DBNull.Value ? 0 : (int)(reader["emp_typ_id"]),
                        EmployeeTypeName = reader["emp_typ_nm"] == DBNull.Value ? string.Empty : (reader["emp_typ_nm"]).ToString(),
                        EmployeeCategoryId = reader["emp_ctg_id"] == DBNull.Value ? 0 : (int)(reader["emp_ctg_id"]),
                        EmployeeCategoryName = reader["emp_ctg_nm"] == DBNull.Value ? string.Empty : (reader["emp_ctg_nm"]).ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return employeeTypesList;
        }

        public async Task<IList<EmployeeType>> GetByIdAsync(int employeeTypeId)
        {
            List<EmployeeType> employeeTypesList = new List<EmployeeType>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT t.emp_typ_id, t.emp_typ_nm, t.emp_ctg_id, c.emp_ctg_nm  ");
            sb.Append("FROM public.ermsttemptyp t ");
            sb.Append("LEFT JOIN public.ermsttempctg c ON c.emp_ctg_id = t.emp_ctg_id ");
            sb.Append("WHERE (t.emp_typ_id = @emp_typ_id); ");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var emp_typ_id = cmd.Parameters.Add("@emp_typ_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                emp_typ_id.Value = employeeTypeId;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    employeeTypesList.Add(new EmployeeType()
                    {
                        EmployeeTypeId = reader["emp_typ_id"] == DBNull.Value ? 0 : (int)(reader["emp_typ_id"]),
                        EmployeeTypeName = reader["emp_typ_nm"] == DBNull.Value ? string.Empty : (reader["emp_typ_nm"]).ToString(),
                        EmployeeCategoryId = reader["emp_ctg_id"] == DBNull.Value ? 0 : (int)(reader["emp_ctg_id"]),
                        EmployeeCategoryName = reader["emp_ctg_nm"] == DBNull.Value ? string.Empty : (reader["emp_ctg_nm"]).ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return employeeTypesList;
        }
    }
}
