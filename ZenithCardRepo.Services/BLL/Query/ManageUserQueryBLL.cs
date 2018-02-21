using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenithCardPerso.Repository.Query;
using ZenithCardRepo.Data.IdentityModels;

namespace ZenithCardRepo.Services.BLL.Query
{
    public class ManageUserQueryBLL : IManageUserQueryBLL
    {
        private IQueryRepository<ApplicationUser> _userQueryBLL;
        public ManageUserQueryBLL(IQueryRepository<ApplicationUser> userQueryBLL)
        {
            _userQueryBLL = userQueryBLL;
        }
        public ApplicationUser GetApplicationUser(string ID)
        {
            var user = _userQueryBLL.GetBy(x => x.Id == ID).FirstOrDefault();

            return user;
        }
    }
}
