using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZenithCardRepo.Data.IdentityModels
{
    public class ApplicationRoleStore : RoleStore<ApplicationRole>
    {
        public ApplicationRoleStore(ApplicationDbContext context) : base(context)
        {

        }
    }
}
