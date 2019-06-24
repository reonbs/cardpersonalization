using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenithCardPerso.Repository.Command;
using ZenithCardPerso.Repository.Query;
using ZenithCardRepo.Data;
using ZenithCardRepo.Data.Models;

namespace ZenithCardRepo.Services.BLL.UnitofWork
{
    public class UnitOfWork : IDisposable
    {
        public ApplicationDbContext context;
        private bool disposed;
        private Dictionary<string, object> repositories;
        
        public UnitOfWork(ApplicationDbContext context)
        {
            this.context = context;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            disposed = true;
        }


    }
}
