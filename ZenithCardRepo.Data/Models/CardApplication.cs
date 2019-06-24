using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZenithCardRepo.Data.Models
{
    public class CardApplication : BaseEntity
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
        [Required, Display(Name = "Requesting Banch Code")]
        public string RequestingBranchCode { get; set; }
        [Required,
         Display(Name = "Main Account No"),
         RegularExpression("^[0-9]*$", ErrorMessage = "Account No must be numeric"),
         MaxLength(10, ErrorMessage = "Number must be 10 digits"),
         MinLength(10, ErrorMessage = "Number must be 10 digits")]
        public string MainAccountNo { get; set; }
        [Display(Name = "Other Account No"), MaxLength(10)]
        public string OtherAccountNo { get; set; }
        [Required, Display(Name = "Name on Card"), MaxLength(21)]
        public string NameonCard { get; set; }
        [Required, Display(Name = "ID Card Type"), MaxLength(2)]
        public string IDCardType { get; set; }
        [Required, Display(Name = "ID No"), MaxLength(20)]
        public string IDNo { get; set; }
        [Required, Display(Name = "ID Issue Date")]
        public DateTime IDIssueDate { get; set; }
        [Required, Display(Name = "ID Expiry Date")]
        public DateTime IDExpiryDate { get; set; }
        [Required, Display(Name = "Socio Prof Code"), MaxLength(3)]
        public string SocioProfCode { get; set; }
        [Required, Display(Name = "Product")]
        public string ProductCode { get; set; }
        [Required, Display(Name = "Date Of Birth")]
        public DateTime DateofBirth { get; set; }
        [Required, Display(Name = "Title"), MaxLength(2)]
        public string TitleCode { get; set; }
        [Required, Display(Name = "Nationality"), MaxLength(3)]
        public string Nationality { get; set; }
        //public string ReferenceNo { get; set; }
        public string ImageLocation { get; set; }
        public bool? isProcessed { get; set; }
        public string Department { get; set; }
        public string ProcessedBatchNo { get; set; }
        public int InstitutionID { get; set; }
        public Institution Institution { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsApproved { get; set; }
        public DateTime? LastDownloadDate { get; set; }

    }
}
