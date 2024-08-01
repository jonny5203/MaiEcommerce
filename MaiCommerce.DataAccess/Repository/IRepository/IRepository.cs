
using System.Linq.Expressions;

namespace MaiCommerce.DataAccess.Repository.IRepository
{
    //Master Repository Interface for database handling
    //It takes in a generic class and has all the method for CRUD
    //operations except for update which will have its own implementation
    public interface IRepository<T> where T : class
    {
        //T - Category
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
        T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracking = false);
        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
    }
}

