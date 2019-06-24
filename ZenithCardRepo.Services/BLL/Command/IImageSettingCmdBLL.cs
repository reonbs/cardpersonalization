using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenithCardRepo.Data.Models;

namespace ZenithCardRepo.Services.BLL.Command
{
    public interface IImageSettingCmdBLL
    {
        void AddSetting(ImageValidationSetting imgValSetting);
        void UpdateSetting(ImageValidationSetting imgValSetting);
    }
}
