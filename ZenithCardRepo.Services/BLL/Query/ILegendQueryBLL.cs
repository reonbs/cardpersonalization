using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenithCardRepo.Data.Models;

namespace ZenithCardRepo.Services.BLL.Query
{
    public interface ILegendQueryBLL
    {
        bool VerifyTitleCode(string titleCode);

        List<TitleCode> Titles();
        TitleCode GetTitleByID(int ID);

        Sex GetSex(int ID);

        List<Sex> Sexes();

        List<MaritalStatus> MaritalStatusList();
        MaritalStatus MaritalStatus(int ID);

        List<City> CityList();
        City City(int ID);

        List<State> StateList();
        State State(int ID);

        List<IDCardType> IDCardTypes();
        IDCardType IDCardType(int ID);

        List<SocioProfCode> SocioProfCodes();
        SocioProfCode SocioProfCode(int ID);

        List<ProductCode> ProductCodes();
        ProductCode ProductCode(int ID);

        List<NationalityCode> NationalityCodes();
        NationalityCode NationalityCode(int ID);

    }
}
