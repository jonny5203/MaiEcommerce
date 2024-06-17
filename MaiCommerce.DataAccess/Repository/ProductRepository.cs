using MaiCommerce.DataAccess.Data;
using MaiCommerce.DataAccess.Repository.IRepository;
using MaiCommerce.Models;

namespace MaiCommerce.DataAccess.Repository;

public class ProductRepository : Repository<Product>, IProductRepository
{
    private readonly ApplicationDBContext _db;

    public ProductRepository(ApplicationDBContext db) : base(db)
    {
        _db = db;
    }


    public void Update(Product product)
    {
        _db.Products.Update(product);
    }
}