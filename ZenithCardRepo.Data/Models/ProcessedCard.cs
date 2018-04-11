using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZenithCardRepo.Data.Models
{
    public class ProcessedCard
    {
        public int ID { get; set; }
        public string BatchNo { get; set; }
        public string DownloadLink { get; set; }
    }
}
