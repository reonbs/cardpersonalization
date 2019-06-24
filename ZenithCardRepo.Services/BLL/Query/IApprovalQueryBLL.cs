using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenithCardRepo.Data.Models;

namespace ZenithCardRepo.Services.BLL.Query
{
    public interface IApprovalQueryBLL
    {
        List<Approval> GetApprovals();

        List<int> GetCardsToApprove(int approvalID);
    }
}
