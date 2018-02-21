using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZenithCardRepo.Data.Models
{
    public class Department
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int InstitutionID { get; set; }
        public Institution Institution { get; set; }
        public string Code { get; set; }
    }
}
