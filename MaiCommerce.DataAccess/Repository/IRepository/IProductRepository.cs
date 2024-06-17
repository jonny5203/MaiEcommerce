using MaiCommerce.Models;

namespace MaiCommerce.DataAccess.Repository.IRepository;

public interface IProductRepository : IRepository<Product>
{
    void Update(Product product);
}