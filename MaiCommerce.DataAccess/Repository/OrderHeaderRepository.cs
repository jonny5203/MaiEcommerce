using System.Linq.Expressions;
using MaiCommerce.DataAccess.Data;
using MaiCommerce.DataAccess.Repository.IRepository;
using MaiCommerce.Models.DataModels;

namespace MaiCommerce.DataAccess.Repository;

//This class will act as a respository for all databse handling regarding
//categories, that inherits from the repository which act as a general baseclass
//and also implements all the methods in the ICategoryRepository
public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
{
    private ApplicationDBContext _db;
    
    //Request custom db context and pass it on to base class
    public OrderHeaderRepository(ApplicationDBContext db) : base(db)
    {
        _db = db;
    }
    
    //using the normal update here, because of extra functionality it has, instead of dbSet
    public void Update(OrderHeader obj)
    {
        _db.OrderHeaders.Update(obj);
    }
    
    // Query the status with the id value from the db instance, then change the status with the parameter value
    public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
    {
        var orderFromDb = _db.OrderHeaders.FirstOrDefault(n => n.Id == id);
        if (orderFromDb != null)
        {
            orderFromDb.OrderStatus = orderStatus;
            if (!string.IsNullOrEmpty(paymentStatus))
            {
                orderFromDb.PaymentStatus = paymentStatus;
            }
        }
    }

    public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
    {
        var orderFromDb = _db.OrderHeaders.FirstOrDefault(n => n.Id == id);
        if (!string.IsNullOrEmpty(sessionId))
        {
            orderFromDb.SessionId = sessionId;
        }
        if (!string.IsNullOrEmpty(paymentIntentId))
        {
            orderFromDb.PaymentIntentId = paymentIntentId;
            orderFromDb.PaymentDate = DateTime.Now;
            
        }
    }
}