using Microsoft.Extensions.Configuration;
using NXPMS.Base.Models.SecurityModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NXPMS.Base.Repositories.SecurityRepositories
{
    public interface IUserPermissionRepository
    {
        IConfiguration _config { get; }

        Task<IList<ApplicationRole>> GetByAppCodeAsync(string applicationCode);
        Task<IList<ApplicationRole>> GetAllAsync();
        Task<IList<ApplicationRole>> GetUnallocatedRolesByUserIdAsync(int userId);
        Task<IList<ApplicationRole>> GetUnallocatedRolesByUserIdAndAppCodeAsync(int userId, string applicationCode);
        Task<IList<UserPermission>> GetByUserIdAsync(int userId);
        Task<IList<UserPermission>> GetByUserIdAndAppCodeAsync(int userId, string appCode);
        Task<bool> AddUserPermissionAsync(UserPermission userPermission);
        Task<bool> DeleteUserPermissionAsync(UserPermission userPermission);
        Task<bool> DeleteUserPermissionAsync(int userPermissionId);
    }
}