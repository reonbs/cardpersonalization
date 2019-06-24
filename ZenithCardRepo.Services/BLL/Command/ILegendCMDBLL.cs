using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenithCardRepo.Data.Models;

namespace ZenithCardRepo.Services.BLL.Command
{
    public interface ILegendCMDBLL
    {
        void ADDTitle(TitleCode title);
        void UpdateTite(TitleCode title);

        void AddSex(Sex sex);
        void UpdateSex(Sex sex);

        void AddMaritasStatus(MaritalStatus maritalStatus);
        void UpdateMaritalStatus(MaritalStatus maritalStatus);

        void AddCity(City city);
        void UpdateCity(City city);

        void AddState(State state);
        void UpdateState(State state);

        void AddIDCardType(IDCardType idCardType);
        void UpdateIDCardType(IDCardType idCardType);

        void AddSocioProfCode(SocioProfCode socioProfCode);
        void UpdateSocioProfCode(SocioProfCode socioProfCode);

        void AddProductCode(ProductCode productCode);
        void UpdateProductCode(ProductCode productCode);

        void AddNationalityCode(NationalityCode nationalityCode);
        void UpdateNationalityCode(NationalityCode nationalityCode);
    }
}
