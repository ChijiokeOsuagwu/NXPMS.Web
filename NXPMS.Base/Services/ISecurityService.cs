using NXPMS.Base.Models.SecurityModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NXPMS.Base.Services
{
    public interface ISecurityService
    {
        #region Application User Service Methods
        string CreatePasswordHash(string plainTextPassword);
        Task<bool> CreateUserAsync(ApplicationUser user);
        Task<bool> DeleteUserAsync(int userId, string deletedBy);
        Task<ApplicationUser> GetUserByFullNameAsync(string FullName);
        Task<ApplicationUser> GetUserByIdAsync(int UserId);
        Task<IList<ApplicationUser>> GetUsersByUsernameAsync(string Username);
        Task<IList<ApplicationUser>> GetUsersByLocationIdAsync(int LocationId);
        Task<IList<ApplicationUser>> GetAllUsersAsync();
        Task<bool> ResetUserPasswordAsync(ApplicationUser user);
        Task<IList<ApplicationUser>> SearchUsersByNameAsync(string Name);
        Task<bool> UpdateUserAsync(ApplicationUser user);
        Task<bool> UsernameIsAvailableAsync(int UserId, string Username);
        bool ValidatePassword(string plainTextPassword, string hashedPassword);
        #endregion

        #region Application Role Service Methods
        Task<IList<ApplicationRole>> GetRolesByApplicationCodeAsync(string ApplicationCode);
        Task<IList<ApplicationRole>> GetRolesAsync();
        Task<IList<ApplicationRole>> GetUnallocatedRolesAsync(int UserId, string ApplicationCode = null);

        #endregion

        #region User Permission Read Service Methods
        Task<IList<UserPermission>> GetPermissionsByUserIdAsync(int UserId, string applicationCode = null);

        #endregion

        #region User Permission Write Service Methods

        Task<bool> GrantPermissionAsync(int usd, string rld, string actionBy);
        Task<bool> RevokePermissionAsync(int usd, string rld, string actionBy);
        Task<bool> RevokePermissionAsync(int pmd);

        #endregion

        #region Login Service Methods
        Task<bool> RegisterLoginAsync(LoginAttempt attempt);
        Task<bool> IncrementFailedLoginCount(string Username);
        Task<bool> ResetFailedLoginCount(string Username);
        Task<int> GetFailedLoginCountAsync(string Username);
        #endregion
    }
}