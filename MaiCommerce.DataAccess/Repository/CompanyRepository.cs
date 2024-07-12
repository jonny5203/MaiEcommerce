using MaiCommerce.DataAccess.Data;
using MaiCommerce.DataAccess.Repository.IRepository;
using MaiCommerce.Models.DataModels;

namespace MaiCommerce.DataAccess.Repository;

public class CompanyRepository : Repository<Company>, ICompanyRepository
{
    private ApplicationDBContext _db;
    
    public CompanyRepository(ApplicationDBContext db) : base(db)
    {
        _db = db;
    }

    public void Update(Company company)
    {
        _db.Companies.Update(company);
    }
}