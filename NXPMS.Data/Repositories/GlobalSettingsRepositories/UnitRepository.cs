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
            sb.Append("WHERE u.unit_cd = @unit_cd;");
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

    }
}
