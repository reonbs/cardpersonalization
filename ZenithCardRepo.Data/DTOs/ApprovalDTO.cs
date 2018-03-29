using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenithCardRepo.Data.Models;

namespace ZenithCardRepo.Data.DTOs
{
    public class ApprovalDTO
    {
        public int ID { get; set; }
        public string Approver { get; set; }
        [DataType(DataType.MultilineText)]
        public string Comment { get; set; }
        public string CardApplicationIDs { get; set; }
        public int Rank { get; set; }
        public DateTime DateCreated { get; set; }

        public static Approval GetModelFromDTO(ApprovalDTO approvalDTO)
        {
            return new Approval
            {
                ID = approvalDTO.ID,
                Approver = approvalDTO.Approver,
                Comment = approvalDTO.Comment,
                CardApplicationIDs = approvalDTO.CardApplicationIDs,
                DateCreated = approvalDTO.DateCreated,
                Rank = approvalDTO.Rank
            };
        }
    }
}
