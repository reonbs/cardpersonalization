using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenithCardRepo.Data.IdentityModels;

namespace ZenithCardRepo.Data.Models
{
    public class RolePermission
    {
        public int ID { get; set; }
        public string RoleID { get; set; }
        public int PermissionID { get; set; }
        
    }
}
