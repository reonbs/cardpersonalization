﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ZenithCardRepo.Data.DTOs;

namespace ZenithCardRepo.Data.ViewModel
{
    public class CardAppViewModel
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        [Display(Name ="Is Completed")]
        public string Processed { get; set; }
        public string BatchNo { get; set; }
        public string Institution { get; set; }
        public string Department { get; set; }
        public string IsApproved { get; set; }

        
    }
}