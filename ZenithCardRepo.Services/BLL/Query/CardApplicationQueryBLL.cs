using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenithCardPerso.Repository.Query;
using ZenithCardRepo.Data.DTOs;
using ZenithCardRepo.Data.Models;
using ZenithCardRepo.Data.ViewModel;

namespace ZenithCardRepo.Services.BLL.Query
{
    public class CardApplicationQueryBLL : ICardApplicationQueryBLL
    {
        private IQueryRepository<CardApplication> _cardAppRepo;
        public CardApplicationQueryBLL(IQueryRepository<CardApplication> cardAppRepo)
        {
            _cardAppRepo = cardAppRepo;
        }

        public IEnumerable<CardApplicationsDTO> CardApplicationSearch(CardAppViewModel cardAppVM)
        {
            var cardApps = _cardAppRepo.GetAll().Where(x => x.IsDeleted != true);
            int resultFound = 0;
            if (!string.IsNullOrEmpty(cardAppVM.FromDate))
            {
                cardApps = cardApps.Where(x => x.DateCreated >= Convert.ToDateTime(cardAppVM.FromDate));
                resultFound = cardApps.Count();
            }
            else if (!string.IsNullOrEmpty(cardAppVM?.ToDate))
            {
                cardApps = cardApps.Where(x => x.DateCreated <= Convert.ToDateTime(cardAppVM.ToDate));
                resultFound = cardApps.Count();
            }
            else if (!string.IsNullOrEmpty(cardAppVM.Processed))
            {
                var isProcessed = Convert.ToBoolean(cardAppVM.Processed);
                cardApps = cardApps.Where(x => x.isProcessed == isProcessed || x.isProcessed == null);
                resultFound = cardApps.Count();
            }
            else if (!string.IsNullOrEmpty(cardAppVM.BatchNo))
            {
                cardApps = cardApps.Where(x => x.ProcessedBatchNo == cardAppVM.BatchNo);
                resultFound = cardApps.Count();
            }
            else if (!string.IsNullOrEmpty(cardAppVM.Department))
            {
                cardApps = cardApps.Where(x => x.InstitutionID == Convert.ToInt32(cardAppVM.Institution) && x.Department == cardAppVM.Department);
                resultFound = cardApps.Count();
            }
            else if (!string.IsNullOrEmpty(cardAppVM.Institution))
            {
                cardApps = cardApps.Where(x => x.InstitutionID == Convert.ToInt32(cardAppVM.Institution));
                resultFound = cardApps.Count();
            }

            if (resultFound > 0)
            {
                return cardApps.Select(CardApplicationsDTO.GetCompleteDTOFromModel);
            }
            else
            {
                return new List<CardApplicationsDTO> { };
            }

        }

        public IEnumerable<CardApplication> CardApplicationToExport()
        {
            return _cardAppRepo.GetAll().Where(x => x.isProcessed != true && x.IsDeleted != true);
        }

        public bool CheckProcessedStatus(List<CardApplicationsDTO> cardAppDTOs)
        {
            return cardAppDTOs.Any(x => x.IsProcessed == true);
        }

        public IEnumerable<CardApplicationsDTO> GetCardApplications()
        {
            return _cardAppRepo.GetAll().Where(x => x.isProcessed != true && x.IsDeleted != true).Select(CardApplicationsDTO.GetDTOFromModel);
        }

        public IEnumerable<CardApplicationsDTO> MyCardApplications(string loggedOnUser)
        {
            return _cardAppRepo.GetBy(x => x.CreatedBy.ToLower() == loggedOnUser.ToLower() && x.IsDeleted != true && x.isProcessed == false).Select(CardApplicationsDTO.GetDTOFromModel).ToList();
        }

        public CardApplicationsDTO GetCardApplication(int ID)
        {
            return _cardAppRepo.GetBy(x => x.ID == ID).Select(CardApplicationsDTO.GetCompleteDTOFromModel).FirstOrDefault();
        }
    }
}
