using MaiCommerce.DataAccess.Data;
using MaiCommerce.DataAccess.Repository.IRepository;
using MaiCommerce.Models.DataModels;

namespace MaiCommerce.DataAccess.Repository;

//This class will act as a respository for all databse handling regarding
//categories, that inherits from the repository which act as a general baseclass
//and also implements all the methods in the ICategoryRepository
public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
{
    private ApplicationDBContext _db;

    //Request custom db context and pass it on to base class
    public ShoppingCartRepository(ApplicationDBContext db) : base(db)
    {
        _db = db;
    }
    
    //using the normal update here, because of extra functionality it has, instead of dbSet
    public void Update(ShoppingCart shoppingCart)
    {
        _db.ShoppingCarts.Update(shoppingCart);
    }
}