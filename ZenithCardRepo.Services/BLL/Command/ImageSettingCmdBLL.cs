using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenithCardPerso.Repository.Command;
using ZenithCardRepo.Data.Models;

namespace ZenithCardRepo.Services.BLL.Command
{
    public class ImageSettingCmdBLL : IImageSettingCmdBLL
    {
        private ICommandRepository<ImageValidationSetting> _imgValSetttingRepo;
        public ImageSettingCmdBLL(ICommandRepository<ImageValidationSetting> imgValSetttingRepo)
        {
            _imgValSetttingRepo = imgValSetttingRepo;
        }
        public void AddSetting(ImageValidationSetting imgValSetting)
        {
            _imgValSetttingRepo.Insert(imgValSetting);
            _imgValSetttingRepo.Save();
        }

        public void UpdateSetting(ImageValidationSetting imgValSetting)
        {
            _imgValSetttingRepo.Update(imgValSetting);
            _imgValSetttingRepo.Save();
        }
    }
}
