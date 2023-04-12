using Microsoft.Extensions.Configuration;
using NXPMS.Base.Models.GlobalSettingsModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NXPMS.Base.Repositories.GlobalSettingsRepositories
{
    public interface IUnitRepository
    {
        IConfiguration _config { get; }

        Task<IList<Unit>> GetAllAsync();
        Task<IList<Unit>> GetByCodeAsync(string unitCode);
    }
}