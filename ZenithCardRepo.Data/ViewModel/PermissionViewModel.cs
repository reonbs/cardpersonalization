using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZenithCardRepo.Data.ViewModel
{
    public class PermissionViewModel
    {
        public int Permissionid { get; set; }
        public string PermissionName { get; set; }
        public Nullable<bool> hasPersission { get; set; }
        public int RolePermissionid { get; set; }
    }
}
