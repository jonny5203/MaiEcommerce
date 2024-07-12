using System.Linq.Expressions;
using MaiCommerce.DataAccess.Data;
using MaiCommerce.DataAccess.Repository.IRepository;
using MaiCommerce.Models.DataModels;
using MaiCommerce.Models.IdentityModels;

namespace MaiCommerce.DataAccess.Repository;

//This class will act as a respository for all databse handling regarding
//categories, that inherits from the repository which act as a general baseclass
//and also implements all the methods in the ICategoryRepository
public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
{
    private ApplicationDBContext _db;
    
    //Request custom db context and pass it on to base class
    public ApplicationUserRepository(ApplicationDBContext db) : base(db)
    {
        _db = db;
    }
}