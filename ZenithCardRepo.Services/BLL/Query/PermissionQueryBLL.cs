using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenithCardPerso.Repository.Query;
using ZenithCardRepo.Data.DTOs;
using ZenithCardRepo.Data.Models;
using ZenithCardRepo.Data.ViewModel;

namespace ZenithCardRepo.Services.BLL.Query
{
    public class PermissionQueryBLL : IPermissionQueryBLL
    {
        private IQueryRepository<Permission> _permissionQueryBLL;
        private IQueryRepository<RolePermission> _rolePermissionQueryBLL;
        private IQueryRepository<UserPermission> _userPermissionQueryBLL;
        public PermissionQueryBLL(
            IQueryRepository<Permission> permissionQueryBLL,
            IQueryRepository<RolePermission> rolePermissionQueryBLL,
            IQueryRepository<UserPermission> userPermissionQueryBLL
            )
        {
            _permissionQueryBLL = permissionQueryBLL;
            _rolePermissionQueryBLL = rolePermissionQueryBLL;
            _userPermissionQueryBLL = userPermissionQueryBLL;
        }

        public Permission GetPermission(int ID)
        {
            return _permissionQueryBLL.GetBy(x => x.ID == ID).FirstOrDefault();
        }

        public List<Permission> GetPermissions()
        {
            return _permissionQueryBLL.GetAll().ToList();
        }

        public List<PermissionViewModel> GetRolePermissions(string roleID)
        {
            var rolePermission = _rolePermissionQueryBLL.GetBy(x => x.RoleID == roleID).Select(RolePermissionDTO.GetDTOFromRolePermission).ToList();
            var permissions = _permissionQueryBLL.GetAll().ToList();
            var assignPermissions = from p in permissions
                                    join r in rolePermission on p.ID equals r.PermissionID into pr
                                    from j in pr.DefaultIfEmpty(new RolePermissionDTO())
                                    select new PermissionViewModel
                                    {
                                        RolePermissionid = j.ID,
                                        Permissionid = p.ID,
                                        PermissionName = p.Name,
                                        hasPersission = (j.PermissionID != 0) ? true : false,
                                    };
            return assignPermissions.ToList();
        }

        public List<UserPermission> FetchUserPermission(string username, string roleName)
        {
            var userPermission = _permissionQueryBLL.StoreprocedureQueryFor<UserPermission>("FLTUserPermissions @username, @roleName", new SqlParameter("username", username), new SqlParameter("roleName", roleName)).ToList();

            return userPermission;
        }

        public List<string> FetchUserRoles(string userID)
        {
            var userRoles = _permissionQueryBLL.StoreprocedureQueryFor<string>("FLTUserPermissionsAndRole @userid", new SqlParameter("userid", userID)).ToList();

            return userRoles;
        }

    }
}
