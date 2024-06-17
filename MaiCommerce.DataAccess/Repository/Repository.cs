using System.Linq.Expressions;
using MaiCommerce.DataAccess.Data;
using MaiCommerce.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MaiCommerce.DataAccess.Repository
{
    //This class will act a DI service in program.cs
    //simply just passing around an object for database handling
    //with Entity Framework, this will be the base class for all crud and query operations
    //you notice that all the methods are general purpose(generic), not spesific to one object type
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDBContext _db;
        private DbSet<T> dbSet;

        public Repository(ApplicationDBContext db)
        {
            _db = db;
            dbSet = _db.Set<T>();
        }

        public IEnumerable<T> GetAll()
        {
            IQueryable<T> query = dbSet;
            return query.ToList();
        }

        public T Get(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = dbSet
                .Where(filter);
            return query.FirstOrDefault();
        }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }
    }
}

