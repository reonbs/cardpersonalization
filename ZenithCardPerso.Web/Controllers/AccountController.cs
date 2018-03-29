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

namespace ZenithCardPerso.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationRoleManager _roleManager;
        private readonly ApplicationSignInManager _signInManager;
        private readonly IAuthenticationManager _authManager;
        private IOrganisationQueryBLL _orgQueryBLL;
        private IManageUserQueryBLL _manageUserQueryBLL;
        private IManageUserCMDBLL _manageUserCMDBLL;


        public AccountController(
            ApplicationUserManager userManager,
            ApplicationRoleManager roleManager,
            ApplicationSignInManager signInManager,
            IAuthenticationManager authManager,
            IOrganisationQueryBLL orgQueryBLL,
            IManageUserQueryBLL manageUserQueryBLL,
            IManageUserCMDBLL manageUserCMDBLL
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _authManager = authManager;
            _orgQueryBLL = orgQueryBLL;
            _manageUserQueryBLL = manageUserQueryBLL;
            _manageUserCMDBLL = manageUserCMDBLL;

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
                var userIdentity = await UserManager.CreateIdentityAsync(usermodel, DefaultAuthenticationTypes.ApplicationCookie);

                userIdentity.AddClaim(new Claim("InstitutionID", usermodel.InstitutionID.ToString()));

                var listIdentity = new List<ClaimsIdentity>();
                listIdentity.Add(userIdentity);
                ClaimsPrincipal c = new ClaimsPrincipal(listIdentity);
                AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = model.RememberMe }, userIdentity);
            }
            else
            {
                ModelState.AddModelError("", "User is disabled");

                return RedirectToAction("Login");
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
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
        public ActionResult Users()
        {
            var users = UserManager.Users.ToList();

            return View(users);
        }

        [ValidateUserPermission(Permissions = "can_edit_user")]
        public ActionResult UserEdit(string ID)
        {
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
        public ActionResult UserEdit(ApplicationUser user, List<UserRoleViewModel> UserRole)
        {
            var loggedOnUser = User.Identity.Name;
            _manageUserCMDBLL.UpdateUser(user, loggedOnUser);

            var selectedRoles = UserRole.Where(x => x.SelectedRole == true).Select(x => x.Role).ToArray();

            UserManager.AddToRoles(user.Id, selectedRoles);

            var deselectedRoles = UserRole.Where(x => x.SelectedRole == false).Select(x => x.Role).ToArray();

            UserManager.RemoveFromRoles(user.Id, deselectedRoles);

            TempData["Message"] = "Success";

            LoadInstitution();

            return RedirectToAction("Users");

            
        }

        [ValidateUserPermission(Permissions = "can_create_user")]
        public ActionResult CreateUser()
        {

            var roles = RoleManager.Roles.ToList();
            List<UserRoleViewModel> userRoleVM = new List<UserRoleViewModel>();
            foreach (var item in roles)
            {
                userRoleVM.Add(new UserRoleViewModel { Role = item.Name });
            }

            var institutionID = ((ClaimsIdentity)User.Identity).FindFirst(Utilities.InstitutionID).Value;

            //RegisterViewModel userVM = new RegisterViewModel { UserRole = userRoleVM };
            ViewBag.UserRoles = userRoleVM;

            var institutions = _orgQueryBLL.GetInstitutions();
            ViewData["Institution"] = new SelectList(institutions, "ID", "Name", institutionID);
            return View();
        }

        [ValidateUserPermission(Permissions = "can_create_user")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateUser(RegisterViewModel model, List<UserRoleViewModel> UserRole)
        {
            var modelState = ModelState.Where(x => x.Value.Errors.Count > 0);

            if (ModelState.IsValid)
            {

                var roles = UserRole.Where(x => x.SelectedRole == true);

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
                    IsDisabled = model.IsDisabled
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
                        TempData["Message"] = "Success";
                        ModelState.Clear();
                    }
                    else
                    {
                        TempData["Message"] = "Failed";
                    }

                    return RedirectToAction("CreateUser");
                }
                //if (result.Succeeded)
                //{
                //    ApplicationUser usermodel = UserManager.Users.FirstOrDefault(m => m.UserName.Trim() == model.Email);
                //    if (usermodel != null)
                //    {
                //        var userIdentity = await UserManager.CreateIdentityAsync(usermodel, DefaultAuthenticationTypes.ApplicationCookie);
                //        userIdentity.AddClaim(new Claim("RegistrationType", usermodel.RegistrationType));
                //        userIdentity.AddClaim(new Claim("OrganizationCode", usermodel.OrganizationCode));


                //        var listIdentity = new List<ClaimsIdentity>();
                //        listIdentity.Add(userIdentity);
                //        ClaimsPrincipal c = new ClaimsPrincipal(listIdentity);
                //        AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, userIdentity);
                //    }

                //    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                //    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                //    // Send an email with this link
                //    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                //    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                //    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                //    return RedirectToAction("Index", "Home");

                //}
                AddErrors(result);
            }

            LoadInstitution();
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [ValidateUserPermission(Permissions = "can_create_role")]
        public ActionResult AddRole()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateUserPermission(Permissions = "can_create_role")]
        public ActionResult AddRole(ApplicationRole role)
        {
            //_roleManager.Roles.ToList();

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
        public ActionResult EditRole(ApplicationRole role)
        {
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
        public ActionResult ResetPassword(string email)
        {
            var user = UserManager.FindByName(email);
            var resetPasswordVM = new ResetPasswordViewModel { FullName = user.FullName,Email = user.Email };
            return View(resetPasswordVM);
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            var code =  UserManager.GeneratePasswordResetToken(user.Id);
            var result = await UserManager.ResetPasswordAsync(user.Id, code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
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
        public ActionResult LogOff()
        {
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