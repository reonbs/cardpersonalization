using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenithCardRepo.Data.IdentityModels;

namespace ZenithCardRepo.Services.BLL.Command
{
    public interface IManageUserCMDBLL
    {
        //void CreateUser(ApplicationUser user);
        void UpdateUser(ApplicationUser user,string loggedOnUser);
        void UpdatedDefaultPassword(ApplicationUser user);
    }
}
