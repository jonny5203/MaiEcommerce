using MaiCommerce.Models;

namespace MaiCommerce.DataAccess.Repository.IRepository;

//this implements everything from IRepository and add save and update to it 
//when for categories handling
public interface ICategoryRepository : IRepository<Category>
{
    void Update(Category category);
}