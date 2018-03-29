
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ZenithCardRepo.Data;

namespace ZenithCardPerso.Repository.Command
{
    public class CommandRepository<TEntity> : ICommandRepository<TEntity> where TEntity : class
    {
        private ApplicationDbContext _context;
        private DbSet<TEntity> _dbSet;

        public CommandRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();

        }

        public void Insert(TEntity entity)
        {
            _dbSet.Add(entity);

        }

        public void InsertRange(List<TEntity> entity) 
        {
            _dbSet.AddRange(entity);
        }

        public void Update(TEntity entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            _dbSet.Remove(entity);
        }

        public void AttachEntity(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            _dbSet.Attach(entity);
        }

        public void DeleteRange(List<TEntity> entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            _dbSet.RemoveRange(entity);
        }

        public int Save()
        {
            return _context.SaveChanges();
        }
        public async Task<int> SaveAync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
