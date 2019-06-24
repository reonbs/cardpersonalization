using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenithCardRepo.Data.Models;

namespace ZenithCardRepo.Data.DTOs
{
    public class RolePermissionDTO
    {
        public int ID { get; set; }
        public string RoleID { get; set; }
        public int PermissionID { get; set; }

        public static RolePermission GetRolePermissionFromDTO(RolePermissionDTO model)
        {
            return new RolePermission
            {
                ID = model.ID,
                RoleID = model.RoleID,
                PermissionID = model.PermissionID
            };
        }

        public static List<RolePermission> GetRolePermissionsFromDTO(List<RolePermissionDTO> models)
        {
            List<RolePermission> rolePermissions = new List<RolePermission>();
            foreach (var model in models)
            {
                rolePermissions.Add(new RolePermission
                {
                    ID = model.ID,
                    RoleID = model.RoleID,
                    PermissionID = model.PermissionID
                });
            }

            return rolePermissions;
        }

        public static RolePermissionDTO GetDTOFromRolePermission(RolePermission model)
        {
            return new RolePermissionDTO
            {
                ID = model.ID,
                RoleID = model.RoleID,
                PermissionID = Convert.ToInt32(model.PermissionID)
            };
        }
    }
}
