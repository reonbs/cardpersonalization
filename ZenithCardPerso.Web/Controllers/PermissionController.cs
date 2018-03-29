using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZenithCardPerso.Web.Filters;
using ZenithCardRepo.Data.Models;
using ZenithCardRepo.Data.ViewModel;
using ZenithCardRepo.Services.BLL.Command;
using ZenithCardRepo.Services.BLL.Query;

namespace ZenithCardPerso.Web.Controllers
{
    public class PermissionController : Controller
    {
        private ILog _log;
        private IPermissionCMDBLL _permissionCMDBLL;
        private IPermissionQueryBLL _permissionQueryBLL;
        
        public PermissionController(
            ILog log,
            IPermissionCMDBLL permissionCMDBLL,
            IPermissionQueryBLL permissionQueryBLL
            )
        {
            _log = log;
            _permissionCMDBLL = permissionCMDBLL;
            _permissionQueryBLL = permissionQueryBLL;
            //Roles = System.Web.Security.Roles.GetRolesForUser();
        }
        
        //[ValidateUserPermission(Permissions = "can_create_permission")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[ValidateUserPermission(Permissions = "can_create_permission")]
        public ActionResult Create(Permission permission)
        {
            try
            {
                _permissionCMDBLL.CreatePermission(permission);

                TempData["Message"] = "Success";

                return RedirectToAction("Permissions");
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            return View();
        }

        //[ValidateUserPermission(Permissions = "can_view_permissions")]
        public ActionResult Permissions()
        {
            try
            {
                var permissions = _permissionQueryBLL.GetPermissions();
                return View(permissions);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            ModelState.AddModelError("", "Error Retrieving List of Permissions");
            return View();
        }

        //[ValidateUserPermission(Permissions = "can_edit_permission")]
        public ActionResult PermissionEdit(int ID)
        {
            try
            {
                if (ID > 0)
                {
                    var permission = _permissionQueryBLL.GetPermission(ID);

                    return View(permission);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            ModelState.AddModelError("", "Select a permission to Edit");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[ValidateUserPermission(Permissions = "can_edit_permission")]
        public ActionResult PermissionEdit(Permission permission)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _permissionCMDBLL.UpdatePermission(permission);
                    TempData["Message"] = "Success";
                    return View();
                }
                else
                {
                    return View(permission);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            ModelState.AddModelError("", "Error updating permission");

            return View();
        }

        //[ValidateUserPermission(Permissions = "can_edit_rolepermission")]
        public ActionResult EditRolePermission(string roleID)
        {
            try
            {
                var rolePermissions = _permissionQueryBLL.GetRolePermissions(roleID);

                ViewData["rolePermissions"] = rolePermissions;
                return View("AssignRolePermissions", rolePermissions);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            ModelState.AddModelError("", "Unable to get role permission");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[ValidateUserPermission(Permissions = "can_edit_rolepermission")]
        public ActionResult EditRolePermission(List<PermissionViewModel> permissions, string roleID)
        {
            try
            {
                if (!string.IsNullOrEmpty(roleID))
                {
                    _permissionCMDBLL.EditRolePermission(permissions, roleID);

                    TempData["Message"] = "Success";

                    return RedirectToAction("EditRolePermission", new { roleID = roleID });
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
                throw;
            }


            return View();
        }
    }
}