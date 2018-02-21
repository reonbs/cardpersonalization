using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenithCardPerso.Repository.Query;
using ZenithCardRepo.Data.Models;

namespace ZenithCardRepo.Services.BLL.Query
{
    public class ImageSettingQueryBLL : IImageSettingQueryBLL
    {
        private IQueryRepository<ImageValidationSetting> _imgValSetQueryRepo;
        public ImageSettingQueryBLL(IQueryRepository<ImageValidationSetting> imgValSetQueryRepo)
        {
            _imgValSetQueryRepo = imgValSetQueryRepo;
        }
        public ImageValidationSetting GetImageSetting()
        {
            return _imgValSetQueryRepo.GetAll().FirstOrDefault();
        }

        public List<ImageValidationSetting> GetImageSettings()
        {
            return _imgValSetQueryRepo.GetAll().ToList();
        }
    }
}
