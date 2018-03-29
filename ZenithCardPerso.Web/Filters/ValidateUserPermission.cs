using Autofac;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ZenithCardRepo.Data;
using ZenithCardRepo.Services.BLL.Infrastructure;
using ZenithCardRepo.Services.BLL.Query;

namespace ZenithCardPerso.Web.Filters
{
    public class ValidateUserPermission : AuthorizeAttribute, IActionFilter
    {
        public ILifetimeScope Resolver { get; set; }
        public IPermissionQueryBLL _permissionQueryBLL { get; set; }

        string controller = HttpContext.Current.Request.RequestContext.RouteData.Values["Controller"].ToString();
        string ActionView = HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString();
        List<string> UserPermissions = new List<string>();
        public string[] InnerRoles = new string[] { };

        /// <summary>
        /// Gets or sets the permissions for the users at both controller and action level
        /// </summary>
        public string Permissions { get; set; }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (UserPermissions.Count > 0)
            {
                try
                {
                    //SessionManager.InitializeSession(WebSecurity.CurrentUserName);
                }
                finally
                {

                }

            }
            else
            {

                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Permission", action = "AccessDenied" }));

                filterContext.Result.ExecuteResult(filterContext.Controller.ControllerContext);
            }
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //string[] StoredUserPermissions = Convert.ToString(HttpContext.Current.Session[Sessions.PERMISSION_LIST]).Split(',');

            //foreach(var value in StoredUserPermissions)
            //{
            //    UserPermissions.Add(value);
            //}

            UserPermissions.Clear();

            if (Permissions != null)
            {
                string[] permissions = Permissions.Split(',');

                //Get the username of the current logged in user.
                String userName = filterContext.HttpContext.User.Identity.Name;
                string userId = filterContext.HttpContext.User.Identity.GetUserId<string>();
                //Get the roles the user belong to
                var roleEnable = System.Web.Security.Roles.Enabled;


                string[] roles = _permissionQueryBLL.FetchUserRoles(userId).ToArray();

                foreach (var role in roles)
                {
                    foreach (string permission in permissions)
                    {
                        string[] storedPermissions = _permissionQueryBLL.FetchUserPermission(userName, role).Select(x => x.Permission).ToArray(); //PermissionDBO.FetchUserPermissions(userName, role);
                       /// var permissionBDO = new PermissionDBO();


                        //If the user permissions is in the list of authorized permissions return true. 
                        for (int k = 0; k < storedPermissions.Length; k++)
                        {
                            //convert the string array of valid permissions to a string
                            //string strPermissions = string.Join(",", Permissions);

                            if (Permissions.Contains(storedPermissions[k]))
                                UserPermissions.Add(permission);
                        }
                    }
                }
            }
        }
    }
}