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
        //private IQueryRepository<ProcessedCard> _processedCardRepo;
        public CardApplicationQueryBLL(IQueryRepository<CardApplication> cardAppRepo//,
                                                                                    //IQueryRepository<ProcessedCard> processedCardRepo
            )
        {
            _cardAppRepo = cardAppRepo;
            //_processedCardRepo = processedCardRepo;
        }

        public async Task<IEnumerable<CardApplicationsDTO>> CardApplicationSearch(CardAppViewModel cardAppVM)
        {
            cardAppVM.FromDate = (!string.IsNullOrEmpty(cardAppVM.FromDate)) ? cardAppVM.FromDate.Replace("s", "/") : "";
            cardAppVM.ToDate = (!string.IsNullOrEmpty(cardAppVM.ToDate)) ? cardAppVM.ToDate.Replace("s", "/") : "";

            var cardApplications = await _cardAppRepo.GetAllAsync();
            var cardApps = cardApplications.Where(x => x.IsDeleted != true);
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
            else if (!string.IsNullOrEmpty(cardAppVM.IsApproved))
            {
                var isApproved = Convert.ToBoolean(cardAppVM.IsApproved);
                cardApps = cardApps.Where(x => x.IsApproved == isApproved);
                resultFound = cardApps.Count();
            }

            if (resultFound > 0)
            {
                return cardApps.Select(CardApplicationsDTO.GetDTOFromModelForSearch);
            }
            else
            {
                return new List<CardApplicationsDTO> { };
            }

        }

        public IEnumerable<CardApplicationsDTO> CardApplicationToExport(List<CardApplicationsDTO> cardApps)
        {
            List<CardApplicationsDTO> cardAppsDTO = new List<CardApplicationsDTO>();
            foreach (var cardApp in cardApps)
            {
                var cardAppls = _cardAppRepo.GetAll().Where(x => x.ID == cardApp.ID).Select(CardApplicationsDTO.GetDTOWithImgLocFromModel).FirstOrDefault();

                cardAppsDTO.Add(cardAppls);
            }

            return cardAppsDTO;
            //return _cardAppRepo.GetAll().Where(x => x.isProcessed != true && x.IsDeleted != true).Select(CardApplicationsDTO.GetDTOWithImgLocFromModel);
        }

        public bool CheckProcessedStatus(List<CardApplicationsDTO> cardAppDTOs)
        {
            return cardAppDTOs.Any(x => x.IsProcessed == true && x.IsApproved != true);
        }

        public IEnumerable<CardApplicationsDTO> GetCardApplications()
        {
            return _cardAppRepo.GetAll().Where(x => x.isProcessed != true && x.IsDeleted != true && x.IsApproved != false).Select(CardApplicationsDTO.GetDTOFromModel);
        }

        public IEnumerable<CardApplicationsDTO> MyCardApplications(string loggedOnUser)
        {
            return _cardAppRepo.GetBy(x => x.CreatedBy.ToLower() == loggedOnUser.ToLower() && x.IsDeleted != true && (x.isProcessed == false || x.isProcessed == null)).Select(CardApplicationsDTO.GetDTOFromModel).ToList();
        }

        public CardApplicationsDTO GetCardApplication(int ID)
        {
            return _cardAppRepo.GetBy(x => x.ID == ID).Select(CardApplicationsDTO.GetCompleteDTOFromModel).FirstOrDefault();
        }

        public List<CardApplicationsDTO> GetCardApplicationsByIDs(List<int> cardAPPIds)
        {
            List<CardApplication> cardApps = new List<CardApplication>();

            foreach (var cardAppID in cardAPPIds)
            {
                var cardApp = _cardAppRepo.GetBy(x => x.ID == cardAppID && x.IsApproved != true && x.IsDeleted == false).FirstOrDefault();
                if (cardApp != null)
                {
                    cardApps.Add(cardApp);
                }
            }

            if (cardApps.Count() > 0)
            {
                return cardApps.Select(CardApplicationsDTO.GetCompleteDTOFromModel).ToList();
            }
            else
            {
                return new List<CardApplicationsDTO> { };
            }

        }

        public bool ValidatedApplication(string IDNo)
        {
           return _cardAppRepo.GetBy(x => x.IDNo.ToUpper() == IDNo.ToUpper()).Any();
        }

        //public List<ProcessedCard> GetProcessedCard()
        //{
        //    return _processedCardRepo.GetAll().ToList();
        //}
    }
}
