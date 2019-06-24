using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenithCardPerso.Repository.Query;
using ZenithCardRepo.Data.Models;
using ZenithCardRepo.Services.BLL.Infrastructure;

namespace ZenithCardRepo.Services.BLL.Query
{
    public class ApprovalQueryBLL : IApprovalQueryBLL
    {
        private IQueryRepository<Approval> _approvalQueryRepo;
        public ApprovalQueryBLL(IQueryRepository<Approval> approvalQueryRepo)
        {
            _approvalQueryRepo = approvalQueryRepo;
        }

        public List<int> GetCardsToApprove(int approvalID)
        {
            var cardAppIDs =  _approvalQueryRepo.GetBy(x => x.ID == approvalID).FirstOrDefault().CardApplicationIDs;
            var cardAppIDJson =  JsonConvert.DeserializeObject<List<int>>(cardAppIDs);

            return cardAppIDJson;
        }

        public List<Approval> GetApprovals()
        {
            return _approvalQueryRepo.GetBy(x => x.Rank != Utilities.Approve_Rank && x.Rank != Utilities.Decline_Rank).ToList();
        }
    }
}
