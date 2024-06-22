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
        //Doing manual mapping of every value, because ImageUrl needs to be checked first
        //before update, no need to call _db.Product.Update(), EF automatically keeps track
        var _objFromDB = _db.Products.FirstOrDefault(n => n.Id == product.Id);

        if (_objFromDB != null)
        {
            _objFromDB.Title = product.Title;
            _objFromDB.Description = product.Description;
            _objFromDB.ISBN = product.ISBN;
            _objFromDB.Author = product.Author;
            _objFromDB.ListPrice = product.ListPrice;
            _objFromDB.Price = product.Price;
            _objFromDB.Price50 = product.Price50;
            _objFromDB.Price100 = product.Price100;
            _objFromDB.ImageUrl = product.ImageUrl;
            _objFromDB.CategoryId = product.CategoryId;

            if (product.ImageUrl != null)
            {
                _objFromDB.ImageUrl = product.ImageUrl;
            }
        }
    }
}