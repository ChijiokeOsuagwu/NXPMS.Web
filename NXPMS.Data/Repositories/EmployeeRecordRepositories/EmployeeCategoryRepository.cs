using Microsoft.Extensions.Configuration;
using Npgsql;
using NXPMS.Base.Models.EmployeesModels;
using NXPMS.Base.Repositories.EmployeeRecordRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NXPMS.Data.Repositories.EmployeeRecordRepositories
{
    public class EmployeeCategoryRepository : IEmployeeCategoryRepository
    {
        public IConfiguration _config { get; }
        public EmployeeCategoryRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<IList<EmployeeCategory>> GetAllAsync()
        {
            List<EmployeeCategory> employeeCategoriesList = new List<EmployeeCategory>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = "SELECT emp_ctg_id, emp_ctg_nm FROM public.ermsttempctg ORDER BY emp_ctg_nm;";
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    employeeCategoriesList.Add(new EmployeeCategory()
                    {
                        EmployeeCategoryId = reader["emp_ctg_id"] == DBNull.Value ? 0 : (int)(reader["emp_ctg_id"]),
                        EmployeeCategoryName = reader["emp_ctg_nm"] == DBNull.Value ? string.Empty : (reader["emp_ctg_nm"]).ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return employeeCategoriesList;
        }

    }
}
