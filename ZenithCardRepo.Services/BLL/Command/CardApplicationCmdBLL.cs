using System;
using System.Collections.Generic;
using System.IO;
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
    public class CardApplicationCmdBLL : ICardApplicationCmdBLL
    {
        private ICommandRepository<CardApplication> _CardAppRepo;
        //private ICommandRepository<ProcessedCard> _processedCardRepo;
        private IQueryRepository<CardApplication> _queryAppRepo;
        private IImageService _imageService;

        public CardApplicationCmdBLL(
            ICommandRepository<CardApplication> CardAppRepo,
            IQueryRepository<CardApplication> queryAppRepo,
            IImageService imageService//,
            //ICommandRepository<ProcessedCard> processedCardRepo
            )
        {
            _CardAppRepo = CardAppRepo;
            _queryAppRepo = queryAppRepo;
            _imageService = imageService;
            //_processedCardRepo = processedCardRepo;
        }
        public void AddCardApplication(CardApplicationsDTO cardApplicationDTO, string ImageByte, string saveLocation, string instCode)
        {
            var referenceNo = instCode + "_" + cardApplicationDTO.Department + "_" + GenerateNO("12");
            var cardApplication = CardApplicationsDTO.GetModelFromDTO(cardApplicationDTO);

            cardApplication.OfficeAddress2 = referenceNo;
            cardApplication.ImageLocation = saveLocation;
            cardApplication.DateCreated = DateTime.Now;
            cardApplication.isProcessed = false;
            cardApplication.NameonCard = cardApplication.NameonCard.ToUpper();
            cardApplication.OfficeAddress1 = cardApplication.OfficeAddress1.ToUpper();
            cardApplication.OfficeAddress2 = cardApplication.OfficeAddress2.ToUpper();

            _CardAppRepo.Insert(cardApplication);
            _CardAppRepo.Save();

            var updateRefNo = referenceNo + "-" + cardApplication.ID;
            cardApplication.OfficeAddress2 = updateRefNo;
            saveLocation = ProcessedImage(cardApplicationDTO, ImageByte, saveLocation, updateRefNo);
            cardApplication.ImageLocation = saveLocation;

            _CardAppRepo.Update(cardApplication);
            _CardAppRepo.Save();
        }

        public string ProcessedImage(CardApplicationsDTO cardApplication, string ImageByte, string saveLocation, string referenceNo)
        {
            string toReplace = $"data:image/jpeg;base64,";
            string img = ImageByte.Replace(toReplace, string.Empty);
            byte[] imageBytes = Convert.FromBase64String(img);


            saveLocation = saveLocation + referenceNo + ".jpeg";
            using (MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                // Convert byte[] to Image

                ms.Write(imageBytes, 0, imageBytes.Length);
                System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
                image.Save(saveLocation, System.Drawing.Imaging.ImageFormat.Png);
            }

            return saveLocation;
        }

        public string GenerateNO(string keyLength)
        {

            Random r = new Random();
            int randNum = r.Next(100000000);
            string sixDigitNumber = randNum.ToString("D8");

            return sixDigitNumber;


        }

        public string GenerateBatchNo(string keyLength)
        {
            string newSerialNumber = "";
            string SerialNumber = Guid.NewGuid().ToString("N").Substring(0, Convert.ToInt32(keyLength)).ToUpper();
            for (int iCount = 0; iCount < Convert.ToInt32(keyLength); iCount += 4)
                newSerialNumber = newSerialNumber + SerialNumber.Substring(iCount, 4) + "-";
            newSerialNumber = newSerialNumber.Substring(0, newSerialNumber.Length - 1);
            return newSerialNumber;
        }

        public List<string> ImageBase64String(string base64Str)
        {

            var res = _imageService.ImageBase64String(base64Str);

            return res;
        }

        public async Task<string> UpdateBatchNo(string downloadLink, List<CardApplicationsDTO> cardAppsList)
        {
            var cardApplication = cardAppsList.Select(CardApplicationsDTO.GetModelFromDTO);
            var batchNumber = GenerateBatchNo("8");

            foreach (var cardApp in cardApplication)
            {
                var application = _queryAppRepo.GetBy(x => x.ID == cardApp.ID).FirstOrDefault();
                application.ProcessedBatchNo = batchNumber;
                application.LastDownloadDate = DateTime.Now;
                //cardApp.ProcessedBatchNo = batchNumber;
                _CardAppRepo.Update(application);
                
            }

            //var processedCard = new ProcessedCard
            //{
            //    BatchNo = batchNumber,
            //    DownloadLink = downloadLink
            //};
            //_processedCardRepo.Insert(processedCard);

            await _CardAppRepo.SaveAync();

            return "";
        }

        public void UpdateStatus(List<CardApplicationsDTO> cardAPPlicationList)
        {
            foreach (var cardAPPlication in cardAPPlicationList)
            {
                var cardAPPs = _queryAppRepo.GetBy(x => x.ID == cardAPPlication.ID && x.IsDeleted != true).FirstOrDefault();
                cardAPPs.isProcessed = true;
                cardAPPs.IsApproved = false;
                _CardAppRepo.Save();
            }

        }

        public void CardApplicationEdit(CardApplication cardApplication)
        {
            _CardAppRepo.Update(cardApplication);
            _CardAppRepo.Save();
        }


        public void DeleteCardApplication(List<CardApplicationsDTO> cardApps)
        {
            var cardAppsToDelete = cardApps.Where(x => x.IsSelected == true);

            foreach (var item in cardAppsToDelete)
            {
                var cardApp = _queryAppRepo.GetBy(x => x.ID == item.ID).FirstOrDefault();
                cardApp.IsDeleted = true;
                _CardAppRepo.Update(cardApp);
                _CardAppRepo.Save();
            }
        }

        public void UpdateCardApplication(CardApplicationsDTO cardApplicationDTO, string ImageByte, string saveLocation, string instCode)
        {
            var referenceNo = instCode + "_" + cardApplicationDTO.Department + "_" + GenerateNO("8") + "-" + cardApplicationDTO.ID;

            var cardApplication = CardApplicationsDTO.GetModelFromDTO(cardApplicationDTO);

            saveLocation = ProcessedImage(cardApplicationDTO, ImageByte, saveLocation, referenceNo);

            //cardApplication.IDNo = referenceNo;
            cardApplication.OfficeAddress2 = referenceNo;
            cardApplication.ImageLocation = saveLocation;

            _CardAppRepo.Update(cardApplication);
            _CardAppRepo.Save();

        }

        public void CardApplicationApprovalUpdate(List<CardApplicationsDTO> cardApps, string Comment)
        {
            cardApps = cardApps.Where(x => x.IsSelected).ToList();

            if (cardApps.Any())
            {
                foreach (var cardApp in cardApps)
                {
                    var cardApplication = _queryAppRepo.GetBy(x => x.ID == cardApp.ID).FirstOrDefault();
                    cardApplication.IsApproved = true;

                    _CardAppRepo.Update(cardApplication);
                }

                _CardAppRepo.Save();
            }
        }
    }
}
