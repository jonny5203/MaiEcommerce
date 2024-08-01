using System.Linq.Expressions;
using MaiCommerce.DataAccess.Data;
using MaiCommerce.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MaiCommerce.DataAccess.Repository
{
    //This class will act a DI service in program.cs
    //simply just passing around an object for database handling, as well as defining CRUD operations
    //with Entity Framework, this will be the base class for all crud and query operations
    //you notice that all the methods are general purpose(generic), not spesific to one object type
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDBContext _db;
        //collection of models, used for the generic type handling, and loose coupling
        private DbSet<T> dbSet;

        public Repository(ApplicationDBContext db)
        {
            _db = db;
            dbSet = _db.Set<T>();
            //Preload the category with the foreign key
            //because of getall(), we have to put it here
            //this is applied to every query regarding Products
            _db.Products
                .Include(n => n.Category)
                .Include(n => n.CategoryId);
        }
        
        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            
            //Check and if there is a Moodel name and then include that mode
            //in the product, the navigation is preloaded in the constructor
            //just passing the name of the model here
            if (!string.IsNullOrEmpty(includeProperties))
            {
                //a dynamic way of including more properties if there is
                //a need for that using string and string manipulation
                //String format ex. "Category, Product", this is because of
                //EF lazy loading(dbset doesn't know about category), it won't load all the foreign key navigation unless
                //explicitly told so, not doing so will result in a null obj for the
                //obj that the foreign key points to, the reference object in the model classes
                foreach (var includeProp in includeProperties
                             .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.ToList();
        }

        public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracking = false)
        {
            IQueryable<T> query;
            if (tracking)
            {
                // A query obect to quyery the database
                query = dbSet
                    .Where(filter);
                
            }
            else
            {
                // A query obect to quyery the database and prevent that the
                // reference object return from the query
                query = dbSet.AsNoTracking()
                    .Where(filter);
            }
            
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties
                             .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    // A commaseparated string list is passed I check if it's empty, split the valuee
                    // loop through every value, and call include on every one of them
                    query = query.Include(includeProp);
                }
            }
            
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

