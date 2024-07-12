using MaiCommerce.Models.DataModels;

namespace MaiCommerce.DataAccess.Repository.IRepository;

public interface ICompanyRepository : IRepository<Company>
{
    void Update(Company company);
}