using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZenithCardRepo.Data.Models
{
    public abstract class BaseEntity
    {
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }

    }
}
