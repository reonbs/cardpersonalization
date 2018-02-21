using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenithCardRepo.Data.DTOs;
using ZenithCardRepo.Data.Models;

namespace ZenithCardRepo.Services.BLL.Command
{
    public interface ICardApplicationCmdBLL
    {
        void AddCardApplication(CardApplicationsDTO cardApplication, string ImageByte, string saveLocation, string institutionCode);
        string GenerateNO(string keyLength);
        void UpdateStatus(List<CardApplication> cardAPPlicationList);
        void UpdateCardApplication(CardApplicationsDTO cardApplicationDTO, string ImageByte, string saveLocation, string instCode);
        List<string> ImageBase64String(string base64Str);

        Task<string> UpdateBatchNo(List<CardApplication> cardAppsList);

        void CardApplicationEdit(CardApplication cardApplication);

        void DeleteCardApplication(List<CardApplicationsDTO> cardApps);
    }
}
