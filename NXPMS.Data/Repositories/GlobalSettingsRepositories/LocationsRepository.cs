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
    public class LocationsRepository : ILocationsRepository
    {

        public IConfiguration _config { get; }
        public LocationsRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<IList<Location>> GetAllAsync()
        {
            List<Location> locationList = new List<Location>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("NxpmsConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT l.locname, l.lochd1, l.lochd2, l.locctr, l.locst, ");
            sb.Append("l.locid, l.loc_typ_id, l.loc_ctg_id, t.loc_typ_nm, ");
            sb.Append("c.loc_ctg_nm, e.fullname as lochd1_nm,  ");
            sb.Append("f.fullname as lochd2_nm FROM public.syscfglocs l  ");
            sb.Append("LEFT JOIN public.syscfgloctyp t ON t.loc_typ_id = l.loc_typ_id ");
            sb.Append("LEFT JOIN public.syscfglocctg c ON c.loc_ctg_id = l.loc_ctg_id ");
            sb.Append("LEFT JOIN public.ermempinf e ON e.empid = l.lochd1 ");
            sb.Append("LEFT JOIN public.ermempinf f ON f.empid = l.lochd2; ");
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
                        LocationTypeId = reader["loc_typ_id"] == DBNull.Value ? 0 : (int)reader["loc_typ_id"],
                        LocationTypeName = reader["loc_typ_nm"] == DBNull.Value ? string.Empty : reader["loc_typ_nm"].ToString(),
                        LocationCategoryId = reader["loc_ctg_id"] == DBNull.Value ? 0 : (int)reader["loc_ctg_id"],
                        LocationCategoryName = reader["loc_ctg_nm"] == DBNull.Value ? string.Empty : reader["loc_ctg_nm"].ToString(),
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


    }
}
