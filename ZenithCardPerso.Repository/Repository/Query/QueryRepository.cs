using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ZenithCardPerso.Repository.Utility;
using ZenithCardRepo.Data;

namespace ZenithCardPerso.Repository.Query
{
    public class QueryRepository<TEntity> : IQueryRepository<TEntity> where TEntity : class
    {
        private ApplicationDbContext _context;
        private DbSet<TEntity> _dbSet;
        private IUtilities _utilities;

        public QueryRepository(ApplicationDbContext context, IUtilities utilities)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
            _utilities = utilities;
        }

        public TEntity FindByKey(int id)
        {
            Expression<Func<TEntity, bool>> lambda = _utilities.BuildLambdaForFindByKey<TEntity>(id);
            return _dbSet.AsNoTracking().SingleOrDefault(lambda);
        }
        public IEnumerable<TEntity> GetAll()
        {
            return _dbSet.AsNoTracking().ToList();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }


        public IQueryable<TEntity> GetBy(Func<TEntity, bool> predicate)
        {
            return _dbSet.Where(predicate).AsQueryable<TEntity>();
        }

       

        public IEnumerable<TEntity> AllInclude(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return GetAllIncluding(includeProperties).ToList();
        }

        public IEnumerable<TEntity> FindByInclude(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = GetAllIncluding(includeProperties);
            IEnumerable<TEntity> results = query.Where(predicate).ToList();
            return results;
        }

        public IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> queryable = _dbSet.AsNoTracking();

            return includeProperties.Aggregate
              (queryable, (current, includeProperty) => current.Include(includeProperty));
        }

        public DbRawSqlQuery<T> StoreprocedureQuery<T>(string storeprocedureName)
        {
            return _context.Database.SqlQuery<T>("EXEC " + storeprocedureName);
        }

        public DbRawSqlQuery<T> StoreprocedureQueryFor<T>(string storeprocedureName, params object[] parameters)
        {
            return _context.Database.SqlQuery<T>("EXEC " + storeprocedureName, parameters);
        }
    }
}
