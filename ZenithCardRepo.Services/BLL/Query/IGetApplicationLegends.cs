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
        Task<IEnumerable<MaritalStatus>> MaritalStatusList();
        Task<List<City>> CityList(string stateCode);
        Task<IEnumerable<IDCardType>> IDCardTypeList();
        Task<IEnumerable<NationalityCode>> NationalityCodeList();
        Task<IEnumerable<ProductCode>> ProductCodeList();
        Task<IEnumerable<Sex>> SexList();
        Task<IEnumerable<SocioProfCode>> SocioProfCodeList();
        Task<IEnumerable<State>> StateList();
        Task<IEnumerable<TitleCode>> TitleCodeList();

    }
}
