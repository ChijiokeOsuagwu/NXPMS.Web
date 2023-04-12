using Microsoft.Extensions.Configuration;
using NXPMS.Base.Models.GlobalSettingsModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NXPMS.Base.Repositories.GlobalSettingsRepositories
{
    public interface ICountryRepository
    {
        IConfiguration _config { get; }

        Task<IList<Country>> GetAllAsync();
    }
}