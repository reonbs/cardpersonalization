using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZenithCardPerso.Repository.Command
{
    public interface ICommandRepository<TEntity> where TEntity : class
    {
        void Insert(TEntity entity);
        void InsertRange(List<TEntity> entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        int Save();
        Task<int> SaveAync();
    }
}
