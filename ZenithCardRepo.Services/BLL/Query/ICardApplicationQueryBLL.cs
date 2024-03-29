﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenithCardRepo.Data.DTOs;
using ZenithCardRepo.Data.Models;
using ZenithCardRepo.Data.ViewModel;

namespace ZenithCardRepo.Services.BLL.Query
{
    public interface ICardApplicationQueryBLL
    {
        IEnumerable<CardApplicationsDTO> GetCardApplications();
        IEnumerable<CardApplicationsDTO> GetCardApplications(int instID);
        IEnumerable<CardApplicationsDTO> CardApplicationToExport(List<CardApplicationsDTO> cardApps);

        Task<IEnumerable<CardApplicationsDTO>> CardApplicationSearch(CardAppViewModel cardAppVM);
        Task<IEnumerable<CardApplicationsDTO>> CardApplicationSearchByInst(CardAppViewModel cardAppVM, int instID);

        IEnumerable<CardApplicationsDTO> MyCardApplications(string loggedOnUser);

        bool CheckProcessedStatus(List<CardApplicationsDTO> cardAppsDTO);

        CardApplicationsDTO GetCardApplication(int ID);

        List<CardApplicationsDTO> GetCardApplicationsByIDs(List<int> cardAPPIds);

        //List<ProcessedCard> GetProcessedCard();
        bool ValidatedApplication(string IDNo);


    }
}
