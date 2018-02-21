using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZenithCardRepo.Services.BLL.Infrastructure
{
    public interface IImageService
    {
        string ImageURL(string url);
        List<string> ImageBase64String(string base64Str);

        List<string> ValidateImage(string imagePath, string validationMethod, string base64Str, string imgUrl);
    }
}
