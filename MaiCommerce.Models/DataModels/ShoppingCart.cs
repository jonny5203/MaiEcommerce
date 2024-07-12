using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MaiCommerce.Models.IdentityModels;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MaiCommerce.Models.DataModels;

public class ShoppingCart
{
    [Key]
    public int Id { get; set; }
    
    public int ProductId { get; set; }
    [ForeignKey("ProductId")]
    [ValidateNever]
    public Product Product { get; set; }
    
    [Range(1, 1000, ErrorMessage = "Quantity must be between 1 and 1000")]
    public int Count { get; set; }
    
    public string ApplicationUserId { get; set; }
    [ForeignKey("ApplicationUserId")]
    [ValidateNever]
    public ApplicationUser ApplicationUser { get; set; }
}