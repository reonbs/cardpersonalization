using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ZenithCardPerso.Repository.Query
{
    public interface IQueryRepository<TEntity> where TEntity : class
    {
        TEntity FindByKey(int id);

        IEnumerable<TEntity> GetAll();
        Task<IEnumerable<TEntity>> GetAllAsync();
        IQueryable<TEntity> GetBy(Func<TEntity, bool> predicate);


        IEnumerable<TEntity> AllInclude(params Expression<Func<TEntity, object>>[] includeProperties);

        IEnumerable<TEntity> FindByInclude(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties);

        IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] includeProperties);

        

    }
}
