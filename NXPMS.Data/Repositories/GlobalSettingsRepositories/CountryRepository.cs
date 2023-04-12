using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NXPMS.Base.Models.GlobalSettingsModels;
using Npgsql;
using NXPMS.Base.Repositories.GlobalSettingsRepositories;

namespace NXPMS.Data.Repositories.GlobalSettingsRepositories
{
    public class CountryRepository : ICountryRepository
    {
        public IConfiguration _config { get; }
        public CountryRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<IList<Country>> GetAllAsync()
        {
            List<Country> countryList = new List<Country>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));

            string query = "SELECT ctr_cd, ctr_nm FROM public.syscfgctrs ORDER BY ctr_nm;";
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    countryList.Add(new Country()
                    {
                        CountryCode = reader["ctr_cd"] == DBNull.Value ? string.Empty : (reader["ctr_cd"]).ToString(),
                        CountryName = reader["ctr_nm"] == DBNull.Value ? string.Empty : (reader["ctr_nm"]).ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return countryList;
        }
    }
}
