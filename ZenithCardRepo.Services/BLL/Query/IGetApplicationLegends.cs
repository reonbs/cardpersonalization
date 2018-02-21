using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenithCardRepo.Data.Models;

namespace ZenithCardRepo.Services.BLL.Query
{
    public interface IGetApplicationLegends
    {
        List<MaritalStatus> MaritalStatusList();
        List<City> CityList(string stateCode);
        List<IDCardType> IDCardTypeList();
        List<NationalityCode> NationalityCodeList();
        List<ProductCode> ProductCodeList();
        List<Sex> SexList();
        List<SocioProfCode> SocioProfCodeList();
        List<State> StateList();
        List<TitleCode> TitleCodeList();

    }
}
