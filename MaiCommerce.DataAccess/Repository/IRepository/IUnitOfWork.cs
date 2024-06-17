namespace MaiCommerce.DataAccess.Repository.IRepository;

public interface IUnitOfWork
{
    ICategoryRepository Category { get; }
    IProductRepository Product { get;  }

    void Save();
}