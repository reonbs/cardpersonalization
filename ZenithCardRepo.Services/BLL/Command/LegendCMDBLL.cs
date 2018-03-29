using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenithCardPerso.Repository.Command;
using ZenithCardRepo.Data.Models;

namespace ZenithCardRepo.Services.BLL.Command
{
    public class LegendCMDBLL : ILegendCMDBLL
    {
        private ICommandRepository<TitleCode> _titleCMDBLL;
        private ICommandRepository<Sex> _sexCMDBLL;
        private ICommandRepository<MaritalStatus> _maritalSatusCMDBLL;
        private ICommandRepository<City> _cityCMDBLL;
        private ICommandRepository<State> _stateCMDBLL;
        private ICommandRepository<IDCardType> _idCardTypeCMDBLL;
        private ICommandRepository<SocioProfCode> _socioProfCMDBLL;
        private ICommandRepository<ProductCode> _productCodeCMDBLL;
        private ICommandRepository<NationalityCode> _nationalityCodeCMDBLL;

        public LegendCMDBLL(ICommandRepository<TitleCode> titleCMDBLL, 
            ICommandRepository<Sex> sexCMDBLL,
            ICommandRepository<MaritalStatus> maritalSatusCMDBLL,
            ICommandRepository<City> cityCMDBLL,
            ICommandRepository<State> stateCMDBLL,
            ICommandRepository<IDCardType> idCardTypeCMDBLL,
            ICommandRepository<SocioProfCode> socioProfCMDBLL,
            ICommandRepository<ProductCode> productCodeCMDBLL,
            ICommandRepository<NationalityCode> nationalityCodeCMDBLL
            )
        {
            _titleCMDBLL = titleCMDBLL;
            _sexCMDBLL = sexCMDBLL;
            _maritalSatusCMDBLL = maritalSatusCMDBLL;
            _cityCMDBLL = cityCMDBLL;
            _stateCMDBLL = stateCMDBLL;
            _idCardTypeCMDBLL = idCardTypeCMDBLL;
            _socioProfCMDBLL = socioProfCMDBLL;
            _productCodeCMDBLL = productCodeCMDBLL;
            _nationalityCodeCMDBLL = nationalityCodeCMDBLL;
        }

        public void AddSex(Sex sex)
        {
            _sexCMDBLL.Insert(sex);
        }
        public void UpdateSex(Sex sex)
        {
            _sexCMDBLL.Update(sex);
            _sexCMDBLL.Save();
        }
        
        public void ADDTitle(TitleCode title)
        {
            _titleCMDBLL.Insert(title);
            _titleCMDBLL.Save();
        }

        public void UpdateTite(TitleCode title)
        {
            _titleCMDBLL.Update(title);
            _titleCMDBLL.Save();
        }

        public void AddMaritasStatus(MaritalStatus maritalStatus)
        {
            _maritalSatusCMDBLL.Insert(maritalStatus);
            _maritalSatusCMDBLL.Save();
        }

        public void UpdateMaritalStatus(MaritalStatus maritalStatus)
        {
            _maritalSatusCMDBLL.Update(maritalStatus);
            _maritalSatusCMDBLL.Save();
        }

        public void AddCity(City city)
        {
            _cityCMDBLL.Insert(city);
            _cityCMDBLL.Save();
        }

        public void UpdateCity(City city)
        {
            _cityCMDBLL.Update(city);
            _cityCMDBLL.Save();
        }

        public void AddState(State state)
        {
            _stateCMDBLL.Insert(state);
            _stateCMDBLL.Save();
        }

        public void UpdateState(State state)
        {
            _stateCMDBLL.Update(state);
            _stateCMDBLL.Save();
        }

        public void AddIDCardType(IDCardType idCardType)
        {
            _idCardTypeCMDBLL.Insert(idCardType);
            _idCardTypeCMDBLL.Save();
        }

        public void UpdateIDCardType(IDCardType idCardType)
        {
            _idCardTypeCMDBLL.Update(idCardType);
            _idCardTypeCMDBLL.Save();
        }

        public void AddSocioProfCode(SocioProfCode socioProfCode)
        {
            _socioProfCMDBLL.Insert(socioProfCode);
            _socioProfCMDBLL.Save();
        }

        public void UpdateSocioProfCode(SocioProfCode socioProfCode)
        {
            _socioProfCMDBLL.Update(socioProfCode);
            _socioProfCMDBLL.Save();
        }

        public void AddProductCode(ProductCode productCode)
        {
            _productCodeCMDBLL.Insert(productCode);
            _productCodeCMDBLL.Save();
        }

        public void UpdateProductCode(ProductCode productCode)
        {
            _productCodeCMDBLL.Update(productCode);
            _productCodeCMDBLL.Save();
        }

        public void AddNationalityCode(NationalityCode nationalityCode)
        {
            _nationalityCodeCMDBLL.Insert(nationalityCode);
            _nationalityCodeCMDBLL.Save();
        }

        public void UpdateNationalityCode(NationalityCode nationalityCode)
        {
            _nationalityCodeCMDBLL.Update(nationalityCode);
            _nationalityCodeCMDBLL.Save();
        }
    }
}
