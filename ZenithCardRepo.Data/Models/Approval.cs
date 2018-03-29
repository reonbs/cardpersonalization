using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZenithCardRepo.Data.Models
{
    public class Approval
    {
        public int ID { get; set; }
        public string Approver { get; set; }
        public string Comment { get; set; }
        public string CardApplicationIDs { get; set; }
        public int Rank { get; set; }
        public DateTime DateCreated { get; set; }


    }
}
