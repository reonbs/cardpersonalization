using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZenithCardRepo.Data.ViewModel
{
    public class AuditViewModel
    {
        [Display(Name = "Start Date")]
        public string StartDate { get; set; }
        [Display(Name = "End Date")]
        public string EndDate { get; set; }
        public string Username { get; set; }
        public string Institution { get; set; }
    }
}
