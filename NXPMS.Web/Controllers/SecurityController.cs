using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using NXPMS.Base.Models.EmployeesModels;
using NXPMS.Base.Models.GlobalSettingsModels;
using NXPMS.Base.Models.SecurityModels;
using NXPMS.Base.Services;
using NXPMS.Web.Models.EmployeesViewModels;
using NXPMS.Web.Models.SecurityModels;
using static NXPMS.Base.Services.SecurityService;

namespace NXPMS.Web.Controllers
{
    [Authorize]
    public class SecurityController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ISecurityService _securityService;
        private readonly IGlobalSettingsService _globalSettingsService;
        private readonly IEmployeeRecordService _employeeRecordService;
        //private readonly IWebHostEnvironment _webHostEnvironment;

        public SecurityController(IConfiguration configuration, ISecurityService securityService,
            IGlobalSettingsService globalSettingsService, IEmployeeRecordService employeeRecordService)
        {
            _configuration = configuration;
            _securityService = securityService;
            _globalSettingsService = globalSettingsService;
            _employeeRecordService = employeeRecordService;
            //_webHostEnvironment = webHostingEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Login Action Methods

        [AllowAnonymous]
        public IActionResult Login(string ReturnUrl = null)
        {
            LoginViewModel model = new LoginViewModel();
            if (!string.IsNullOrEmpty(ReturnUrl))
            {
                model.ReturnUrl = ReturnUrl;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {

            if (!ModelState.IsValid)
            {
                model.ViewModelErrorMessage = "Invalid Login Attempt.";
                return View(model);
            }

            try
            {
                string UserName = model.Username;
                bool IsPersistent = model.IsPersistent;

                var users = _securityService.GetUsersByUsernameAsync(model.Username).Result;
                if (users == null || users.Count < 1)
                {
                    model.ViewModelErrorMessage = "Invalid Login Attempt.";
                    model.OperationIsSuccessful = false;
                    return View(model);
                }
                ApplicationUser user = users.FirstOrDefault();
                if (user.LockoutEnabled)
                {
                    model.ViewModelErrorMessage = "Sorry, this user account is locked!";
                    model.OperationIsSuccessful = false;
                    return View(model);
                }
                if (user.Username == model.Username && _securityService.ValidatePassword(model.Password, user.PasswordHash))
                {
                    List<UserPermission> permissionList = new List<UserPermission>();
                    var claims = new List<Claim>();
                    if (!string.IsNullOrEmpty(user.FullName)) { claims.Add(new Claim(ClaimTypes.Name, user.FullName)); }
                    if (user.Id > 0) { claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())); }

                    var entities = await _securityService.GetPermissionsByUserIdAsync(user.Id);
                    if (entities != null && entities.Count > 0)
                    {
                        permissionList = entities.ToList();
                        foreach (var permission in permissionList)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, permission.RoleCode));
                        }
                    }

                    var authProperties = new AuthenticationProperties
                    {
                        AllowRefresh = true,
                        // Refreshing the authentication session should be allowed.

                        //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                        // The time at which the authentication ticket expires. A 
                        // value set here overrides the ExpireTimeSpan option of 
                        // CookieAuthenticationOptions set with AddCookie.

                        IsPersistent = model.IsPersistent
                        // Whether the authentication session is persisted across 
                        // multiple requests. When used with cookies, controls
                        // whether the cookie's lifetime is absolute (matching the
                        // lifetime of the authentication ticket) or session-based.

                        //IssuedUtc = <DateTimeOffset>,
                        // The time at which the authentication ticket was issued.

                        //RedirectUri = <string>
                        // The full path or absolute URI to be used as an http 
                        // redirect response value.
                    };

                    var identity = new ClaimsIdentity(claims, SecurityKeys.NxpmsCookieAuth);
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(SecurityKeys.NxpmsCookieAuth, claimsPrincipal, authProperties);

                    LoginAttempt attempt = new LoginAttempt
                    {
                        IsSuccessful = true,
                        LoginTime = DateTime.UtcNow,
                        Username = user.Username,
                    };

                    await _securityService.RegisterLoginAsync(attempt);

                    return Redirect(model.ReturnUrl == null ? "/Home/Index" : model.ReturnUrl);
                }
                else
                {
                    LoginAttempt attempt = new LoginAttempt
                    {
                        IsSuccessful = false,
                        LoginTime = DateTime.UtcNow,
                        Username = model.Username,
                    };
                    await _securityService.RegisterLoginAsync(attempt);
                    await _securityService.IncrementFailedLoginCount(model.Username);
                    int afc = await _securityService.GetFailedLoginCountAsync(model.Username);
                    if (afc > 5)
                    {

                    }
                    model.ViewModelErrorMessage = "Invalid Login Attempt.";
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message; //"Login Error!";
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Security");
        }
        #endregion

