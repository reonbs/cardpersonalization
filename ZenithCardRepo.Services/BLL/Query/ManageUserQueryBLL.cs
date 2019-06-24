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
        private IQueryRepository<ApplicationRole> _roleQueryBLL;
        public ManageUserQueryBLL(IQueryRepository<ApplicationUser> userQueryBLL, IQueryRepository<ApplicationRole> roleQueryBLL)
        {
            _userQueryBLL = userQueryBLL;
            _roleQueryBLL = roleQueryBLL;
        }
        public ApplicationUser GetApplicationUser(string ID)
        {
            var user = _userQueryBLL.GetBy(x => x.Id == ID).FirstOrDefault();

            return user;
        }
        public string GetRoleID(string roleName)
        {
            return _roleQueryBLL.GetBy(x => x.Name.ToLower() == roleName.ToLower()).Select(x => x.Id).FirstOrDefault();
        }
    }
}
