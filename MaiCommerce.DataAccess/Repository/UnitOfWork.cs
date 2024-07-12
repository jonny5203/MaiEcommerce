using MaiCommerce.DataAccess.Data;
using MaiCommerce.DataAccess.Repository.IRepository;

namespace MaiCommerce.DataAccess.Repository;

public class UnitOfWork : IUnitOfWork
{
    private ApplicationDBContext _db;
    
    //Reference to all repositories, composition
    public ICategoryRepository Category { get; private set; }
    public IProductRepository Product { get; private set; }
    public ICompanyRepository Company { get; private set; }
    public IShoppingCartRepository ShoppingCart { get; private set; }
    public IApplicationUserRepository ApplicationUser { get; private set; }
    
    //Request custom db context and creating a categoryrepository with the context
    //for future flexibility, and then inject(DI) the context to all the compositions(fields)
    public UnitOfWork(ApplicationDBContext db)
    {
        _db = db;
        Category = new CategoryRepository(_db);
        Product = new ProductRepository(_db);
        Company = new CompanyRepository(_db);
        ShoppingCart = new ShoppingCartRepository(_db);
        ApplicationUser = new ApplicationUserRepository(_db);
    }

    //have it's own Save function so that it can work
    //as a common save function across repos
    public void Save()
    {
        _db.SaveChanges();
    }
}