        #region Security Utility Action Methods
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }


        #endregion

        #region Users Action Methods

        [Authorize(Roles = "SCTADM, XXACC")]
        public async Task<IActionResult> Users(int? id = null, string sp = null)
        {
            UserListViewModel model = new UserListViewModel();
            try
            {
                if (!string.IsNullOrWhiteSpace(sp))
                {
                    ViewData["SearchString"] = sp;
                    ApplicationUser user = await _securityService.GetUserByFullNameAsync(sp);
                    if (user == null || user.Id < 1)
                    {
                        var entities = await _securityService.SearchUsersByNameAsync(sp);
                        if (entities != null && entities.Count > 0)
                        {
                            model.UserList = entities.ToList();
                            return View(model);
                        }
                    }
                    else
                    {
                        return RedirectToAction("Profile", new { id = user.Id });
                    }
                }
                else if (id != null && id.Value > 0)
                {
                    var entities = await _securityService.GetUsersByLocationIdAsync(id.Value);
                    if (entities != null && entities.Count > 0)
                    {
                        model.UserList = entities.ToList();
                    }
                }
                else
                {
                    var entities = await _securityService.GetAllUsersAsync();
                    if (entities != null && entities.Count > 0)
                    {
                        model.UserList = entities.ToList();
                    }
                }

                var locations = await _globalSettingsService.GetLocationsAsync();
                ViewBag.LocationList = new SelectList(locations.ToList(), "LocationId", "LocationName");
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [Authorize(Roles = "SCTADM, XXACC")]
        public async Task<IActionResult> ManageUser(int id = 0, int? ed = null)
        {
            UserViewModel model = new UserViewModel();
            try
            {
                if (id == 0)
                {
                    model.ViewPageTitle = "New User";
                    if (ed != null && ed.Value > 0)
                    {
                        Employee employee = await _employeeRecordService.GetEmployeeByIdAsync(ed.Value);
                        if (employee != null && !string.IsNullOrWhiteSpace(employee.FullName))
                        {
                            model.EmployeeId = employee.EmployeeID;
                            model.FullName = employee.FullName;
                            model.Username = employee.EmployeeNo;
                        }
                    }
                }
                else
                {
                    ApplicationUser user = new ApplicationUser();
                    user = await _securityService.GetUserByIdAsync(id);
                    if (user != null)
                    {
                        model = model.ExtractFromUser(user);
                    }
                    model.ViewPageTitle = "Edit User";
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [Authorize(Roles = "SCTADM, XXACC")]
        [HttpPost]
        public async Task<IActionResult> ManageUser(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = model.ConvertToUser();
                try
                {
                    if (model.UserId < 1)
                    {
                        model.ViewPageTitle = "New User";
                        user.PasswordHash = _securityService.CreatePasswordHash(model.Password);
                        user.CreatedBy = HttpContext.User.Identity.Name;
                        user.CreatedTime = DateTime.UtcNow;
                        bool UserIsAdded = await _securityService.CreateUserAsync(user);
                        if (UserIsAdded)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "New User added successfully!";
                        }
                    }
                    else
                    {
                        model.ViewPageTitle = "New User";
                        user.ModifiedBy = HttpContext.User.Identity.Name;
                        user.ModifiedTime = DateTime.UtcNow;
                        bool UserIsUpdated = await _securityService.UpdateUserAsync(user);
                        if (UserIsUpdated)
                        {
                            model.ViewModelSuccessMessage = "User updated successfully!";
                        }
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }

        public async Task<IActionResult> UserInfo(int id = 0)
        {
            UserViewModel model = new UserViewModel();
            ApplicationUser user = new ApplicationUser();
            user = await _securityService.GetUserByIdAsync(id);
            if (user != null)
            {
                model = model.ExtractFromUser(user);
            }
            return View(model);
        }

        [Authorize(Roles = "SCTADM, XXACC")]
        public async Task<IActionResult> SelectEmployee(string est)
        {
            EmployeeSearchViewModel model = new EmployeeSearchViewModel();
            if (!string.IsNullOrWhiteSpace(est))
            {
                //EmployeeViewModel employee = new EmployeeViewModel();
                Employee entity = await _employeeRecordService.GetEmployeeByFullNameAsync(est);
                if (entity != null && entity.EmployeeID > 0)
                {
                    return RedirectToAction("ManageUser", new { ed = entity.EmployeeID });
                }
                else
                {
                    var entities = await _employeeRecordService.SearchNonUserEmployeesByNameAsync(est);
                    if (entities != null && entities.Count > 0)
                    {
                        List<Employee> employees = new List<Employee>();
                        employees = entities.ToList();
                        model.EmployeesList = employees;
                    }
                }
            }
            return View(model);
        }

        [Authorize(Roles = "SCTADM, XXACC")]
        public async Task<IActionResult> ResetPassword(int id)
        {
            ResetPasswordViewModel model = new ResetPasswordViewModel();
            try
            {
                if (id == 0)
                {
                    model.ViewModelErrorMessage = "Sorry, no record was found for the selected user.";
                    model.OperationIsSuccessful = true;
                }
                else
                {
                    ApplicationUser user = new ApplicationUser();
                    user = await _securityService.GetUserByIdAsync(id);
                    if (user != null)
                    {
                        model.UserFullName = user.FullName;
                        model.Username = user.Username;
                        model.UserId = user.Id;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [Authorize(Roles = "SCTADM, XXACC")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ApplicationUser user = new ApplicationUser();
                    if (model.UserId > 0)
                    {
                        user.Id = model.UserId;
                        user.FullName = model.UserFullName;
                        user.Username = model.Username;
                        user.PasswordHash = _securityService.CreatePasswordHash(model.Password);
                        user.ModifiedBy = HttpContext.User.Identity.Name;
                        user.ModifiedTime = DateTime.UtcNow;
                        bool UserIsUpdated = await _securityService.ResetUserPasswordAsync(user);
                        if (UserIsUpdated)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "User password was updated successfully!";
                        }
                    }
                    else
                    {
                        model.OperationIsSuccessful = true;
                        model.ViewModelErrorMessage = "Sorry, password update failed due to invalid parameter values.";
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }

        [Authorize(Roles = "SCTADM, XXACC")]
        public async Task<IActionResult> Permissions(int id, string pd = null)
        {
            PermissionsListViewModel model = new PermissionsListViewModel();
            List<SystemApplication> systemApplications = new List<SystemApplication>();

            model.UserID = id;
            if (!string.IsNullOrWhiteSpace(pd))
            {
                var entities = await _securityService.GetPermissionsByUserIdAsync(id, pd);
                if (entities != null && entities.Count > 0)
                {
                    model.PermissionsList = entities.ToList();
                }
            }
            else
            {
                var entities = await _securityService.GetPermissionsByUserIdAsync(id);
                if (entities != null && entities.Count > 0)
                {
                    model.PermissionsList = entities.ToList();
                }
            }

            var apps = await _globalSettingsService.GetSystemApplicationsAsync();
            if(apps != null && apps.Count > 0)
            {
                systemApplications = apps.ToList();
            }
            ViewBag.ApplicationsList = new SelectList(systemApplications, "Code", "Description", pd);
            return View(model);
        }

        [Authorize(Roles = "SCTADM, XXACC")]
        public async Task<IActionResult> NewPermissions(int id, string pd = null)
        {
            NewPermissionsListViewModel model = new NewPermissionsListViewModel();
            List<SystemApplication> systemApplications = new List<SystemApplication>();

            model.UserID = id;
            if (!string.IsNullOrWhiteSpace(pd))
            {
                var entities = await _securityService.GetUnallocatedRolesAsync(id, pd);
                if (entities != null && entities.Count > 0)
                {
                    model.ApplicationRolesList = entities.ToList();
                }
            }
            else
            {
                var entities = await _securityService.GetUnallocatedRolesAsync(id);
                if (entities != null && entities.Count > 0)
                {
                    model.ApplicationRolesList = entities.ToList();
                }
            }

            var apps = await _globalSettingsService.GetSystemApplicationsAsync();
            if (apps != null && apps.Count > 0)
            {
                systemApplications = apps.ToList();
            }
            ViewBag.ApplicationsList = new SelectList(systemApplications, "Code", "Description", pd);
            return View(model);
        }

        [Authorize(Roles = "SCTADM, XXACC")]
        [HttpPost]
        public string GrantUserPermission(int usd, string rld)
        {
            if ((usd < 1) && string.IsNullOrWhiteSpace(rld)) { return "parameter"; }
            string actionBy = HttpContext.User.Identity.Name;
            try
            {
                if (_securityService.GrantPermissionAsync(usd, rld, actionBy).Result)
                {
                    return "granted";
                }
                else
                {
                    return "failed";
                }
            }
            catch
            {
                return "failed";
            }
        }

        [Authorize(Roles = "SCTADM, XXACC")]
        [HttpPost]
        public string RevokeUserPermission(int usd, string rld)
        {
            if ((usd < 1) || string.IsNullOrWhiteSpace(rld)) { return "parameter"; }
            string actionBy = HttpContext.User.Identity.Name;
            try
            {
                if (_securityService.RevokePermissionAsync(usd, rld, actionBy).Result)
                {
                    return "revoked";
                }
                else
                {
                    return "failed";
                }
            }
            catch
            {
                return "failed";
            }
        }

        #endregion
    }
}