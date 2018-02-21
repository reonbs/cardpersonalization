using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication6.IdentityModels;
using ZenithCardRepo.Data.IdentityModels;

namespace ZenithCardRepo.Data.Models
{
    public class ApplicationUserStore:UserStore<ApplicationUser>
    {
        public ApplicationUserStore(ApplicationDbContext context):base(context)
        {

        }
    }
   
}