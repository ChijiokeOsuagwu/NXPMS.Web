using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NXPMS.Base.Models.GlobalSettingsModels;
using Npgsql;
using NXPMS.Base.Repositories.GlobalSettingsRepositories;
using NpgsqlTypes;

namespace NXPMS.Data.Repositories.GlobalSettingsRepositories
{
    public class StateRepository : IStateRepository
    {
        public IConfiguration _config { get; }
        public StateRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<IList<State>> GetAllAsync()
        {
            List<State> stateList = new List<State>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT stts_cd, stts_nm, stts_rg, stts_ct  ");
            sb.Append("FROM public.syscfgstts ORDER BY stts_nm;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    stateList.Add(new State()
                    {
                        StateCode = reader["stts_cd"] == DBNull.Value ? string.Empty : reader["stts_cd"].ToString(),
                        StateName = reader["stts_nm"] == DBNull.Value ? string.Empty : reader["stts_nm"].ToString(),
                        Region = reader["stts_rg"] == DBNull.Value ? string.Empty : reader["stts_rg"].ToString(),
                        CountryName = reader["stts_ct"] == DBNull.Value ? string.Empty : reader["stts_ct"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return stateList;
        }

        public async Task<IList<State>> GetByNameAsync(string stateName)
        {
            List<State> stateList = new List<State>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT stts_cd, stts_nm, stts_rg, stts_ct  ");
            sb.Append("FROM public.syscfgstts ");
            sb.Append("WHERE LOWER(stts_nm) = LOWER(@stts_nm) ");
            sb.Append("ORDER BY stts_nm;");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var stts_nm = cmd.Parameters.Add("@stts_nm", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                stts_nm.Value = stateName;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    stateList.Add(new State()
                    {
                        StateCode = reader["stts_cd"] == DBNull.Value ? string.Empty : reader["stts_cd"].ToString(),
                        StateName = reader["stts_nm"] == DBNull.Value ? string.Empty : reader["stts_nm"].ToString(),
                        Region = reader["stts_rg"] == DBNull.Value ? string.Empty : reader["stts_rg"].ToString(),
                        CountryName = reader["stts_ct"] == DBNull.Value ? string.Empty : reader["stts_ct"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return stateList;
        }
    }
}
