using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MaiCommerce.Models.DataModels;

// this is the actual details about the order, which product, total price and count
public class OrderDetail
{
    public int Id { get; set; }
    [Required]
    public int OrderHeaderId { get; set; }
    [ForeignKey("OrderHeaderId")]
    [ValidateNever]
    public OrderHeader OrderHeader { get; set; }
    
    [Required]
    public int ProductId { get; set; }
    [ForeignKey("ProductId")]
    [ValidateNever]
    public Product Product { get; set; }
    
    public int Count { get; set; }
    public double Price { get; set; }
}