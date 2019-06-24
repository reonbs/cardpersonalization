using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZenithCardRepo.Data.Models
{
    public class AuditRecord
    {

        // public string SessionID { get; set; }
        public Guid AuditRecordID { get; set; }
        public string UserName { get; set; }
        public string IPAddress { get; set; }
        public string AreaAccessed { get; set; }
        public DateTime TimeAccessed { get; set; }
        public string Data { get; set; }
        public string Module { get; set; }
        public string Action { get; set; }

        public string Description { get; set; }
        public int InstitutionID { get; set; }
        public Institution Institution { get; set; }

    }
}
