using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenithCardPerso.Repository.Command;
using ZenithCardPerso.Repository.Query;
using ZenithCardRepo.Data.IdentityModels;
using ZenithCardRepo.Services.BLL.Query;

namespace ZenithCardRepo.Services.BLL.Command
{
    public class ManageUserCMDBLL : IManageUserCMDBLL
    {
        private ICommandRepository<ApplicationUser> _userCMDRepo;
        private IManageUserQueryBLL _manageUserQueryRepo;
        private IQueryRepository<ApplicationUser> _userQueryRepo;
        public ManageUserCMDBLL(ICommandRepository<ApplicationUser> userCMDRepo, 
            IQueryRepository<ApplicationUser> userQueryRepo,
            IManageUserQueryBLL manageUserQueryRepo)
        {
            _userCMDRepo = userCMDRepo;
            _userQueryRepo = userQueryRepo;
            _manageUserQueryRepo = manageUserQueryRepo;
        }
        public void UpdateUser(ApplicationUser user, string loggedOnUser)
        {
            var applicationUser = _userQueryRepo.GetBy(x => x.Id == user.Id ).FirstOrDefault();
            applicationUser.InstitutionID = user.InstitutionID;
            applicationUser.FirstName = user.FirstName;
            applicationUser.MiddleName = user.MiddleName;
            applicationUser.LastName = user.LastName;
            applicationUser.DateModified = DateTime.Now;
            applicationUser.ModifiedBy = loggedOnUser;
            applicationUser.IsDisabled = user.IsDisabled;

            _userCMDRepo.Update(applicationUser);
            _userCMDRepo.Save();
        }

        public void RemoveUserFromRoles(string[] roles)
        {
            foreach (var role in roles)
            {
                var roleID = _manageUserQueryRepo.GetRoleID(role);
                
            }
            
        }

        public void UpdatedDefaultPassword(ApplicationUser user)
        {
            var applicationUser = _userQueryRepo.GetBy(x => x.UserName == user.UserName).FirstOrDefault();
            applicationUser.IsDefaultPassword = false;

            _userCMDRepo.Update(applicationUser);
            _userCMDRepo.Save();
        }
    }
}
