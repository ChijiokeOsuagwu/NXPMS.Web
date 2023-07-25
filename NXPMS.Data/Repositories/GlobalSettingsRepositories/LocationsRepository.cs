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
    public class LocationsRepository : ILocationsRepository
    {

        public IConfiguration _config { get; }
        public LocationsRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region Locations Read Action Methods
        public async Task<IList<Location>> GetAllAsync()
        {
            List<Location> locationList = new List<Location>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT l.locname, l.lochd1, l.lochd2, l.locctr, ");
            sb.Append("l.locst, l.locid, e.fullname as lochd1_nm, ");
            sb.Append("f.fullname as lochd2_nm FROM public.syscfglocs l  ");
            sb.Append("LEFT JOIN public.ermempinf e ON e.empid = l.lochd1 ");
            sb.Append("LEFT JOIN public.ermempinf f ON f.empid = l.lochd2 ");
            sb.Append("ORDER BY l.locname;");
            query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    locationList.Add(new Location()
                    {
                        LocationId = reader["locid"] == DBNull.Value ? 0 : (int)(reader["locid"]),
                        LocationName = reader["locname"] == DBNull.Value ? string.Empty : (reader["locname"]).ToString(),
                        LocationHeadId = reader["lochd1"] == DBNull.Value ? 0 : (int)reader["lochd1"],
                        LocationHeadName = reader["lochd1_nm"] == DBNull.Value ? string.Empty : reader["lochd1_nm"].ToString(),
                        LocationAltHeadId = reader["lochd2"] == DBNull.Value ? 0 : (int)reader["lochd2"],
                        LocationAltHeadName = reader["lochd2_nm"] == DBNull.Value ? string.Empty : reader["lochd2_nm"].ToString(),
                        LocationCountry = reader["locctr"] == DBNull.Value ? string.Empty : reader["locctr"].ToString(),
                        LocationState = reader["locst"] == DBNull.Value ? string.Empty : reader["locst"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return locationList;
        }
        public async Task<IList<Location>> GetByIdAsync(int locationId)
        {
            List<Location> locationList = new List<Location>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT l.locname, l.lochd1, l.lochd2, l.locctr, ");
            sb.Append("l.locst, l.locid, e.fullname as lochd1_nm, ");
            sb.Append("f.fullname as lochd2_nm FROM public.syscfglocs l  ");
            sb.Append("LEFT JOIN public.ermempinf e ON e.empid = l.lochd1 ");
            sb.Append("LEFT JOIN public.ermempinf f ON f.empid = l.lochd2 ");
            sb.Append("WHERE (l.locid = @locid);");
            query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var locid = cmd.Parameters.Add("@locid", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                locid.Value = locationId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    locationList.Add(new Location()
                    {
                        LocationId = reader["locid"] == DBNull.Value ? 0 : (int)(reader["locid"]),
                        LocationName = reader["locname"] == DBNull.Value ? string.Empty : (reader["locname"]).ToString(),
                        LocationHeadId = reader["lochd1"] == DBNull.Value ? 0 : (int)reader["lochd1"],
                        LocationHeadName = reader["lochd1_nm"] == DBNull.Value ? string.Empty : reader["lochd1_nm"].ToString(),
                        LocationAltHeadId = reader["lochd2"] == DBNull.Value ? 0 : (int)reader["lochd2"],
                        LocationAltHeadName = reader["lochd2_nm"] == DBNull.Value ? string.Empty : reader["lochd2_nm"].ToString(),
                        LocationCountry = reader["locctr"] == DBNull.Value ? string.Empty : reader["locctr"].ToString(),
                        LocationState = reader["locst"] == DBNull.Value ? string.Empty : reader["locst"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return locationList;
        }
        public async Task<IList<Location>> GetByNameAsync(string locationName)
        {
            List<Location> locationList = new List<Location>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT l.locname, l.lochd1, l.lochd2, l.locctr, ");
            sb.Append("l.locst, l.locid, e.fullname as lochd1_nm, ");
            sb.Append("f.fullname as lochd2_nm FROM public.syscfglocs l  ");
            sb.Append("LEFT JOIN public.ermempinf e ON e.empid = l.lochd1 ");
            sb.Append("LEFT JOIN public.ermempinf f ON f.empid = l.lochd2 ");
            sb.Append("WHERE LOWER(l.locname) = LOWER(@locname) ");
            sb.Append("ORDER BY l.locname;");
            query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var locname = cmd.Parameters.Add("@locname", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                locname.Value = locationName;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    locationList.Add(new Location()
                    {
                        LocationId = reader["locid"] == DBNull.Value ? 0 : (int)(reader["locid"]),
                        LocationName = reader["locname"] == DBNull.Value ? string.Empty : (reader["locname"]).ToString(),
                        LocationHeadId = reader["lochd1"] == DBNull.Value ? 0 : (int)reader["lochd1"],
                        LocationHeadName = reader["lochd1_nm"] == DBNull.Value ? string.Empty : reader["lochd1_nm"].ToString(),
                        LocationAltHeadId = reader["lochd2"] == DBNull.Value ? 0 : (int)reader["lochd2"],
                        LocationAltHeadName = reader["lochd2_nm"] == DBNull.Value ? string.Empty : reader["lochd2_nm"].ToString(),
                        LocationCountry = reader["locctr"] == DBNull.Value ? string.Empty : reader["locctr"].ToString(),
                        LocationState = reader["locst"] == DBNull.Value ? string.Empty : reader["locst"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return locationList;
        }

        public async Task<bool> AddAsync(Location location)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.syscfglocs(locname, locctr, locst, lochd1, ");
            sb.Append("lochd2) VALUES (@locname, @locctr, @locst, @lochd1, @lochd2); ");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var locname = cmd.Parameters.Add("@locname", NpgsqlDbType.Text);
                    var locctr = cmd.Parameters.Add("@locctr", NpgsqlDbType.Text);
                    var locst = cmd.Parameters.Add("@locst", NpgsqlDbType.Text);
                    var lochd1 = cmd.Parameters.Add("@lochd1", NpgsqlDbType.Integer);
                    var lochd2 = cmd.Parameters.Add("@lochd2", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    locname.Value = location.LocationName;
                    locctr.Value = location.LocationCountry;
                    locst.Value = location.LocationState;
                    lochd1.Value = location.LocationHeadId ?? (object)DBNull.Value;
                    lochd2.Value = location.LocationAltHeadId ?? (object)DBNull.Value;

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

        public async Task<bool> UpdateAsync(Location location)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.syscfglocs SET locname=@locname, locctr=@locctr, ");
            sb.Append("locst=@locst, lochd1=@lochd1, lochd2=@lochd2 ");
            sb.Append("WHERE locid = @locid; ");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var locid = cmd.Parameters.Add("@locid", NpgsqlDbType.Integer);
                    var locname = cmd.Parameters.Add("@locname", NpgsqlDbType.Text);
                    var locctr = cmd.Parameters.Add("@locctr", NpgsqlDbType.Text);
                    var locst = cmd.Parameters.Add("@locst", NpgsqlDbType.Text);
                    var lochd1 = cmd.Parameters.Add("@lochd1", NpgsqlDbType.Integer);
                    var lochd2 = cmd.Parameters.Add("@lochd2", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    locid.Value = location.LocationId;
                    locname.Value = location.LocationName;
                    locctr.Value = location.LocationCountry ?? (object)DBNull.Value;
                    locst.Value = location.LocationState ?? (object)DBNull.Value;
                    lochd1.Value = location.LocationHeadId ?? (object)DBNull.Value;
                    lochd2.Value = location.LocationAltHeadId ?? (object)DBNull.Value;

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

        public async Task<bool> DeleteAsync(int locationId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));

            string query = "DELETE FROM public.syscfglocs WHERE (locid = @locid);";
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var locid = cmd.Parameters.Add("@locid", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    locid.Value = locationId;

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
