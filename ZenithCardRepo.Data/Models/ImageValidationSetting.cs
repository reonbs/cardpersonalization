using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZenithCardRepo.Data.Models
{
    public class ImageValidationSetting
    {
        public int ID { get; set; }
        [Display(Name = "Width")]
        public int Width { get; set; }
        [Display(Name = "Height")]
        public int Height { get; set; }
        [Display(Name = "Degree of Head Tilt")]
        public int HeadTilt { get; set; }
        [Display(Name = "Max Image Size")]
        public int ImageSize { get; set; }
        public string ImageFormat { get; set; }

    }
}
