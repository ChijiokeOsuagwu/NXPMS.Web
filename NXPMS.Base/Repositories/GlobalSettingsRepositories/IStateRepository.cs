using Microsoft.Extensions.Configuration;
using NXPMS.Base.Models.GlobalSettingsModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NXPMS.Base.Repositories.GlobalSettingsRepositories
{
    public interface IStateRepository
    {
        IConfiguration _config { get; }

        Task<IList<State>> GetAllAsync();
        Task<IList<State>> GetByNameAsync(string stateName);
    }
}