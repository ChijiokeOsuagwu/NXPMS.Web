using Microsoft.Extensions.Configuration;
using NXPMS.Base.Models.SecurityModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NXPMS.Base.Repositories.SecurityRepositories
{
    public interface IUserRepository
    {
        IConfiguration _config { get; }

        #region User Read Action Methods
        Task<IList<ApplicationUser>> FindByNameAsync(string fullname);
        Task<IList<ApplicationUser>> GetByNameAsync(string fullname);
        Task<IList<ApplicationUser>> GetByIdAsync(int userId);
        Task<IList<ApplicationUser>> GetByUsernameAsync(string username);
        Task<IList<ApplicationUser>> GetByLocationIdAsync(int locationId);
        Task<IList<ApplicationUser>> GetAllAsync();
        Task<IList<ApplicationUser>> GetOtherUsersWithSameUsernameAsync(int userId, string username);
        #endregion

        #region User Write Action Methods
        Task<bool> AddAsync(ApplicationUser applicationUser);
        Task<bool> DeleteByIdAsync(int userId);
        Task<bool> UpdateAsync(ApplicationUser applicationUser);
        Task<bool> UpdateUserPasswordAsync(ApplicationUser applicationUser);
        #endregion

        #region Login Attempt Action Methods
        Task<bool> AddAttemptAsync(LoginAttempt loginAttempt);
        Task<bool> IncreaseFailedLoginCountAsync(string userName);
        Task<bool> ClearFailedLoginCountAsync(string userName);
        Task<int> GetFailedLoginCountByUsernameAsync(string username);
        Task<bool> UpdateLockOutStatusAsync(string userName, bool setToLock, DateTime? endLockDate = null);
        #endregion
    }
}