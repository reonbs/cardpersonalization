using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenithCardRepo.Data.IdentityModels;

namespace ZenithCardRepo.Services.BLL.Query
{
    public interface IManageUserQueryBLL
    {
        ApplicationUser GetApplicationUser(string ID);
        //ApplicationRole GetApplicationRole();
    }
}
