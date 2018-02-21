using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZenithCardRepo.Data.Models
{
    public class OrganizationProfile
    {
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required,Display(Name ="Organisation Code")]
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
