using Microsoft.Extensions.Configuration;
using NXPMS.Base.Models.GlobalSettingsModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NXPMS.Base.Repositories.GlobalSettingsRepositories
{
    public interface ILocationsRepository
    {
        IConfiguration _config { get; }

        Task<IList<Location>> GetAllAsync();
        Task<IList<Location>> GetByIdAsync(int locationId);
        Task<IList<Location>> GetByNameAsync(string locationName);
        Task<bool> AddAsync(Location location);
        Task<bool> UpdateAsync(Location location);
        Task<bool> DeleteAsync(int locationId);
    }
}