using MaiCommerce.DataAccess.Data;
using MaiCommerce.DataAccess.Repository.IRepository;
using MaiCommerce.Models;

namespace MaiCommerce.DataAccess.Repository;

public class UnitOfWork : IUnitOfWork
{
    private ApplicationDBContext _db;
    
    //Reference to all repositories, composition
    public ICategoryRepository Category { get; private set; }
    public IProductRepository Product { get; private set; }
    
    //Request custom db context and creating a categoryrepository with the context
    //for future flexibility
    public UnitOfWork(ApplicationDBContext db)
    {
        _db = db;
        Category = new CategoryRepository(db);
        Product = new ProductRepository(db);
    }

    //have it's own Save function so that it can work
    //as a common save function across repos
    public void Save()
    {
        _db.SaveChanges();
    }
}