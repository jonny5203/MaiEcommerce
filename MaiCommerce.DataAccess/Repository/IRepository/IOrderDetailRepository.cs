using MaiCommerce.Models.DataModels;

namespace MaiCommerce.DataAccess.Repository.IRepository;

public interface IOrderDetailRepository : IRepository<OrderDetail>
{
    void Update(OrderDetail orderDetail);
}