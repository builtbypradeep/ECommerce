using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        //T == Category
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeproperties = null);
        T Get(Expression<Func<T, bool>> filter, string? includeproperties = null, bool tracked = false);

        void Add(T entity);
        //void Update(T entity);
        void Remove(T entity);

        void RemoveRange(IEnumerable<T> entity);

    }
}
