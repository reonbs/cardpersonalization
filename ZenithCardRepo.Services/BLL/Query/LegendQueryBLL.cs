using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenithCardPerso.Repository.Query;
using ZenithCardRepo.Data.Models;

namespace ZenithCardRepo.Services.BLL.Query
{
    public class LegendQueryBLL : ILegendQueryBLL
    {
        private IQueryRepository<TitleCode> _titleQueryRepo;
        private IQueryRepository<Sex> _sexQueryRepo;
        private IQueryRepository<MaritalStatus> _maritalStatusQueryRepo;
        private IQueryRepository<City> _cityQueryRepo;
        private IQueryRepository<State> _stateQueryRepo;
        private IQueryRepository<IDCardType> _idCardTypeQueryRepo;
        private IQueryRepository<SocioProfCode> _socioProfCodeQueryRepo;
        private IQueryRepository<ProductCode> _productCodeQueryRepo;
        private IQueryRepository<NationalityCode> _nationalityCodeQueryRepo;

        public LegendQueryBLL(IQueryRepository<TitleCode> titleQueryRepo,
            IQueryRepository<Sex> sexQueryRepo,
            IQueryRepository<MaritalStatus> maritalStatusQueryRepo,
            IQueryRepository<City> cityQueryRepo,
            IQueryRepository<State> stateQueryRepo,
            IQueryRepository<IDCardType> idCardTypeQueryRepo,
            IQueryRepository<SocioProfCode> socioProfCodeQueryRepo,
            IQueryRepository<ProductCode> productCodeQueryRepo,
            IQueryRepository<NationalityCode> nationalityCodeQueryRepo
            )
        {
            _titleQueryRepo = titleQueryRepo;
            _sexQueryRepo = sexQueryRepo;
            _maritalStatusQueryRepo = maritalStatusQueryRepo;
            _cityQueryRepo = cityQueryRepo;
            _stateQueryRepo = stateQueryRepo;
            _idCardTypeQueryRepo = idCardTypeQueryRepo;
            _socioProfCodeQueryRepo = socioProfCodeQueryRepo;
            _productCodeQueryRepo = productCodeQueryRepo;
            _nationalityCodeQueryRepo = nationalityCodeQueryRepo;
        }

        public City City(int ID)
        {
            return _cityQueryRepo.GetBy(x => x.ID == ID).FirstOrDefault();
        }

        public List<City> CityList()
        {
            return _cityQueryRepo.GetAll().ToList();
        }

        public Sex GetSex(int ID)
        {
            return _sexQueryRepo.GetBy(x => x.ID == ID).FirstOrDefault();
        }

        public TitleCode GetTitleByID(int ID)
        {
            return _titleQueryRepo.GetBy(x => x.ID == ID).FirstOrDefault();
        }

        public IDCardType IDCardType(int ID)
        {
            return _idCardTypeQueryRepo.GetBy(x => x.ID == ID).FirstOrDefault();
        }

        public List<IDCardType> IDCardTypes()
        {
            return _idCardTypeQueryRepo.GetAll().ToList();
        }

        public MaritalStatus MaritalStatus(int ID)
        {
            return _maritalStatusQueryRepo.GetBy(x => x.ID == ID).FirstOrDefault();
        }

        public List<MaritalStatus> MaritalStatusList()
        {
            return _maritalStatusQueryRepo.GetAll().ToList();
        }

        public List<ProductCode> ProductCodes()
        {
            return _productCodeQueryRepo.GetAll().ToList();
        }

        public ProductCode ProductCode(int ID)
        {
            return _productCodeQueryRepo.GetBy(x => x.ID == ID).FirstOrDefault();
        }

        public List<Sex> Sexes()
        {
            return _sexQueryRepo.GetAll().ToList();
        }

        public SocioProfCode SocioProfCode(int ID)
        {
            return _socioProfCodeQueryRepo.GetBy(x => x.ID == ID).FirstOrDefault();
        }

        public List<SocioProfCode> SocioProfCodes()
        {
            return _socioProfCodeQueryRepo.GetAll().ToList();
        }

        public State State(int ID)
        {
            return _stateQueryRepo.GetBy(x => x.ID == ID).FirstOrDefault();
        }

        public List<State> StateList()
        {
            return _stateQueryRepo.GetAll().ToList();
        }

        public List<TitleCode> Titles()
        {
            return _titleQueryRepo.GetAll().ToList();
        }

        public bool VerifyTitleCode(string titleCode)
        {
            return _titleQueryRepo.GetBy(x => x.Code.ToLower() == titleCode.ToLower()).Any();
        }

        public List<NationalityCode> NationalityCodes()
        {
            return _nationalityCodeQueryRepo.GetAll().ToList();
        }

        public NationalityCode NationalityCode(int ID)
        {
            return _nationalityCodeQueryRepo.GetBy(x => x.ID == ID).FirstOrDefault();
        }
    }
}
