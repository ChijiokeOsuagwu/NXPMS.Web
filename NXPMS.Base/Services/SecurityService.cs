using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using NXPMS.Base.Models.SecurityModels;
using NXPMS.Base.Repositories.SecurityRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NXPMS.Base.Services
{
    public class SecurityService : ISecurityService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserPermissionRepository _userPermissionRepository;
        //private readonly IRoleRepository _roleRepository;

        public SecurityService(IUserRepository userRepository, IUserPermissionRepository userPermissionRepository)
        {
            _userRepository = userRepository;
            _userPermissionRepository = userPermissionRepository;
            //_userLoginRepository = userLoginRepository;
            //_roleRepository = roleRepository;
        }

        #region User Read Action Methods
        public async Task<IList<ApplicationUser>> GetUsersByUsernameAsync(string Username)
        {
            List<ApplicationUser> applicationUsers = new List<ApplicationUser>();
            var entities = await _userRepository.GetByUsernameAsync(Username);
            if (entities != null && entities.Count > 0)
            {
                applicationUsers = entities.ToList();
            }
            return applicationUsers;
        }
        public async Task<IList<ApplicationUser>> SearchUsersByNameAsync(string Name)
        {
            List<ApplicationUser> applicationUsers = new List<ApplicationUser>();
            var entities = await _userRepository.FindByNameAsync(Name);
            if (entities != null && entities.Count > 0)
            {
                applicationUsers = entities.ToList();
            }
            return applicationUsers;
        }
        public async Task<IList<ApplicationUser>> GetAllUsersAsync()
        {
            List<ApplicationUser> applicationUsers = new List<ApplicationUser>();
            var entities = await _userRepository.GetAllAsync();
            if (entities != null && entities.Count > 0)
            {
                applicationUsers = entities.ToList();
            }
            return applicationUsers;
        }
        public async Task<IList<ApplicationUser>> GetUsersByLocationIdAsync(int LocationId)
        {
            List<ApplicationUser> applicationUsers = new List<ApplicationUser>();
            var entities = await _userRepository.GetByLocationIdAsync(LocationId);
            if (entities != null && entities.Count > 0)
            {
                applicationUsers = entities.ToList();
            }
            return applicationUsers;
        }
        public async Task<ApplicationUser> GetUserByFullNameAsync(string FullName)
        {
            ApplicationUser applicationUser = new ApplicationUser();
            var entities = await _userRepository.GetByNameAsync(FullName);
            if (entities != null && entities.Count > 0)
            {
                applicationUser = entities.ToList().FirstOrDefault();
            }
            return applicationUser;
        }
        public async Task<ApplicationUser> GetUserByIdAsync(int UserId)
        {
            ApplicationUser applicationUser = new ApplicationUser();

            var entities = await _userRepository.GetByIdAsync(UserId);
            if (entities != null && entities.Count > 0)
            {
                applicationUser = entities.FirstOrDefault();
            }
            return applicationUser;
        }
        public async Task<bool> UsernameIsAvailableAsync(int UserId, string Username)
        {
            List<ApplicationUser> applicationUsers = new List<ApplicationUser>();
            var entities = await _userRepository.GetOtherUsersWithSameUsernameAsync(UserId, Username);
            if (entities != null && entities.Count > 0)
            {
                applicationUsers = entities.ToList();
            }
            return applicationUsers.Count < 1;
        }
        #endregion

        #region User Write Action Methods
        public async Task<bool> CreateUserAsync(ApplicationUser user)
        {
            if (user == null) { throw new ArgumentNullException(nameof(user), "The required parameter [User] is missing."); }

            var entitiesWithSameUsername = await _userRepository.GetByUsernameAsync(user.Username);
            if (entitiesWithSameUsername.Count > 0)
            {
                throw new Exception("The Username you entered already exists.");
            }

            return await _userRepository.AddAsync(user);
        }
        public async Task<bool> ResetUserPasswordAsync(ApplicationUser user)
        {
            if (user == null) { throw new ArgumentNullException(nameof(user), "The required parameter [user] is missing."); }
            if (string.IsNullOrEmpty(user.PasswordHash)) { throw new ArgumentNullException(nameof(user.PasswordHash), "The required parameter [PasswordHash] is missing."); }
            return await _userRepository.UpdateUserPasswordAsync(user);
        }
        public async Task<bool> UpdateUserAsync(ApplicationUser user)
        {
            if (user == null) { throw new ArgumentNullException(nameof(user), "The required parameter [user] is missing."); }

            return await _userRepository.UpdateAsync(user);
        }
        public async Task<bool> DeleteUserAsync(int userId, string deletedBy)
        {
            if (userId < 1) { throw new ArgumentNullException(nameof(userId), "The required parameter [User ID] is missing."); }
            return await _userRepository.DeleteByIdAsync(userId);
        }

        #endregion

        #region Application Role Read Action Methods
        public async Task<IList<ApplicationRole>> GetRolesByApplicationCodeAsync(string ApplicationCode)
        {
            List<ApplicationRole> applicationRoles = new List<ApplicationRole>();
            var entities = await _userPermissionRepository.GetByAppCodeAsync(ApplicationCode);
            if (entities != null && entities.Count > 0)
            {
                applicationRoles = entities.ToList();
            }
            return applicationRoles;
        }
        public async Task<IList<ApplicationRole>> GetRolesAsync()
        {
            List<ApplicationRole> applicationRoles = new List<ApplicationRole>();
            var entities = await _userPermissionRepository.GetAllAsync();
            if (entities != null && entities.Count > 0)
            {
                applicationRoles = entities.ToList();
            }
            return applicationRoles;
        }

        public async Task<IList<ApplicationRole>> GetUnallocatedRolesAsync(int UserId, string ApplicationCode = null)
        {
            List<ApplicationRole> applicationRoles = new List<ApplicationRole>();
            if (!string.IsNullOrWhiteSpace(ApplicationCode))
            {
                var entities = await _userPermissionRepository.GetUnallocatedRolesByUserIdAndAppCodeAsync(UserId, ApplicationCode);
                if (entities != null && entities.Count > 0)
                {
                    applicationRoles = entities.ToList();
                }
            }
            else
            {
                var entities = await _userPermissionRepository.GetUnallocatedRolesByUserIdAsync(UserId);
                if (entities != null && entities.Count > 0)
                {
                    applicationRoles = entities.ToList();
                }
            }
            return applicationRoles;
        }


        #endregion

        #region User Permissions Read Service Methods
        public async Task<IList<UserPermission>> GetPermissionsByUserIdAsync(int UserId, string applicationCode = null)
        {
            List<UserPermission> userPermissions = new List<UserPermission>();
            if (string.IsNullOrWhiteSpace(applicationCode))
            {
                var entities = await _userPermissionRepository.GetByUserIdAsync(UserId);
                if (entities != null && entities.Count > 0)
                {
                    userPermissions = entities.ToList();
                }
            }
            else
            {
                var entities = await _userPermissionRepository.GetByUserIdAndAppCodeAsync(UserId, applicationCode);
                if (entities != null && entities.Count > 0)
                {
                    userPermissions = entities.ToList();
                }
            }

            return userPermissions;
        }

        #endregion

        #region User Permission Write Service Method
        public async Task<bool> GrantPermissionAsync(int usd, string rld, string actionBy)
        {
            UserPermission permission = new UserPermission();
            permission.RoleCode = rld;
            permission.UserID = usd;
            permission.LastModifiedBy = actionBy;
            permission.LastModifiedTime = DateTime.UtcNow;
            return await _userPermissionRepository.AddUserPermissionAsync(permission);
        }

        public async Task<bool> RevokePermissionAsync(int usd, string rld, string actionBy)
        {
            UserPermission permission = new UserPermission();
            permission.RoleCode = rld;
            permission.UserID = usd;
            permission.LastModifiedBy = actionBy;
            permission.LastModifiedTime = DateTime.UtcNow;
            return await _userPermissionRepository.DeleteUserPermissionAsync(permission);
        }

        public async Task<bool> RevokePermissionAsync(int pmd)
        {
            return await _userPermissionRepository.DeleteUserPermissionAsync(pmd);
        }

        #endregion


        #region Login Action Methods
        public async Task<bool> RegisterLoginAsync(LoginAttempt attempt)
        {
            if (attempt == null) { throw new ArgumentNullException(nameof(attempt), "The required parameter [attempt] is missing."); }
            return await _userRepository.AddAttemptAsync(attempt);
        }

        public async Task<bool> IncrementFailedLoginCount(string Username)
        {
            return await _userRepository.IncreaseFailedLoginCountAsync(Username);
        }

        public async Task<bool> ResetFailedLoginCount(string Username)
        {
            return await _userRepository.ClearFailedLoginCountAsync(Username);
        }

        public async Task<int> GetFailedLoginCountAsync(string Username)
        {
            return await _userRepository.GetFailedLoginCountByUsernameAsync(Username);
        }

        public async Task<bool> LockUserAccountAsync(string Username, DateTime? EndLockOutDate = null)
        {
            return await _userRepository.UpdateLockOutStatusAsync(Username, true, EndLockOutDate);
        }

        public async Task<bool> UnLockUserAccountAsync(string Username)
        {
            return await _userRepository.UpdateLockOutStatusAsync(Username, false, null);
        }



        #endregion
        //============ Cryptography Action Methods =====================================//
        #region Cryptography Action Methods
        public string CreatePasswordHash(string plainTextPassword)
        {
            var valueBytes = KeyDerivation.Pbkdf2(
                                password: plainTextPassword,
                                salt: Encoding.UTF8.GetBytes(PasswordSalt),
                                prf: KeyDerivationPrf.HMACSHA512,
                                iterationCount: 10000,
                                numBytesRequested: 256 / 8);

            return Convert.ToBase64String(valueBytes);
        }

        public bool ValidatePassword(string plainTextPassword, string hashedPassword)
          => CreatePasswordHash(plainTextPassword) == hashedPassword;

        private const string PasswordSalt = "CGYzqrN4plZekNC35Uxp1Q==";
        #endregion
        
        //============== Protection Keys ===========================-===================//
        #region Protection Keys 
        public class SecurityKeys
        {
            public const string NxpmsCookieAuth = "OmAuthsVqr8b0zt";
        }

        public class DataProtectionEncryptionStrings
        {
            public readonly string RouteValuesEncryptionCode = "NxpmsEncryptionCode";
        }
        #endregion
    }
}
