using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenithCardRepo.Data.Models;
using System.Globalization;
using System.IO;

namespace ZenithCardRepo.Data.DTOs
{
    public class CardApplicationsDTO : BaseEntity
    {
        public int ID { get; set; }
        [Required, Display(Name = "First Name"), MaxLength(20)]
        public string FirstName { get; set; }
        [Required, Display(Name = "Middle Name"), MaxLength(20)]
        public string MiddleName { get; set; }
        [Required, Display(Name = "Last Name"), MaxLength(20)]
        public string LastName { get; set; }
        [Required, MaxLength(1)]
        public string Sex { get; set; }
        [Required, Display(Name = "Marital Status"), MaxLength(1)]
        public string MaritalStatus { get; set; }
        [Required, Display(Name = "Office Phone No"), MaxLength(15)]
        public string OfficePhoneNo { get; set; }
        [Required, Display(Name = "GSM No"), MaxLength(15)]
        public string GSMNo { get; set; }
        [Display(Name = "Email Address"), MaxLength(50)]
        public string EmailAddress { get; set; }
        [Required, Display(Name = "Office Address 1"), DataType(DataType.MultilineText), MaxLength(50)]
        public string OfficeAddress1 { get; set; }
        [Required, Display(Name = "Office Address 2"), DataType(DataType.MultilineText), MaxLength(50)]
        public string OfficeAddress2 { get; set; }
        [Required, MaxLength(5)]
        public string City { get; set; }
        [Required, MaxLength(3)]
        public string State { get; set; }
        [Required, Display(Name = "Requesting Branch Code")]
        public string RequestingBranchCode { get; set; }
        [Display(Name = "Main Account No"), MaxLength(10)]
        public string MainAccountNo { get; set; }
        [Display(Name = "Other Account No"), MaxLength(10)]
        public string OtherAccountNo { get; set; }
        [Required, Display(Name = "Name on Card"), MaxLength(23)]
        public string NameonCard { get; set; }
        [Required, Display(Name = "ID Card Type"), MaxLength(2)]
        public string IDCardType { get; set; }
        [Required, Display(Name = "ID No"), MaxLength(20)]
        public string IDNo { get; set; }
        [Required, Display(Name = "ID Issue Date")]
        public string IDIssueDate { get; set; }
        [Required, Display(Name = "ID Expiry Date")]
        public string IDExpiryDate { get; set; }
        [Required, Display(Name = "Socio Prof Code"), MaxLength(3)]
        public string SocioProfCode { get; set; }
        [Required, Display(Name = "Product")]
        public string ProductCode { get; set; }
        [Required, Display(Name = "Date Of Birth")]
        public string DateofBirth { get; set; }
        [Required, Display(Name = "Title"), MaxLength(2)]
        public string TitleCode { get; set; }
        [Required, Display(Name = "Nationality"), MaxLength(3)]
        public string Nationality { get; set; }
        //public string ReferenceNo { get; set; }
        public string ImageLocation { get; set; }
        //public bool? isProcessed { get; set; }
        public string Department { get; set; }
        public string ProcessedBatchNo { get; set; }
        public int InstitutionID { get; set; }
        public Institution Institution { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsProcessed { get; set; }
        public bool IsSelected { get; set; }
        public string FullName { get; set; }

        public static CardApplicationsDTO GetDTOFromModel(CardApplication cardApplication)
        {
            return new CardApplicationsDTO
            {
                ID = cardApplication.ID,
                FullName = cardApplication.FirstName + " " + cardApplication.MiddleName + " " + cardApplication.LastName,
                OfficePhoneNo = cardApplication.OfficePhoneNo,
                Sex = cardApplication.Sex,
                RequestingBranchCode = cardApplication.RequestingBranchCode,
                IsProcessed = (cardApplication.isProcessed == null) ? false : (bool)cardApplication.isProcessed

            };
        }

        public static CardApplicationsDTO GetCompleteDTOFromModel(CardApplication cardApplication)
        {
            var ii = Convert.ToBase64String(File.ReadAllBytes(cardApplication.ImageLocation));

            return new CardApplicationsDTO
            {
                FullName = cardApplication.FirstName + " " + cardApplication.MiddleName + " " + cardApplication.LastName,
                IsProcessed = (cardApplication.isProcessed == null) ? false : (bool)cardApplication.isProcessed,
                City = cardApplication.City,
                DateofBirth = cardApplication.DateofBirth.ToString("dd/MM/yyyy"),
                GSMNo = cardApplication.GSMNo,
                ID = cardApplication.ID,
                IDCardType = cardApplication.IDCardType,
                IDExpiryDate = cardApplication.IDExpiryDate.ToString("dd/MM/yyyy"),
                IDIssueDate = cardApplication.IDIssueDate.ToString("dd/MM/yyyy"),
                IDNo = cardApplication.IDNo,
                Department = cardApplication.Department,
                EmailAddress = cardApplication.EmailAddress,
                FirstName = cardApplication.FirstName,
                ImageLocation = Convert.ToBase64String(File.ReadAllBytes(cardApplication.ImageLocation)),
                InstitutionID = cardApplication.InstitutionID,
                LastName = cardApplication.LastName,
                MainAccountNo = cardApplication.MainAccountNo,
                MaritalStatus = cardApplication.MaritalStatus,
                MiddleName = cardApplication.MiddleName,
                NameonCard = cardApplication.NameonCard,
                Nationality = cardApplication.Nationality,
                OfficeAddress1 = cardApplication.OfficeAddress1,
                OfficeAddress2 = cardApplication.OfficeAddress2,
                OfficePhoneNo = cardApplication.OfficePhoneNo,
                OtherAccountNo = cardApplication.OtherAccountNo,
                ProcessedBatchNo = cardApplication.ProcessedBatchNo,
                ProductCode = cardApplication.ProductCode,
                RequestingBranchCode = cardApplication.RequestingBranchCode,
                Sex = cardApplication.Sex,
                SocioProfCode = cardApplication.SocioProfCode,
                State = cardApplication.State,
                TitleCode = cardApplication.TitleCode,
                CreatedBy = cardApplication.CreatedBy,
                DateCreated = cardApplication.DateCreated,
                DateModified = cardApplication.DateModified,
                ModifiedBy = cardApplication.ModifiedBy

            };
        }

        public static CardApplication GetModelFromDTO(CardApplicationsDTO cardApplicationDTO)
        {
            return new CardApplication
            {
                
                City = cardApplicationDTO.City,
                DateofBirth = DateTime.ParseExact(cardApplicationDTO.DateofBirth, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                GSMNo = cardApplicationDTO.GSMNo,
                ID = cardApplicationDTO.ID,
                IDCardType = cardApplicationDTO.IDCardType,
                IDExpiryDate = DateTime.ParseExact(cardApplicationDTO.IDExpiryDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                IDIssueDate = DateTime.ParseExact(cardApplicationDTO.IDIssueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                IDNo = cardApplicationDTO.IDNo,
                Department = cardApplicationDTO.Department,
                EmailAddress = cardApplicationDTO.EmailAddress,
                FirstName = cardApplicationDTO.FirstName,
                ImageLocation = cardApplicationDTO.ImageLocation,
                InstitutionID = cardApplicationDTO.InstitutionID,
                LastName = cardApplicationDTO.LastName,
                MainAccountNo = cardApplicationDTO.MainAccountNo,
                MaritalStatus = cardApplicationDTO.MaritalStatus,
                MiddleName = cardApplicationDTO.MiddleName,
                NameonCard = cardApplicationDTO.NameonCard,
                Nationality = cardApplicationDTO.Nationality,
                OfficeAddress1 = cardApplicationDTO.OfficeAddress1,
                OfficeAddress2 = cardApplicationDTO.OfficeAddress2,
                OfficePhoneNo = cardApplicationDTO.OfficePhoneNo,
                OtherAccountNo = cardApplicationDTO.OtherAccountNo,
                ProcessedBatchNo = cardApplicationDTO.ProcessedBatchNo,
                ProductCode = cardApplicationDTO.ProductCode,
                RequestingBranchCode = cardApplicationDTO.RequestingBranchCode,
                Sex = cardApplicationDTO.Sex,
                SocioProfCode = cardApplicationDTO.SocioProfCode,
                State = cardApplicationDTO.State,
                TitleCode = cardApplicationDTO.TitleCode,
                CreatedBy = cardApplicationDTO.CreatedBy,
                DateCreated = cardApplicationDTO.DateCreated,
                DateModified = cardApplicationDTO.DateModified,
                ModifiedBy = cardApplicationDTO.ModifiedBy


            };
        }
    }
}
