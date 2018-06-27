using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using WebApplication6.IdentityModels;
using ZenithCardPerso.Web.Filters;
using ZenithCardPerso.Web.Models;
using ZenithCardRepo.Data.IdentityModels;
using ZenithCardRepo.Services.BLL.Command;
using ZenithCardRepo.Services.BLL.Infrastructure;
using ZenithCardRepo.Services.BLL.Query;
using System.Web.Security.AntiXss;
using ZenithCardRepo.Services.BLL.UnitofWork;
using System.Net;

namespace ZenithCardPerso.Web.Controllers
{
    [Authorize]
    //[RequireHttps]
    public class AccountController : Controller
    {
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationRoleManager _roleManager;
        private readonly ApplicationSignInManager _signInManager;
        private readonly IAuthenticationManager _authManager;
        private IOrganisationQueryBLL _orgQueryBLL;
        private IManageUserQueryBLL _manageUserQueryBLL;
        private IManageUserCMDBLL _manageUserCMDBLL;
        public IPermissionQueryBLL _permissionQueryBLL;
        private UnitOfWork _unitOfWork;

        public AccountController(
            ApplicationUserManager userManager,
            ApplicationRoleManager roleManager,
            ApplicationSignInManager signInManager,
            IAuthenticationManager authManager,
            IOrganisationQueryBLL orgQueryBLL,
            IManageUserQueryBLL manageUserQueryBLL,
            IManageUserCMDBLL manageUserCMDBLL,
            IPermissionQueryBLL permissionQueryBLL,
            UnitOfWork unitOfWork
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _authManager = authManager;
            _orgQueryBLL = orgQueryBLL;
            _manageUserQueryBLL = manageUserQueryBLL;
            _manageUserCMDBLL = manageUserCMDBLL;
            _permissionQueryBLL = permissionQueryBLL;
            _unitOfWork = unitOfWork;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager;
            }

        }


        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager;
            }

        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager;
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return _authManager;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ApplicationUser usermodel = UserManager.Users.FirstOrDefault(m => m.UserName.Trim() == model.Email && m.IsDisabled == false);


            if (usermodel != null)
            {
                ClaimsIdentity userIdentity = await UserManager.CreateIdentityAsync(usermodel, DefaultAuthenticationTypes.ApplicationCookie);

                var userID = usermodel.Roles.Select(x => x.UserId).FirstOrDefault();
                var roles = UserManager.GetRoles(userID).ToArray(); ;

                
                var storedPermissions = _permissionQueryBLL.FetchUserPermission(usermodel.UserName, roles);

                userIdentity.AddClaim(new Claim("FullName",usermodel.FullName));

                userIdentity.AddClaim(new Claim("InstitutionID", usermodel.InstitutionID.ToString()));

                userIdentity.AddClaim(new Claim("UserPermissions", storedPermissions));

                var listIdentity = new List<ClaimsIdentity>();
                listIdentity.Add(userIdentity);
                ClaimsPrincipal c = new ClaimsPrincipal(listIdentity);
                AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = model.RememberMe }, userIdentity);

                if (usermodel.IsDefaultPassword)
                {
                    return RedirectToAction("ResetPassword", new { Id = usermodel.Id });
                }
            }
            else
            {
                ModelState.AddModelError("", "username or password is incorrect");
                TempData["Message"] = "Failed";
                return RedirectToAction("Login");
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    //TempData[Utilities.Activity_Log_Details] = $"User {model.Email} has successfully logged on";
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                    TempData["Message"] = "Failed";
                    return RedirectToAction("Login");
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }
        
        [ValidateUserPermission(Permissions = "can_view_users")]
        [Audit]
        public ActionResult Users()
        {
            var users = UserManager.Users.ToList();
            TempData[Utilities.Activity_Log_Details] = $"Users list was viewed successfully";
            return View(users);
        }

        
        [ValidateUserPermission(Permissions = "can_edit_user")]
        [Audit]
        public ActionResult UserEdit(string ID)
        {
            TempData[Utilities.Activity_Log_Details] = $"User details with id: {ID} has been viewed";

            var userManger = _manageUserQueryBLL.GetApplicationUser(ID);
            LoadInstitution();

            var userRoles = UserManager.GetRoles(ID);

            var allroles = RoleManager.Roles.ToList();

            List<UserRoleViewModel> UserRoleVM = new List<UserRoleViewModel>();
            //foreach (var role in userRoles)
            //{
            //    role.Name.Contains();
            //    UserRoleVM.Add(new UserRoleViewModel { SelectedRole= true, Role = role });
            //}

            foreach (var role in allroles)
            {

                UserRoleVM.Add(new UserRoleViewModel { SelectedRole = false, Role = role.Name });
            }

            foreach (var userRoleVM in UserRoleVM)
            {
                var isContained = userRoles.Contains(userRoleVM.Role);

                if (isContained)
                {
                    userRoleVM.SelectedRole = true;
                }
            }

            ViewBag.UserRoles = UserRoleVM;
            

            return View(userManger);
        }
        public void LoadInstitution()
        {
            var institutions = _orgQueryBLL.GetInstitutions();
            ViewData["Institution"] = new SelectList(institutions, "ID", "Name", "");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateUserPermission(Permissions = "can_edit_user")]
        [Audit]
        public async Task<ActionResult> UserEdit(ApplicationUser user, List<UserRoleViewModel> UserRole)
        {
            TempData[Utilities.Activity_Log_Details] = $"User {user.Id} was updated successfully";

            var loggedOnUser = User.Identity.Name;
            _manageUserCMDBLL.UpdateUser(user, loggedOnUser);

            var selectedRoles = UserRole.Where(x => x.SelectedRole == true).Select(x => x.Role).ToArray();

            foreach (var selectedRole in selectedRoles)
            {
                //UserManager.AddToRole(user.Id, selectedRole);

                await UserManager.AddToRoleAsync(user.Id, selectedRole);
            }


            var deselectedRoles = UserRole.Where(x => x.SelectedRole == false).Select(x => x.Role).ToArray();

            foreach (var deselectedRole in deselectedRoles)
            {
                //UserManager.RemoveFromRole(user.Id, deselectedRole);
                await UserManager.RemoveFromRoleAsync(user.Id, deselectedRole);
            }

            TempData["Message"] = "Success";

            LoadInstitution();

            return RedirectToAction("Users");
        }

        [ValidateUserPermission(Permissions = "can_create_user,can_create_institutionusers")]
        public ActionResult CreateUser()
        {
            var institutionID = ((ClaimsIdentity)User.Identity).FindFirst(Utilities.InstitutionID).Value;

            LoadApplicationRoles();

            var institutions = _orgQueryBLL.GetInstitutions();
            ViewData["Institution"] = new SelectList(institutions, "ID", "Name", institutionID);
            return View();
        }

        

        [ValidateUserPermission(Permissions = "can_create_user,can_create_institutionusers")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Audit]
        public async Task<ActionResult> CreateUser(RegisterViewModel model, List<UserRoleViewModel> UserRole)
        {
            TempData[Utilities.Activity_Log_Details] = $"User with username: {model.UserName} has been created successfully";
            var modelState = ModelState.Where(x => x.Value.Errors.Count > 0);

            if (ModelState.IsValid)
            {
                List<UserRoleViewModel> roles = new List<UserRoleViewModel>();
                if (UserRole != null)
                {
                    roles = UserRole.Where(x => x.SelectedRole == true).ToList();
                }
                else
                {
                    roles.Add(new UserRoleViewModel { SelectedRole = true, Role = "Enroller" });
                }

                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    InstitutionID = model.InstitutionID,
                    FirstName = model.FirstName,
                    MiddleName = model.MiddleName,
                    LastName = model.LastName,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    IsDisabled = model.IsDisabled,
                    IsDefaultPassword = true
                };

                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    ApplicationUser usermodel = UserManager.Users.FirstOrDefault(m => m.UserName.Trim() == model.Email);

                    List<string> selectecRoles = new List<string>();


                    foreach (var role in roles)
                    {
                        selectecRoles.Add(role.Role);
                    }


                    var addRoleResult = await UserManager.AddToRolesAsync(usermodel.Id, selectecRoles.ToArray<string>());

                    if (addRoleResult.Succeeded)
                    {
                        TempData[Utilities.Activity_Log_Details] = $"User with username: {usermodel.UserName} has been created successfully";
                        TempData["Message"] = "Success";
                        ModelState.Clear();
                    }
                    else
                    {
                        TempData["Message"] = "Failed";
                    }

                    return RedirectToAction("CreateUser");
                }
                else
                {
                    
                    AddErrors(result);
                    LoadApplicationRoles();
                    LoadInstitution();
                    return View();
                }


                
            }
            

            LoadInstitution();
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public void LoadApplicationRoles()
        {
            var roles = RoleManager.Roles.ToList();
            List<UserRoleViewModel> userRoleVM = new List<UserRoleViewModel>();
            foreach (var item in roles)
            {
                userRoleVM.Add(new UserRoleViewModel { Role = item.Name });
            }



            //RegisterViewModel userVM = new RegisterViewModel { UserRole = userRoleVM };
            ViewBag.UserRoles = userRoleVM;
        }
        public ActionResult CreateInstitutionUser()
        {

            return View();
        }
        [ValidateUserPermission(Permissions = "can_create_role")]
        public ActionResult AddRole()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateUserPermission(Permissions = "can_create_role")]
        [Audit]
        public ActionResult AddRole(ApplicationRole role)
        {
            //_roleManager.Roles.ToList();

            TempData[Utilities.Activity_Log_Details] = $"Role {role.Name} was created successfully";

            RoleManager.Create(role);

            ModelState.Clear();


            TempData["Message"] = "Success";
            return View();
        }

        [ValidateUserPermission(Permissions = "can_view_roles")]
        public ActionResult Roles()
        {
            var roles = RoleManager.Roles.ToList();

            return View(roles);
        }

        [ValidateUserPermission(Permissions = "can_edit_role")]
        public ActionResult EditRole(string ID)
        {
            var role = RoleManager.Roles.FirstOrDefault(x => x.Id == ID);
            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateUserPermission(Permissions = "can_edit_role")]
        [Audit]
        public ActionResult EditRole(ApplicationRole role)
        {
            TempData[Utilities.Activity_Log_Details] = $"Role {role.Name} has been edited";
            RoleManager.Update(role);

            
            TempData["Message"] = "Success";
            return View();
        }
        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        public ActionResult ResetPassword(string Id)
        {
            var user = UserManager.FindById(Id);
            var resetPasswordVM = new ResetPasswordViewModel { FullName = user.FullName, Email = user.Email };
            return View(resetPasswordVM);
        }


        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Audit]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            //TempData[Utilities.Activity_Log_Details] = $"User {model.Email} has carried out a password reset";

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await ResetUser(model);

            if (result.Succeeded)
            {
                TempData[Utilities.Activity_Log_Details] = $"User {model.FullName} has carried out a password reset";
                TempData["Message"] = "Success";
                return RedirectToAction("Login", "Account");
            }
            AddErrors(result);
            return View();
        }

        [ValidateUserPermission(Permissions ="can_reset_password")]
        public ActionResult ResetPasswordAdmin(string Id)
        {
            
            var user = UserManager.FindById(Id);
            if (user != null)
            {
                var resetPasswordVM = new ResetPasswordViewModel { FullName = user.FullName, Email = user.Email };
                return View(resetPasswordVM);
            }
            else
            {
                TempData["Message"] = "Failed";
                return RedirectToAction("Users");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateUserPermission(Permissions = "can_reset_password")]
        public async Task<ActionResult> ResetPasswordAdmin(ResetPasswordViewModel model)
        {
            TempData[Utilities.Activity_Log_Details] = $"User with username {model.Email} password was reset";

            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var result = await ResetUser(model);

            if (result.Succeeded)
            {
                
                TempData["Message"] = "Success";
                return RedirectToAction("Users", "Account");
            }
            AddErrors(result);
            return View();
        }

        public async Task<IdentityResult> ResetUser(ResetPasswordViewModel model)
        {
            var user = await UserManager.FindByIdAsync(model.Id);

            UserManager.RemovePassword(user.Id);

            var result = UserManager.AddPassword(user.Id, model.Password);
            if (result.Succeeded)
            {
                _manageUserCMDBLL.UpdatedDefaultPassword(user);
            }
            return result;
        }
        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Audit]
        public ActionResult LogOff()
        {
            TempData[Utilities.Activity_Log_Details] = $"User {User.Identity.Name} logged off";
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("CardApplicationCreate", "CardApplication");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ValidateUserPermission(Permissions ="can_delete_users")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var user = await _userManager.FindByIdAsync(id);
                var logins = user.Logins;
                var rolesForUser = await _userManager.GetRolesAsync(id);

                using (var transaction = _unitOfWork.context.Database.BeginTransaction())
                {
                    foreach (var login in logins.ToList())
                    {
                        await _userManager.RemoveLoginAsync(login.UserId, new UserLoginInfo(login.LoginProvider, login.ProviderKey));
                    }

                    if (rolesForUser.Count() > 0)
                    {
                        foreach (var item in rolesForUser.ToList())
                        {
                            // item should be the name of the role
                            var result = await _userManager.RemoveFromRoleAsync(user.Id, item);
                        }
                    }

                    await _userManager.DeleteAsync(user);
                    transaction.Commit();
                }

                return RedirectToAction("Users");
            }
            else
            {
                return View();
            }
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";


        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("CardApplicationCreate", "CardApplication");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}