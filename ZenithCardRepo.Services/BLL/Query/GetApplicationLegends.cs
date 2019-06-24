using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        public async Task<List<City>> CityList(string stateCode)
        {
            return await _cityRepo.GetBy(x => x.StateCode.ToLower() == stateCode.ToLower()).ToListAsync();
        }

        public async Task<IEnumerable<IDCardType>> IDCardTypeList()
        {
            return await _idCardTypeRepo.GetAllAsync();
        }

        public async Task<IEnumerable<MaritalStatus>> MaritalStatusList()
        {
            return await _maritalStatusRepo.GetAllAsync();
        }

        public async Task<IEnumerable<NationalityCode>> NationalityCodeList()
        {
            return await _nationalityCodeRepo.GetAllAsync();
        }

        public async Task<IEnumerable<ProductCode>> ProductCodeList()
        {
            return await _productCodeRepo.GetAllAsync();
        }

        public async Task<IEnumerable<Sex>> SexList()
        {
            return await _sexRepo.GetAllAsync();
        }

        public async Task<IEnumerable<SocioProfCode>> SocioProfCodeList()
        {
            return await _socioProfCodeRepo.GetAllAsync();
        }

        public async Task<IEnumerable<State>> StateList()
        {
            return await _stateRepo.GetAllAsync();
        }

        public async Task<IEnumerable<TitleCode>> TitleCodeList()
        {
            return await _titleRepo.GetAllAsync();
        }
    }
}

