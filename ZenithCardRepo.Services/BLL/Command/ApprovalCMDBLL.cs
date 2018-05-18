using Newtonsoft.Json;
using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenithCardPerso.Repository.Command;
using ZenithCardPerso.Repository.Query;

using ZenithCardRepo.Data.DTOs;
using ZenithCardRepo.Data.Models;
using ZenithCardRepo.Services.BLL.Infrastructure;

namespace ZenithCardRepo.Services.BLL.Command
{
    public class ApprovalCMDBLL : IApprovalCMDBLL
    {
        private ICommandRepository<Approval> _approvalCMDRepo;
        private IQueryRepository<Approval> _approvalQueryepo;
        private ICommandRepository<CardApplication> _cardApplicationCMDRepo;
        private IQueryRepository<CardApplication> _cardApplicationQueryRepo;
        public ApprovalCMDBLL(
            ICommandRepository<Approval> approvalCMDRepo,
            IQueryRepository<CardApplication> cardApplicationQueryRepo,
            ICommandRepository<CardApplication> cardApplicationCMDRepo,
            IQueryRepository<Approval> approvalQueryepo
            )
        {
            _approvalCMDRepo = approvalCMDRepo;
            _cardApplicationQueryRepo = cardApplicationQueryRepo;
            _cardApplicationCMDRepo = cardApplicationCMDRepo;
            _approvalQueryepo = approvalQueryepo;
        }
        public void AddApproval(List<CardApplicationsDTO> cardApps, string requestBy, string comment)
        {
            try
            {
                var selectedCardApps = cardApps.Where(x => x.IsSelected == true);
                if (selectedCardApps.Count() > 0)
                {


                    List<int> selectedIDs = new List<int>();

                    foreach (var selectedCardApp in selectedCardApps)
                    {
                        selectedIDs.Add(selectedCardApp.ID);
                    }

                    var cardAppIDs = JsonConvert.SerializeObject(selectedIDs);

                    var approval = new Approval
                    {
                        Approver = requestBy,
                        Comment = comment,
                        CardApplicationIDs = cardAppIDs,
                        DateCreated = DateTime.Now,
                        Rank = 1
                    };

                    _approvalCMDRepo.Insert(approval);
                    _approvalCMDRepo.Save();
                }


            }
            catch (Exception ex)
            {

                throw;
            }

        }



        public void UpdateApproval(int approvalID, string approvalType, List<CardApplicationsDTO> cardApps, bool allApproved)
        {
            var approval = _approvalQueryepo.GetBy(x => x.ID == approvalID).FirstOrDefault();
            

            if (approval != null)
            {
                switch (approvalType)
                {
                    case Utilities.Approve:
                        approval.Rank = (allApproved) ? Utilities.Approve_Rank : 1;
                        break;
                    case Utilities.Decline:
                        approval.Rank = Utilities.Decline_Rank;
                        break;
                    default:
                        break;
                }


                _approvalCMDRepo.Update(approval);
                _approvalCMDRepo.Save();
            }

        }

        
    }
}
