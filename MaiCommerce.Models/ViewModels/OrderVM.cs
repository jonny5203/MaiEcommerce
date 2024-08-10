using MaiCommerce.Models.DataModels;

namespace MaiCommerce.Models.ViewModels;

public class OrderVM
{
    public OrderHeader orderHeader { get; set; }
    public IEnumerable<OrderDetail> orderDetails { get; set; }
}