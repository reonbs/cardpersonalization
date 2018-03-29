using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenithCardRepo.Data.DTOs;
using ZenithCardRepo.Data.Models;
using ZenithCardRepo.Data.ViewModel;

namespace ZenithCardRepo.Services.BLL.Command
{
    public interface IPermissionCMDBLL
    {
        void CreatePermission(Permission permission);
        void UpdatePermission(Permission permission);
        void EditRolePermission(List<PermissionViewModel> rolePermission, string roleID);
        void BulkInsertRolePermission(List<RolePermissionDTO> rolePermissionDTO);
        void DeleteRolePermission(List<RolePermissionDTO> rolePermissionDTOs);
    }
}
