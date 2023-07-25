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
    public class UnitRepository : IUnitRepository
    {
        public IConfiguration _config { get; }
        public UnitRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<IList<Unit>> GetAllAsync()
        {
            List<Unit> unitList = new List<Unit>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT u.unit_cd, u.unit_nm, u.dept_cd, u.unithd1, ");
            sb.Append("u.unithd2, d.dept_nm, e.fullname as unithd1_nm, ");
            sb.Append("f.fullname as unithd2_nm FROM public.syscfgunts u ");
            sb.Append("INNER JOIN public.syscfgdpts d ON d.dept_cd = u.dept_cd ");
            sb.Append("LEFT JOIN public.ermempinf e ON e.empid = u.unithd1 ");
            sb.Append("LEFT JOIN public.ermempinf f ON f.empid = u.unithd2; ");
            query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    unitList.Add(new Unit()
                    {
                        UnitCode = reader["unit_cd"] == DBNull.Value ? string.Empty : (reader["unit_cd"]).ToString(),
                        UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : (reader["unit_nm"]).ToString(),
                        UnitHeadId = reader["unithd1"] == DBNull.Value ? 0 : (int)reader["unithd1"],
                        UnitHeadName = reader["unithd1_nm"] == DBNull.Value ? string.Empty : reader["unithd1_nm"].ToString(),
                        UnitAltHeadId = reader["unithd2"] == DBNull.Value ? 0 : (int)reader["unithd2"],
                        UnitAltHeadName = reader["unithd2_nm"] == DBNull.Value ? string.Empty : reader["unithd2_nm"].ToString(),
                        DepartmentCode = reader["dept_cd"] == DBNull.Value ? string.Empty : (reader["dept_cd"]).ToString(),
                        DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : (reader["dept_nm"]).ToString(),

                    });
                }
            }
            await conn.CloseAsync();
            return unitList;
        }

        public async Task<IList<Unit>> GetByCodeAsync(string unitCode)
        {
            List<Unit> unitList = new List<Unit>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT u.unit_cd, u.unit_nm, u.dept_cd, u.unithd1, ");
            sb.Append("u.unithd2, d.dept_nm, e.fullname as unithd1_nm, ");
            sb.Append("f.fullname as unithd2_nm FROM public.syscfgunts u ");
            sb.Append("INNER JOIN public.syscfgdpts d ON d.dept_cd = u.dept_cd ");
            sb.Append("LEFT JOIN public.ermempinf e ON e.empid = u.unithd1 ");
            sb.Append("LEFT JOIN public.ermempinf f ON f.empid = u.unithd2 ");
            sb.Append("WHERE LOWER(u.unit_cd) = LOWER(@unit_cd) ");
            sb.Append("ORDER BY u.unit_nm;");
            query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var unit_cd = cmd.Parameters.Add("@unit_cd", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                unit_cd.Value = unitCode;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    unitList.Add(new Unit()
                    {
                        UnitCode = reader["unit_cd"] == DBNull.Value ? string.Empty : (reader["unit_cd"]).ToString(),
                        UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : (reader["unit_nm"]).ToString(),
                        UnitHeadId = reader["unithd1"] == DBNull.Value ? 0 : (int)reader["unithd1"],
                        UnitHeadName = reader["unithd1_nm"] == DBNull.Value ? string.Empty : reader["unithd1_nm"].ToString(),
                        UnitAltHeadId = reader["unithd2"] == DBNull.Value ? 0 : (int)reader["unithd2"],
                        UnitAltHeadName = reader["unithd2_nm"] == DBNull.Value ? string.Empty : reader["unithd2_nm"].ToString(),
                        DepartmentCode = reader["dept_cd"] == DBNull.Value ? string.Empty : (reader["dept_cd"]).ToString(),
                        DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : (reader["dept_nm"]).ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return unitList;
        }

        public async Task<IList<Unit>> GetByDepartmentCodeAsync(string departmentCode)
        {
            List<Unit> unitList = new List<Unit>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT u.unit_cd, u.unit_nm, u.dept_cd, u.unithd1, ");
            sb.Append("u.unithd2, d.dept_nm, e.fullname as unithd1_nm, ");
            sb.Append("f.fullname as unithd2_nm FROM public.syscfgunts u ");
            sb.Append("INNER JOIN public.syscfgdpts d ON d.dept_cd = u.dept_cd ");
            sb.Append("LEFT JOIN public.ermempinf e ON e.empid = u.unithd1 ");
            sb.Append("LEFT JOIN public.ermempinf f ON f.empid = u.unithd2 ");
            sb.Append("WHERE LOWER(u.dept_cd) = LOWER(@dept_cd) ");
            sb.Append("ORDER BY u.unit_nm;");
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
                    unitList.Add(new Unit()
                    {
                        UnitCode = reader["unit_cd"] == DBNull.Value ? string.Empty : (reader["unit_cd"]).ToString(),
                        UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : (reader["unit_nm"]).ToString(),
                        UnitHeadId = reader["unithd1"] == DBNull.Value ? 0 : (int)reader["unithd1"],
                        UnitHeadName = reader["unithd1_nm"] == DBNull.Value ? string.Empty : reader["unithd1_nm"].ToString(),
                        UnitAltHeadId = reader["unithd2"] == DBNull.Value ? 0 : (int)reader["unithd2"],
                        UnitAltHeadName = reader["unithd2_nm"] == DBNull.Value ? string.Empty : reader["unithd2_nm"].ToString(),
                        DepartmentCode = reader["dept_cd"] == DBNull.Value ? string.Empty : (reader["dept_cd"]).ToString(),
                        DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : (reader["dept_nm"]).ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return unitList;
        }

        public async Task<IList<Unit>> GetByNameAsync(string unitName)
        {
            List<Unit> unitList = new List<Unit>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT u.unit_cd, u.unit_nm, u.dept_cd, u.unithd1, ");
            sb.Append("u.unithd2, d.dept_nm, e.fullname as unithd1_nm, ");
            sb.Append("f.fullname as unithd2_nm FROM public.syscfgunts u ");
            sb.Append("INNER JOIN public.syscfgdpts d ON d.dept_cd = u.dept_cd ");
            sb.Append("LEFT JOIN public.ermempinf e ON e.empid = u.unithd1 ");
            sb.Append("LEFT JOIN public.ermempinf f ON f.empid = u.unithd2 ");
            sb.Append("WHERE LOWER(u.unit_nm) = LOWER(@unit_nm) ");
            sb.Append("ORDER BY u.unit_nm;");
            query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var unit_nm = cmd.Parameters.Add("@unit_nm", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                unit_nm.Value = unitName;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    unitList.Add(new Unit()
                    {
                        UnitCode = reader["unit_cd"] == DBNull.Value ? string.Empty : (reader["unit_cd"]).ToString(),
                        UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : (reader["unit_nm"]).ToString(),
                        UnitHeadId = reader["unithd1"] == DBNull.Value ? 0 : (int)reader["unithd1"],
                        UnitHeadName = reader["unithd1_nm"] == DBNull.Value ? string.Empty : reader["unithd1_nm"].ToString(),
                        UnitAltHeadId = reader["unithd2"] == DBNull.Value ? 0 : (int)reader["unithd2"],
                        UnitAltHeadName = reader["unithd2_nm"] == DBNull.Value ? string.Empty : reader["unithd2_nm"].ToString(),
                        DepartmentCode = reader["dept_cd"] == DBNull.Value ? string.Empty : (reader["dept_cd"]).ToString(),
                        DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : (reader["dept_nm"]).ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return unitList;
        }

        public async Task<bool> AddAsync(Unit unit)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.syscfgunts(unit_cd,  ");
            sb.Append("unit_nm, dept_cd, unithd1, unithd2) ");
            sb.Append("VALUES (@unit_cd, @unit_nm, @dept_cd, ");
            sb.Append("@unithd1, @unithd2); ");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var unit_cd = cmd.Parameters.Add("@unit_cd", NpgsqlDbType.Text);
                    var unit_nm = cmd.Parameters.Add("@unit_nm", NpgsqlDbType.Text);
                    var dept_cd = cmd.Parameters.Add("@dept_cd", NpgsqlDbType.Text);
                    var unithd1 = cmd.Parameters.Add("@unithd1", NpgsqlDbType.Integer);
                    var unithd2 = cmd.Parameters.Add("@unithd2", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    dept_cd.Value = unit.DepartmentCode;
                    unit_cd.Value = unit.UnitCode;
                    unit_nm.Value = unit.UnitName;
                    unithd1.Value = unit.UnitHeadId ?? (object)DBNull.Value;
                    unithd2.Value = unit.UnitAltHeadId ?? (object)DBNull.Value;

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

        public async Task<bool> UpdateAsync(Unit unit)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.syscfgunts SET unit_nm=@unit_nm, ");
            sb.Append("dept_cd=@dept_cd, unithd1=@unithd1, unithd2=@unithd2 ");
            sb.Append("WHERE LOWER(unit_cd) = LOWER(@unit_cd); ");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var unit_cd = cmd.Parameters.Add("@unit_cd", NpgsqlDbType.Text);
                    var unit_nm = cmd.Parameters.Add("@unit_nm", NpgsqlDbType.Text);
                    var dept_cd = cmd.Parameters.Add("@dept_cd", NpgsqlDbType.Text);
                    var unithd1 = cmd.Parameters.Add("@unithd1", NpgsqlDbType.Integer);
                    var unithd2 = cmd.Parameters.Add("@unithd2", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    dept_cd.Value = unit.DepartmentCode;
                    unit_cd.Value = unit.UnitCode;
                    unit_nm.Value = unit.UnitName;
                    unithd1.Value = unit.UnitHeadId ?? (object)DBNull.Value;
                    unithd2.Value = unit.UnitAltHeadId ?? (object)DBNull.Value;

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

        public async Task<bool> DeleteAsync(string unitCode)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));

            string query = "DELETE FROM public.syscfgunts WHERE LOWER(unit_cd) = LOWER(@unit_cd);";
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var unit_cd = cmd.Parameters.Add("@unit_cd", NpgsqlDbType.Text);
                    cmd.Prepare();
                    unit_cd.Value = unitCode;

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
    }
}
