using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenithCardRepo.Data.DTOs;
using ZenithCardRepo.Data.Models;
using ZenithCardRepo.Data.ViewModel;
using ZenithCardRepo.Services.BLL.Infrastructure;

namespace ZenithCardRepo.Services.BLL.Query
{
    public interface IPermissionQueryBLL
    {
        List<Permission> GetPermissions();
        Permission GetPermission(int ID);
        List<PermissionViewModel> GetRolePermissions(string roleID);
        List<UserPermission> FetchUserPermission(string username, string roleName);
        List<string> FetchUserRoles(string userID);
        string FetchUserPermission(string userName, string[] roles);
        List<ApproversDTO> GetApprovers(string permissionName);
    }
}
