using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ZenithCardPerso.Repository.Utility
{
    public interface IUtilities
    {
        Expression<Func<TEntity, bool>> BuildLambdaForFindByKey<TEntity>(int id);
       
    }
}
