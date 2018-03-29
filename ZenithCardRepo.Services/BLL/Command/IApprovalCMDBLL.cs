using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenithCardRepo.Data.DTOs;


namespace ZenithCardRepo.Services.BLL.Command
{
    public interface IApprovalCMDBLL
    {
        void AddApproval(List<CardApplicationsDTO> cardApps, string requestBy, string comment);

        void UpdateApproval(int approvalID, string approvalType);
    }
}
