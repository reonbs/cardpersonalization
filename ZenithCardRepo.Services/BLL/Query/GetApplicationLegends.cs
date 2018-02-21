using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenithCardPerso.Repository.Query;
using ZenithCardRepo.Data.Models;

namespace ZenithCardRepo.Services.BLL.Query
{
    public class GetApplicationLegends : IGetApplicationLegends
    {
        private IQueryRepository<City> _cityRepo;
        private IQueryRepository<IDCardType> _idCardTypeRepo;
        private IQueryRepository<MaritalStatus> _maritalStatusRepo;
        private IQueryRepository<NationalityCode> _nationalityCodeRepo;
        private IQueryRepository<ProductCode> _productCodeRepo;
        private IQueryRepository<Sex> _sexRepo;
        private IQueryRepository<SocioProfCode> _socioProfCodeRepo;
        private IQueryRepository<State> _stateRepo;
        private IQueryRepository<TitleCode> _titleRepo;

        public GetApplicationLegends(
            IQueryRepository<City> cityRepo,
            IQueryRepository<IDCardType> idCardTypeRepo,
            IQueryRepository<MaritalStatus> maritalStatusRepo,
            IQueryRepository<NationalityCode> nationalityCodeRepo,
            IQueryRepository<ProductCode> productCodeRepo,
            IQueryRepository<Sex> sexRepo,
            IQueryRepository<SocioProfCode> socioProfCodeRepo,
            IQueryRepository<State> stateRepo,
            IQueryRepository<TitleCode> titleRepo
            )
        {
            _cityRepo = cityRepo;
            _idCardTypeRepo = idCardTypeRepo;
            _maritalStatusRepo = maritalStatusRepo;
            _nationalityCodeRepo = nationalityCodeRepo;
            _productCodeRepo = productCodeRepo;
            _sexRepo = sexRepo;
            _socioProfCodeRepo = socioProfCodeRepo;
            _stateRepo = stateRepo;
            _titleRepo = titleRepo;




        }
        public List<City> CityList(string stateCode)
        {
            return _cityRepo.GetBy(x => x.StateCode.ToLower() == stateCode.ToLower()).ToList();
        }

        public List<IDCardType> IDCardTypeList()
        {
            return _idCardTypeRepo.GetAll().ToList();
        }

        public List<MaritalStatus> MaritalStatusList()
        {
            return _maritalStatusRepo.GetAll().ToList();
        }

        public List<NationalityCode> NationalityCodeList()
        {
            return _nationalityCodeRepo.GetAll().ToList();
        }

        public List<ProductCode> ProductCodeList()
        {
            return _productCodeRepo.GetAll().ToList();
        }

        public List<Sex> SexList()
        {
            return _sexRepo.GetAll().ToList();
        }

        public List<SocioProfCode> SocioProfCodeList()
        {
            return _socioProfCodeRepo.GetAll().ToList();
        }

        public List<State> StateList()
        {
            return _stateRepo.GetAll().ToList();
        }

        public List<TitleCode> TitleCodeList()
        {
            return _titleRepo.GetAll().ToList();
        }
    }
}

