using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenithCardPerso.Repository.Command;
using ZenithCardPerso.Repository.Query;
using ZenithCardRepo.Data.DTOs;
using ZenithCardRepo.Data.Models;
using ZenithCardRepo.Data.ViewModel;

namespace ZenithCardRepo.Services.BLL.Command
{
    public class PermissionCMDBLL : IPermissionCMDBLL
    {
        private ICommandRepository<Permission> _permissionCMDRepo;
        private ICommandRepository<RolePermission> _rolePermissionCMDRepo;
        private IQueryRepository<RolePermission> _rolePermissionQueryRepo;
        public PermissionCMDBLL(
            ICommandRepository<Permission> permissionCMDRepo,
            IQueryRepository<RolePermission> rolePermissionQueryRepo,
            ICommandRepository<RolePermission> rolePermissionCMDRepo
            )
        {
            _permissionCMDRepo = permissionCMDRepo;
            _rolePermissionQueryRepo = rolePermissionQueryRepo;
            _rolePermissionCMDRepo = rolePermissionCMDRepo;
        }
        public void CreatePermission(Permission permission)
        {
            _permissionCMDRepo.Insert(permission);
            _permissionCMDRepo.Save();
        }

        public void UpdatePermission(Permission permission)
        {
            _permissionCMDRepo.Update(permission);
            _permissionCMDRepo.Save();
        }

        public void EditRolePermission(List<PermissionViewModel> rolePermissions, string roleID)
        {
            List<RolePermissionDTO> rolePermissionDTOTrue = new List<RolePermissionDTO>();

            foreach (var rolePermission in rolePermissions.Where(x => x.hasPersission == true))// && x.RolePermissionid != 0))
            {
                var checkIFExist = _rolePermissionQueryRepo.FindByKey(rolePermission.RolePermissionid);

                if (checkIFExist == null)
                {
                    rolePermissionDTOTrue.Add(new RolePermissionDTO
                    {
                        PermissionID = rolePermission.Permissionid,
                        RoleID = roleID
                    });
                }
            }

            BulkInsertRolePermission(rolePermissionDTOTrue);

            List<RolePermissionDTO> rolePermissionDTOFalse = new List<RolePermissionDTO>();

            foreach (var rolePermission in rolePermissions.Where(x => x.hasPersission == false && x.RolePermissionid != 0))
            {
                rolePermissionDTOFalse.Add(new RolePermissionDTO
                {
                    ID = rolePermission.RolePermissionid,
                    PermissionID = rolePermission.Permissionid,
                    RoleID = roleID
                });
            }

            DeleteRolePermission(rolePermissionDTOFalse);
        }

        public void BulkInsertRolePermission(List<RolePermissionDTO> rolePermissionDTO)
        {
            if (rolePermissionDTO.Count > 0)
            {
                var rolePermissions = RolePermissionDTO.GetRolePermissionsFromDTO(rolePermissionDTO);
                _rolePermissionCMDRepo.InsertRange(rolePermissions);
                _rolePermissionCMDRepo.Save();
            }
        }

        public void DeleteRolePermission(List<RolePermissionDTO> rolePermissionDTOs)
        {
            if (rolePermissionDTOs.Count > 0)
            {
                var rolePermissions = RolePermissionDTO.GetRolePermissionsFromDTO(rolePermissionDTOs);
                foreach (var rolePermission in rolePermissions)
                {
                    _rolePermissionCMDRepo.AttachEntity(rolePermission);
                }

                _rolePermissionCMDRepo.DeleteRange(rolePermissions);
                _rolePermissionCMDRepo.Save();
            }
        }
    }
}
