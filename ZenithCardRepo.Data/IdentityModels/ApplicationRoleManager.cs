using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZenithCardRepo.Data.IdentityModels
{
    public class ApplicationRoleManager:RoleManager<ApplicationRole>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole, string> store)
       : base(store)
        {
            
        }
    }
}